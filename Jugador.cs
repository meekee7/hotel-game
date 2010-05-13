using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Jugador
    {
        public short n_billetes_50 = 0;
        public short n_billetes_100 = 0;
        public short n_billetes_500 = 0;
        public short n_billetes_1000 = 0;
        public short n_billetes_5000 = 0;
        public int dinero_total = 0;
        public Hotel[] hoteles;
        public Casilla posicion;

        public Tipos.Tcolor color;

        public Jugador (short n_50, short n_100, short n_500, short n_1000, short n_5000, Tipos.Tcolor color)
        {
            this.n_billetes_50 = n_50;
            this.n_billetes_100 = n_100;
            this.n_billetes_500 = n_500;
            this.n_billetes_1000 = n_1000;
            this.n_billetes_5000 = n_5000;
            this.color = color;
            this.hoteles = new Hotel[Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length];
            this.posicion = new Casilla (0);
            this.calcular_dinero_total ();
        }

        void calcular_dinero_total ()
        {
            this.dinero_total = (5000 * this.n_billetes_5000) + (1000 * this.n_billetes_1000) +
                                (500 * this.n_billetes_500) + (100 * this.n_billetes_100) +
                                (50 * this.n_billetes_50);
        }

        ~Jugador()
        {
            this.hoteles = null;
            this.posicion = null;
        }
    }
}