using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Juego
    {
        public int n_hoteles;
        public Hotel[] hoteles;
        public int n_jugadores;
        public int n_jugadores_activos;
        public Jugador[] jugadores;
        public Jugador banca;
        public Dado dado = new Dado(6);
        public int jug_inicial;
        public int jug_actual;
        public Jugador jugador_actual;
        public int ultimo_res_dado;
        public int ultimo_avance_auto;
        public Casilla[] casillas;
        public List<Tuple<String, String>> lista_jugadores_online;
        public List<String> estado_hoteles_online;
        public System.Threading.Semaphore sem_dado_cons;

        public Juego()
        {
            this.n_jugadores = 0;
            this.n_jugadores_activos = 0;
            this.n_hoteles = Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length - 1;
            this.hoteles = new Hotel[this.n_hoteles];
            Crear_Hoteles(ref this.hoteles);
            this.banca = new Jugador(5, 5, 5, 5, 5, Tipos.Tcolor.banca, -1);
            this.sem_dado_cons = new System.Threading.Semaphore(0, 1);
            // Creando casillas
            this.casillas = new Casilla[32];
            for (int i = 0 ; i < 32 ; i++)
            {
                this.casillas[i] = new Casilla(i);
            }
        }

        public static void Crear_Hoteles (ref Hotel[] hoteles)
        {
            foreach (Tipos.Tnombre_hotel nombre in Enum.GetValues(typeof(Tipos.Tnombre_hotel)))
            {
                // Creamos todos los hoteles definidos en Tnombre_hotel
                if (nombre != Tipos.Tnombre_hotel.Ninguno)
                {
                    hoteles[(int)nombre] = new Hotel(nombre);
                    switch (hoteles[(int)nombre].nombre.ToString())
                    {
                        case "Boomerang": hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Boomerang_tarjeta;
                            break;
                        case "President": hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.President_tarjeta;
                            break;
                        case "Royal":     hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Royal_tarjeta;
                            break;
                        case "Letoile":   hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Letoile_tarjeta;
                            break;
                        case "Fujiyama":  hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Fujiyama_tarjeta;
                            break;
                        case "Waikiki":   hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Waikiki_tarjeta;
                            break;
                        case "Taj_Mahal": hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Taj_Mahal_tarjeta;
                            break;
                        case "Safari":    hoteles[(int)nombre].img_tarjeta = global::Juego_Hotel.Properties.Resources.Safari_tarjeta;
                            break;
                    }
                }
            }
        }

        public void Cambiar_jugador_actual()
        {
            this.jugador_actual = this.jugadores[this.jug_actual - 1];
        }

        public void Eliminar_Jugador(Jugador jugador, Jugador jugador_que_cobra)
        {
            jugador.Eliminar();
            this.n_jugadores_activos--;
            if (jugador_que_cobra != null) // Hay que darle todo el dinero del jugador al jugador que cobra
                jugador.Pagar_Noches(ref jugador_que_cobra, jugador.n_billetes_5000, jugador.n_billetes_1000, jugador.n_billetes_500, jugador.n_billetes_100, jugador.n_billetes_50);
        }

        ~Juego()
        {
            this.hoteles = null;
            this.hoteles = null;
            this.casillas = null;
            this.jugadores = null;
            this.n_jugadores = 0;
            this.n_hoteles = 0;
        }
    }
}
