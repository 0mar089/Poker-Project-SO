using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1 {
    public partial class Sala : Form {

        public PictureBox[] pictureBoxes = new PictureBox[11];

        public Sala()
        {
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

        private void Sala_Load(object sender , EventArgs e)
        {

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
            foreach (PictureBox pb in pictureBoxes) {
                pb.Image = backCardImage;
                pb.Size = new Size(80 , 115);
                pb.Anchor = AnchorStyles.None;
            }

            
            // Fijamos las cartas en el form
            pictureBox1.Location = new Point(338,282);
            pictureBox2.Location = new Point(430,282);
            pictureBox3.Location = new Point(523,282);
            pictureBox4.Location = new Point(615,282);
            pictureBox5.Location = new Point(708,282);
            pictureBox6.Location = new Point(429,439);
            pictureBox7.Location = new Point(534,439);
            pictureBox8.Location = new Point(628,439);
            pictureBox9.Location = new Point(638,118);
            pictureBox10.Location = new Point(534,118);
            pictureBox11.Location = new Point(429,118);



            CallButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            RaiseButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            FoldButton.Size = new Size(120 , 50);  // Tamaño fijo del botón
            CheckButton.Size = new Size(120 , 50);  // Tamaño fijo del botón

            CallButton.Location = new Point(1027 , 508); 
            RaiseButton.Location = new Point(1027 , 612);  
            FoldButton.Location = new Point(1027 , 560); 
            CheckButton.Location = new Point(1027 , 456); 

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





    }
}
