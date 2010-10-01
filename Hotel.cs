using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Hotel
    {
        public Tipos.Tnombre_hotel nombre;
        public String nombre_txt;
        public int precio, precio_expropiacion;
        public bool suelo_comprado;
        public int n_ampliaciones_max;
        public int n_ampliaciones_construidas;
        public int precio_entrada;
        public Jugador dueño;
        int[,] matriz_precios;
        int[] precios_ampliaciones;
        public System.Drawing.Bitmap img_tarjeta;

        public Hotel (Tipos.Tnombre_hotel nombre)
        {
            this.nombre = nombre;
            this.suelo_comprado = false;
            this.n_ampliaciones_construidas = 0;
            this.dueño = null;
            switch (this.nombre)
            {
                case Tipos.Tnombre_hotel.Boomerang :
                    this.nombre_txt = "Boomerang";
                    this.precio = 500;
                    this.precio_expropiacion = 250;
                    this.precio_entrada = 100;
                    this.n_ampliaciones_max = 2;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[2] { 1800, 250 };
                    this.matriz_precios = new int[2,6] { { 400, 800, 1200, 1600, 2000, 2400 },
                                                           { 600, 1200, 1800, 2400, 3000, 3600 } };
                    break;
                case Tipos.Tnombre_hotel.Fujiyama :
                    this.nombre_txt = "Fujiyama";
                    this.precio = 1000;
                    this.precio_expropiacion = 500;
                    this.precio_entrada = 100;
                    this.n_ampliaciones_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[4] { 2200, 1400, 1400, 500 };
                    this.matriz_precios = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 200, 400, 600, 800, 1000, 1200 },
                                                            { 400, 800, 1200, 1600, 2000, 2400 } };
                    break;
                case Tipos.Tnombre_hotel.President :
                    this.nombre_txt = "President";
                    this.precio = 3500;
                    this.precio_expropiacion = 1750;
                    this.precio_entrada = 250;
                    this.n_ampliaciones_max = 5;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[5] { 5000, 3000, 2250, 1750, 5000 };
                    this.matriz_precios = new int[5, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                            { 400, 800, 1200, 1600, 2000, 2400 },
                                                            { 600, 1200, 1800, 2400, 3000, 3600 },
                                                            { 800, 1600, 2400, 3200, 4000, 4800 },
                                                            { 1100, 2200, 3300, 4400, 5500, 6600 } };
                    break;
                case Tipos.Tnombre_hotel.Taj_Mahal :
                    this.nombre_txt = "Taj Mahal";
                    this.precio = 1500;
                    this.precio_expropiacion = 750;
                    this.precio_entrada = 100;
                    this.n_ampliaciones_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[4] { 2400, 1000, 500, 1000 };
                    this.matriz_precios = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 200, 400, 600, 800, 1000, 1200 },
                                                            { 300, 600, 900, 1200, 1500, 1800 } };
                    break;
                case Tipos.Tnombre_hotel.Waikiki :
                    this.nombre_txt = "Waikiki";
                    this.precio = 2500;
                    this.precio_expropiacion = 1250;
                    this.precio_entrada = 200;
                    this.n_ampliaciones_max = 6;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[6] { 3500, 2500, 2500, 1750, 1750, 2500 };
                    this.matriz_precios = new int[6, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                            { 350, 700, 1050, 1400, 1750, 2100 },
                                                            { 500, 1000, 1500, 2000, 2500, 3000 },
                                                            { 500, 1000, 1500, 2000, 2500, 3000 },
                                                            { 650, 1300, 1950, 2600, 3250, 3900 },
                                                            { 1000, 2000, 3000, 4000, 5000, 6000 } };
                    break;
                case Tipos.Tnombre_hotel.Royal:
                    this.nombre_txt = "Royal";
                    this.precio = 2500;
                    this.precio_expropiacion = 1250;
                    this.precio_entrada = 200;
                    this.n_ampliaciones_max = 5;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[5] { 3600, 2600, 1800, 1800, 3000 };
                    this.matriz_precios = new int[5, 6] { { 150, 300, 450, 600, 750, 900 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 450, 900, 1350, 1800, 2250, 2700 },
                                                            { 600, 1200, 1800, 2400, 3000, 3600 } };
                    break;
                case Tipos.Tnombre_hotel.Safari:
                    this.nombre_txt = "Safari";
                    this.precio = 2000;
                    this.precio_expropiacion = 1000;
                    this.precio_entrada = 150;
                    this.n_ampliaciones_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[4] { 2600, 1200, 1200, 2000 };
                    this.matriz_precios = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 250, 500, 750, 1000, 1250, 1500 },
                                                            { 500, 1000, 1500, 2000, 2500, 3000 } };
                    break;
                case Tipos.Tnombre_hotel.Letoile:
                    this.nombre_txt = "L'etoile";
                    this.precio = 3000;
                    this.precio_expropiacion = 1500;
                    this.precio_entrada = 250;
                    this.n_ampliaciones_max = 6;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[6] { 3300, 2200, 1800, 1800, 1800, 4000 };
                    this.matriz_precios = new int[6, 6] { { 150, 300, 450, 600, 750, 900 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 450, 900, 1350, 1800, 2250, 2700 },
                                                            { 750, 1500, 2250, 3000, 3750, 4500 } };
                    break;
                default: break;
            }
        }

        public void Ampliar()
        {
            // Se presupone que sólo será llamado cuando se pueda ampliar, no se comprueba
            this.n_ampliaciones_construidas++;
            if (this.n_ampliaciones_construidas == this.n_ampliaciones_max)
                this.suelo_comprado = true;
        }

        public int Precio_Sig_Ampliacion()
        {
            return this.precios_ampliaciones[this.n_ampliaciones_construidas];
        }

        public bool Ampliable()
        {
            return !this.suelo_comprado;
        }

        public int Calcular_noches(int cuantas)
        {
            return this.matriz_precios[this.n_ampliaciones_construidas, cuantas];
        }
    }
}
