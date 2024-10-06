DROP DATABASE IF EXISTS PokerDB;
CREATE DATABASE PokerDB CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
use PokerDB;
CREATE TABLE Mesa (id_mesa INTEGER PRIMARY KEY NOT NULL, num_jug INTEGER NOT NULL);
CREATE TABLE Jugadores (
	id INTEGER PRIMARY KEY NOT NULL, 
	nombre TEXT NOT NULL,
	capital DECIMAL(30,2) NOT NULL,
	cuenta TEXT NOT NULL, 
	contrasenya TEXT NOT NULL);
CREATE TABLE Historial(
	id_mesa INTEGER NOT NULL, 
	date_id DATETIME NOT NULL,
	ganador INTEGER NOT NULL,
	FOREIGN KEY (id_mesa) REFERENCES Mesa(id_mesa), 
	FOREIGN KEY (ganador) REFERENCES Jugadores(id));

INSERT INTO Mesa VALUES(1, 4);
INSERT INTO Mesa VALUES(2, 3);
INSERT INTO Mesa VALUES(3,4);
INSERT INTO Jugadores VALUES(101, 'Juan', 3000, 'juan@gmail.com', 'contraseña');	 
INSERT INTO Jugadores VALUES(102, 'Maria', 2500, 'maria@gmail.com', '1234');
INSERT INTO Jugadores VALUES(103, 'Luis', 3500, 'luis@gmail.com', 'luis123');
INSERT INTO Historial VALUES(2, '2024-09-30 17:12:00', 103);
INSERT INTO Historial VALUES(3, '2024-10-06 10:12:00', 102);
INSERT INTO Historial VALUES(3, '2024-10-06 18:12:00', 102);