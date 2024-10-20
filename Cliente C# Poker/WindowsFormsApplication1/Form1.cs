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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button6.Visible = false;
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse(IP.Text);
            IPEndPoint ipep = new IPEndPoint(direc, 9130);
            

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }

        // Botón para registrar
        private void buttonRegister_Click_1(object sender, EventArgs e)
        {
            string mensaje = "REGISTER/" + nombre.Text + "/" + cuenta.Text + "/" + contraseña.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            byte[] msg2 = new byte[512];
            int bytesRecibidos = server.Receive(msg2);
            string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Trim('\0');  // Limpiar la respuesta

            if (respuesta == "REGISTERED") {
                MessageBox.Show("Registro exitoso.");
                button6.Visible = true;
            }
                
            else
                MessageBox.Show("Error en el registro.");
        }

        // Botón para iniciar sesión
        private void buttonLogin_Click_1(object sender, EventArgs e)
        {
            string mensaje = "LOGIN/" + cuenta.Text + "/" + contraseña.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            byte[] msg2 = new byte[512];
            int bytesRecibidos = server.Receive(msg2);
            string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Trim('\0');  // Limpiar la respuesta

            if (respuesta == "LOGGED_IN") {
                MessageBox.Show("Inicio de sesión exitoso.");
                button6.Visible = true;
            }
                
            else if (respuesta == "ERROR AL INSERTAR EL NUEVO USUARIO")
                MessageBox.Show("ERROR AL INSERTAR EL NUEVO USUARIO");
            else if (respuesta == "ERROR USUARIO CON LA MISMA CUENTA")
                MessageBox.Show("ERROR USUARIO CON LA MISMA CUENTA");
            else
                        MessageBox.Show("Error en el inicio de sesión.");
        }

        private void Desconectar_Click(object sender, EventArgs e)
        {
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Preparamos el mensaje para solicitar quién tiene más dinero
            string mensaje = "MAX_MONEY/";

            // Convertimos el mensaje a bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);

            // Enviamos el mensaje al servidor
            server.Send(msg);

            // Recibimos la respuesta del servidor
            byte[] buffer = new byte[512];
            int bytesRec = server.Receive(buffer);

            // Convertimos el buffer a string y mostramos la respuesta
            string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            MessageBox.Show("El jugador con más dinero es: " + respuesta);

            string mensaje_adios = "0/";

            byte[] msg_adios = System.Text.Encoding.ASCII.GetBytes(mensaje_adios);
            server.Send(msg_adios);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Preparamos el mensaje para solicitar en qué mesa ha ganado Luis
            string mensaje = "GANAR_LUIS/";

            // Convertimos el mensaje a bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);

            // Enviamos el mensaje al servidor
            server.Send(msg);

            // Recibimos la respuesta del servidor
            byte[] buffer = new byte[512];
            int bytesRec = server.Receive(buffer);

            // Convertimos el buffer a string y mostramos la respuesta
            string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            MessageBox.Show("Luis ha ganado en la(s) siguiente(s) mesa(s): " + respuesta);

            string mensaje_adios = "0/";

            byte[] msg_adios = System.Text.Encoding.ASCII.GetBytes(mensaje_adios);
            server.Send(msg_adios);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Preparamos el mensaje para solicitar la última vez que se jugó en la mesa 3
            string mensaje = "ULTIMA_JUGO_MESA3/";

            // Convertimos el mensaje a bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);

            // Enviamos el mensaje al servidor
            server.Send(msg);

            // Recibimos la respuesta del servidor
            byte[] buffer = new byte[512];
            int bytesRec = server.Receive(buffer);

            // Convertimos el buffer a string y mostramos la respuesta
            string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            MessageBox.Show("Última vez que se jugó en la mesa 3: " + respuesta);

            string mensaje_adios = "0/";

            byte[] msg_adios = System.Text.Encoding.ASCII.GetBytes(mensaje_adios);
            server.Send(msg_adios);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Preparamos el mensaje para solicitar el ganador de la última partida en la mesa 2
            string mensaje = "ULTIMO_GANADOR_MESA2/";

            // Convertimos el mensaje a bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);

            // Enviamos el mensaje al servidor
            server.Send(msg);

            // Recibimos la respuesta del servidor
            byte[] buffer = new byte[512];
            int bytesRec = server.Receive(buffer);

            // Convertimos el buffer a string y mostramos la respuesta
            string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            MessageBox.Show("Último ganador en la mesa 2: " + respuesta);
            if (server != null && server.Connected) {
                string mensaje_adios = "0/";

                byte[] msg_adios = System.Text.Encoding.ASCII.GetBytes(mensaje_adios);
                server.Send(msg_adios);

                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            else {
                MessageBox.Show("No estás conectado al servidor.");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Preparamos el mensaje para solicitar la lista de conectados
            string mensaje = "DAME_CONECTADOS/";

            // Convertimos el mensaje a bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);

            // Verificamos la conexión al servidor
            if (!server.Connected) {
                MessageBox.Show("No estás conectado al servidor.");
                return;
            }

            try {
                // Enviamos el mensaje al servidor
                server.Send(msg);

                // Recibimos la respuesta del servidor
                byte[] buffer = new byte[2048]; // Aumentamos el tamaño del buffer
                int bytesRec = server.Receive(buffer);

                // Convertimos el buffer a string y mostramos la respuesta
                string respuesta = Encoding.ASCII.GetString(buffer, 0, bytesRec).Trim('\0'); // Limpiar el string

                // Separa los nombres en una lista
                List<string> nombresConectados = respuesta.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                // Creamos una instancia del nuevo formulario "ListaConectados"
                ListaConectados listaConectadosForm = new ListaConectados();
                listaConectadosForm.SetConectados(nombresConectados);

                // Mostramos el nuevo formulario
                listaConectadosForm.Show();
            }
            catch (SocketException ex) {
                MessageBox.Show("Error al enviar o recibir el mensaje: " + ex.Message);
            }
        }

    }
}
