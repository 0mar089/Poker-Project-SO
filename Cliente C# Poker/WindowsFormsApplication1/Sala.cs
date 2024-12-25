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
using DeckCard;

namespace WindowsFormsApplication1 {
    public partial class Sala : Form {

        public PictureBox[] Imagenes = new PictureBox[9];

        List<string> cartasJugador;
        List<string> cartasComunitarias;

        public string usuario { get; set; }
        public Socket server;
        public int num_sala { get; set; }

        public bool IsHost;

        public Sala(string usuario , int num_sala , Socket server, string host) {
            InitializeComponent();

            this.usuario = usuario;
            this.server = server;
            this.num_sala = num_sala;

            if(this.usuario == host ) {
                this.IsHost = true;
            }
            else {
                this.IsHost = false;
            }
        }



        /*
         
        EN EL LOAD CARGAMOS: 
            - FOTOS Y SUS POSICIONES
            - BOTONES Y SUS POSICIONES
            - LOS LABELS CON EL DINERO Y NOMBRE DEL QUE JUEGA -> Consulta sql a la base de datos 

        Ya que la ventana de juego varia mucho y la hemos hecho fija, y los objetos tienen location dinámico
        por lo tanto tienes que elegir tu, donde va a ir cada objeto.

         */

        private void Sala_Load(object sender , EventArgs e) {

            StartBtn.Enabled = false;
            StartBtn.Hide();
            this.ClientSize = new Size(1200 , 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  // Impide que el formulario cambie de tamaño
            this.MaximizeBox = false;  // Desactiva el botón de maximizar

            Image backCardImage = Properties.Resources.back;

            // Crea un arreglo de PictureBox
            PictureBox[] pictureBoxes =
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                pictureBox6, pictureBox7, pictureBox9, pictureBox8
            };



            // Asigna la imagen a todos los PictureBox usando un bucle
            foreach ( PictureBox pb in pictureBoxes ) {
                pb.Image = backCardImage;
                pb.Size = new Size(80 , 115);
                pb.Anchor = AnchorStyles.None;
            }


            // Fijamos las cartas en el form, del 0 al 4 son comunitarias y 5,6 son del jugador y las demás oponente
            pictureBoxes[0].Location = new Point(338 , 282);
            pictureBoxes[1].Location = new Point(430 , 282);
            pictureBoxes[2].Location = new Point(523 , 282);
            pictureBoxes[3].Location = new Point(615 , 282);
            pictureBoxes[4].Location = new Point(708 , 282);

            pictureBoxes[5].Location = new Point(468 , 439);
            pictureBoxes[6].Location = new Point(569 , 439);

            pictureBoxes[7].Location = new Point(468 , 118);
            pictureBoxes[8].Location = new Point(569 , 118);
            



            CallButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            RaiseButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            FoldButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            CheckButton.Size = new Size(120 , 50);  // Tamaño fijo del botón

            CallButton.Location = new Point(1027 , 508);
            RaiseButton.Location = new Point(1027 , 612);
            FoldButton.Location = new Point(1027 , 560);
            CheckButton.Location = new Point(1027 , 456);


            player2Lbl.Location = new Point(469 , 58);
            player1Lbl.Location = new Point(469 , 592);

            this.Imagenes = pictureBoxes;

            player1Lbl.Text = this.usuario;

            if ( this.IsHost ) {
                StartBtn.Enabled = true;
                StartBtn.Show();
            }
        }


        public void SetCartas(string[] trozos) {
            cartasJugador = new List<string>();
            cartasComunitarias = new List<string>();
            for ( int i = 2, j = 5; i < 9; i++) {

                string elemento = trozos[i].Trim('\0');

                if ( elemento == "0" || string.IsNullOrEmpty(elemento) ) {
                    break;
                }

                if ( i <= 6 ) {

                    string cartaComunitaria = elemento;
                    
                    this.cartasComunitarias.Add(cartaComunitaria);
                }
                else {

                    string cartaJugador = elemento;
                    // Asignar imagen a PictureBox
                    this.cartasJugador.Add(cartaJugador);
                    SetImagenCarta(cartaJugador , j);
                    j++;
                }
            }
            //ya se han puesto todas las cartas, ahora avisamos de que empieza la partida
            MessageBox.Show("¡Empieza la partida!");
        }

        private void SetImagenCarta(string nombreCarta , int index) {

            try {
                var imageProperty = Properties.Resources.ResourceManager.GetObject(nombreCarta) as Image;
                if ( imageProperty != null ) {

                    Imagenes[index].Image = imageProperty;
                }
                else {
                    // Si no se encuentra la imagen, podrías manejar el error, por ejemplo asignando una imagen predeterminada
                    Imagenes[index].Image = Properties.Resources.back; // Imagen por defecto (o de carta invertida)
                }
            }
            catch ( Exception ex ) {
                MessageBox.Show("Error al cargar la imagen: " + ex.Message);
            }
        }



        public void SetNombres(string[] trozos, string usuario) {

            // recibo esto: 7/gente/numSala/Nombre1/Nombre2.../balance/0/0/0/0

            string nombre;

            // Empieza en el índice 3 donde comienzan los nombres
            for ( int i = 3; i < trozos.Length; i++ ) {
                // Limpia cualquier cadena con caracteres nulos y verifica si está vacía
                string elemento = trozos[i].Trim('\0');

                // Si el elemento es "0" o está vacío después de limpiar, termina el bucle
                if ( elemento == "0" || string.IsNullOrEmpty(elemento) ) {
                    break;
                }

                if(float.TryParse(elemento, out float numero) ) {
                    labelDynamicBalance.Text = elemento;
                    break;
                }

                // Asignamos el nombre actual
                nombre = elemento;

                // Compara con el usuario para asignar el nombre a los labels
                if ( nombre == usuario ) {
                    player1Lbl.Text = usuario;
                }
                else {
                    player2Lbl.Text = nombre;
                }
            }


        }

        private void Salir_Sala_Btn_Click(object sender, EventArgs e)
        {
            string mensaje = "10/" + this.usuario + "/" + this.num_sala;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            this.Close();
        }

        private void StartBtn_Click(object sender , EventArgs e) {
            string mensaje = "9/" + this.usuario + "/" + this.num_sala;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
    }
}
