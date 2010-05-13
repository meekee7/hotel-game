using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Hotel
    {
        public Tipos.Tnombre_hotel nombre;
        public short precio, precio_expropiacion;
        public bool suelo_comprado;
        public short n_ampliaciones_max;
        public short n_ampliaciones_construidas;
        public short precio_entrada;
        public Jugador dueño;
        short[,] matriz_precios;
        short[] precios_ampliaciones;

        public Hotel (Tipos.Tnombre_hotel nombre)
        {
            this.nombre = nombre;
            this.suelo_comprado = false;
            this.n_ampliaciones_construidas = 0;
            this.dueño = null;
            switch (this.nombre)
            {
                case Tipos.Tnombre_hotel.Boomerang :
                    this.precio = 500;
                    this.precio_expropiacion = 250;
                    this.precio_entrada = 100;
                    this.n_ampliaciones_max = 2;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new short[2] { 1800, 250 };
                    this.matriz_precios = new short[2,6] { { 400, 800, 1200, 1600, 2000, 2400 },
                                                           { 600, 1200, 1800, 2400, 3000, 3600 } };
                    break;
                case Tipos.Tnombre_hotel.Fujiyama :
                    this.precio = 1000;
                    this.precio_expropiacion = 500;
                    this.precio_entrada = 100;
                    this.n_ampliaciones_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new short[4] { 2200, 1400, 1400, 500 };
                    this.matriz_precios = new short[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 200, 400, 600, 800, 1000, 1200 },
                                                            { 400, 800, 1200, 1600, 2000, 2400 } };
                    break;
                case Tipos.Tnombre_hotel.President :
                    this.precio = 3500;
                    this.precio_expropiacion = 1750;
                    this.precio_entrada = 250;
                    this.n_ampliaciones_max = 5;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new short[5] { 5000, 3000, 2250, 1750, 5000 };
                    this.matriz_precios = new short[5, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                            { 400, 800, 1200, 1600, 2000, 2400 },
                                                            { 600, 1200, 1800, 2400, 3000, 3600 },
                                                            { 800, 1600, 2400, 3200, 4000, 4800 },
                                                            { 1100, 2200, 3300, 4400, 5500, 6600 } };
                    break;
                case Tipos.Tnombre_hotel.Taj_Mahal :
                    this.precio = 1500;
                    this.precio_expropiacion = 750;
                    this.precio_entrada = 100;
                    this.n_ampliaciones_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new short[4] { 2400, 1000, 500, 1000 };
                    this.matriz_precios = new short[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 200, 400, 600, 800, 1000, 1200 },
                                                            { 300, 600, 900, 1200, 1500, 1800 } };
                    break;
                default: break;
            }
        }
    }
}
