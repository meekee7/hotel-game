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

        public Subastas(ref Juego juego, Principal interfaz)
        {
            InitializeComponent();
            this.n_5000 = 0;
            this.n_1000 = 0;
            this.n_500 = 0;
            this.n_100 = 0;
            this.n_50 = 0;
            this.juego = juego;
            this.interfaz = interfaz;
            this.Rellenar_Lista_Hoteles();
        }

        private void Rellenar_Lista_Hoteles()
        {
            LinkedList<Hotel> lista = this.juego.jugador_actual.hoteles;
            this.listaHoteles.BeginUpdate();
            this.listaHoteles.Items.Clear();
            foreach (Hotel hotel in lista)
                this.listaHoteles.Items.Add(hotel.nombre_txt);
            this.listaHoteles.EndUpdate();
            this.listaHoteles.SelectedText = "";
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
                cantidad.Enabled = true;
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
                this.bVender.Enabled = true;
                this.bCerrar.Enabled = false;
                if (this.interfaz.online)
                    this.interfaz.frm_online.enviar_comando("auction_start", this.interfaz.game_id.ToString(), this.hotel_seleccionado.nombre_txt.ToString());
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
                    this.Rellenar_Lista_Hoteles();
                    this.bJ1.Enabled = false;
                    this.bJ2.Enabled = false;
                    this.bJ3.Enabled = false;
                    this.bJ4.Enabled = false;
                    this.bVender.Enabled = false;
                    this.bSubastar.Enabled = false;
                    this.bCerrar.Enabled = true;
                }
            }
        }

        private void bJ1_Click(object sender, EventArgs e)
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
            else if (cantidad_num > this.juego.jugadores[0].dinero_total)
                MessageBox.Show("La cantidad es mayor que el dinero poseído por J1");
            else if (cantidad_num % 50 != 0)
                MessageBox.Show("La cantidad no es múltiplo de 50");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la puja? No puede deshacerse",
                                    "Confirmación de puja", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.n_precio_mayor = cantidad_num;
                    this.precio_mayor.Text = cantidad_num.ToString();
                    this.mayor_postor.Text = "J1 - " + this.juego.jugadores[0].color.ToString();
                    this.n_mayor_postor = 0;
                    if (this.interfaz.online)
                        this.interfaz.frm_online.enviar_comando("auction_bid", this.interfaz.game_id.ToString(), cantidad_num.ToString());
                }
            }
        }

        private void bJ2_Click(object sender, EventArgs e)
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
            else if (cantidad_num > this.juego.jugadores[1].dinero_total)
                MessageBox.Show("La cantidad es mayor que el dinero poseído por J2");
            else if (cantidad_num % 50 != 0)
                MessageBox.Show("La cantidad no es múltiplo de 50");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la puja?",
                                    "Confirmación de puja", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.n_precio_mayor = cantidad_num;
                    this.precio_mayor.Text = cantidad_num.ToString();
                    this.mayor_postor.Text = "J2 - " + this.juego.jugadores[1].color.ToString();
                    this.n_mayor_postor = 1;
                }
            }
        }

        private void bJ3_Click(object sender, EventArgs e)
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
            else if (cantidad_num > this.juego.jugadores[2].dinero_total)
                MessageBox.Show("La cantidad es mayor que el dinero poseído por J2");
            else if (cantidad_num % 50 != 0)
                MessageBox.Show("La cantidad no es múltiplo de 50");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la puja?",
                                    "Confirmación de puja", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.n_precio_mayor = cantidad_num;
                    this.precio_mayor.Text = cantidad_num.ToString();
                    this.mayor_postor.Text = "J3 - " + this.juego.jugadores[2].color.ToString();
                    this.n_mayor_postor = 2;
                }
            }
        }

        private void bJ4_Click(object sender, EventArgs e)
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
            else if (cantidad_num > this.juego.jugadores[3].dinero_total)
                MessageBox.Show("La cantidad es mayor que el dinero poseído por J4");
            else if (cantidad_num % 50 != 0)
                MessageBox.Show("La cantidad no es múltiplo de 50");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la puja?",
                                    "Confirmación de puja", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.n_precio_mayor = cantidad_num;
                    this.precio_mayor.Text = cantidad_num.ToString();
                    this.mayor_postor.Text = "J4 - " + this.juego.jugadores[3].color.ToString();
                    this.n_mayor_postor = 3;
                }
            }
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
