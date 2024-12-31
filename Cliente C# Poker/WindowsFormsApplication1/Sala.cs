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
            cartasJugador = new List<string>();
            cartasComunitarias = new List<string>();
            for ( int i = 2, j = 0; i < 9; i++ ) {

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
        //public class Jugador
        //{
        //    public string Nombre { get; set; }
        //    public List<Carta> Mano { get; set; } = new List<Carta>();
        //    public ManoPoker ManoValor { get; set; } // Para almacenar el valor de la mano evaluada
        //}
        //public class Carta
        //{
        //    public string Valor { get; set; }
        //    public string Palo { get; set; }
        //}
        //public enum ManoPoker
        //{
        //    AltaCarta,
        //    Par,
        //    DosPares,
        //    Trio,
        //    Escalera,
        //    Color,
        //    FullHouse,
        //    Poker,
        //    EscaleraColor,
        //    EscaleraReal
        //}
        //public List<Carta> CrearMazo()
        //{
        //    List<Carta> mazo = new List<Carta>();
        //    string[] valores = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        //    string[] palos = { "Corazones", "Diamantes", "Tréboles", "Picas" };

        //    foreach (string valor in valores)
        //    {
        //        foreach (string palo in palos)
        //        {
        //            mazo.Add(new Carta { Valor = valor, Palo = palo });
        //        }
        //    }

        //    // Barajar el mazo
        //    Random random = new Random();
        //    mazo = mazo.OrderBy(carta => random.Next()).ToList();

        //    return mazo;
        //}
        ////public void Repartircartas(List<Jugador> jugadores)
        ////{
        ////    // Crear un mazo barajado
        ////    List<Carta> mazo = CrearMazo();

        ////    // Repartición de cartas
        ////    int indiceMazo = 0;
        ////    foreach (Jugador jugador in jugadores)
        ////    {
        ////        for (int i = 0; i < 2; i++)
        ////        {
        ////            jugador.Mano.Add(mazo[indiceMazo]);
        ////            indiceMazo++;
        ////        }
        ////    }
        ////    // Actualizar la interfaz gráfica
        ////    for (int i = 0; i < jugadores.Count; i++)
        ////    {
        ////        ActualizarCartas(jugadores[i].Mano, i); // Pasar el índice del jugador para asignar las cartas a los PictureBox correspondientes
        ////    }
        ////}

        //public ManoPoker EvaluarMano(List<Carta> mano)
        //{
        //    // Ordenar las cartas por valor y palo para facilitar la comparación
        //    mano = mano.OrderBy(c => c.Valor).ThenBy(c => c.Palo).ToList();

        //    // Implementar la lógica para cada tipo de mano
        //    if (EsEscaleraReal(mano)) return ManoPoker.EscaleraReal;
        //    if (EsEscaleraColor(mano)) return ManoPoker.EscaleraColor;
        //    if (EsPoker(mano)) return ManoPoker.Poker;
        //    if (EsFullHouse(mano)) return ManoPoker.FullHouse;
        //    if (EsColor(mano)) return ManoPoker.Color;
        //    if (EsEscalera(mano)) return ManoPoker.Escalera;
        //    if (EsTrio(mano)) return ManoPoker.Trio;
        //    if (EsPar(mano)) return ManoPoker.Par;
        //    // Si ninguna mano más fuerte se encuentra, se considera una alta carta
        //    return ManoPoker.AltaCarta;
        //}
        //bool EsPar(List<Carta> mano)
        //{
        //    for (int i = 0; i < mano.Count - 1; i++)
        //    {
        //        if (mano[i].Valor == mano[i + 1].Valor)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //bool EsTrio(List<Carta> mano)
        //{
        //    for (int i = 0; i < mano.Count - 2; i++)
        //    {
        //        int contador = 1;
        //        for (int j = i + 1; j < mano.Count; j++)
        //        {
        //            if (mano[i].Valor == mano[j].Valor)
        //            {
        //                contador++;
        //            }
        //        }
        //        if (contador == 3)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //bool EsEscalera(List<Carta> mano)
        //{
        //    // Ordenamos las cartas por valor
        //    mano.Sort((a, b) => a.Valor.CompareTo(b.Valor));

        //    // Verificamos si las cartas son consecutivas
        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        if (GetValorNumerico(mano[i].Valor) - GetValorNumerico(mano[i - 1].Valor) != 1)
        //        {
        //            return false; // Si la diferencia entre dos cartas consecutivas no es 1, no es una escalera
        //        }
        //    }

        //    return true;
        //}

        //// Función auxiliar para obtener el valor numérico de una carta
        //int GetValorNumerico(string valor)
        //{
        //    switch (valor)
        //    {
        //        case "2": return 2;
        //        case "3": return 3;
        //        case "4": return 4;
        //        case "5": return 5;
        //        case "6": return 6;
        //        case "7": return 7;
        //        case "8": return 8;
        //        case "9": return 9;
        //        case "10": return 10;
        //        case "J": return 11;
        //        case "Q": return 12;
        //        case "K": return 13;
        //        case "A": return 14;
        //        default: return 0;
        //    }
        //}
        //// Diccionario para mapear los valores de las cartas a números
        //Dictionary<string, int> valoresCartas = new Dictionary<string, int>
        //{
        //    {"2", 2},
        //    {"3", 3},
        //    {"4", 4},
        //    {"5", 5},
        //    {"6", 6},
        //    {"7", 7},
        //    {"8", 8},
        //    {"9", 9},
        //    {"10", 10},
        //    {"J", 11},
        //    {"Q", 12},
        //    {"K", 13},
        //    {"A", 14}
        //};
        //bool EsColor(List<Carta> mano)
        //{
        //    // Tomamos el palo de la primera carta como referencia
        //    string paloReferencia = mano[0].Palo;

        //    // Iteramos sobre todas las cartas, excepto la primera
        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        // Si el palo de la carta actual es diferente al palo de referencia, no es un color
        //        if (mano[i].Palo != paloReferencia)
        //        {
        //            return false;
        //        }
        //    }

        //    // Si llegamos al final del bucle sin encontrar ningún palo diferente, es un color
        //    return true;
        //}
        //bool EsFullHouse(List<Carta> mano)
        //{
        //    // Ordenamos las cartas por valor para facilitar el conteo
        //    mano.Sort((a, b) => a.Valor.CompareTo(b.Valor));

        //    // Inicializamos variables para contar las ocurrencias de cada valor
        //    string valorAnterior = mano[0].Valor;
        //    int contador = 1;
        //    string valorTrio = "";
        //    string valorPar = "";

        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        if (mano[i].Valor == valorAnterior)
        //        {
        //            contador++;
        //        }
        //        else
        //        {
        //            // Si cambiamos de valor, verificamos si tenemos un trío o un par
        //            if (contador == 3)
        //            {
        //                valorTrio = valorAnterior;
        //            }
        //            else if (contador == 2)
        //            {
        //                valorPar = valorAnterior;
        //            }

        //            // Reseteamos el contador y actualizamos el valor anterior
        //            contador = 1;
        //            valorAnterior = mano[i].Valor;
        //        }
        //    }

        //    // Verificamos si encontramos un trío y un par
        //    return valorTrio != "" && valorPar != "";
        //}
        //bool EsPoker(List<Carta> mano)
        //{
        //    // Ordenamos las cartas por valor
        //    mano.Sort((a, b) => a.Valor.CompareTo(b.Valor));

        //    // Comparamos las cuatro primeras cartas para ver si son iguales
        //    return mano[0].Valor == mano[3].Valor;
        //}
        //bool EsEscaleraColor(List<Carta> mano)
        //{
        //    // Ordenamos las cartas por valor y palo
        //    mano.Sort((a, b) => a.Valor.CompareTo(b.Valor) != 0 ? a.Valor.CompareTo(b.Valor) : a.Palo.CompareTo(b.Palo));

        //    // Verificamos si todas las cartas tienen el mismo palo
        //    string paloReferencia = mano[0].Palo;
        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        if (mano[i].Palo != paloReferencia)
        //        {
        //            return false; // Si algún palo es diferente, no es una escalera de color
        //        }
        //    }

        //    // Verificamos si las cartas son consecutivas
        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        if (GetValorNumerico(mano[i].Valor) - GetValorNumerico(mano[i - 1].Valor) != 1)
        //        {
        //            return false; // Si la diferencia entre dos cartas consecutivas no es 1, no es una escalera
        //        }
        //    }

        //    return true; // Si pasamos todas las verificaciones, es una escalera de color
        //}
        //bool EsEscaleraReal(List<Carta> mano)
        //{
        //    // Ordenamos las cartas por valor y palo (As siempre es el valor más alto)
        //    mano.Sort((a, b) => a.Valor.CompareTo(b.Valor) != 0 ? a.Valor.CompareTo(b.Valor) : a.Palo.CompareTo(b.Palo));

        //    // Verificamos si el As es la primera carta y el 10 es la última
        //    if (mano[0].Valor != "A" || mano[4].Valor != "10")
        //    {
        //        return false;
        //    }

        //    // Verificamos si todas las cartas tienen el mismo palo
        //    string paloReferencia = mano[0].Palo;
        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        if (mano[i].Palo != paloReferencia)
        //        {
        //            return false;
        //        }
        //    }

        //    // Verificamos si las cartas son consecutivas (As es considerado el valor más alto)
        //    for (int i = 1; i < mano.Count; i++)
        //    {
        //        if (GetValorNumerico(mano[i].Valor) - GetValorNumerico(mano[i - 1].Valor) != 1)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
        public void SetTurnoJugador(string personaTurno) {

            TurnoLbl.Text = $"Turno de: {personaTurno}";
            ApostarBtn.Enabled = true;
            RetirarBtn.Enabled = true;

        }
        
        // Aquí como se tienen que esperar, no puedes usar los botones. 
        public void SetEsperaJugador(string personaTurno) {

            TurnoLbl.Text = $"Turno de: {personaTurno}";

            ApostarBtn.Enabled = false;
            RetirarBtn.Enabled = false;

            if(this.Apuesta == 0) {
                // Enviamos otro mensaje al servidor para hacer que genere la apuesta inicial. 
                string mensaje = "12/" + this.num_sala;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            
        }

        public void SetApuesta(float apuestaInical) {
            ApuestaLbl.Text = $"APUESTA: {apuestaInical}";
            this.Apuesta = apuestaInical;
        }
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
