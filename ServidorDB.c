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

int EliminarWithSocket(ListaConectados *lista, int sock) {
	
	int pos = DamePosSock(lista, sock);
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


int DamePosSock(ListaConectados *lista, int socket) {
	int i = 0;
	while(i < lista->num) {
		if(lista->conectados[i].socket == socket) {
			return i;  // Si encontramos el socket, devolvemos la posicion
		}
		i++;  // Incrementamos el ??ndice en cada iteracion
	}
	return -1;  // Si no encontramos el socket, devolvemos -1
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
	int file_executed = 0;
	while (fgets(line, sizeof(line), file)) {
		if (line[strlen(line) - 1] == '\n') {
			line[strlen(line) - 1] = '\0';
		}
		
		strcat(query, line);
		if (strchr(line, ';')) {
			if (mysql_query(conn, query)) {
				printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
			} 
			else {
				file_executed = 1;
			}
			query[0] = '\0';
		}
	}
	if(file_executed) printf("Script de la base de datos ejecutado");
	fclose(file);
}



// FUNCION REGISTER USER
void RegisterUser(MYSQL *conn, char *nombre, char *cuenta, char *contrasenya, int sock_conn, char *response) {
	
	sprintf(response, "SELECT * FROM Jugadores WHERE cuenta='%s';", cuenta);
	if (mysql_query(conn, response)) {
		printf("Error al ejecutar consulta SELECT: %s\n", mysql_error(conn));
		strcpy(response, "ERROR AL REALIZAR LA CONSULTA DE VERIFICACION");
	} 
	else {
		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			printf("Error al obtener resultado: %s\n", mysql_error(conn));
			strcpy(response, "ERROR AL OBTENER RESULTADO");
		} 
		else if (mysql_num_rows(res) > 0) {
			strcpy(response, "YA HAY UN USUARIO CON ESA CUENTA");
		}
		else {
			sprintf(response, "SELECT MAX(id) FROM Jugadores;");
			if (mysql_query(conn, response)) {
				printf("Error al ejecutar consulta SELECT MAX(id): %s\n", mysql_error(conn));
				strcpy(response, "ERROR AL OBTENER EL ID MAS GRANDE");
			}
			else {
				MYSQL_RES *res = mysql_store_result(conn);
				int nuevo_id = 1;
				if (res && mysql_num_rows(res) > 0) {
					MYSQL_ROW row = mysql_fetch_row(res);
					if (row[0] != NULL) {
						nuevo_id = atoi(row[0]) + 1;
					}
					mysql_free_result(res);
				}
				sprintf(response, "INSERT INTO Jugadores (id, nombre, cuenta, contrasenya, capital) VALUES (%d, '%s', '%s', '%s', 0.00);", nuevo_id, nombre, cuenta, contrasenya);
				if (mysql_query(conn, response)) {
					printf("Error al insertar nuevo usuario: %s\n", mysql_error(conn));
					strcpy(response, "ERROR AL INSERTAR EL NUEVO USUARIO");
				}
				else {
					strcpy(response, "REGISTERED");
					pthread_mutex_lock(&mutexLista);
					AddPlayer(&conectados, nombre, sock_conn);
					
					pthread_mutex_unlock(&mutexLista);				}
			}
		}
		mysql_free_result(res);
	}
}


// FUNCION LOGIN USER
void LoginUser(MYSQL *conn, char *cuenta, char *contrasenya, int sock_conn, char *response) {
	
	sprintf(response, "SELECT * FROM Jugadores WHERE cuenta='%s' AND contrasenya='%s';", cuenta, contrasenya);
	
	if (mysql_query(conn, response)) {
		
		printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
		strcpy(response, "ERROR");
	}
	else {
		
		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			
			printf("Error al obtener el resultado: %s\n", mysql_error(conn));
			strcpy(response, "ERROR");
		}
		else if (mysql_num_rows(res) > 0) {
			
			char nombre[20];
			strcpy(response, "LOGGED_IN");
			char query_nombre[100];
			sprintf(query_nombre, "SELECT nombre FROM Jugadores WHERE cuenta='%s' AND contrasenya='%s';", cuenta, contrasenya);
			
			if (mysql_query(conn, query_nombre) == 0) {
				
				MYSQL_RES *res_nombre = mysql_store_result(conn);
				
				if (res_nombre && mysql_num_rows(res_nombre) > 0) {
					
					MYSQL_ROW row = mysql_fetch_row(res_nombre);
					strcpy(nombre, row[0]);
					mysql_free_result(res_nombre);
					pthread_mutex_lock(&mutexLista);
					AddPlayer(&conectados, nombre, sock_conn);
					pthread_mutex_unlock(&mutexLista);
					char Misconectados[300];
					DameConectados(&conectados, Misconectados);
					printf("Resultado: %s\n", Misconectados);
				}
			}
		} 
		else {
			strcpy(response, "LOGIN_FAILED");
		}
		mysql_free_result(res);
	}
}

void* AtenderCliente(void* socket_desc) {
	int sock_conn = *(int*)socket_desc;
	free(socket_desc);
	MYSQL *conn;
	conn = mysql_init(NULL);
	conn = mysql_real_connect(conn, "localhost", "root", "mysql", NULL, 0, NULL, 0);
	mysql_select_db(conn, "PokerDB");
	char buff[512];
	char response[512];
	
	
	while(1) {
		int ret = read(sock_conn, buff, sizeof(buff));
		if (ret > 0) {
			buff[ret] = '\0';
			printf("Mensaje recibido: %s\n", buff);
		}
		else {
			printf("Error al recibir datos\n");
			close(sock_conn);
			return 0;
		}
		char *p = strtok(buff, "/");
		if (strcmp(p, "0") == 0) {
			
			printf("Cliente desconectado.\n");
			
			strcpy(response, "DISCONNECT");
			
			write(sock_conn, response, strlen(response) + 1);
			
			pthread_mutex_lock(&mutexLista);
			EliminarWithSocket(&conectados, sock_conn);
			pthread_mutex_unlock(&mutexLista);
			
			printf("11111");
			
			close(sock_conn);
			return 0;
		}
		else if (strcmp(p, "REGISTER") == 0) {
			char *nombre = strtok(NULL, "/");
			char *cuenta = strtok(NULL, "/");
			char *contrasenya = strtok(NULL, "/");
			RegisterUser(conn, nombre, cuenta, contrasenya, sock_conn, response);
		}
		else if (strcmp(p, "LOGIN") == 0) {
			char *cuenta = strtok(NULL, "/");
			char *contrasenya = strtok(NULL, "/");
			LoginUser(conn, cuenta, contrasenya, sock_conn, response);
		}
		else if (strcmp(p, "DAME_CONECTADOS") == 0) {
			char Misconectados[300];
			DameConectados(&conectados, Misconectados);
			strcpy(response, Misconectados);
		}
		write(sock_conn, response, strlen(response) + 1);
	}
	
	mysql_close(conn);
	return 0;
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
	conn = mysql_real_connect(conn, "localhost", "root", "mysql", NULL, 0, NULL, 0); // AQUI VA SHIVA2 
	if (conn == NULL) {
		printf("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	EjecutarScript(conn, "PokerDB.sql");
	
	if (mysql_select_db(conn, "PokerDB") != 0) {
		printf("Error seleccionando la base de datos: %s\n", mysql_error(conn));
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
	serv_adr.sin_port = htons(9300);
	
	
	
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




