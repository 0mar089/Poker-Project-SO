#include <mysql.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>


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
	MYSQL *conn;
	conn = mysql_init(NULL);
	if (conn == NULL) {
		printf("Error al crear la conexiￃﾳn: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	conn = mysql_real_connect(conn, "localhost", "root", "mysql", NULL, 0, NULL, 0);
	if (conn == NULL) {
		printf("Error al inicializar la conexiￃﾳn: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	if (mysql_select_db(conn, "PokerDB") != 0) {
		printf("Error seleccionando la base de datos: %s\n", mysql_error(conn));
		exit(1);  // O manejar el error de otra forma
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
	serv_adr.sin_port = htons(9000);
	
	
	
	if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
		printf("Error en el bind");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el listen");
	
	printf("Servidor a la espera...\n");
	
	while (1) {
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexiￃﾳn recibida\n");
		
		int ret = read(sock_conn, buff, sizeof(buff));
		
		if (ret > 0) {
			buff[ret] = '\0';
			printf("Mensaje recibido: %s\n", buff);  // Imprimir el mensaje recibido para depuraciￃﾳn
		} else {
			printf("Error al recibir datos\n");
			close(sock_conn);
			continue;  // Saltar a la siguiente iteraciￃﾳn del bucle si hay error al recibir
		}
		// Procesar el mensaje
		char *p = strtok(buff, "/");
		
		// Comprobar si el cliente quiere desconectar
		if (strcmp(p, "0") == 0) {
			printf("Cliente desconectado.\n");
			strcpy(response, "DISCONNECT");
			write(sock_conn, response, strlen(response) + 1);  // Enviar la confirmaciￃﾳn de desconexiￃﾳn al cliente
			close(sock_conn);
			continue;  // Saltar a la siguiente iteraciￃﾳn del bucle
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
					
					// Obtener el ID mￃﾡs grande
					sprintf(response, "SELECT MAX(id) FROM Jugadores;");
					if (mysql_query(conn, response)) {
						printf("Error al ejecutar consulta SELECT MAX(id): %s\n", mysql_error(conn));
						strcpy(response, "ERROR AL OBTENER EL ID MￃS GRANDE");
					}
					else {
						MYSQL_RES *res = mysql_store_result(conn);
						if (res == NULL) {
							printf("Error al obtener resultado MAX(id): %s\n", mysql_error(conn));
							strcpy(response, "ERROR AL OBTENER EL ID MￃS GRANDE");
						}
						else {
							MYSQL_ROW row = mysql_fetch_row(res);
							int nuevo_id = 1;  // Por defecto, si la tabla estￃﾡ vacￃﾭa, el primer ID serￃﾡ 1
							if (row[0] != NULL) {
								nuevo_id = atoi(row[0]) + 1;  // Si no estￃﾡ vacￃﾭa, toma el valor de MAX(id) y le suma 1
							}
							mysql_free_result(res);
							// Insertar nuevo usuario con el nuevo id
							sprintf(response, "INSERT INTO Jugadores (id, nombre, cuenta, contrasenya, capital) VALUES (%d, '%s', '%s', '%s', 0.00);", nuevo_id, nombre, cuenta, contrasenya);
							if (mysql_query(conn, response)) {
								printf("Error al insertar nuevo usuario: %s\n", mysql_error(conn));
								strcpy(response, "ERROR AL INSERTAR EL NUEVO USUARIO");
							} else {
								strcpy(response, "REGISTERED");
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
			printf("Consulta SQL: %s\n", response);  // Imprime la consulta para depuraciￃﾳn
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
					strcpy(response, "LOGGED_IN\0");
				} else {
					strcpy(response, "LOGIN_FAILED\0");
				}
				mysql_free_result(res);
			}
			
		}
		else if (strcmp(p, "MAX_MONEY") == 0) {
			// Consulta para obtener el jugador con mￃﾡs dinero
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
			// Consulta para obtener la ￃﾺltima vez que se jugￃﾳ en la mesa 3
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
			// Consulta para obtener el ID del ganador de la ￃﾺltima partida en la mesa 2
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
									strcpy(response, "No se encontrￃﾳ al ganador.");
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

		write(sock_conn, response, strlen(response) + 1);  // Enviar el tamaￃﾱo de la cadena incluyendo el \0
		close(sock_conn);    // Cierra la conexiￃﾳn individual
		
	}
	
	
	mysql_close(conn);
	return 0;
}



