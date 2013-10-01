using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class VerHoteles : Form 
    {
        Juego juego;
        Hotel hotel_seleccionado;

        public VerHoteles(ref Juego juego, int jugador, Boolean global)
        {
            InitializeComponent();
            this.juego = juego;
            if (global == false)
            {
                Hotel[] lista = new Hotel[this.juego.jugadores[jugador].hoteles.Count];
                this.juego.jugadores[jugador].hoteles.CopyTo(lista, 0);
                this.Rellenar_lista(ref lista);
            }
            else
            {
                int n_hoteles = Enum.GetNames(typeof(Tipos.Tnombre_hotel)).Length - 1;
                Hotel[] hoteles = new Hotel[n_hoteles];
                Juego_Hotel.Juego.Crear_Hoteles(ref hoteles);
                this.Rellenar_lista(ref hoteles);
            }
        }

        void Rellenar_lista(ref Hotel[] lista)
        {
            // Ya se ha comprobado que la lista tiene hoteles
            this.listaHoteles.BeginUpdate();
            this.listaHoteles.Items.Clear();
            foreach (Hotel hotel in lista)
                this.listaHoteles.Items.Add(hotel.nombre_txt);
            this.listaHoteles.EndUpdate();
            // No se necesita para ComboBox
            //this.listaHoteles.Height = (this.listaHoteles.Items.Count + 1) * this.listaHoteles.ItemHeight;
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.hotel_seleccionado = this.juego.hoteles.First(Hotel => Hotel.nombre_txt == this.listaHoteles.SelectedItem.ToString());
            this.imgTarjeta.Image = this.hotel_seleccionado.img_tarjeta;
            this.n_amplis.Text = Mensajes.mensajeNumAmpliaciones + "\r\n"+Mensajes.mensajeConstruidas + this.hotel_seleccionado.n_fases_construidas.ToString();
            this.sueloComprado.Text = Mensajes.mensajeSueloComprado + ((this.hotel_seleccionado.suelo_comprado) ? Mensajes.mensajeSi : Mensajes.mensajeNo);
            String color = (this.hotel_seleccionado.dueño != null ? this.hotel_seleccionado.dueño.Nombre_color() : "-");
            this.dueño.Text = Mensajes.mensajeDuenio + ((this.hotel_seleccionado.dueño == null) ? "-" : String.Format(Mensajes.mensajeJugador, Char.ToUpper(color[0]) + color.Substring(1)));
            this.n_entradas.Text = Mensajes.mensajeNumEntradas + this.hotel_seleccionado.n_entradas.ToString();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                if (this.Owner is Principal)
                    (this.Owner as Principal).ReactivarVerHoteles();
                if (this.Owner is Subastas)
                    (this.Owner as Subastas).bVerHoteles.Enabled = true;
                if (this.Owner is Construir)
                    (this.Owner as Construir).bVerHoteles.Enabled = true;
            }
            this.Close();
        }

        private void VerHoteles_Shown(object sender, EventArgs e)
        {
            if (this.listaHoteles.Items.Count > 0)
                this.listaHoteles.SelectedIndex = 0;
        }
    }
}
