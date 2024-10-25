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
            IPEndPoint ipep = new IPEndPoint(direc, 9300);
            

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
            try {
                string mensaje = "REGISTER/" + nombre.Text + "/" + cuenta.Text + "/" + contraseña.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] msg2 = new byte[512];
                int bytesRecibidos = server.Receive(msg2);
                string respuesta = Encoding.ASCII.GetString(msg2 , 0 , bytesRecibidos).Trim('\0');  // Limpiar la respuesta

                if (respuesta == "REGISTERED") {
                    MessageBox.Show("Registro exitoso.");
                    button6.Visible = true;
                }

                else
                    MessageBox.Show("Error en el registro.");
            }
            catch(Exception ex) {

                MessageBox.Show("Error");
                Desconnect();
            }
        }

        // Botón para iniciar sesión
        private void buttonLogin_Click_1(object sender, EventArgs e)
        {
            try {
                string mensaje = "LOGIN/" + cuenta.Text + "/" + contraseña.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] msg2 = new byte[512];
                int bytesRecibidos = server.Receive(msg2);
                string respuesta = Encoding.ASCII.GetString(msg2 , 0 , bytesRecibidos).Trim('\0');  // Limpiar la respuesta

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
            catch(Exception ex) {
                MessageBox.Show("Error");
                Desconnect();
            }
        }

        private void Desconectar_Click(object sender, EventArgs e)
        {
            try {
                string mensaje = "0/";

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            catch(Exception ex) {
                MessageBox.Show("Desconectado...");
                this.BackColor = Color.Gray;
            }
            
          
        }

        public void Desconnect()
        {
            try {
                string mensaje = "0/";

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            catch (Exception ex) {
                MessageBox.Show("Desconectado...");
                this.BackColor = Color.Gray;
            }
        }



        private void button6_Click(object sender , EventArgs e)
        {
            try {

                string mensaje = "DAME_CONECTADOS/";

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);

                if (!server.Connected) {
                    MessageBox.Show("No estás conectado al servidor.");
                    return;
                }

                server.Send(msg);

                byte[] buffer = new byte[2048]; 
                int bytesRec = server.Receive(buffer);

                string respuesta = Encoding.ASCII.GetString(buffer , 0 , bytesRec).Trim('\0'); // Limpiar el string

                // Separa los nombres en un array de strings
                string[] nombresConectados = respuesta.Split(new[] { '/' } , StringSplitOptions.RemoveEmptyEntries);

                DataTable dt = new DataTable();

                dt.Columns.Add("Nombre");

                if(nombresConectados.Length == 0) {
                    MessageBox.Show("No hay gente conectada");
                }
                else {
                    for(int i = 0; i< nombresConectados.Length; i++) {
                        dt.Rows.Add(nombresConectados[i]);
                    }
                    dataGridViewConectados.DataSource = dt;
                }

            }
            catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message);
                Desconnect(); 
            }
        }


    }
}
