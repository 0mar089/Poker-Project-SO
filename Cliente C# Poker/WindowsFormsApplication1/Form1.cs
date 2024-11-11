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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
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

                        

                        int numConectados = Convert.ToInt32(mensaje);


                        DataTable dt = new DataTable();
                        dt.Columns.Add("Nombre");


                        for (int i = 0; i < numConectados; i++) {

                            string Nombres = trozos[i + 2].Split('\0')[0];
                            dt.Rows.Add(Nombres); 
                        }


                        dataGridViewConectados.DataSource = dt;

                        break;

                }

            }
                
        }

        // CONECTARSE AL SERVIDOR

        private void Conectar_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse(IP.Text);
            IPEndPoint ipep = new IPEndPoint(direc, 1300);
            

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
                string mensaje = "1/" + nombre.Text + "/" + cuenta.Text + "/" + contraseña.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch(Exception ex) {

                MessageBox.Show("Error");
                Desconnect();
            }
            // pongo en marcha en thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        // Botón para iniciar sesión
        private void buttonLogin_Click_1(object sender, EventArgs e)
        {
            try {
                string mensaje = "2/" + cuenta.Text + "/" + contraseña.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch(Exception ex) {
                MessageBox.Show("Error");
                Desconnect();
            }
            // pongo en marcha en thread que atenderá los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void Desconectar_Click(object sender, EventArgs e)
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
    }
}
