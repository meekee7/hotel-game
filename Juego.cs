using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Juego
    {
        public short n_hoteles;
        public Hotel[] hoteles;
        public short n_jugadores;
        public Jugador[] jugadores;
        public Jugador banca;
        public Dado dado = new Dado(6);
        public Dado_construccion dado_cons = new Dado_construccion();
        public short jug_inicial;
        public short jug_actual;
        public Jugador jugador_actual;
        public short ultimo_res_dado;
        public Casilla[] casillas;

        public Juego()
        {
            this.n_jugadores = 0;
            this.n_hoteles = (short) (Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length - 1);
            this.hoteles = new Hotel[n_hoteles];
            foreach (Tipos.Tnombre_hotel nombre in Enum.GetValues(typeof(Tipos.Tnombre_hotel)))
            {
                // Creamos todos los hoteles definidos en Tnombre_hotel
                if (nombre != Tipos.Tnombre_hotel.Ninguno)
                    hoteles[(int) nombre] = new Hotel (nombre);
            }
            this.banca = new Jugador(5, 5, 5, 5, 5, Tipos.Tcolor.banca);
            // Creando casillas
            this.casillas = new Casilla[32];
            for (short i = 0 ; i < 32 ; i++)
            {
                this.casillas[i] = new Casilla(i);
            }
        }

        public void Cambiar_jugador_actual()
        {
            this.jugador_actual = this.jugadores[this.jug_actual - 1];
        }

        ~Juego()
        {
            this.hoteles = null;
            this.n_hoteles = 0;
            this.hoteles = null;
            this.n_jugadores = 0;
            this.jugadores = null;
            this.casillas = null;
        }
    }
}
