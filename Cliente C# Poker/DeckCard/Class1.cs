using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DeckCard
{
    public class Carta {
        public string Palo { get; set; } // Ejemplo: "H = Hearts, C = Clubs, S = Spades y D = Diamonds"
        public string Valor { get; set; } // Ejemplo: "A, K, Q, J, ...., 2"

        public string carta;

        public Carta(string valor , string palo)
        {
            Valor = valor;
            Palo = palo;
            carta = valor + palo;
        }

        public string ObtenerNombreImagen() {

            // Verificar si el valor de la carta es un número (del 2 al 10)
            string nombreImagen;

            if ( int.TryParse(this.Valor , out _) )  // Si el valor es un número (por ejemplo, 2, 3, 4,...)
            {
                nombreImagen = $"_{this.Valor}{this.Palo}";  // Agrega _ solo si es un número
            }
            else {
                nombreImagen = $"{this.Valor}{this.Palo}";  // No agrega _ si es una letra (A, K, Q, J)
            }

            return nombreImagen;
        }

    }


    public class Mazo {


        private List<Carta> cartas;
        private List<Carta> cartasOrdenadas;

        public Mazo() {

            cartas = new List<Carta>();
            cartasOrdenadas = new List<Carta>();
            string[] palos = { "H" , "D" , "C" , "S" };
            string[] valores = { "2" , "3" , "4" , "5" , "6" , "7" , "8" , "9" , "10" , "J" , "Q" , "K" , "A" };


            // TE METE EN LA LISTA DE CARTAS TODAS LAS 52 COMBINACIONES DE CARTAS
            foreach ( string palo in palos ) {

                foreach ( string valor in valores ) {

                    cartas.Add(new Carta(valor , palo));
                    cartasOrdenadas.Add(new Carta(valor , palo));
                }
            }
        
        }

        // Mazo mezclado para repartir

        public void ShuffleMazo() {

            Random random = new Random();
            cartas = cartas.OrderBy(x => random.Next()).ToList();

        }

        public Carta GetRandomCard() {
            if ( cartas.Count == 0 )
                throw new InvalidOperationException("No cards left in the deck!");

            Random random = new Random();
            int index = random.Next(cartas.Count); // Genera un índice aleatorio basado en el tamaño del mazo
            Carta selectedCard = cartas[index];    // Selecciona la carta en ese índice
            cartas.RemoveAt(index);               // Elimina la carta del mazo
            return selectedCard;                 // Devuelve la carta seleccionada
        }


        public List<Carta> PlayerDeck() {

            List<Carta> MazoJugador = new List<Carta>();

            MazoJugador.Add(GetRandomCard());
            MazoJugador.Add(GetRandomCard());
            MazoJugador.Add(GetRandomCard());

            return MazoJugador;
        }

    }


    // Clase para el jugador solo para Cartas del juego

    public class Player {

        private Mazo mazoPartida;

        private List<Carta> player1Hand;
        private List<Carta> player2Hand;

        public Player() {

            mazoPartida = new Mazo(); 
            mazoPartida.ShuffleMazo(); 
            player1Hand = mazoPartida.PlayerDeck(); 
            player2Hand = mazoPartida.PlayerDeck();
        }

        public List<Carta> GetPlayer1Hand() {
            return this.player1Hand;
        }
        public List<Carta> GetPlayer2Hand() {
            return this.player2Hand;
        }
    }

}
