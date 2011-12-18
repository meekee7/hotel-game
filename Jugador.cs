using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Jugador
    {
        public int n_billetes_50 = 0;
        public int n_billetes_100 = 0;
        public int n_billetes_500 = 0;
        public int n_billetes_1000 = 0;
        public int n_billetes_5000 = 0;
        public int dinero_total = 0;
        public LinkedList <Hotel> hoteles;
        public int n_hoteles, n_jugador;
        public Casilla posicion;
        public Tipos.Tcolor color;
        public Boolean pago_ultimo_turno;
        public Boolean eliminado;

        public Jugador (int n_5000, int n_1000, int n_500, int n_100, int n_50, Tipos.Tcolor color, int n_jugador)
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
            this.pago_ultimo_turno = false;
            this.eliminado = false;
            this.n_jugador = n_jugador;
        }

        public void calcular_dinero_total ()
        {
            this.dinero_total = (5000 * this.n_billetes_5000) + (1000 * this.n_billetes_1000) +
                                (500 * this.n_billetes_500) + (100 * this.n_billetes_100) +
                                (50 * this.n_billetes_50);
        }

        public void Eliminar()
        {
            this.eliminado = true;
            foreach (Hotel hotel in this.hoteles)
                hotel.Devolver_a_banca();
            this.hoteles = null;
            this.n_hoteles = 0;
        }

        public Boolean Eliminado()
        {
            return this.eliminado;
        }

        public void Comprar_Hotel (ref Hotel hotel, int n5000, int n1000, int n500, int n100, int n50)
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

        public void Comprar_Hotel(ref Hotel hotel, ref Jugador dueño, int n5000, int n1000, int n500, int n100, int n50)
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
            dueño.n_billetes_5000 += n5000;
            dueño.n_billetes_1000 += n1000;
            dueño.n_billetes_500 += n500;
            dueño.n_billetes_100 += n100;
            dueño.n_billetes_50 += n50;
            dueño.calcular_dinero_total();
        }

        public void Pagar_Noches (ref Jugador al_jugador, int n5000, int n1000, int n500, int n100, int n50)
        {
            // Transfiere los fondos del jugador que paga al que cobra, el precio viene calculado de la IU
            this.n_billetes_5000 -= n5000;
            this.n_billetes_1000 -= n1000;
            this.n_billetes_500 -= n500;
            this.n_billetes_100 -= n100;
            this.n_billetes_50 -= n50;
            this.calcular_dinero_total();
            al_jugador.n_billetes_5000 += n5000;
            al_jugador.n_billetes_1000 += n1000;
            al_jugador.n_billetes_500 += n500;
            al_jugador.n_billetes_100 += n100;
            al_jugador.n_billetes_50 += n50;
            al_jugador.calcular_dinero_total();
        }

        public void Hotel_Expropiado (ref Hotel hotel)
        {
            this.hoteles.Remove(hotel);
            this.n_hoteles--;
            // El dueño será cambiado automáticamente por la IU
        }

        public void Devolver_cambio(int n_5000, int n_1000, int n_500, int n_100, int n_50)
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

        public void Pagar_Ampliacion_o_Entrada(int n5000, int n1000, int n500, int n100, int n50)
        {
            this.n_billetes_5000 -= n5000;
            this.n_billetes_1000 -= n1000;
            this.n_billetes_500 -= n500;
            this.n_billetes_100 -= n100;
            this.n_billetes_50 -= n50;
            this.calcular_dinero_total();
        }

        public void Quitar_5000_sin_tener_b5000(out int n_5000, out int n_1000, out int n_500, out int n_100, out int n_50)
        {
            n_5000 = 0;
            n_1000 = 0;
            n_500 = 0;
            n_100 = 0;
            n_50 = 0;

            int acumulado = 0;

            while (acumulado < 5000)
            {
                if (this.n_billetes_5000 > 0)
                {
                    acumulado += 5000;
                    n_5000++;
                    this.n_billetes_5000--;
                }
                else if (this.n_billetes_1000 > 0)
                {
                    acumulado += 1000;
                    n_1000++;
                    this.n_billetes_1000--;
                }
                else if (this.n_billetes_500 > 0)
                {
                    acumulado += 500;
                    n_500++;
                    this.n_billetes_500--;
                }
                else if (this.n_billetes_100 > 0)
                {
                    acumulado += 100;
                    n_100++;
                    this.n_billetes_100--;
                }
                else if (this.n_billetes_50 > 0)
                {
                    acumulado += 50;
                    n_50++;
                    this.n_billetes_50--;
                }
            }
        }

        public void Quitar_1000_sin_tener_b1000(out int n_1000, out int n_500, out int n_100, out int n_50)
        {
            n_1000 = 0;
            n_500 = 0;
            n_100 = 0;
            n_50 = 0;

            int acumulado = 0;

            while (acumulado < 1000)
            {
                if (this.n_billetes_1000 > 0)
                {
                    acumulado += 1000;
                    n_1000++;
                    this.n_billetes_1000--;
                }
                else if (this.n_billetes_500 > 0)
                {
                    acumulado += 500;
                    n_500++;
                    this.n_billetes_500--;
                }
                else if (this.n_billetes_100 > 0)
                {
                    acumulado += 100;
                    n_100++;
                    this.n_billetes_100--;
                }
                else if (this.n_billetes_50 > 0)
                {
                    acumulado += 50;
                    n_50++;
                    this.n_billetes_50--;
                }
                else // Caso para cuando no hay billetes disponibles y hay que cambiar los que hay por otros usando la banca
                {
                    if (this.n_billetes_5000 > 0)
                    {
                        this.n_billetes_5000--;
                        this.n_billetes_1000 += 5;
                    }
                }
            }
        }

        public void Quitar_500_sin_tener_b500(out int n_500, out int n_100, out int n_50)
        {
            n_500 = 0;
            n_100 = 0;
            n_50 = 0;

            int acumulado = 0;

            while (acumulado < 500)
            {
                if (this.n_billetes_500 > 0)
                {
                    acumulado += 500;
                    n_500++;
                    this.n_billetes_500--;
                }
                else if (this.n_billetes_100 > 0)
                {
                    acumulado += 100;
                    n_100++;
                    this.n_billetes_100--;
                }
                else if (this.n_billetes_50 > 0)
                {
                    acumulado += 50;
                    n_50++;
                    this.n_billetes_50--;
                }
                else // Caso para cuando no hay billetes disponibles y hay que cambiar los que hay por otros usando la banca
                {
                    if (this.n_billetes_1000 > 0)
                    {
                        this.n_billetes_1000--;
                        this.n_billetes_500 += 2;
                    }
                    else if (this.n_billetes_5000 > 0)
                    {
                        this.n_billetes_5000--;
                        this.n_billetes_1000 += 5;
                    }
                }
            }
        }

        public void Quitar_100_sin_tener_b100(out int n_100, out int n_50)
        {
            n_100 = 0;
            n_50 = 0;

            int acumulado = 0;

            while (acumulado < 100)
            {
                if (this.n_billetes_100 > 0)
                {
                    acumulado += 100;
                    n_100++;
                    this.n_billetes_100--;
                }
                else if (this.n_billetes_50 > 0)
                {
                    acumulado += 50;
                    n_50++;
                    this.n_billetes_50--;
                }
                else // Caso para cuando no hay billetes disponibles y hay que cambiar los que hay por otros usando la banca
                {
                    if (this.n_billetes_500 > 0)
                    {
                        this.n_billetes_500--;
                        this.n_billetes_100 += 5;
                    }
                    else if (this.n_billetes_1000 > 0)
                    {
                        this.n_billetes_1000--;
                        this.n_billetes_500 += 2;
                    }
                    else if (this.n_billetes_5000 > 0)
                    {
                        this.n_billetes_5000--;
                        this.n_billetes_1000 += 5;
                    }
                }
            }
        }

        public void Quitar_50_sin_tener_b50(out int n_50)
        {
            n_50 = 0;

            int acumulado = 0;

            while (acumulado < 50)
            {
                if (this.n_billetes_50 > 0)
                {
                    acumulado += 50;
                    n_50++;
                    this.n_billetes_50--;
                }
                else // Caso para cuando no hay billetes disponibles y hay que cambiar los que hay por otros usando la banca
                {
                    if (this.n_billetes_100 > 0)
                    {
                        this.n_billetes_100--;
                        this.n_billetes_50 += 2;
                    }
                    else if (this.n_billetes_500 > 0)
                    {
                        this.n_billetes_500--;
                        this.n_billetes_100 += 5;
                    }
                    else if (this.n_billetes_1000 > 0)
                    {
                        this.n_billetes_1000--;
                        this.n_billetes_500 += 2;
                    }
                    else if (this.n_billetes_5000 > 0)
                    {
                        this.n_billetes_5000--;
                        this.n_billetes_1000 += 5;
                    }
                }
            }
        }

        ~Jugador()
        {
            this.hoteles = null;
            this.posicion = null;
        }
    }
}