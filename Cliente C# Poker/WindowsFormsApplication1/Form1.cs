using System;
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

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {

        Socket server;
        Thread atender;
        string usuario;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender , EventArgs e)
        {

        }


        private void AtenderServidor()
        {
            while (true) {
                byte[] msg = new byte[80];
                server.Receive(msg);
                string[] trozos = Encoding.ASCII.GetString(msg).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];

                switch (codigo) {
                    case 1:
                        // Respuesta al Register
                        if (mensaje == "REGISTERED") {

                            MessageBox.Show("Registro Exitoso");
                        }
                        else {

                            MessageBox.Show("Registro Fallido");
                        }
                        break;

                    case 2:
                        // Respuesta al Login
                        if (mensaje == "LOGGED_IN") {

                            MessageBox.Show("Login Exitoso");
                        }
                        else {

                            MessageBox.Show("Login Fallido");
                        }
                        break;

                    case 4:


                        int numConectados;

                        // Verifica que el mensaje contiene un número válido de usuarios conectados
                        if (!int.TryParse(mensaje , out numConectados) || numConectados < 0) {
                            MessageBox.Show("Datos inválidos recibidos del servidor.");
                            break;
                        }

                        DataTable dt = new DataTable();
                        dt.Columns.Add("Nombre");

                        for (int i = 0; i < numConectados; i++) {
                            // Verifica que el índice existe en el array
                            if (i + 2 < trozos.Length) {
                                string Nombres = trozos[i + 2].Split('\0')[0];
                                dt.Rows.Add(Nombres);
                            }
                            else {
                                MessageBox.Show("Datos incompletos recibidos del servidor.");
                                break;
                            }
                        }

                        // Asignar el DataSource
                        this.Invoke((MethodInvoker)delegate {
                            dataGridViewConectados.DataSource = dt;
                        });
                        break;

                    case 5:
                        // El mensaje recibido es del tipo: 5/name/Te ha invitado name

                        if (trozos.Length >= 3) // Verifica que el mensaje contiene al menos 3 partes
                        {
                            string nombre = trozos[1]; // Nombre del usuario que invita
                            string mensajeInvitacion = trozos[2].Split('\0')[0]; // Mensaje completo de invitación

                            // Muestra un mensaje en la interfaz
                            MessageBox.Show($"{mensajeInvitacion}" , $"Invitación recibida de {nombre}");
                        }
                        else {
                            // Manejo de error si el mensaje no tiene el formato esperado
                            MessageBox.Show("Mensaje de invitación incompleto recibido.");
                        }

                        break;

                    case 6:

                        // Asignar el DataSource
                        this.Invoke((MethodInvoker)delegate {
                            Chat.Items.Add(trozos[1]);
                        });
                        break;

                }

            }

        }


        // Botón para registrar
        private void buttonRegister_Click_1(object sender , EventArgs e)
        {
            try {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc , 50058);

                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);
                try {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado");

                }
                catch (SocketException ex) {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    this.Close();
                }

                string mensaje = "1/" + nombre.Text + "/" + cuenta.Text + "/" + contraseña.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch (Exception ex) {

                MessageBox.Show("Error");
                Desconnect();
            }
            // pongo en marcha en thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }




        // Botón para iniciar sesión
        private void buttonLogin_Click_1(object sender , EventArgs e)
        {
            try {

                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc , 50055);

                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);
                try {
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado");

                }
                catch (SocketException ex) {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    this.Close();
                }

                string mensaje = "2/" + cuenta.Text + "/" + contraseña.Text;
                this.usuario = nombre.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch (Exception ex) {
                MessageBox.Show("Error de servidor o no has puesto todos los campos");
                Desconnect();
            }
            // pongo en marcha en thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }


        public void Desconnect()
        {

            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);

            atender.Abort();
            MessageBox.Show("Desconectando...");

            this.BackColor = Color.Gray;
            server.Close();
        }

        private void buttonInvite_Click(object sender , EventArgs e)
        {
            try {
                // Verifica que hay un usuario seleccionado en el DataGridView
                if (dataGridViewConectados.CurrentRow == null) {
                    MessageBox.Show("Selecciona un usuario para invitar.");
                    return;
                }

                // Obtén el nombre del usuario seleccionado
                string usuarioInvitado = dataGridViewConectados.CurrentRow.Cells[0].Value.ToString();

                // Envía la invitación al servidor
                string mensaje = "5/" + this.usuario + "/" + usuarioInvitado;
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                MessageBox.Show($"Se ha enviado una invitacion a {usuarioInvitado}.");
            }
            catch (Exception ex) {
                MessageBox.Show("Error al enviar la invitación: " + ex.Message);
            }
        }

        private void Escribir_Click(object sender , EventArgs e)
        {
            string mensaje = "6/" + nombre.Text + "/" + textChat.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            textChat.Clear();
        }


        // BOTON PARA SALIR
        private void button1_Click(object sender , EventArgs e)
        {
            Desconnect();
            Application.Exit();
        }
    }
}
