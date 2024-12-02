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


// Añade nuevo conectado. Retorna 0 si es exitoso y -1 si la lista esta¡ llena.
int AddPlayer(ListaConectados *lista, char nombre[20], int socket) {

	if (lista->num == 100) {
		// La lista esta¡ llena, no se puede aÃ±adir mÃ¡s usuarios
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



// TE DEVUELVE LA POSICION DEL USUARIO A PARTIR DE SU NOMBRE EN LA LISTA DE CONECTADOS
int DamePosicion(ListaConectados *lista, char nombre[20]) {
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
// TE DEVUELVE LA POSICION DEL USUARIO EN LA LISTA A PARTIR DEL SOCKET
int DamePosSock(ListaConectados *lista, int socket) {
	int i = 0;
	while(i < lista->num) {
		if(lista->conectados[i].socket == socket) {
			return i;  // Si encontramos el socket, devolvemos la posicion
		}
		i++;  // Incrementamos el Ã­ndice en cada iteracion
	}
	return -1;  // Si no encontramos el socket, devolvemos -1
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


// FUNCION REGISTER USER
void RegisterUser(MYSQL *conn, char *nombre, char *cuenta, char *contrasenya, int sock_conn, char *response) {
	
	sprintf(response, "SELECT * FROM Jugadores WHERE cuenta='%s';", cuenta);
	if (mysql_query(conn, response)) {
		printf("Error al ejecutar consulta SELECT: %s\n", mysql_error(conn));
		strcpy(response, "1/ERROR AL REALIZAR LA CONSULTA DE VERIFICACION");
	} 
	else {
		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			printf("Error al obtener resultado: %s\n", mysql_error(conn));
			strcpy(response, "1/ERROR AL OBTENER RESULTADO");
		} 
		else if (mysql_num_rows(res) > 0) {
			strcpy(response, "1/YA HAY UN USUARIO CON ESA CUENTA");
		}
		else {
			sprintf(response, "SELECT MAX(id) FROM Jugadores;");
			if (mysql_query(conn, response)) {
				printf("Error al ejecutar consulta SELECT MAX(id): %s\n", mysql_error(conn));
				strcpy(response, "1/ERROR AL OBTENER EL ID MAS GRANDE");
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
					strcpy(response, "1/ERROR AL INSERTAR EL NUEVO USUARIO");
				}
				else {
					strcpy(response, "1/REGISTERED");
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
			strcpy(response, "2/LOGGED_IN");
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
			strcpy(response, "2/LOGIN_FAILED");
		}
		mysql_free_result(res);
	}
}


// REVISTA SI HAY GENTE EN LA SALA. 0 SI ESTA VACIA, -1 SI ESTA LLENA. Solo cuando alguien intenta unirse
int CheckRoom(MYSQL *conn, int NumSala, char *response, char *nombre){
	
	sprintf(response, "SELECT num_jug FROM Mesa WHERE id_mesa='%d';", NumSala);
	if(mysql_query(conn, response)) {
		printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
		strcpy(response, "ERROR");
	}
	else{
		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			
			printf("Error al obtener el resultado: %s\n", mysql_error(conn));
			strcpy(response, "ERROR");
		}
		else{
			MYSQL_ROW row = mysql_fetch_row(res);
			int numGenteSala = atoi(row[0]);
			printf("%s quiere entrar en la sala %d que hay %d personas\n", nombre, NumSala, numGenteSala);
			
			
			if( numGenteSala < 4 ) {
				char query[500];
				sprintf(query,"UPDATE Mesa SET num_jug=num_jug+1 WHERE id_mesa='%d';", NumSala);
				mysql_query(conn, response);
				return numGenteSala;
			}
			return -1;
			
		}
	}
	
}
	
void CheckAllRooms(MYSQL *conn, char *response, int GenteSala[4]){
	sprintf(response, "SELECT num_jug FROM Mesa");
	if(mysql_query(conn, response)) {
		printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
		strcpy(response, "ERROR");
	}
	else{
		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			
			printf("Error al obtener el resultado: %s\n", mysql_error(conn));
			strcpy(response, "ERROR");
		}
		else{
			MYSQL_ROW row = mysql_fetch_row(res);
			
			for (int i = 0; i < 4; i++) {
				GenteSala[i] = 0;
			}
			int j = 0;
			while(row != NULL){
				GenteSala[j] = atoi(row[0]);
				row = mysql_fetch_row(res);
				printf("Sala %d: %d\n", j, GenteSala[j]); 
				j++;
			}
			
		}
		mysql_free_result(res);
	}
	
	
}





int socket_num;
int sockets[300];

void* AtenderCliente(void* socket_desc) {
	int sock_conn = *(int*)socket_desc;
	//free(socket_desc);
	MYSQL *conn;
	conn = mysql_init(NULL);
	conn = mysql_real_connect(conn, "shiva2.upc.es", "root", "mysql", "T2_BBDDPoker", 0, NULL, 0);
	mysql_select_db(conn, "T2_BBDDPoker");
	char buff[512];
	char response[512];
	int stop = 0;
	
	while(stop == 0) {
		int ret = read(sock_conn, buff, sizeof(buff));
		if (ret > 0) {
			buff[ret] = '\0';
			printf("Mensaje recibido: %s\n", buff);

		}
		else {
			printf("Error al recibir datos\n");
			close(sock_conn);
			stop = 1;
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
			
		}
		// REGISTER
		else if (strcmp(p, "1") == 0) {
			char *nombre = strtok(NULL, "/");
			char *cuenta = strtok(NULL, "/");
			char *contrasenya = strtok(NULL, "/");
			RegisterUser(conn, nombre, cuenta, contrasenya, sock_conn, response);
			write (sock_conn, response, strlen(response));
		}
		// LOGIN
		else if (strcmp(p, "2") == 0) {
			char *cuenta = strtok(NULL, "/");
			char *contrasenya = strtok(NULL, "/");
			LoginUser(conn, cuenta, contrasenya, sock_conn, response);
			write (sock_conn, response, strlen(response));
		}
		
		
		// Para invitar a alguien
		if ( strcmp(p, "5") == 0 ) {
			char *name = strtok(NULL, "/");
			char *nameInvited = strtok(NULL, "/");
			if (name == NULL) {
				printf("Error: name es NULL\n");
			} else {
				printf("El que invita: '%s'\n", name);
				printf("El que es invitado: '%s'\n", nameInvited);
			}
			int pos = DamePosicion(&conectados, nameInvited);
			int socketInvited = conectados.conectados[pos].socket;
			sprintf(response, "5/%s/Te ha invitado %s", nameInvited, name);
			write(sockets[pos], response, strlen(response));
			printf("Invitación enviada a %s (socket: %d)\n", nameInvited, socketInvited);
		}
		
		// Para enviar mensaje al chat
		if ( strcmp(p, "6") == 0) {
			char nombreAutor[20];
			char mensajeChat[512];
			char chatMessage[512]; //mensaje a enviar por el chat

			p = strtok(NULL, "/");
			strcpy(nombreAutor, p);
			p = strtok(NULL, "/");
			strcpy(mensajeChat, p);

			sprintf(chatMessage, "%s: %s", nombreAutor, mensajeChat);
			sprintf(response, "6/%s\n", chatMessage);
			int j;
			for (j = 0; j<conectados.num; j++) {
				
				write (sockets[j], response, strlen(response));
			}
		}
		if( strcmp(p, "7") == 0 ) {
			
			char nombreCliente[30];
			int numSala;
			p = strtok(NULL, "/");
			strcpy(nombreCliente, p);
			p = strtok(NULL, "/");
			numSala = atoi(p);
			
			// Ahora como sabemos el numero de sala, podemos llamar a la funcion que compruebe el num de sala si esta lleno
			int err = CheckRoom(conn, numSala, response, nombreCliente);
			if(err != -1){
				// Devolvemos el numero de gente si no esta llena
				sprintf(response, "7/%d/%d", err, numSala);
				
				// AQUI EN TEORIA SE DEBERIA DE CAMBIAR EN LA BASE DE DATOS CUANTA GENTE HAY PERO DE MOMENTO NO
				write (sock_conn, response, strlen(response));
			}
			else{
				sprintf(response, "7/-1");
				write (sock_conn, response, strlen(response));
			}
			
		}
		
		// Lista de conectados
		if( (strcmp(p,"1") == 0 ) || (strcmp(p,"2") == 0 ) ){
			// Creo un string llamado notificacion que guardara la lista de conectados para enviarla al cliente
			char notificacion[900];
			char connectedUsers[300];
			DameConectados(&conectados, connectedUsers);
			sprintf(notificacion, "4/%s", connectedUsers);
			int j;
			for (j = 0; j<conectados.num; j++) {
				
				write (sockets[j], notificacion, strlen(notificacion));
			}
			printf("Lista Conectados: %s\n", notificacion);
		}
		
		
		// CAMBIAR NUMERO DE JUGADORES EN LA SALA CON NOTIFICACIONES
		if( (strcmp(p,"1") == 0 ) || (strcmp(p,"2") == 0 ) || (strcmp(p, "7") == 0) ){
			
			int GenteNumSala[4];
			char notificacion[900];
			CheckAllRooms(conn, response, GenteNumSala);
			int j;
			sprintf(notificacion, "8/%d/%d/%d/%d", GenteNumSala[0], GenteNumSala[1], GenteNumSala[2], GenteNumSala[3]);
			for (j = 0; j<conectados.num; j++) {
				
				write (sockets[j], notificacion, strlen(notificacion));
			}
			printf("Actualizando Salas... \n");
			printf("Mensaje Recibido: %s", notificacion);
		}
		
		
		
		if(strcmp(p, "0") == 0) {
			// Creo un string llamado notificacion que guardara la lista de conectados para enviarla al cliente
			char notificacion[900];
			char connectedUsers[300];
			DameConectados(&conectados, connectedUsers);
			sprintf(notificacion, "4/%s", connectedUsers);
			int j;
			for (j = 0; j<conectados.num; j++) {
				
				write (sockets[j], notificacion, strlen(notificacion));
			}
			printf("Se ha ido un usuario \n");
			printf("Lista Conectados: %s\n", notificacion);
			close(sock_conn);
			stop = 1;
			return 0;
		}
		// write(sock_conn, response, strlen(response) + 1);
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
	conn = mysql_real_connect(conn, "shiva2.upc.es", "root", "mysql", "T2_BBDDPoker", 0, NULL, 0); // AQUI VA SHIVA2 
	if (conn == NULL) {
		printf("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	// EjecutarScript(conn, "PokerDB.sql");
	
	if (mysql_select_db(conn, "T2_BBDDPoker") != 0) {
		printf("Error seleccionando la base de datos: %s\n", mysql_error(conn));
		exit(1);
	}
	
	// EJECUTAMOS UNA CONSULTA PARA PONER EN 0 EL NOMBRE D EJUGADORES DE LAS SALAS
	char query[500];
	sprintf(query, "UPDATE Mesa SET num_jug = 0;");
	mysql_query(conn, query);
	if(mysql_query(conn, query)){
		printf("Error al vaciar las salas");
		exit(1);
	}
	printf("Salas vaciadas\n");
	
	
	
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
	serv_adr.sin_port = htons(50055);
	
	
	
	if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
		printf("Error en el bind");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el listen");
	
	printf("Servidor a la espera...\n");
	
	pthread_t thread;
	socket_num = 0;
	
	while (1) {
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida\n");
		
		//int* new_sock = malloc(sizeof(int));
		//*new_sock = sock_conn;
		
		
		sockets[socket_num] = sock_conn;
		pthread_create(&thread, NULL, AtenderCliente, &sockets[socket_num]);
		socket_num=socket_num+1;
		// El hilo se encarga de liberar los recursos, por lo que no necesitamos esperar a que termine
		// pthread_detach(thread);
	}
	
	mysql_close(conn);
	return 0;
}



