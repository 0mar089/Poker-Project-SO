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
using System.Xml.Linq;
using DeckCard;

namespace WindowsFormsApplication1 {
    public partial class Sala : Form {

        public PictureBox[] Imagenes = new PictureBox[9];

        List<string> cartasJugador1;
        List<string> cartasComunitarias;
        List<string> cartasJugador2;

        public string usuario { get; set; }
        public Socket server;
        public int num_sala { get; set; }

        public bool IsHost;

        public float Apuesta;

        public Sala(string usuario , int num_sala , Socket server , string host) {
            InitializeComponent();

            this.usuario = usuario;
            this.server = server;
            this.num_sala = num_sala;

            if ( this.usuario == host ) {
                this.IsHost = true;
            }
            else {
                this.IsHost = false;
            }
        }

        public event EventHandler CartasActualizadas;

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

            ApostarBtn.Location = new Point(1009 , 500);
            RetirarBtn.Location = new Point(1009, 580);


            player2Lbl.Location = new Point(469 , 58);
            player1Lbl.Location = new Point(469 , 592);
            ApostarBtn.Enabled = false;
            RetirarBtn.Enabled = false;

            this.Imagenes = pictureBoxes;

            player1Lbl.Text = this.usuario;

            if ( this.IsHost ) {
                StartBtn.Enabled = true;
                StartBtn.Show();
            }
        }


        public void SetCartas(string[] trozos) {
            cartasJugador1 = new List<string>();
            cartasComunitarias = new List<string>();
            cartasJugador2 = new List<string>();
            for ( int i = 2, j = 0; i < 11; i++ ) {

                string elemento = trozos[i].Trim('\0');

                if ( elemento == "0" || string.IsNullOrEmpty(elemento) ) {
                    break;
                }

                if ( i <= 6 ) {

                    string cartaComunitaria = elemento;

                    this.cartasComunitarias.Add(cartaComunitaria);
                    if ( i < 5 ) {
                        SetImagenCarta(cartaComunitaria , j);    
                    }
                    j++;

                }
                else if(7 <= i && i <= 8){

                    string cartaJugador = elemento;
                    // Asignar imagen a PictureBox
                    this.cartasJugador1.Add(cartaJugador);
                    SetImagenCarta(cartaJugador , j);
                    j++;
                }
                else {
                    string cartaJugador = elemento;
                    this.cartasJugador2.Add(cartaJugador);
                }
            }
            //ya se han puesto todas las cartas, ahora avisamos de que empieza la partida
            MessageBox.Show("¡Empieza la partida!");
            if ( this.IsHost ) {
                // Avisamos al server de que empieza la partida
                string mensaje = "11/" + this.num_sala;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            CartasActualizadas?.Invoke(this , EventArgs.Empty);
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



        public void SetNombres(string[] trozos , string usuario) {

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

                if ( float.TryParse(elemento , out float numero) ) {
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
        
        public void SetTurnoJugador(string personaTurno) {

            TurnoLbl.Text = $"Turno de: {personaTurno}";
            ApostarBtn.Enabled = true;
            RetirarBtn.Enabled = true;

            if ( this.Apuesta == 0 ) {
                // Enviamos otro mensaje al servidor para hacer que genere la apuesta inicial. 
                string mensaje = "12/" + this.num_sala;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

        }
        
        // Aquí como se tienen que esperar, no puedes usar los botones. 
        public void SetEsperaJugador(string personaTurno) {

            TurnoLbl.Text = $"Turno de: {personaTurno}";

            ApostarBtn.Enabled = false;
            RetirarBtn.Enabled = false;
            
        }
        
        public void SetNuevoBalance(string balance) {

            labelDynamicBalance.Text = balance;
        }

        public void QuitarTurnos() {
            TurnoLbl.Text = $"Turno de: ";
        }
        public void CalcularGanador() {

            PokerHand game = new PokerHand(this.cartasComunitarias);

            List<List<string>> cartas = new List<List<string>>();
            cartas.Add(this.cartasJugador1);
            cartas.Add(this.cartasJugador2);
            var (mejorJugador, resultado) = game.CompararManos(cartas);

            // SABEMOS YA EL GANADOR POR LO TANTO ENVIAMOS AL SERVIDOR QUIEN HA GANADO EN LA SALA
            string mensaje = "14/" + mejorJugador + "/" + this.num_sala + "/" + resultado ;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);


        }

        public void SetApuesta(float apuestaInical) {
            ApuestaLbl.Text = $"APUESTA: {apuestaInical}";
            this.Apuesta = apuestaInical;
        }


        public void SetTurnosRonda(int ronda, string nombreTurno) {

            if(this.usuario == nombreTurno ) {

                SetTurnoJugador(nombreTurno);
            }
            else {
                SetEsperaJugador(nombreTurno);
            }

            if(ronda == 0 ) {

                // Levantamos la 4 carta comunitaria

                string cartaComunitaria4 = this.cartasComunitarias[3];

                SetImagenCarta(cartaComunitaria4 , 3);

            }
            else {

                // Levantamos la 5 carta comunitaria

                string cartaComunitaria4 = this.cartasComunitarias[4];

                SetImagenCarta(cartaComunitaria4 , 4);

            }

        }




        // BOTONES
        private void Salir_Sala_Btn_Click(object sender , EventArgs e) {
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

        private void ApostarBtn_Click(object sender , EventArgs e) {

            string mensaje = "13/" + this.usuario + "/" + this.num_sala + "/" + this.Apuesta;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }
    }
}
