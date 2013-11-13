using System;
using System.Linq;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class VerHoteles : Form 
    {
        readonly Juego juego;
        Hotel hotel_seleccionado;

        public VerHoteles(ref Juego juego, int jugador, Boolean global)
        {
            InitializeComponent();
            this.juego = juego;
            if (global == false)
            {
                var lista = new Hotel[this.juego.jugadores[jugador].hoteles.Count];
                this.juego.jugadores[jugador].hoteles.CopyTo(lista, 0);
                Rellenar_lista(ref lista);
            }
            else
            {
                int n_hoteles = Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length - 1;
                var hoteles = new Hotel[n_hoteles];
                Juego.Crear_Hoteles(ref hoteles);
                Rellenar_lista(ref hoteles);
            }
        }

        void Rellenar_lista(ref Hotel[] lista)
        {
            // Ya se ha comprobado que la lista tiene hoteles
            listaHoteles.BeginUpdate();
            listaHoteles.Items.Clear();
            foreach (Hotel hotel in lista)
                listaHoteles.Items.Add(hotel.nombre_txt);
            listaHoteles.EndUpdate();
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            hotel_seleccionado = juego.hoteles.First(Hotel => Hotel.nombre_txt == listaHoteles.SelectedItem.ToString());
            imgTarjeta.Image = hotel_seleccionado.img_tarjeta;
            n_amplis.Text = Mensajes.mensajeNumAmpliaciones + Environment.NewLine + Mensajes.mensajeConstruidas + hotel_seleccionado.n_fases_construidas;
            sueloComprado.Text = Mensajes.mensajeSueloComprado + ((hotel_seleccionado.suelo_comprado) ? Mensajes.mensajeSi : Mensajes.mensajeNo);
            String color = (hotel_seleccionado.dueño != null ? hotel_seleccionado.dueño.Nombre_color() : "-");
            dueño.Text = Mensajes.mensajeDuenio + ((hotel_seleccionado.dueño == null) ? "-" : String.Format(Mensajes.mensajeJugador, Char.ToUpper(color[0]) + color.Substring(1)));
            n_entradas.Text = Mensajes.mensajeNumEntradas + hotel_seleccionado.n_entradas;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                if (Owner is Principal)
                    (Owner as Principal).ReactivarVerHoteles();
                if (Owner is Subastas)
                    (Owner as Subastas).bVerHoteles.Enabled = true;
                if (Owner is Construir)
                    (Owner as Construir).bVerHoteles.Enabled = true;
            }
            Close();
        }

        private void VerHoteles_Shown(object sender, EventArgs e)
        {
            if (listaHoteles.Items.Count > 0)
                listaHoteles.SelectedIndex = 0;
        }
    }
}
