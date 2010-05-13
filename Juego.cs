using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Juego
    {
        public Tablero tablero;
        public short n_hoteles;
        public Hotel[] hoteles;
        public short n_jugadores;
        public Jugador[] jugadores;
        public Jugador banca;
        public Dado dado = new Dado(6);
        public short jug_inicial;

        public Juego()
        {
            this.tablero = new Tablero ();
            this.n_jugadores = 0;
            this.n_hoteles = (short) Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length;
            hoteles = new Hotel[n_hoteles];
            foreach(Tipos.Tnombre_hotel nombre in Enum.GetValues(typeof(Tipos.Tnombre_hotel)))
            {
                // Creamos todos los hoteles definidos en Tnombre_hotel
                hoteles[(int) nombre] = new Hotel (nombre);
            }
            banca = new Jugador(5, 5, 5, 5, 5, Tipos.Tcolor.banca);
        }

        ~Juego()
        {
            this.hoteles = null;
            this.tablero = null;
            this.n_hoteles = 0;
            this.hoteles = null;
            this.n_jugadores = 0;
            this.jugadores = null;
        }
    }
}
