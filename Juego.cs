using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Juego_Hotel
{
    public class Juego
    {
        public int n_hoteles;
        public Hotel[] hoteles;
        public int n_jugadores;
        public int n_jugadores_activos;
        public Jugador[] jugadores;
        public Dado dado = new Dado(6);
        public int jug_inicial;
        public int jug_actual;
        public Jugador jugador_actual;
        public int ultimo_res_dado;
        public int ultimo_avance_auto;
        public Casilla[] casillas;
        public List<Tuple<String, String>> lista_jugadores_online;
        public List<String> estado_hoteles_online;
        public Semaphore sem_dado_cons;

        public Juego()
        {
            n_jugadores = 0;
            n_jugadores_activos = 0;
            n_hoteles = Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length - 1;
            hoteles = new Hotel[n_hoteles];
            Crear_Hoteles(ref hoteles);
            sem_dado_cons = new Semaphore(0, 1);
            // Creando casillas
            casillas = new Casilla[32];
            for (int i = 0 ; i < 32 ; i++)
            {
                casillas[i] = new Casilla(i);
            }
        }

        public static void Crear_Hoteles (ref Hotel[] hoteles)
        {
            foreach (Tipos.Tnombre_hotel nombre in
                Enum.GetValues(typeof(Tipos.Tnombre_hotel)).Cast<Tipos.Tnombre_hotel>().Where(nombre => nombre != Tipos.Tnombre_hotel.Ninguno))
            {
                hoteles[(int)nombre] = new Hotel(nombre);
                switch (hoteles[(int)nombre].nombre.ToString())
                {
                    case "Boomerang": hoteles[(int)nombre].img_tarjeta = Properties.Resources.Boomerang_tarjeta;
                        break;
                    case "President": hoteles[(int)nombre].img_tarjeta = Properties.Resources.President_tarjeta;
                        break;
                    case "Royal":     hoteles[(int)nombre].img_tarjeta = Properties.Resources.Royal_tarjeta;
                        break;
                    case "Letoile":   hoteles[(int)nombre].img_tarjeta = Properties.Resources.Letoile_tarjeta;
                        break;
                    case "Fujiyama":  hoteles[(int)nombre].img_tarjeta = Properties.Resources.Fujiyama_tarjeta;
                        break;
                    case "Waikiki":   hoteles[(int)nombre].img_tarjeta = Properties.Resources.Waikiki_tarjeta;
                        break;
                    case "Taj_Mahal": hoteles[(int)nombre].img_tarjeta = Properties.Resources.Taj_Mahal_tarjeta;
                        break;
                    case "Safari":    hoteles[(int)nombre].img_tarjeta = Properties.Resources.Safari_tarjeta;
                        break;
                }
            }
        }

        public void Cambiar_jugador_actual()
        {
            jugador_actual = jugadores[jug_actual - 1];
        }

        public void Eliminar_Jugador(Jugador jugador, Jugador jugador_que_cobra)
        {
            jugador.Eliminar();
            n_jugadores_activos--;
            if (jugador_que_cobra != null) // Hay que darle todo el dinero del jugador al jugador que cobra
                jugador.Pagar_Noches(ref jugador_que_cobra, jugador.n_billetes_5000, jugador.n_billetes_1000, jugador.n_billetes_500, jugador.n_billetes_100, jugador.n_billetes_50);
        }

        ~Juego()
        {
            hoteles = null;
            hoteles = null;
            casillas = null;
            jugadores = null;
            n_jugadores = 0;
            n_hoteles = 0;
        }
    }
}
