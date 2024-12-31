#include <mysql.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <pthread.h>  // Para manejar hilos
#include <unistd.h>
#include <time.h>



typedef struct{
	
	char nombre[20];
	int socket;
	
}Conectado;

typedef struct {
	
	Conectado conectados[8];
	int num;
}ListaConectados;


typedef struct{
	char nombre[20];
	int socket;
	float apuesta;
}Player;

typedef struct{
	
	Player players[4];
	int num_players;
	int turnoActual; // Indica el indice de la lista de jugadores, del 0 al 3; osea 4 jugadores, el cual le toca jugar actualmente
}Sala;

typedef struct{
	Sala salas[4];
}ListaSalas;


// Añade nuevo conectado. Retorna 0 si es exitoso y -1 si la lista esta  llena.
int AddPlayer(ListaConectados *lista, char nombre[20], int socket) {

	if (lista->num == 100) {
		// La lista esta  llena, no se puede añadir más usuarios
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
		i++;  // Incrementamos el índice en cada iteracion
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
ListaSalas salas;

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
				sprintf(response, "INSERT INTO Jugadores (id, nombre, cuenta, contrasenya, capital) VALUES (%d, '%s', '%s', '%s', 1000.00);", nuevo_id, nombre, cuenta, contrasenya);
				if (mysql_query(conn, response)) {
					printf("Error al insertar nuevo usuario: %s\n", mysql_error(conn));
					strcpy(response, "1/ERROR AL INSERTAR EL NUEVO USUARIO");
				}
				else {
					strcpy(response, "1/REGISTERED/");
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
			strcpy(response, "2/LOGGED_IN/");
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
			strcpy(response, "2/LOGIN_FAILED/");
		}
		mysql_free_result(res);
	}
}

float ObtenerBalanceCuenta(MYSQL *conn, ListaConectados *lista, int socket) {
    // Obtener la posición del usuario a partir del socket
    int pos = DamePosSock(lista, socket);
    if (pos == -1) {
        printf("Error: No se encontró el usuario para el socket proporcionado.\n");
        return -1.0f; // Indicador de error
    }

    // Obtener el nombre del usuario
    char nombre[20];
    strcpy(nombre, lista->conectados[pos].nombre);

    // Consultar el balance en la base de datos
    char query[256];
    sprintf(query, "SELECT capital FROM Jugadores WHERE nombre='%s';", nombre);

    if (mysql_query(conn, query)) {
        printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
        return -1.0f; // Indicador de error
    }

    MYSQL_RES *res = mysql_store_result(conn);
    if (res == NULL) {
        printf("Error al obtener el resultado: %s\n", mysql_error(conn));
        return -1.0f; // Indicador de error
    }

    MYSQL_ROW row = mysql_fetch_row(res);
    float balance = 0.0f;
    if (row != NULL) {
        balance = atof(row[0]); // Convertir el balance de texto a flotante
    } else {
        printf("No se encontró el balance para el usuario %s.\n", nombre);
        balance = -1.0f; // Indicador de error
    }

    mysql_free_result(res);
    return balance;
}



// REVISA SI HAY GENTE EN LA SALA. 0 SI ESTA VACIA, -1 SI ESTA LLENA. Solo cuando alguien intenta unirse
int CheckRoom(MYSQL *conn, int NumSala, char *nombre){
	
	char consulta[500];
	sprintf(consulta, "SELECT num_jug FROM Mesa WHERE id_mesa='%d';", NumSala);
	if(mysql_query(conn, consulta)) {
		printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
		return -1;
	}
	else{
		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			
			printf("Error al obtener el resultado: %s\n", mysql_error(conn));
			return -1;
		}
		else{
			
			MYSQL_ROW row = mysql_fetch_row(res);
			int numGenteSala = atoi(row[0]);
			printf("%s quiere entrar en la sala %d que hay %d personas\n", nombre, NumSala, numGenteSala);
			
			mysql_free_result(res);
			
			if( numGenteSala < 4 ) {

				char query[500];
				sprintf(query,"UPDATE Mesa SET num_jug=num_jug+1 WHERE id_mesa='%d';", NumSala);
				mysql_query(conn, query);
				printf("+1 persona en la Mesa: %d \n", NumSala);

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
				printf("Numero de gente en la Sala %d: %d\n", j+1, GenteSala[j]); 
				j++;
			}
			
		}
		mysql_free_result(res);
	}
	
	
}

int AddPlayerSala(ListaSalas *salas, char nombre[20], int numSala, int socket) {
    int salaIndex = numSala - 1;

    // Guardamos el índice actual de num_players antes de incrementarlo
    int gente = salas->salas[salaIndex].num_players;

    // Añadimos el jugador
    strcpy(salas->salas[salaIndex].players[gente].nombre, nombre);
    salas->salas[salaIndex].players[gente].socket = socket;
    salas->salas[salaIndex].num_players++; // Incrementamos después de usar el índice

    printf("Jugador %s añadido a la Estructura sala[%d] en la posición %d\n", 
           nombre, salaIndex, gente);
    return gente; // Retornamos la cantidad total de jugadores en la sala
}


// Elimina jugador de la Sala
int DeletePlayerSala(MYSQL *conn, ListaSalas *salas, char nombre[20], int numSala, int socket) 
{
	
	
    int salaIndex = numSala - 1; // Convertir número de sala a índice

    // Buscar al jugador por nombre en la sala
    int jugadorIndex = -1;
    for (int i = 0; i < salas->salas[salaIndex].num_players; i++) {
        if (strcmp(salas->salas[salaIndex].players[i].nombre, nombre) == 0) {
            jugadorIndex = i;
            break;
        }
    }

    // Si el jugador no se encuentra, devolver error
    if (jugadorIndex == -1) {
        printf("Jugador %s no encontrado en la sala %d.\n", nombre, numSala);
        return -1; // Error: jugador no encontrado
    }

    // Eliminar al jugador y reorganizar la lista
    for (int i = jugadorIndex; i < salas->salas[salaIndex].num_players - 1; i++) {
        salas->salas[salaIndex].players[i] = salas->salas[salaIndex].players[i + 1]; // Mover jugadores hacia atrás
    }

    // Reducir el número de jugadores en la sala
    salas->salas[salaIndex].num_players--;

    // Limpiar la última posición (opcional, para evitar datos residuales)
    memset(&salas->salas[salaIndex].players[salas->salas[salaIndex].num_players], 0, sizeof(Player));

    printf("Jugador %s eliminado de la sala %d. Ahora hay %d jugadores.\n",
           nombre, numSala, salas->salas[salaIndex].num_players);
	
   	char query[500];
   	sprintf(query,"UPDATE Mesa SET num_jug=num_jug-1 WHERE id_mesa='%d';", numSala);
   	mysql_query(conn, query);
	
    return salas->salas[salaIndex].num_players; // Devolver el nuevo número de jugadores
}




void ObtenerPlayersSala(ListaSalas *salas, int numSala, char nombres[100]) {
	
	int salaIndex = numSala - 1;
    int i;	
	printf("%s\n", nombres);

    // Recorremos los jugadores de la sala y añadimos sus nombres separados por '/'
    for (i = 0; i < salas->salas[salaIndex].num_players; i++) {
        // Añadimos el nombre del jugador al string
        strcat(nombres, salas->salas[salaIndex].players[i].nombre);
		strcat(nombres, "/");
    }
	printf("Se enviará a la funcion 7: %s\n",nombres);
}



void ObtenerSocketsPlayersSala(ListaSalas *salas, int numSala, int SocketsPlayers[4]) {

	
	int salaIndex = numSala - 1;
    int i;

    // Inicializamos el arreglo de sockets a -1 (valor de error o vacío)
    for (i = 0; i < 4; i++) {
        SocketsPlayers[i] = -1;
    }

    // Recorremos los jugadores de la sala y almacenamos sus sockets
    for (i = 0; i < salas->salas[salaIndex].num_players; i++) {
        SocketsPlayers[i] = salas->salas[salaIndex].players[i].socket;
    }
	
}



void CrearMazo(char mazo[52][5]) {

    const char *mazoCombinaciones[] = {
        "_2S", "_3S", "_4S", "_5S", "_6S", "_7S", "_8S", "_9S", "_10S", "JS", "QS", "KS", "AS",
        "_2H", "_3H", "_4H", "_5H", "_6H", "_7H", "_8H", "_9H", "_10H", "JH", "QH", "KH", "AH",
        "_2D", "_3D", "_4D", "_5D", "_6D", "_7D", "_8D", "_9D", "_10D", "JD", "QD", "KD", "AD",
        "_2C", "_3C", "_4C", "_5C", "_6C", "_7C", "_8C", "_9C", "_10C", "JC", "QC", "KC", "AC"
    };

    // Copiar las cartas al mazo
    for (int i = 0; i < 52; i++) {
        strcpy(mazo[i], mazoCombinaciones[i]);
    }

}


// Te devuelve el mazo mezclado de 52 
void MezclarMazo(char mazo[52][5]) {
    for (int i = 0; i < 52; i++) {
        int randomIndex = rand() % 52;
        char temp[4];
        strcpy(temp, mazo[i]);
        strcpy(mazo[i], mazo[randomIndex]);
        strcpy(mazo[randomIndex], temp);
    }

}

// Te devuelve las cartas comunitarias, las del jugador 1 y las del jugador 2
void RepartirCartas(char mazo[52][5], char comunitarias[5][5], char jugador1[2][5], char jugador2[2][5]) {
    int index = 0;


    // Repartir 5 cartas comunitarias
    for (int i = 0; i < 5; i++) {
        strcpy(comunitarias[i], mazo[index]);
		index++;
    }


    // Repartir 2 cartas al jugador 1
    for (int i = 0; i < 2; i++) {
        strcpy(jugador1[i], mazo[index]);
		index++;
    }

    // Repartir 2 cartas al jugador 2
    for (int i = 0; i < 2; i++) {
        strcpy(jugador2[i], mazo[index]);
		index++;
    }

}

	//FUNCION PARA NOTIFICAR A TODOS LOS CONECTADOS CUANDO SE LOGUEA/REGISTRA ALGUIEN (Siempre que se loguea/registre alguien o se desconecte)

void NotificarListaConectados(ListaConectados *conectados) {
	char notificacion[900];
	memset(notificacion, 0, sizeof(notificacion));


	char connectedUsers[300];
	DameConectados(conectados, connectedUsers); 
	sprintf(notificacion, "4/%s/", connectedUsers);


	for (int i = 0; i < conectados->num; i++) {
		write(conectados->conectados[i].socket, notificacion, strlen(notificacion));
		printf("Notificación enviada a %s: %s\n", conectados->conectados[i].nombre, notificacion);
	}
	usleep(100000); // Para evitar sobrecarga
}


	//FUNCION CUANDO SE UNE ALGUIEN O SE VA DE LA SALA (siempre que se vaya alguien o se una)

void NotificarEstadoSalas(MYSQL *conn, ListaConectados *conectados) {
    int GenteNumSala[4];
    char notificacion[900];
    memset(notificacion, 0, sizeof(notificacion));

    // Obtener el estado de todas las salas
    CheckAllRooms(conn, notificacion, GenteNumSala);

    // Construir el mensaje con el estado de las salas
    sprintf(notificacion, "8/%d/%d/%d/%d/", 
            GenteNumSala[0], GenteNumSala[1], GenteNumSala[2], GenteNumSala[3]);

    // Enviar el mensaje a todos los clientes conectados
    for (int j = 0; j < conectados->num; j++) {
        write(conectados->conectados[j].socket, notificacion, strlen(notificacion));
        usleep(100000); // Evitar saturación
    }

    printf("Actualizando Salas: %s\n", notificacion);
}


float generarApuesta() {
    int num = rand() % 50 + 1; // Genera un número entre 1 y 50

    return (float) num; // Convierte el número entero a float
}

float RestarCapital(MYSQL *conn, char jugador[40], float apuesta) {

    char consulta[256];

    // Realizar el UPDATE directamente
    snprintf(consulta, sizeof(consulta), 
        "UPDATE jugadores SET capital = capital - %.2f WHERE nombre = '%s' AND capital >= %.2f;", 
        apuesta, jugador, apuesta);

    // Ejecutar la consulta
    if (mysql_query(conn, consulta)) {
        printf("Error al actualizar el capital del jugador '%s': %s\n", jugador, mysql_error(conn));
        return 0;
    }

    // Verificar si se actualizó alguna fila
    if (mysql_affected_rows(conn) == 0) {
        printf("No se pudo restar la apuesta. El jugador '%s' podría no tener suficiente capital.\n", jugador);
		return -1;
    } 
	
	else {

        printf("Capital actualizado correctamente para el jugador '%s'. Apuesta restada: %.2f\n", jugador, apuesta);
		char query[256];
		snprintf(query, sizeof(query), 
        "SELECT capital FROM jugadores WHERE nombre='%s';", jugador);

		if (mysql_query(conn, query)) {
			printf("Error al ejecutar la consulta: %s\n", mysql_error(conn));
			return;
		}

		MYSQL_RES *res = mysql_store_result(conn);
		if (res == NULL) {
			printf("Error al obtener el resultado: %s\n", mysql_error(conn));
			return;
		}

		MYSQL_ROW row = mysql_fetch_row(res);
		if (row == NULL) {
			printf("Jugador '%s' no encontrado.\n", jugador);
			mysql_free_result(res);
			return;
		}

		// Obtener el capital actual del jugador
		float capitalActual = atof(row[0]);
		mysql_free_result(res);
		return capitalActual;

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
	
	

	while (stop == 0) {
		int ret = read(sock_conn, buff, sizeof(buff));
		if (ret > 0) {
			buff[ret] = '\0';
			printf("Mensaje recibido: %s\n", buff);
		} else {
			printf("Error al recibir datos\n");
			close(sock_conn);
			stop = 1;
			return 0;
		}

		char *p = strtok(buff, "/");

		switch (atoi(p)) { // Convierte `p` a un número para simplificar
			case 0: { // Desconexión

				printf("Cliente desconectado.\n");
				memset(response, 0, sizeof(response));
				strcpy(response, "DISCONNECT");
				write(sock_conn, response, strlen(response) + 1);
				usleep(100000);

				pthread_mutex_lock(&mutexLista);
				EliminarWithSocket(&conectados, sock_conn);
				NotificarListaConectados(&conectados);
				pthread_mutex_unlock(&mutexLista);
				break;
			}

			case 1: { // Registro

				char *nombre = strtok(NULL, "/");
				char *cuenta = strtok(NULL, "/");
				char *contrasenya = strtok(NULL, "/");
				memset(response, 0, sizeof(response));
				RegisterUser(conn, nombre, cuenta, contrasenya, sock_conn, response);
				write(sock_conn, response, strlen(response));
				usleep(100000);
				if (strcmp(response, "1/REGISTERED/") == 0) {
					NotificarListaConectados(&conectados);
					NotificarEstadoSalas(conn, &conectados);
				}
				break;
			}

			case 2: { // Inicio de sesión

				char *cuenta = strtok(NULL, "/");
				char *contrasenya = strtok(NULL, "/");
				memset(response, 0, sizeof(response));
				LoginUser(conn, cuenta, contrasenya, sock_conn, response);
				write(sock_conn, response, strlen(response));
				usleep(100000);
				if (strcmp(response, "2/LOGGED_IN/") == 0) {
					NotificarListaConectados(&conectados);
					NotificarEstadoSalas(conn, &conectados);
				}
				break;
			}

			case 5:{ // Invitar

				char *decision = strtok(NULL, "/");
				char *name = strtok(NULL, "/");
				char *nameInvited = strtok(NULL, "/");
				if (strcmp(decision, "2") == 0) {
					printf("El que invita: '%s'\n", name);
					printf("El que es invitado: '%s'\n", nameInvited);
					memset(response, 0, sizeof(response));
					int pos = DamePosicion(&conectados, nameInvited);
					int socketInvited = conectados.conectados[pos].socket;
					sprintf(response, "5/2/%s/", name);
					write(sockets[pos], response, strlen(response));
					usleep(100000);
					printf("Invitación enviada a %s (socket: %d)\n", nameInvited, socketInvited);
				} else if (strcmp(decision, "1") == 0) {
					char *respuesta = strtok(NULL, "/");
					if (strcmp(respuesta, "NO") == 0) {
						int pos = DamePosicion(&conectados, name);
						printf("%s ha rechazado la invitación de: %s\n", nameInvited, name);
						sprintf(response, "5/1/%s/NO/", nameInvited);
						write(sockets[pos], response, strlen(response));
						usleep(100000);
					} else if (strcmp(respuesta, "SI") == 0) {
						int pos = DamePosicion(&conectados, name);
						printf("%s ha aceptado la invitación de: %s\n", nameInvited, name);
						sprintf(response, "5/1/%s/SI/", nameInvited);
						write(sockets[pos], response, strlen(response));
						usleep(100000);
					}
				}
				break;
			}

			case 6: { // Enviar mensaje de chat

				char nombreAutor[20];
				char mensajeChat[512];
				char chatMessage[512];
				p = strtok(NULL, "/");
				strcpy(nombreAutor, p);
				p = strtok(NULL, "/");
				strcpy(mensajeChat, p);
				sprintf(chatMessage, "%s: %s", nombreAutor, mensajeChat);
				memset(response, 0, sizeof(response));
				sprintf(response, "6/%s/", chatMessage);
				for (int j = 0; j < conectados.num; j++) {
					write(sockets[j], response, strlen(response));
				}
				usleep(100000);
				break;
			}

			case 7: { // Unirse a una sala

				char nombreCliente[30];
				int numSala;
				p = strtok(NULL, "/");
				strcpy(nombreCliente, p);
				p = strtok(NULL, "/");
				numSala = atoi(p); 

				memset(response, 0, sizeof(response));

				int err = CheckRoom(conn, numSala, nombreCliente);
				if (err != -1) {

					pthread_mutex_lock(&mutexLista);

					int gente = AddPlayerSala(&salas, nombreCliente, numSala, sock_conn);
					char nombres[100] = {0};
					ObtenerPlayersSala(&salas, numSala, nombres);
					int sockets_players[4];
					ObtenerSocketsPlayersSala(&salas, numSala, sockets_players);

					pthread_mutex_unlock(&mutexLista);

					for (int j = 0; j < gente + 1; j++) {
						char notificacion[300];
						sprintf(notificacion, "7/%d/%d/%s", gente, numSala, nombres);
						float balance = ObtenerBalanceCuenta(conn, &conectados, sockets_players[j]);
						char balanceString[20];
						sprintf(balanceString, "%.2f", balance);
						strcat(notificacion, balanceString);
						write(sockets_players[j], notificacion, strlen(notificacion));
						printf("\nEnviando notificación a jugadores: %s\n", notificacion);
					}
					usleep(1000000);
				} else {
					sprintf(response, "7/-1/");
					write(sock_conn, response, strlen(response));
					printf("Sala llena, mensaje enviado: %s\n", response);
					usleep(1000000);
				}
				NotificarEstadoSalas(conn, &conectados);
				break;
			}

				

			case 9: {
				// Iniciar partida
				srand(time(NULL));

				char nombreHost[30];
				int numSala;
				p = strtok(NULL, "/");
				strcpy(nombreHost, p);
				p = strtok(NULL, "/");
				numSala = atoi(p);

				char mazo[52][5];
				char comunitarias[5][5];
				char jugador1[2][5];
				char jugador2[2][5];

				// Aqui ya estan las comunitarias, jugador1 y jugador2
				CrearMazo(mazo);
				MezclarMazo(mazo);
				

				RepartirCartas(mazo, comunitarias, jugador1, jugador2);

				int socketsPlayers[4];
				ObtenerSocketsPlayersSala(&salas, numSala, socketsPlayers);


				char cartasComunitarias[100];
				snprintf(cartasComunitarias, sizeof(cartasComunitarias), "%s/%s/%s/%s/%s", 
						comunitarias[0], comunitarias[1], comunitarias[2], comunitarias[3], comunitarias[4]);

				char cartasJugador1[30];
				snprintf(cartasJugador1, sizeof(cartasJugador1), "%s/%s", jugador1[0], jugador1[1]);

				char cartasJugador2[30];
				snprintf(cartasJugador2, sizeof(cartasJugador2), "%s/%s", jugador2[0], jugador2[1]);
				
				printf("\n-------------------------------\n");
				printf("SALA %d\n", numSala);
				printf("Comunitarias: %s\n", cartasComunitarias);
				printf("Jugador 1: %s\n", cartasJugador1);
				printf("Jugador 2: %s\n", cartasJugador2);
				printf("-------------------------------\n");


				// DE MOMENTO PARA DOS JUGADORES, SE VUELVE A COPIAR DOS VECES MAS Y CAMBIAR LA GENERACION DE CARTAS SI QUEREMOS PARA 4 JUGADORES
				strcpy(response, "");
				snprintf(response, sizeof(response), "9/%d/%s/%s/", numSala, cartasComunitarias, cartasJugador1);
				write(socketsPlayers[0], response, strlen(response));

				strcpy(response, "");
				snprintf(response, sizeof(response), "9/%d/%s/%s/", numSala, cartasComunitarias, cartasJugador2);
				write(socketsPlayers[1], response, strlen(response));
				usleep(100000);
				break;
			}

			case 10: {
				// Salir de una sala
				char nombreCliente[30];
				int numSala;
				p = strtok(NULL, "/");
				strcpy(nombreCliente, p);
				p = strtok(NULL, "/");
				numSala = atoi(p);

				int a=DeletePlayerSala(conn,&salas, nombreCliente,numSala, sock_conn);
				
				char notificacion[300];
				sprintf(notificacion, "10/%d/%d", numSala, a);
				int j;
				for (j = 0; j<conectados.num; j++) {
					
					write (sockets[j], notificacion, strlen(notificacion));
				}
				usleep(100000);
				break;
			}

			case 11: {
				// Gestionar turno
				int numSala;
				p = strtok(NULL, "/");
				numSala = atoi(p);
				// enviamos a cada uno si es su turno o se tiene que esperar

				pthread_mutex_lock(&mutexLista);
				int sockets_players[4];
				ObtenerSocketsPlayersSala(&salas, numSala, sockets_players);

				// Como sockets_players esta en orden de cuando se añade la gente en la sala, va de 0 a 3. Por lo tanto va sincronizado con el turno de la gente. 
				int indexSala = numSala - 1;
				int turnoActual = salas.salas[indexSala].turnoActual;

				// Conseguimos el nombre de la persona que le toca. 
				
				char nombreTurno[60] = "";
				strncpy(nombreTurno, salas.salas[indexSala].players[turnoActual].nombre, sizeof(nombreTurno) - 1);
				printf("\nSALA %d\n", numSala);
				printf("Turno de: %s\n", nombreTurno);
				pthread_mutex_unlock(&mutexLista);

				for(int i = 0; i<4; i++){
					
					if(sockets_players[i] != -1){
						char turno[60] = "";
						snprintf(turno, sizeof(turno), "11/0/%d/%s", numSala, nombreTurno);
						write(sockets_players[i], turno, strlen(turno));
						usleep(1000000);
					}
					if(sockets_players[i] == sock_conn){
						char turno[60];
						snprintf(turno, sizeof(turno), "11/1/%d/%s", numSala, nombreTurno);
						write(sock_conn, turno, strlen(turno));
						usleep(1000000);
					}

				}
				usleep(1000000);
				break;
			}

			case 12: {
				
				// Empieza la partida y generamos todas las apuestas y ciclar turnos
				int numSala;
				p = strtok(NULL, "/");
				numSala = atoi(p);

				//Generamos la apuesta incial. 

				float apuestaInicial = generarApuesta();
				printf("APUESTA INCIAL: %.2f", apuestaInicial);

				// Enviamos la apuesta a todos los jugadores de esa sala
				pthread_mutex_lock(&mutexLista);

				int sockets_players[4];
				ObtenerSocketsPlayersSala(&salas, numSala, sockets_players);
				char notificaciónSala[100];
				for(int i = 0; i<4; i++){
					
					if(sockets_players[i] != -1){
						snprintf(notificaciónSala, sizeof(notificaciónSala), "12/%.2f/%d", apuestaInicial, numSala);
						write(sockets_players[i], notificaciónSala, strlen(notificaciónSala));
						
					}
				}
				pthread_mutex_unlock(&mutexLista);
				usleep(1000000);
			}

			/*case 13: {

			
				int numSala;
				char apostante[40];
				float apuesta;
				p = strtok(NULL, "/");
				strcpy(apostante, p);
				p = strtok(NULL, "/");
				numSala = atoi(p);
				p = strtok(NULL, "/");
				apuesta = atof(p);


				// Ahora que tenemos todo esto, ponemos la apuesta en el jugador y ademas le restamos de su capital.

				/*
				1. Restar la apuesta en el capital de la apostante
				2. Seteamos su apuesta en su estructura de jugador
				3. Enviamos a todos los jugadores de la mesa si ha apostado o no y ademas al apostante su nuevo capital
				 
				
				float err = RestarCapital(conn, apostante, apuesta);
				printf("Capital total ya despues de restar: %.2f", err);

			}*/

			default:{
				printf("Comando no reconocido: %s\n", p);
				break;
			}
		}
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
	serv_adr.sin_port = htons(50057);
	
	
	
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



