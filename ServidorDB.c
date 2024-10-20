#include <mysql.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <pthread.h>  // Para manejar hilos


typedef struct{
	
	char nombre[20];
	int socket;
	
}Conectado;

typedef struct {
	
	Conectado conectados[8];
	int num;
}ListaConectados;



int AddPlayer(ListaConectados *lista, char nombre[20], int socket) {
	// A?ade nuevo conectado. Retorna 0 si es exitoso y -1 si la lista esta? llena.
	
	if (lista->num == 100) {
		// La lista esta? llena, no se puede a??adir m??s usuarios
		return -1;
	} else {
		// Copiar el nombre del usuario y asignar el socket
		strcpy(lista->conectados[lista->num].nombre, nombre);
		lista->conectados[lista->num].socket = socket;
		
		// Incrementar el numero de usuarios conectados
		lista->num++;
		return 0;
	}
}

int DamePosicion(ListaConectados *lista, char nombre[20]) {
	// Esta funcion devuelve el socket del usuario que le des de la lista de conectados
	int i=0;
	int encontrado = 0;
	while ((i < lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre,nombre) == 0)
			encontrado =1;
		if (!encontrado)
			i=i+1;
	}
	if(encontrado)
		return i;
	else
		return -1;
}

int Eliminar(ListaConectados *lista, char nombre[20]) {
	
	int pos = DamePosicion(lista, nombre);
	if(pos == -1)
		return -1;
	
	else {
		int i;
		for(i = pos; i<lista->num-1;i++) {
			lista->conectados[i] = lista->conectados[i+1];
		}
		lista->num--;
		return 0;
	}
}

void DameConectados(ListaConectados *lista, char conectados[300]) {
	// Pone en conectados los nombres de todos los conectados separados
	// por /. Primero pone el numero de conectados. Ejemplo:
	// "3/Juan/Maria/Pedro"
	
	sprintf(conectados, "%d", lista->num);
	int i;
	
	for (i = 0; i < lista->num; i++)
		sprintf(conectados, "%s/%s", conectados, lista->conectados[i].nombre);
}

pthread_mutex_t mutexLista = PTHREAD_MUTEX_INITIALIZER;
ListaConectados conectados;


void* AtenderCliente(void* socket_desc);

// Funcion que ejecuta scripts SQL
void EjecutarScript(MYSQL *conn, const char *filename) {
	FILE *file = fopen(filename, "r");
	if (file == NULL) {
		printf("No se pudo abrir el archivo %s\n", filename);
		exit(1);
	}
	
	char query[4096];
	char line[1024];
	query[0] = '\0';
	while (fgets(line, sizeof(line), file)) {
		if (line[strlen(line) - 1] == '\n') {
			line[strlen(line) - 1] = '\0';
		}
		
		strcat(query, line);
		if (strchr(line, ';')) {
			if (mysql_query(conn, query)) {
				printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
			} else {
				printf("Consulta ejecutada\n");
			}
			query[0] = '\0';
		}
	}
	fclose(file);
}





int main(int argc, char *argv[]) {
	
	// Inicializar MySQL
	conectados.num = 0;
	MYSQL *conn;
	conn = mysql_init(NULL);
	if (conn == NULL) {
		printf("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	conn = mysql_real_connect(conn, "localhost", "root", "mysql", NULL, 0, NULL, 0);
	if (conn == NULL) {
		printf("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	if (mysql_select_db(conn, "PokerDB") != 0) {
		printf("Error seleccionando la base de datos: %s\n", mysql_error(conn));
		EjecutarScript(conn, "PokerDB.sql");
		
		if (mysql_select_db(conn, "PokerDB") != 0) {
			printf("Error seleccionando la base de datos: %s\n", mysql_error(conn));
			exit(1);  // O manejar el error de otra forma
		}
	}
	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	char buff[512];
	char response[512];
	
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creando socket");
	
	
	int opt = 1;
	if (setsockopt(sock_listen, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt)) < 0) {
		perror("setsockopt");
		exit(1);
	}
	
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(9130);
	
	
	
	if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
		printf("Error en el bind");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el listen");
	
	printf("Servidor a la espera...\n");
	
	while (1) {
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida\n");
		
		// Crear un nuevo hilo para manejar esta conexi??n
		pthread_t thread;
		int* new_sock = malloc(sizeof(int));
		*new_sock = sock_conn;
		
		if (pthread_create(&thread, NULL, AtenderCliente, (void*)new_sock) < 0) {
			perror("No se pudo crear el hilo");
			return 1;
		}
		
		// El hilo se encarga de liberar los recursos, por lo que no necesitamos esperar a que termine
		pthread_detach(thread);
	}
	
	mysql_close(conn);
	return 0;
}



void* AtenderCliente(void* socket_desc) {
	int sock_conn = *(int*)socket_desc;
	free(socket_desc);  // Liberar memoria
	
	MYSQL *conn;
	conn = mysql_init(NULL);
	conn = mysql_real_connect(conn, "localhost", "root", "mysql", NULL, 0, NULL, 0);
	mysql_select_db(conn, "PokerDB");
	
	char buff[512];
	char response[512];
	int ret = read(sock_conn, buff, sizeof(buff));
	
	if (ret > 0) {
		buff[ret] = '\0';
		printf("Mensaje recibido: %s\n", buff);  // Imprimir el mensaje recibido para depuraci??n
	} else {
		printf("Error al recibir datos\n");
		close(sock_conn);
		return 0;  // Terminar el hilo
	}
	
	// Procesar el mensaje
	char *p = strtok(buff, "/");
	
	// Comprobar si el cliente quiere desconectar
	if (strcmp(p, "0") == 0) {
		printf("Cliente desconectado.\n");
		strcpy(response, "DISCONNECT");
		write(sock_conn, response, strlen(response) + 1);  // Enviar la confirmaci??n de desconexi??n al cliente
		close(sock_conn);
		return 0;  // Terminar el hilo
	}
	
	// Resto del codigo de procesamiento (REGISTER y LOGIN)
	if (strcmp(p, "REGISTER") == 0) {
		// Registro de usuario
		char *nombre = strtok(NULL, "/");
		char *cuenta = strtok(NULL, "/");
		char *contrasenya = strtok(NULL, "/");
		// Resto del codigo de procesamiento (REGISTER y LOGIN)
		// Registro de usuario
		
		// Verificar si ya existe un usuario con la misma cuenta
		
		sprintf(response, "SELECT * FROM Jugadores WHERE cuenta='%s';", cuenta);
		if (mysql_query(conn, response)) {
			printf("Error al ejecutar consulta SELECT: %s\n", mysql_error(conn)); // Agregar mensaje de error
			strcpy(response, "ERROR AL REALIZAR LA CONSULTA DE VERIFICACION");
		}
		
		else {
			MYSQL_RES *res = mysql_store_result(conn);
			if (res == NULL) {
				// Si hay error al obtener el resultado
				printf("Error al obtener resultado: %s\n", mysql_error(conn)); // Imprimir error detallado
				strcpy(response, "ERROR AL OBTENER RESULTADO");
			} else if (mysql_num_rows(res) > 0) {
				// Ya existe un usuario con la misma cuenta
				strcpy(response, "REGISTER_FAILED"); // Usuario ya registrado
			} else {
				
				// Obtener el ID m??\u0192???s grande
				sprintf(response, "SELECT MAX(id) FROM Jugadores;");
				if (mysql_query(conn, response)) {
					printf("Error al ejecutar consulta SELECT MAX(id): %s\n", mysql_error(conn));
					strcpy(response, "ERROR AL OBTENER EL ID M??\u0192S GRANDE");
				}
				else {
					MYSQL_RES *res = mysql_store_result(conn);
					if (res == NULL) {
						printf("Error al obtener resultado MAX(id): %s\n", mysql_error(conn));
						strcpy(response, "ERROR AL OBTENER EL ID M??\u0192S GRANDE");
					}
					else {
						MYSQL_ROW row = mysql_fetch_row(res);
						int nuevo_id = 1;  // Por defecto, si la tabla est??\u0192??? vac??\u0192???a, el primer ID ser??\u0192??? 1
						if (row[0] != NULL) {
							nuevo_id = atoi(row[0]) + 1;  // Si no est??\u0192??? vac??\u0192???a, toma el valor de MAX(id) y le suma 1
						}
						mysql_free_result(res);
						// Insertar nuevo usuario con el nuevo id
						sprintf(response, "INSERT INTO Jugadores (id, nombre, cuenta, contrasenya, capital) VALUES (%d, '%s', '%s', '%s', 0.00);", nuevo_id, nombre, cuenta, contrasenya);
						if (mysql_query(conn, response)) {
							printf("Error al insertar nuevo usuario: %s\n", mysql_error(conn));
							strcpy(response, "ERROR AL INSERTAR EL NUEVO USUARIO");
						} else {
							strcpy(response, "REGISTERED");
							pthread_mutex_lock(&mutexLista);							
							AddPlayer(&conectados, nombre, sock_conn);
							pthread_mutex_unlock(&mutexLista);
						}
					}
				}
			}
			mysql_free_result(res);  // Liberar memoria solo si res no es NULL
		}
		
	}
	else if (strcmp(p, "LOGIN") == 0) {
		char *cuenta = strtok(NULL, "/");
		char *contrasenya = strtok(NULL, "/");
		// Genera la consulta SQL
		sprintf(response, "SELECT * FROM Jugadores WHERE cuenta='%s' AND contrasenya='%s';", cuenta, contrasenya);
		printf("Consulta SQL: %s\n", response);  // Imprime la consulta para depuraci??\u0192???n
		if (mysql_query(conn, response)) {
			printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));  // Imprime el error de MySQL
			strcpy(response, "ERROR\0");
		}
		else {
			MYSQL_RES *res = mysql_store_result(conn);
			if (res == NULL) {
				printf("Error al obtener el resultado: %s\n", mysql_error(conn));  // Si no se puede obtener el resultado
				strcpy(response, "ERROR\0");
			} else if (mysql_num_rows(res) > 0) {
				char nombre[20];
				strcpy(response, "LOGGED_IN\0");
				
				// Ejecutar la consulta para obtener el nombre
				char query_nombre[100];
				sprintf(query_nombre, "SELECT nombre FROM Jugadores WHERE cuenta='%s' AND contrasenya='%s';", cuenta, contrasenya);
				if (mysql_query(conn, query_nombre)) {
					printf("FAILED QUERY");
				} 
				else {
					MYSQL_RES *res = mysql_store_result(conn);
					if (res && mysql_num_rows(res) > 0) {
						MYSQL_ROW row = mysql_fetch_row(res);
						strcpy(nombre, row[0]); // Asignar el nombre obtenido de la consulta a la variable nombre
						mysql_free_result(res);
						// Agregar el jugador a la lista de conectados
						pthread_mutex_lock(&mutexLista);						
						AddPlayer(&conectados, nombre, sock_conn);
						pthread_mutex_unlock(&mutexLista);
						// Obtener y mostrar la lista de conectados
						char Misconectados[300];
						DameConectados(&conectados, Misconectados);
						printf("Resultado: %s\n", Misconectados);
					}
				}		
				
			} else {
				strcpy(response, "LOGIN_FAILED\0");
			}
			mysql_free_result(res);
		}
		
	}
	else if (strcmp(p, "MAX_MONEY") == 0) {
		// Consulta para obtener el jugador con m??\u0192???s dinero
		sprintf(response, "SELECT nombre, capital FROM Jugadores ORDER BY capital DESC LIMIT 1;");
		
		if (mysql_query(conn, response)) {
			printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));  // Mensaje de error
			strcpy(response, "ERROR");
		}
		else {
			MYSQL_RES *res = mysql_store_result(conn);
			if (res == NULL) {
				printf("Error al obtener el resultado: %s\n", mysql_error(conn));  // Mensaje de error
				strcpy(response, "ERROR");
			} else {
				MYSQL_ROW row = mysql_fetch_row(res);
				if (row != NULL) {
					// Enviar el nombre del jugador y su capital
					sprintf(response, "%s tiene %.2f euros de capital", row[0], atof(row[1]));
				}else {
					strcpy(response, "No hay jugadores en la base de datos.");
				}
				mysql_free_result(res);
			}
		}
		write(sock_conn, response, strlen(response) + 1);  // Enviar el mensaje al cliente
	}
	else if (strcmp(p, "GANAR_LUIS") == 0) {
		// Consulta para obtener las mesas donde Luis ha ganado
		sprintf(response, "SELECT id_mesa FROM Historial WHERE ganador=(SELECT id FROM Jugadores WHERE nombre='Luis');");
		
		if (mysql_query(conn, response)) {
			printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));  // Mensaje de error
			strcpy(response, "ERROR");
		}
		else {
			MYSQL_RES *res = mysql_store_result(conn);
			if (res == NULL) {
				printf("Error al obtener el resultado: %s\n", mysql_error(conn));  // Mensaje de error
				strcpy(response, "ERROR");
			} else {
				char mesas[1024] = "";
				MYSQL_ROW row;
				int first = 1;  // Flag para manejar la coma
				while ((row = mysql_fetch_row(res))) {
					// Concatenar mesas en el resultado
					if (!first) {
						strcat(mesas, ", ");  // Agregar coma entre mesas
					}
					strcat(mesas, row[0]);  // Agregar id_mesa
					first = 0;
				}if (strlen(mesas) == 0) {
					strcpy(response, "Luis no ha ganado en ninguna mesa.");
				} else {
					strcpy(response, mesas);  // Enviar las mesas encontradas
				}
				mysql_free_result(res);
			}
		}
		write(sock_conn, response, strlen(response) + 1);  // Enviar el mensaje al cliente
	}
	else if (strcmp(p, "ULTIMA_JUGO_MESA3") == 0) {
		// Consulta para obtener la ??\u0192???ltima vez que se jug??\u0192??? en la mesa 3
		sprintf(response, "SELECT MAX(date_id) FROM Historial WHERE id_mesa=3;");
		
		if (mysql_query(conn, response)) {
			printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));  // Mensaje de error
			strcpy(response, "ERROR");
		}else {
			MYSQL_RES *res = mysql_store_result(conn);
			if (res == NULL) {
				printf("Error al obtener el resultado: %s\n", mysql_error(conn));  // Mensaje de error
				strcpy(response, "ERROR");
			}else {
				MYSQL_ROW row = mysql_fetch_row(res);
				if (row[0] != NULL) {
					// Si hay una fecha, la enviamos
					strcpy(response, row[0]);
				} else {
					// Si no hay resultados, indicamos que no se ha jugado
					strcpy(response, "No hay partidas registradas en la mesa 3.");
				}
				mysql_free_result(res);
			}
		}
		write(sock_conn, response, strlen(response) + 1);  // Enviar el mensaje al cliente
	}
	if (strcmp(p, "ULTIMO_GANADOR_MESA2") == 0) {
		// Consulta para obtener el ID del ganador de la ultima partida en la mesa 2
		sprintf(response, "SELECT ganador FROM Historial WHERE id_mesa=2 ORDER BY date_id DESC LIMIT 1;");
		
		if (mysql_query(conn, response)) {
			printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));  // Mensaje de error
			strcpy(response, "ERROR");
		}else {
			MYSQL_RES *res = mysql_store_result(conn);
			if (res == NULL) {
				printf("Error al obtener el resultado: %s\n", mysql_error(conn));  // Mensaje de error
				strcpy(response, "ERROR");
			} else {
				MYSQL_ROW row = mysql_fetch_row(res);
				if (row[0] != NULL) {
					// Si hay un ganador, obtenemos su nombre
					int ganador_id = atoi(row[0]);
					// Consulta para obtener el nombre del ganador
					sprintf(response, "SELECT nombre FROM Jugadores WHERE id=%d;", ganador_id);
					if (mysql_query(conn, response)) {
						printf("Error al ejecutar la consulta para obtener el nombre: %s\n", mysql_error(conn));
						strcpy(response, "ERROR");
					} else {
						MYSQL_RES *res_nombre = mysql_store_result(conn);
						if (res_nombre == NULL) {
							printf("Error al obtener el resultado del nombre: %s\n", mysql_error(conn));
							strcpy(response, "ERROR");
						} else {
							MYSQL_ROW row_nombre = mysql_fetch_row(res_nombre);
							if (row_nombre[0] != NULL) {
								strcpy(response, row_nombre[0]);  // Guardamos el nombre del ganador
							} else {
								strcpy(response, "No se encontro al ganador.");
							}
							mysql_free_result(res_nombre);
						}
					}
				} else {
					// Si no hay resultados, indicamos que no se ha jugado
					strcpy(response, "No hay partidas registradas en la mesa 2.");
				}
				mysql_free_result(res);
			}
		}
		write(sock_conn, response, strlen(response) + 1);  // Enviar el mensaje al cliente
	}
	else if(strcmp(p, "DAME_CONECTADOS") == 0) {
		printf("Dame conectados");
		char Misconectados[300];
		DameConectados(&conectados, Misconectados);
		strcpy(response, Misconectados);
		
	}
	
	printf("1111");
	write(sock_conn, response, strlen(response) + 1);  // Enviar el tama??o de la cadena incluyendo el \0
	close(sock_conn);  // Cierra la conexi??n individual
	mysql_close(conn);
	
	return 0;  // Terminar el hilo
}









