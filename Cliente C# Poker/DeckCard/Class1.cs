using System;
using System.Collections.Generic;
using System.Linq;

namespace DeckCard {
    public class PokerHand {
        // Cartas comunitarias
        List<string> cartasComunitarias;
        // Función para convertir el valor de la carta en un valor numérico
        public int ConvertirValorCarta(char valor) {
            switch ( valor ) {
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'T': return 10; // 10 es 'T'
                case 'J': return 11;
                case 'Q': return 12;
                case 'K': return 13;
                case 'A': return 14;
                default: return 0;
            }
        }

        public PokerHand(List<string> communityCards) {
            this.cartasComunitarias = communityCards;
        }


        // Función para obtener el palo de la carta
        public char ObtenerPaloCarta(string carta) {
            // Si la carta empieza con '_', el palo está en la posición 2
            return carta[0] == '_' ? carta[2] : carta[1];
        }

        // Función para obtener los valores de las cartas (sólo los valores)
        public List<int> ObtenerValoresCartas(List<string> cartas) {
            // Si la carta empieza con '_', el valor está en la posición 1; de lo contrario, en la posición 0
            return cartas
                .Select(c => ConvertirValorCarta(c[0] == '_' ? c[1] : c[0]))
                .OrderBy(v => v)
                .ToList();
        }

        // Función para comprobar si hay una escalera
        public bool EsEscalera(List<int> valores) {
            // Las cartas deben estar en secuencia
            for ( int i = 0; i < valores.Count - 1; i++ ) {
                if ( valores[i] != valores[i + 1] - 1 ) {
                    return false;
                }
            }
            return true;
        }

        // Función para comprobar si hay un color (todas las cartas del mismo palo)
        public bool EsColor(List<string> cartas) {
            char palo = ObtenerPaloCarta(cartas[0]);
            foreach ( var carta in cartas ) {
                if ( ObtenerPaloCarta(carta) != palo ) {
                    return false;
                }
            }
            return true;
        }

        // Función para obtener las combinaciones de manos (por ejemplo, Pareja, Full House, etc.)
        public int EvaluarMano(List<string> cartas) {
            var valores = ObtenerValoresCartas(cartas);

            if ( EsColor(cartas) && EsEscalera(valores) ) {
                return 9; // Escalera de color (mejor mano)
            }
            if ( EsColor(cartas) ) {
                return 6; // Color
            }
            if ( EsEscalera(valores) ) {
                return 5; // Escalera
            }

            // Frecuencia de los valores
            var frecuenciaValores = valores.GroupBy(v => v).ToDictionary(g => g.Key , g => g.Count());

            // Aquí comparamos la frecuencia de los valores para detectar las manos
            if ( frecuenciaValores.Values.Contains(4) ) // Póker
            {
                return 8;
            }
            if ( frecuenciaValores.Values.Contains(3) && frecuenciaValores.Values.Contains(2) ) // Full House
            {
                return 7;
            }
            if ( frecuenciaValores.Values.Contains(3) ) // Trío
            {
                return 4;
            }
            if ( frecuenciaValores.Values.Count(v => v == 2) == 2 ) // Doble pareja
            {
                return 3;
            }
            if ( frecuenciaValores.Values.Contains(2) ) // Pareja
            {
                return 2;
            }

            return 1; // Carta alta (peor mano)
        }
        public string ObtenerDescripcionMano(int evaluacion) {
            switch ( evaluacion ) {
                case 9: return "Escalera de Color";
                case 8: return "Póker";
                case 7: return "Full House";
                case 6: return "Color";
                case 5: return "Escalera";
                case 4: return "Trío";
                case 3: return "Doble Pareja";
                case 2: return "Pareja";
                default: return "Carta Alta";
            }
        }
        // Función para comparar las manos de todos los jugadores
        public (int mejorJugador, string descripcionMano) CompararManos(List<List<string>> manosJugadores) {
            // Evaluar las manos de todos los jugadores
            List<int> puntajesManos = new List<int>();
            for ( int i = 0; i < manosJugadores.Count; i++ ) {
                // Combina las cartas del jugador con las comunitarias
                List<string> cartasJugador = manosJugadores[i].Concat(cartasComunitarias).ToList();
                // Evalúa la mano
                puntajesManos.Add(EvaluarMano(cartasJugador));
            }

            // Encuentra el índice del jugador con la mejor mano
            int mejorJugador = puntajesManos.IndexOf(puntajesManos.Max());

            // Obtener la descripción de la mano ganadora
            string descripcionMano = ObtenerDescripcionMano(puntajesManos[mejorJugador]);

            // Devuelve quién tiene la mejor mano y qué combinación tiene
            return (mejorJugador, $"El jugador {mejorJugador + 1} tiene la mejor mano: {descripcionMano}.");
        }
    }


    //namespace Pokercillo {
    //    internal class Program {
    //        public static void Main(string[] args) {

    //            JuegoPoker juego = new JuegoPoker();

    //            List<List<string>> manosJugadores = new List<List<string>>()
    //            {
    //                new List<string> { "_KC", "_10D" }, // Jugador 1
    //                new List<string> { "_KH", "_JC" }, // Jugador 2
    //                new List<string> { "_10H", "_9S" }, // Jugador 3
    //                //new List<string> { "_QS", "_8C" }  
    //                // Jugador 4 (si se quiere agregar más jugadores, simplemente descomenta esta línea)
    //            };

    //            // Comparar las manos y mostrar el resultado
    //            string resultado = juego.CompararManos(manosJugadores);
    //            Console.WriteLine(resultado);
    //        }
    //    }
    //}

}
