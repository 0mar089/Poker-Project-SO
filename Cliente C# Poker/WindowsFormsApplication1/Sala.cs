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
using System.Xml.Linq;

namespace WindowsFormsApplication1 {
    public partial class Sala : Form {

        public PictureBox[] Imagenes = new PictureBox[9];

        List<string> cartasComunitarias;
        List<string> cartasJugador1;
        List<string> cartasJugador2;
        List<string> cartasJugador3;
        List<string> cartasJugador4;

        public string usuario { get; set; }
        public Socket server;
        public int num_sala { get; set; }

        public bool IsHost;

        public float Apuesta;

        int numJugadores;

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
                pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13
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


            pictureBoxes[7].Location = new Point(569 , 118);
            pictureBoxes[8].Location = new Point(468 , 118);

            pictureBoxes[9].Location = new Point(1071 , 258);
            pictureBoxes[10].Location = new Point(970 , 258);

            pictureBoxes[11].Location = new Point(138 , 266);
            pictureBoxes[12].Location = new Point(37 , 266);



            ApostarBtn.Location = new Point(1009 , 500);
            RetirarBtn.Location = new Point(1009, 580);


            player1Lbl.Location = new Point(469 , 592);
            player2Lbl.Location = new Point(469 , 58);
            player3Lbl.Location = new Point(979, 188);
            player4Lbl.Location = new Point(46 , 196);

            ApostarBtn.Enabled = false;
            RetirarBtn.Enabled = false;

            this.Imagenes = pictureBoxes;

            player1Lbl.Text = this.usuario;

            if ( this.IsHost ) {
                StartBtn.Enabled = true;
                StartBtn.Show();
            }
        }

        public void SetNumJugadores(int numJugadores) {
            this.numJugadores = numJugadores;
        }

        public void SetCartas(string[] trozos) {

            cartasComunitarias = new List<string>();
            cartasJugador1 = new List<string>();
            cartasJugador2 = null;
            cartasJugador3 = null;
            cartasJugador4 = null;


            int numJugadores = int.Parse(trozos[1]);
            if(numJugadores == 2 ) {

                cartasJugador2 = new List<string>();
            }
            else if(numJugadores == 3) {

                cartasJugador2 = new List<string>();
                cartasJugador3 = new List<string>();

            }
            else if ( numJugadores == 4 ) {

                cartasJugador2 = new List<string>();
                cartasJugador3 = new List<string>();
                cartasJugador4 = new List<string>();

            }

            string[] comunitarias = trozos[3].Split('/'); 

            int j = 0;
            for ( int i = 3; i < 8; i++, j++ ) {
                
                string cartaComunitaria = trozos[i].Trim('\0');
                this.cartasComunitarias.Add(cartaComunitaria);
                if ( i < 6 ) {
                    SetImagenCarta(cartaComunitaria , j);
                }
                
            }

            j = 5;
            int jugadorIndex = 0;
            for ( int i = 8; i < 8 + numJugadores*2; i++, j++ ) 
            {
                string cartaJugador = trozos[i].Trim('0'); // Cada jugador tiene sus cartas separadas por '/'

                if ( numJugadores == 2 || numJugadores == 1) {
                    // 2 jugadores. Primero vienen tus cartas y luego la de los demás en orden, de 1 a 4
                    if(i < 10 ) {
                        this.cartasJugador1.Add(cartaJugador);
                        SetImagenCarta(cartaJugador , j);
                    }
                    else {
                        this.cartasJugador2.Add(cartaJugador);
                    }
                }
                else if ( numJugadores == 3 ) {

                    if ( i < 10 ) {
                        this.cartasJugador1.Add(cartaJugador);
                        SetImagenCarta(cartaJugador , j);
                    }
                    else if (i == 10 || i == 11) {
                        this.cartasJugador2.Add(cartaJugador);
                    }
                    else {
                        this.cartasJugador3.Add(cartaJugador);
                    }

                }
                else if ( numJugadores == 4 ) {

                    if ( i < 10 ) {
                        this.cartasJugador1.Add(cartaJugador);
                        SetImagenCarta(cartaJugador , j);
                    }
                    else if ( i == 10 || i == 11 ) {
                        this.cartasJugador2.Add(cartaJugador);
                    }
                    else if ( i == 12 || i == 13){
                        this.cartasJugador3.Add(cartaJugador);
                    }
                    else {
                        this.cartasJugador4.Add(cartaJugador);
                    }
                }


                jugadorIndex++;
            }

            // Confirmación de que las cartas han sido asignadas
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


        public void SetGanador(string mensaje) {

            MessageBox.Show(mensaje);
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
