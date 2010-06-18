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
        public LinkedList <Hotel> hoteles;
        public short n_hoteles;
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
            this.hoteles = new LinkedList<Hotel>();
            this.n_hoteles = 0;
            this.posicion = new Casilla (0);
            this.calcular_dinero_total ();
        }

        public void calcular_dinero_total ()
        {
            this.dinero_total = (5000 * this.n_billetes_5000) + (1000 * this.n_billetes_1000) +
                                (500 * this.n_billetes_500) + (100 * this.n_billetes_100) +
                                (50 * this.n_billetes_50);
        }

        public void Comprar_Hotel (ref Hotel hotel, short n5000, short n1000, short n500, short n100, short n50)
        {
            // Se supone que el hotel no tenía dueño o es expropiable y que hay fondos, todo ya comprobado desde la IU
            this.hoteles.AddLast(hotel);
            this.n_hoteles++;
            hotel.dueño = this;
            this.n_billetes_5000 -= n5000;
            this.n_billetes_1000 -= n1000;
            this.n_billetes_500 -= n500;
            this.n_billetes_100 -= n100;
            this.n_billetes_50 -= n50;
            this.calcular_dinero_total();
        }

        public void Pagar_Noches (ref Jugador jugador, short n5000, short n1000, short n500, short n100, short n50)
        {
            // Transfiere los fondos del jugador que paga a los que cobra, el precio viene calculado de la IU
            jugador.n_billetes_5000 += n5000;
            jugador.n_billetes_1000 += n1000;
            jugador.n_billetes_500 += n500;
            jugador.n_billetes_100 += n100;
            jugador.n_billetes_50 += n50;
            jugador.calcular_dinero_total();
            this.n_billetes_5000 -= n5000;
            this.n_billetes_1000 -= n1000;
            this.n_billetes_500 -= n500;
            this.n_billetes_100 -= n100;
            this.n_billetes_50 -= n50;
            this.calcular_dinero_total();
        }

        public void Hotel_Expropiado (ref Hotel hotel)
        {
            this.hoteles.Remove(hotel);
            this.n_hoteles--;
            // El dueño será cambiado automáticamente por la IU
        }

        public void Devolver_cambio(short n_5000, short n_1000, short n_500, short n_100, short n_50)
        {
            this.n_billetes_5000 += n_5000;
            this.n_billetes_1000 += n_1000;
            this.n_billetes_500 += n_500;
            this.n_billetes_100 += n_100;
            this.n_billetes_50 += n_50;
            this.calcular_dinero_total();
        }

        public void Cobrar_Banco()
        {
            this.n_billetes_1000 += 2;
            this.calcular_dinero_total();
        }

        public void Pagar_Ampliacion(short n5000, short n1000, short n500, short n100, short n50)
        {
            this.n_billetes_5000 -= n5000;
            this.n_billetes_1000 -= n1000;
            this.n_billetes_500 -= n500;
            this.n_billetes_100 -= n100;
            this.n_billetes_50 -= n50;
            this.calcular_dinero_total();
        }

        ~Jugador()
        {
            this.hoteles = null;
            this.posicion = null;
        }
    }
}