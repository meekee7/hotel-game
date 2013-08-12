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
        public Boolean suelo_comprado;
        public Boolean entrada_comprada_ultimo_turno;
        public int n_fases_max;
        public int n_fases_construidas;
        public int precio_entrada;
        public int n_entradas;
        public Jugador dueño;
        int[,] matriz_precios;
        int[] precios_ampliaciones;
        public LinkedList<Casilla> entradas;
        public System.Drawing.Bitmap img_tarjeta;
        // Posiciones de las fases en el tablero
        public LinkedList<Tipos.Posicion> posiciones_fases;

        public Hotel (Tipos.Tnombre_hotel nombre)
        {
            this.nombre = nombre;
            this.suelo_comprado = false;
            this.entrada_comprada_ultimo_turno = false;
            this.n_fases_construidas = 0;
            this.n_entradas = 0;
            this.dueño = null;
            this.entradas = new LinkedList<Casilla>();
            this.posiciones_fases = new LinkedList<Tipos.Posicion>();
            Tipos.Posicion pos;
            switch (this.nombre)
            {
                case Tipos.Tnombre_hotel.Boomerang :
                    this.nombre_txt = "Boomerang";
                    this.precio = 500;
                    this.precio_expropiacion = 250;
                    this.precio_entrada = 100;
                    this.n_fases_max = 2;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[2] { 1800, 250 };
                    this.matriz_precios = new int[2,6] { { 400, 800, 1200, 1600, 2000, 2400 },
                                                           { 600, 1200, 1800, 2400, 3000, 3600 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 75, 68); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 44, 27); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.Fujiyama :
                    this.nombre_txt = "Fujiyama";
                    this.precio = 1000;
                    this.precio_expropiacion = 500;
                    this.precio_entrada = 100;
                    this.n_fases_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[4] { 2200, 1400, 1400, 500 };
                    this.matriz_precios = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 200, 400, 600, 800, 1000, 1200 },
                                                            { 400, 800, 1200, 1600, 2000, 2400 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 172, 200); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 139, 220); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 210, 215); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 136, 213); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.President :
                    this.nombre_txt = "President";
                    this.precio = 3500;
                    this.precio_expropiacion = 1750;
                    this.precio_entrada = 250;
                    this.n_fases_max = 5;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[5] { 5000, 3000, 2250, 1750, 5000 };
                    this.matriz_precios = new int[5, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                            { 400, 800, 1200, 1600, 2000, 2400 },
                                                            { 600, 1200, 1800, 2400, 3000, 3600 },
                                                            { 800, 1600, 2400, 3200, 4000, 4800 },
                                                            { 1100, 2200, 3300, 4400, 5500, 6600 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 623, 73); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 622, 117); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 573, 73); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 523, 73); // 3ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 600, 28); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.Taj_Mahal :
                    this.nombre_txt = "Taj Mahal";
                    this.precio = 1500;
                    this.precio_expropiacion = 750;
                    this.precio_entrada = 100;
                    this.n_fases_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[4] { 2400, 1000, 500, 1000 };
                    this.matriz_precios = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 200, 400, 600, 800, 1000, 1200 },
                                                            { 300, 600, 900, 1200, 1500, 1800 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 366, 462); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 337, 442); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 398, 442); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 493, 565); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.Waikiki :
                    this.nombre_txt = "Waikiki";
                    this.precio = 2500;
                    this.precio_expropiacion = 1250;
                    this.precio_entrada = 200;
                    this.n_fases_max = 6;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[6] { 3500, 2500, 2500, 1750, 1750, 2500 };
                    this.matriz_precios = new int[6, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                            { 350, 700, 1050, 1400, 1750, 2100 },
                                                            { 500, 1000, 1500, 2000, 2500, 3000 },
                                                            { 500, 1000, 1500, 2000, 2500, 3000 },
                                                            { 650, 1300, 1950, 2600, 3250, 3900 },
                                                            { 1000, 2000, 3000, 4000, 5000, 6000 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 486, 452); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 540, 467); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 579, 451); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 600, 424); // 3ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 620, 399); // 4ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 645, 603); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.Royal:
                    this.nombre_txt = "Royal";
                    this.precio = 2500;
                    this.precio_expropiacion = 1250;
                    this.precio_entrada = 200;
                    this.n_fases_max = 5;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[5] { 3600, 2600, 1800, 1800, 3000 };
                    this.matriz_precios = new int[5, 6] { { 150, 300, 450, 600, 750, 900 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 450, 900, 1350, 1800, 2250, 2700 },
                                                            { 600, 1200, 1800, 2400, 3000, 3600 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 488, 279); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 479, 308); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 443, 273); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 529, 299); // 3ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 641, 323); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.Safari:
                    this.nombre_txt = "Safari";
                    this.precio = 2000;
                    this.precio_expropiacion = 1000;
                    this.precio_entrada = 150;
                    this.n_fases_max = 4;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[4] { 2600, 1200, 1200, 2000 };
                    this.matriz_precios = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                            { 100, 200, 300, 400, 500, 600 },
                                                            { 250, 500, 750, 1000, 1250, 1500 },
                                                            { 500, 1000, 1500, 2000, 2500, 3000 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 76, 466); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 43, 422); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 43, 372); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 90, 563); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                case Tipos.Tnombre_hotel.Letoile:
                    this.nombre_txt = "L'etoile";
                    this.precio = 3000;
                    this.precio_expropiacion = 1500;
                    this.precio_entrada = 250;
                    this.n_fases_max = 6;
                    // Ahora a meter datos de precios específicos del hotel
                    this.precios_ampliaciones = new int[6] { 3300, 2200, 1800, 1800, 1800, 4000 };
                    this.matriz_precios = new int[6, 6] { { 150, 300, 450, 600, 750, 900 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 300, 600, 900, 1200, 1500, 1800 },
                                                            { 450, 900, 1350, 1800, 2250, 2700 },
                                                            { 750, 1500, 2250, 3000, 3750, 4500 } };
                    // Posiciones de las fases en el tablero (tantas como fases tenga)
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 352, 186); // Edif ppal
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 396, 230); // 1ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 291, 206); // 2ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 374, 275); // 3ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 288, 254); // 4ª ampliación
                    this.posiciones_fases.AddLast(pos);
                    pos = new Tipos.Posicion();
                    pos.Establecer(0, 300, 315); // Suelo
                    this.posiciones_fases.AddLast(pos);
                    break;
                default: break;
            }
        }

        public void Ampliar()
        {
            // Se presupone que sólo será llamado cuando se pueda ampliar, no se comprueba
            this.n_fases_construidas++;
            if (this.n_fases_construidas == this.n_fases_max)
                this.suelo_comprado = true;
        }

        public int Precio_Sig_Ampliacion()
        {
            return this.precios_ampliaciones[this.n_fases_construidas];
        }

        public int Precio_Ampliacion(int num)
        {
            return this.precios_ampliaciones[num];
        }

        public int Precio_Suelo()
        {
            return this.precios_ampliaciones[this.n_fases_max - 1];
        }

        public bool Ampliable()
        {
            return !this.suelo_comprado;
        }

        public int Calcular_noches(int cuantas)
        {
            return this.matriz_precios[this.n_fases_construidas-1, cuantas-1]; // Restamos para acceder correctamente a la matriz de precios
        }

        public void Devolver_a_banca()
        {
            // El hotel de queda con las entradas que tenía, solo hay que desasignarlo del dueño
            // Se dará la opción al recomprarlo con o sin entradas
            this.dueño = null;
        }
    }
}
