﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Globalization;
using System.Xml.Linq;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {

        Socket server;
        Thread atender;
        string usuario;
        string email;
        string password;
        int num_sala;
        List<Sala> salas = new List<Sala>();

        public Form1() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender , EventArgs e) {
            ButtonInvite.Enabled = false;
        }


        private void AtenderServidor() {
            while ( true ) {
                byte[] msg = new byte[1024];
                server.Receive(msg);
                string[] trozos = Encoding.ASCII.GetString(msg).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];

                switch ( codigo ) {
                    case 1:
                        // Respuesta al Register
                        if ( mensaje == "REGISTERED" ) {

                            MessageBox.Show("Registro Exitoso");
                            this.BackColor = Color.Green;
                            string ack = "15/";
                            byte[] msg2 = Encoding.ASCII.GetBytes(ack);
                            server.Send(msg2);

                        }
                        else {

                            MessageBox.Show("Registro Fallido");
                            Desconnect();
                        }
                        break;

                    case 2:
                        // Respuesta al Login
                        if ( mensaje == "LOGGED_IN" ) {

                            MessageBox.Show("Login Exitoso");
                            this.BackColor = Color.Green;
                            string ack = "15/";
                            byte[] msg2 = Encoding.ASCII.GetBytes(ack);
                            server.Send(msg2);
                        }
                        else {

                            MessageBox.Show("Login Fallido");
                            Desconnect();
                        }
                        break;

                    case 3: // Case para avisar al usuario si su cuenta aha sido eliminada o no

                        if ( mensaje == "ELIMINATED" ) {
                            MessageBox.Show("Usuario Eliminado Con Exito");
                            Desconnect();
                        }
                        else MessageBox.Show("Error al eliminar el usuario");

                        break;

                    case 4:


                        int numConectados;

                        // Verifica que el mensaje contiene un número válido de usuarios conectados
                        if ( !int.TryParse(mensaje , out numConectados) || numConectados < 0 ) {
                            MessageBox.Show("Datos inválidos recibidos del servidor.");
                            break;
                        }

                        DataTable dt = new DataTable();
                        dt.Columns.Add("Nombre");

                        for ( int i = 0; i < numConectados; i++ ) {
                            // Verifica que el índice existe en el array
                            if ( i + 2 < trozos.Length ) {
                                string Nombres = trozos[i + 2].Split('\0')[0];
                                dt.Rows.Add(Nombres);
                            }
                            else {
                                MessageBox.Show("Datos incompletos recibidos del servidor.");
                                break;
                            }
                        }

                        // Asignar el DataSource
                        this.Invoke((MethodInvoker) delegate {
                            dataGridViewConectados.DataSource = dt;
                            dataGridViewConectados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridViewConectados.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        });
                        break;

                    case 5:
                        // El mensaje recibido es del tipo: 5/n/name/Te ha invitado name

                        if ( mensaje == "2" ) {
                            if ( trozos.Length >= 4 ) // Verifica que el mensaje contiene al menos 3 partes
                            {
                                string nombre = trozos[2]; // Nombre del usuario que invita
                                //num_sala = Convert.ToInt32(trozos[3]);
                                // Muestra un MessageBox con botones "Sí" y "No"
                                DialogResult resultado = MessageBox.Show(
                                    $"Te ha invitado: {nombre}" , "" ,
                                    MessageBoxButtons.OKCancel // Botones Aceptar y Cnacelar
                                );

                                if ( resultado == DialogResult.OK ) {
                                    // Lógica para aceptar la invitación
                                    string mensaje_decision = "5/1/" + nombre + "/" + this.usuario + "/SI";
                                    byte[] msg2 = Encoding.ASCII.GetBytes(mensaje_decision);
                                    server.Send(msg2);


                                    MessageBox.Show("Has aceptado la invitación." , "Resultado");

                                    int numSalaInvitador = Convert.ToInt32(trozos[3]);
                                    string mensaje_unir_sala_invitador = "7/" + this.usuario + "/" + numSalaInvitador;
                                    byte[] msg5 = System.Text.Encoding.ASCII.GetBytes(mensaje_unir_sala_invitador);
                                    server.Send(msg5);
                                    textChat.Clear();

                                }
                                else if ( resultado == DialogResult.Cancel ) {
                                    // Lógica para rechazar la invitación
                                    string mensaje_decision = "5/1/" + nombre + "/" + this.usuario + "/NO";
                                    byte[] msg2 = Encoding.ASCII.GetBytes(mensaje_decision);
                                    server.Send(msg2);
                                    MessageBox.Show("Has rechazado la invitación." , "Resultado");
                                }
                            }
                            else {
                                // Manejo de error si el mensaje no tiene el formato esperado
                                MessageBox.Show("Mensaje de invitación incompleto recibido.");
                            }


                        }
                        else if ( mensaje == "1" ) {

                            string mensajeInvitacion = trozos[2]; // Nombre del usuario que invita
                            string respuesta = trozos[3]; // Su respuesta

                            if ( respuesta == "SI" ) {
                                DialogResult resultado = MessageBox.Show(
                                $" {mensajeInvitacion} ha aceptado tu invitación");
                            }
                            else if ( respuesta == "NO" ) {
                                DialogResult resultado = MessageBox.Show(
                                $" {mensajeInvitacion} ha rechazado tu invitación");
                            }

                        }
                        break;
                    case 6:

                        // Asignar el DataSource
                        this.Invoke((MethodInvoker) delegate {
                            Chat.Items.Add(trozos[1]);
                        });
                        break;


                    // CASE 7 Y 8 SON PARA EL NUMERO DE SALAS Y QUIEN SE INTENTA UNIR

                    case 7:

                        // Se recibe tipo numGente/numSala/nombre1/nombre2.../balanceDeLaPersona
                        int gente = Convert.ToInt32(mensaje);
                        int numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                        // Verificamos si la sala ya está abierta
                        Sala salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                        if ( salaExistente == null ) {
                            // Si no existe, es un nuevo jugador uniéndose
                            MessageBox.Show("Te has unido a la sala " + numSala);

                            // Actualizamos el label correspondiente según el número de sala
                            switch ( numSala ) {
                                case 1:
                                    label1.Text = $"{gente + 1}/4";
                                    break;
                                case 2:
                                    label5.Text = $"{gente + 1}/4";
                                    break;
                                case 3:
                                    label6.Text = $"{gente + 1}/4";
                                    break;
                                case 4:
                                    label7.Text = $"{gente + 1}/4";
                                    break;
                            }

                            // Creamos la sala y lanzamos el nuevo hilo
                            this.num_sala = numSala;
                            ThreadStart ts = delegate { EntrarSalaPoker(trozos); };
                            Thread t = new Thread(ts);
                            t.Start();
                        }
                        else {
                            // Si ya existe, simplemente actualizamos los nombres en la sala
                            salaExistente.SetNombres(trozos , this.usuario);
                        }

                        break;


                    case 8:

                        /*
                         Aqui recibimos el mensaje que tiene la gente que hay en las 4 salas divididas en / , Pero
                         lo que hay que hacer primero es que cada vez que alguien se una a una sala hay que cambiar la DB
                         en la tabla Mesa. 
                         */

                        int numGenteSala = Convert.ToInt32(mensaje);
                        label1.Text = $"{numGenteSala}/4";

                        for ( int i = 2; i < 5; i++ ) {
                            numGenteSala = Convert.ToInt32(trozos[i].Split('\0')[0]);
                            if ( i == 2 ) {
                                label5.Text = $"{numGenteSala}/4";
                            }
                            if ( i == 3 ) {
                                label6.Text = $"{numGenteSala}/4";
                            }
                            if ( i == 4 ) {
                                label7.Text = $"{numGenteSala}/4";
                            }
                        }
                        break;

                    case 9:
                        // se recibe tipo numSala/numJugadores/carta1/carta2/carta3/carta4/carta5/carta1/carta2/
                        // de la 1 a la 5 son comunitarias, y las pareja para un jugador ( solo tienes tus cartas )
                        numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);
                        int numJugadores = Convert.ToInt32(trozos[2].Split('\0')[0]);

                        salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);
                        salaExistente.SetCartas(trozos);

                        break;

                    case 10:
                        int num_Sala = Convert.ToInt32(mensaje);
                        int numpersonas = Convert.ToInt32(trozos[2].Split('\0')[0]);
                        string nombrejugador = trozos[3].Split('\0')[0];
                        switch ( num_Sala ) {
                            case 1:
                                label1.Text = $"{numpersonas}/4";
                                ButtonInvite.Enabled = false;
                                break;
                            case 2:
                                label5.Text = $"{numpersonas}/4";
                                ButtonInvite.Enabled = false;
                                break;
                            case 3:
                                label6.Text = $"{numpersonas}/4";
                                ButtonInvite.Enabled = false;
                                break;
                            case 4:
                                label7.Text = $"{numpersonas}/4";
                                ButtonInvite.Enabled = false;
                                break;
                        }
                        salaExistente = salas.FirstOrDefault(s => s.num_sala == num_Sala);
                        salaExistente.QuitarJugador(nombrejugador);

                        break;

                    case 11:

                        if ( mensaje == "1" ) {
                            // SI ES UN 1 ES QUE LE TOCA JUGAR AL JUGADOR

                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                            string personaTurno = trozos[3].Split('\0')[0];

                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                            salaExistente.SetTurnoJugador(personaTurno);

                        }
                        else {
                            // SI ES UN 0 ES QUE LE TOCA ESPERAR AL JUGADOR


                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                            string personaTurno = trozos[3].Split('\0')[0];

                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                            salaExistente.SetEsperaJugador(personaTurno);
                        }
                        break;

                    case 12:

                        float apuestaInicial = float.Parse(trozos[1] , CultureInfo.InvariantCulture);
                        numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                        salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                        salaExistente.SetApuesta(apuestaInicial);

                        break;

                    case 13: // ROTACIÓN DE TURNOS CUANDO LA GENTE APUESTA

                        if(mensaje == "1" ) {

                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                            string personaTurno = trozos[3].Split('\0')[0];

                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                            salaExistente.SetTurnoJugador(personaTurno);

                        }
                        else if(mensaje == "0" ) {

                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                            string personaTurno = trozos[3].Split('\0')[0];

                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                            salaExistente.SetEsperaJugador(personaTurno);
                            
                        }
                        else if(mensaje == "2" ) {

                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);

                            string personaTurno = trozos[3].Split('\0')[0];

                            string balanceNuevo = trozos[4].Trim('\0');

                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);

                            if (float.TryParse(balanceNuevo , out float numero) ){
                                salaExistente.SetNuevoBalance(balanceNuevo);
                            }

                            salaExistente.SetEsperaJugador(personaTurno);

                        }


                        break;

                    case 14: // CUANDO SE HA ACABADO LA PARTIDA Y TODAS LAS RONDAS

                        if(mensaje == "1" ) {
                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);
                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);
                            salaExistente.QuitarTurnos();
                            salaExistente.CalcularGanador();
                        }
                        else {

                            numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);
                            salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);
                            salaExistente.QuitarTurnos();

                        }

                        

                        break;

                    case 15: // ROTACIÓN DE RONDAS CUANDO SE ACABA UNA


                        numSala = Convert.ToInt32(trozos[2].Split('\0')[0]);
                        int ronda = Convert.ToInt32(trozos[3].Split('\0')[0]);
                        string nombreTurno = trozos[4].Split('\0')[0];
                        salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);
                        if(mensaje == "1") {
                            salaExistente.SetNuevoBalance(trozos[5].Split('\0')[0]);
                        }
                        salaExistente.SetTurnosRonda(ronda, nombreTurno);

                        break;

                    case 16: // Case para declarar el ganador de la sala

                        numSala = Convert.ToInt32(trozos[1].Split('\0')[0]);
                        salaExistente = salas.FirstOrDefault(s => s.num_sala == numSala);
                        salaExistente.SetGanador(trozos[2].Split('\0')[0]);

                        break;

                    case 17:

                        // Split de la cadena en trozos separados por "/"

                        // Configurar el DataGridView
                        dataGridViewHistorial.Columns.Clear();
                        dataGridViewHistorial.Rows.Clear();
                        dataGridViewHistorial.Columns.Add("Sala" , "SALA");
                        dataGridViewHistorial.Columns.Add("Fecha" , "FECHA");
                        dataGridViewConectados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridViewConectados.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                        for ( int i = 1; i < trozos.Length; i += 2 ) {
                            // Verificar si hemos alcanzado el final de la información
                            if ( trozos[i] == "0" || string.IsNullOrEmpty(trozos[i]) )
                                break;

                            // Asegurarse de que haya una pareja de sala/fecha
                            if ( i + 1 < trozos.Length ) {

                                string sala = trozos[i];
                                string fecha = trozos[i + 1];

                                // Agregar fila al DataGridView
                                dataGridViewHistorial.Rows.Add(sala , fecha);
                            }
                        }

                        break;



                    
                }

            }

        }



        private void EntrarSalaPoker(string[] trozos) {

            Sala s = new Sala(this.usuario , this.num_sala , server , trozos[3]);
            salas.Add(s);
            ButtonInvite.Enabled = true;
            s.SetNombres(trozos , this.usuario);
            s.ShowDialog();

        }

        // Botón para registrar
        private void buttonRegister_Click_1(object sender , EventArgs e) {
            try {
                // Verificamos que los campos no estén vacíos
                if ( string.IsNullOrWhiteSpace(nombre.Text) ) {
                    MessageBox.Show("El campo 'Nombre' es obligatorio." , "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
                    nombre.Focus();
                    return;
                }

                if ( string.IsNullOrWhiteSpace(cuenta.Text) ) {
                    MessageBox.Show("El campo 'Cuenta' es obligatorio." , "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
                    cuenta.Focus();
                    return;
                }

                if ( string.IsNullOrWhiteSpace(contraseña.Text) ) {
                    MessageBox.Show("El campo 'Contraseña' es obligatorio." , "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
                    contraseña.Focus();
                    return;
                }

                // Creamos un IPEndPoint con el IP del servidor y puerto del servidor al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc , 50059);

                // Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);
                try {
                    server.Connect(ipep); // Intentamos conectar el socket
                    MessageBox.Show("Conectado");
                }
                catch ( SocketException ex ) {
                    // Si hay excepción imprimimos error y salimos del programa con return
                    MessageBox.Show("No he podido conectar con el servidor");
                    this.Close();
                    return;
                }

                // Asignamos las variables y enviamos el mensaje al servidor
                this.email = cuenta.Text;
                this.password = contraseña.Text;
                this.usuario = nombre.Text;
                string mensaje = "1/" + nombre.Text + "/" + cuenta.Text + "/" + contraseña.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch ( Exception ex ) {
                MessageBox.Show("Error");
                Desconnect();
            }

            // Pongo en marcha el thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        // Botón para iniciar sesión
        private void buttonLogin_Click_1(object sender , EventArgs e) {
            try {
                // Verificamos que los campos no estén vacíos
                if ( string.IsNullOrWhiteSpace(cuenta.Text) ) {
                    MessageBox.Show("El campo 'Cuenta' es obligatorio." , "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
                    cuenta.Focus();
                    return;
                }

                if ( string.IsNullOrWhiteSpace(contraseña.Text) ) {
                    MessageBox.Show("El campo 'Contraseña' es obligatorio." , "Error" , MessageBoxButtons.OK , MessageBoxIcon.Error);
                    contraseña.Focus();
                    return;
                }

                // Creamos un IPEndPoint con el IP del servidor y puerto del servidor al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc , 50059);

                // Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);
                try {
                    server.Connect(ipep); // Intentamos conectar el socket
                    MessageBox.Show("Conectado");
                }
                catch ( SocketException ex ) {
                    // Si hay excepción imprimimos error y salimos del programa con return
                    MessageBox.Show("No he podido conectar con el servidor");
                    this.Close();
                    return;
                }

                // Asignamos las variables y enviamos el mensaje al servidor
                this.usuario = nombre.Text;
                this.email = cuenta.Text;
                this.password = contraseña.Text;

                string mensaje = "2/" + cuenta.Text + "/" + contraseña.Text;

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch ( Exception ex ) {
                MessageBox.Show("Error de servidor o no has puesto todos los campos");
                Desconnect();
            }

            // Pongo en marcha el thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }



        public void Desconnect() {

            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            server.Shutdown(SocketShutdown.Both);
            MessageBox.Show("Desconectando...");
            nombre.Text = "";
            cuenta.Text = "";
            contraseña.Text = "";

            this.BackColor = Color.Gray;
            server.Close();

            atender.Abort();
            
        }

        private void buttonInvite_Click(object sender , EventArgs e) {
            try {
                // Verifica que hay un usuario seleccionado en el DataGridView
                if ( dataGridViewConectados.CurrentRow == null ) {
                    MessageBox.Show("Selecciona un usuario para invitar.");
                    return;
                }

                // Obtén el nombre del usuario seleccionado
                string usuarioInvitado = dataGridViewConectados.CurrentRow.Cells[0].Value.ToString();

                // Envía la invitación al servidor
                string mensaje = "5/2/" + this.usuario + "/" + usuarioInvitado;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                MessageBox.Show($"Se ha enviado una invitacion a {usuarioInvitado}.");
            }
            catch ( Exception ex ) {
                MessageBox.Show("Error al enviar la invitación: " + ex.Message);
            }
        }

        private void Escribir_Click(object sender , EventArgs e) {
            string mensaje = "6/" + nombre.Text + "/" + textChat.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textChat.Clear();
        }


        // BOTON PARA SALIR
        private void button1_Click(object sender , EventArgs e) {
            Desconnect();
            Application.Exit();
        }

        private void contraseña_TextChanged(object sender , EventArgs e) {
            contraseña.PasswordChar = '*';
        }



        // BOTONES PARA UNIRSE A LAS SALAS, CADA UNO ENVIA EL MENSAJE PERO CAMBIA EL NUM DE SALA

        private void BtnSala1_Click(object sender , EventArgs e) {
            string mensaje = "7/" + this.usuario + "/1";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textChat.Clear();
        }

        private void BtnSala2_Click(object sender , EventArgs e) {
            string mensaje = "7/" + this.usuario + "/2";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textChat.Clear();
        }

        private void BtnSala3_Click(object sender , EventArgs e) {
            string mensaje = "7/" + this.usuario + "/3";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textChat.Clear();
        }

        private void BtnSala4_Click(object sender , EventArgs e) {
            string mensaje = "7/" + this.usuario + "/4";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textChat.Clear();
        }

        

        private void panel1_Click(object sender, EventArgs e)
        {
            if (!ButtonInvite.Enabled)
            {
                MessageBox.Show("Si quieres invitar a alguien, antes debes unirte a una sala.");
            }
            else
            {
                // Aquí puedes pasar el clic al botón si lo deseas.
                ButtonInvite.PerformClick();
            }
        }

        private void Baja_Click(object sender, EventArgs e)
        {
            string mensaje = "3/" + this.usuario + "/" + this.email + "/" + this.password;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void menuStrip1_ItemClicked(object sender , ToolStripItemClickedEventArgs e) {

        }
    }
}
