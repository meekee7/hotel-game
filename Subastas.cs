using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Juego_Hotel
{
    public partial class Subastas : Form
    {
        public int n_5000;
        public int n_1000;
        public int n_500;
        public int n_100;
        public int n_50;
        Juego juego;
        int n_mayor_postor; // nº de jugador
        int n_precio_mayor = 0;
        public Hotel hotel_seleccionado;
        private Principal interfaz;
        Boolean online;

        public Subastas(ref Juego juego, Principal interfaz, Boolean online)
        {
            InitializeComponent();
            this.n_5000 = 0;
            this.n_1000 = 0;
            this.n_500 = 0;
            this.n_100 = 0;
            this.n_50 = 0;
            this.juego = juego;
            this.interfaz = interfaz;
            this.online = online;
            this.Rellenar_Lista_Hoteles(this.online);
            if (this.online) // Sólo activar los botones de puja si no eres el creador
            {
                if (this.juego.jugador_actual.nombre_online != this.interfaz.nombre_online)
                {
                    this.listaHoteles.Enabled = false;
                    this.bSubastar.Enabled = false;
                    this.bVender.Enabled = false;
                    this.cantidad.Enabled = true;
                    this.bCerrar.Enabled = false;
                    switch (this.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == this.interfaz.nombre_online).n_jugador)
                    {
                        case 0: this.bJ1.Enabled = true;
                            break;
                        case 1: this.bJ2.Enabled = true;
                            break;
                        case 2: this.bJ3.Enabled = true;
                             break;
                        case 3: this.bJ4.Enabled = true;
                            break;
                    }
                }
            }
        }

        private void Rellenar_Lista_Hoteles(Boolean online)
        {
            if ((!online) || (this.online && (this.juego.jugador_actual.nombre_online == this.interfaz.nombre_online)))
            {
                LinkedList<Hotel> lista = this.juego.jugador_actual.hoteles;
                this.listaHoteles.BeginUpdate();
                this.listaHoteles.Items.Clear();
                foreach (Hotel hotel in lista)
                    this.listaHoteles.Items.Add(hotel.nombre_txt);
                this.listaHoteles.EndUpdate();
                this.listaHoteles.SelectedText = "";
            }
            else
            {
                this.listaHoteles.BeginUpdate();
                this.listaHoteles.Items.Clear();
                this.listaHoteles.Items.Add(this.interfaz.hotel_a_subastar_online.nombre_txt);
                this.listaHoteles.EndUpdate();
                this.listaHoteles.SelectedIndex = 0;
            }
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.bSubastar.Enabled = true;
            this.hotel_seleccionado = this.juego.hoteles.First(Hotel => Hotel.nombre_txt == this.listaHoteles.SelectedItem.ToString());
        }

        private void bSubastar_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Estás seguro de que quieres subastar el hotel?\nEsta operación no se puede cancelar",
                                               "Confirmación de inicio de subasta", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                this.cantidad.Enabled = true;
                this.bVender.Enabled = true;
                this.bCerrar.Enabled = false;
                this.listaHoteles.Enabled = false;
                if (!this.online)
                {
                    if (this.juego.jug_actual != 1)
                        bJ1.Enabled = true;
                    else
                        bJ1.Enabled = false;
                    if (this.juego.jug_actual != 2)
                        bJ2.Enabled = true;
                    else
                        bJ2.Enabled = false;
                    if (this.juego.n_jugadores > 2)
                    {
                        if (this.juego.jug_actual != 3)
                            bJ3.Enabled = true;
                        else
                            bJ3.Enabled = false;
                    }
                    else
                        bJ3.Enabled = false;
                    if (this.juego.n_jugadores > 3)
                    {
                        if (this.juego.jug_actual != 4)
                            bJ4.Enabled = true;
                        else
                            bJ4.Enabled = false;
                    }
                    else
                        bJ4.Enabled = false;
                }
                else
                {
                    // En la ventana del creador de la subasta no se le permite pujar
                    this.bJ1.Enabled = false;
                    this.bJ2.Enabled = false;
                    this.bJ3.Enabled = false;
                    this.bJ4.Enabled = false;
                    int id = this.interfaz.game_id;
                    this.interfaz.frm_online.enviar_comando("auction_start", id.ToString(), this.hotel_seleccionado.nombre_txt.ToString());
                }
                this.bSubastar.Enabled = false;
            }
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, true);
            frm_ver_hoteles.Show();
        }

        private void bVender_Click(object sender, EventArgs e)
        {
            if (this.precio_mayor.Text.ToString().Trim() == "")
                MessageBox.Show("No se ha pujado todavía");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la venta final? No puede deshacerse\n" +
                                    "El jugador " + this.mayor_postor.Text + " deberá abonar " + this.precio_mayor.Text,
                                    "Confirmación de venta", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PedirPago frm_pago = new PedirPago(this.n_precio_mayor, ref this.juego, true, this.juego.jugadores[n_mayor_postor], this.interfaz); // Subasta deshabilitada al estar ya en una subasta
                    frm_pago.ShowDialog();
                    Jugador dueño_ant = hotel_seleccionado.dueño;
                    dueño_ant.Hotel_Expropiado(ref hotel_seleccionado);
                    this.juego.jugadores[n_mayor_postor].Comprar_Hotel(ref hotel_seleccionado, ref dueño_ant, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                    if (frm_pago.total_seleccionado > this.n_precio_mayor)
                    {
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        Principal.Calcular_Devolucion(ref dueño_ant, (frm_pago.total_seleccionado - this.n_precio_mayor), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                        this.juego.jugadores[n_mayor_postor].Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                    }
                    frm_pago.Close();
                    this.Rellenar_Lista_Hoteles(this.online);
                    this.bJ1.Enabled = false;
                    this.bJ2.Enabled = false;
                    this.bJ3.Enabled = false;
                    this.bJ4.Enabled = false;
                    this.bVender.Enabled = false;
                    this.bSubastar.Enabled = false;
                    this.listaHoteles.Enabled = true;
                    this.bCerrar.Enabled = true;
                }
            }
        }

        private void Pujar(int n_jugador)
        {
            String cantidad = this.cantidad.Text.ToString().Trim();
            int cantidad_num;
            if (cantidad == "" || cantidad == "0")
                MessageBox.Show("No se ha introducido una cantidad válida");
            else if (int.TryParse(cantidad, out cantidad_num) == false)
                MessageBox.Show("No se ha introducido un número entero");
            else if (cantidad_num < 0)
                MessageBox.Show("No se pueden introducir cantidades negativas");
            else if (cantidad_num <= n_precio_mayor)
                MessageBox.Show("La cantidad es menor o igual que la puja máxima");
            else if (cantidad_num > this.juego.jugadores[n_jugador].dinero_total)
                MessageBox.Show("La cantidad es mayor que el dinero total del jugador " + this.juego.jugadores[n_jugador].color);
            else if (cantidad_num % 50 != 0)
                MessageBox.Show("La cantidad no es múltiplo de 50");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la puja? No puede deshacerse",
                                    "Confirmación de puja", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (this.interfaz.online)
                    {
                        int id = this.interfaz.game_id;
                        this.interfaz.frm_online.enviar_comando("auction_bid", id.ToString(), cantidad_num.ToString());
                    }
                    else
                    {
                        this.n_precio_mayor = cantidad_num;
                        this.precio_mayor.Text = cantidad_num.ToString();
                        this.mayor_postor.Text = "J" + (n_jugador + 1) + " - " + this.juego.jugadores[n_jugador].color.ToString();
                        this.n_mayor_postor = n_jugador;
                    }
                }
            }
        }

        private void bJ1_Click(object sender, EventArgs e)
        {
            this.Pujar(0);
        }

        private void bJ2_Click(object sender, EventArgs e)
        {
            this.Pujar(1);
        }

        private void bJ3_Click(object sender, EventArgs e)
        {
            this.Pujar(2);
        }

        private void bJ4_Click(object sender, EventArgs e)
        {
            this.Pujar(3);
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void Nueva_puja(Jugador jugador, int cantidad)
        {
            this.n_precio_mayor = cantidad;
            this.precio_mayor.Text = cantidad.ToString();
            this.mayor_postor.Text = "J" + (jugador.n_jugador + 1) + " - " + jugador.color.ToString();
            this.n_mayor_postor = jugador.n_jugador;
        }
    }
}
