using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeckCard;

namespace WindowsFormsApplication1 {
    public partial class Sala : Form {

        public PictureBox[] Imagenes = new PictureBox[10];
        List<Carta> player1;
        List<Carta> player2;

        public Sala() {
            InitializeComponent();
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

            this.ClientSize = new Size(1200 , 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  // Impide que el formulario cambie de tamaño
            this.MaximizeBox = false;  // Desactiva el botón de maximizar

            Image backCardImage = Properties.Resources.back;

            // Crea un arreglo de PictureBox
            PictureBox[] pictureBoxes =
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11
            };



            // Asigna la imagen a todos los PictureBox usando un bucle
            foreach ( PictureBox pb in pictureBoxes ) {
                pb.Image = backCardImage;
                pb.Size = new Size(80 , 115);
                pb.Anchor = AnchorStyles.None;
            }


            // Fijamos las cartas en el form, del 1 al 5 son comunitarias y 6,7,8 son del jugador y las demás oponente
            pictureBoxes[0].Location = new Point(338 , 282);
            pictureBoxes[1].Location = new Point(430 , 282);
            pictureBoxes[2].Location = new Point(523 , 282);
            pictureBoxes[3].Location = new Point(615 , 282);
            pictureBoxes[4].Location = new Point(708 , 282);
            pictureBoxes[5].Location = new Point(429 , 439);
            pictureBoxes[6].Location = new Point(534 , 439);
            pictureBoxes[7].Location = new Point(628 , 439);
            pictureBoxes[8].Location = new Point(638 , 118);
            pictureBoxes[9].Location = new Point(534 , 118);
            pictureBoxes[10].Location = new Point(429 , 118);



            CallButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            RaiseButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            FoldButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            CheckButton.Size = new Size(120 , 50);  // Tamaño fijo del botón

            CallButton.Location = new Point(1027 , 508);
            RaiseButton.Location = new Point(1027 , 612);
            FoldButton.Location = new Point(1027 , 560);
            CheckButton.Location = new Point(1027 , 456);


            player1Label.Location = new Point(469 , 58);
            player2Label.Location = new Point(469 , 592);

            this.Imagenes = pictureBoxes;

        }







        /*
         ----------------------------------------------------------------------------------------------------------------
         Código del juego / sala. 

         COSAS QUE HAY QUE HACER: 

            2. Interfaz de los botones y del dinero
            3. Generar cartas random
            4. Actualizar labels y cambiarles la posición
            
         ----------------------------------------------------------------------------------------------------------------
         */



        // CODIGO DE PRUEBA DEL RANDOM PARA EL JUGADOR 1 Y 2: 

        private void randomCards_Click(object sender , EventArgs e) {

            Player players = new Player();
            this.player1 = players.GetPlayer1Hand();
            this.player2 = players.GetPlayer2Hand();

            // ASIGNAMOS LAS IMAGENES RANDOM EN LA INTERFAZ

            // PLAYER 1
            for(int i = 0, j = 5; i<3 && j<8; i++, j++) {
            
                string photoName = this.player1[i].ObtenerNombreImagen();
                var imageProperty = Properties.Resources.ResourceManager.GetObject(photoName) as Image;
                this.Imagenes[j].Image = imageProperty;
            }

            // PLAYER 2
            for ( int i = 0, j = 8; i < 3 && j < 11; i++, j++ ) {

                string photoName = this.player2[i].ObtenerNombreImagen();
                var imageProperty = Properties.Resources.ResourceManager.GetObject(photoName) as Image;
                this.Imagenes[j].Image = imageProperty;
            }



        }

    }
}
