using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Tipos
    {
        public enum Tcolor { rojo, azul, amarillo, verde };
        public enum Tcasilla { salida, comprar, construir, entrada_gratis, fase_gratis };
        public enum Tnombre_hotel { Fujiyama, Boomerang, Letoile, President, Royal, Waikiki, Taj_Mahal, Safari, Ninguno };
        public enum Resultado_dado_cons { Permitido, Gratis, Doble, Denegado };
        // Para dibujar el coche, las entradas y los edificios de los hoteles correctamente sobre el tablero en cada casilla
        public struct Posicion
        {
            public float grados;
            public int X;
            public int Y;

            public void Establecer(float grados, int X, int Y)
            {
                this.grados = grados;
                this.X = X;
                this.Y = Y;
            }
        }
    }
}
