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

        public Subastas(ref Juego juego)
        {
            InitializeComponent();
            this.n_5000 = 0;
            this.n_1000 = 0;
            this.n_500 = 0;
            this.n_100 = 0;
            this.n_50 = 0;
            this.juego = juego;
            LinkedList<Hotel> lista = this.juego.jugador_actual.hoteles;
            this.listaHoteles.BeginUpdate();
            this.listaHoteles.Items.Clear();
            foreach (Hotel hotel in lista)
                this.listaHoteles.Items.Add(hotel.nombre_txt);
            this.listaHoteles.EndUpdate();
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.bHotel.Enabled = true;
        }

        private void bHotel_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Estás seguro de que quieres subastar el hotel?\nEsta operación no se puede cancelar",
                                               "Confirmación de inicio de subasta", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                cantidad.Enabled = true;
                bJ1.Enabled = true;
                bJ2.Enabled = true;
                if (this.juego.n_jugadores > 2)
                    bJ3.Enabled = true;
                if (this.juego.n_jugadores > 3)
                    bJ4.Enabled = true;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, true);
            frm_ver_hoteles.ShowDialog();
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, true);
            frm_ver_hoteles.ShowDialog();
        }

        private void bVender_Click(object sender, EventArgs e)
        {
            if (this.precio_mayor.Text.ToString().Trim() == "")
                MessageBox.Show("No se ha pujado todavía");
            else
            {
                if (MessageBox.Show("¿Estás seguro de que quieres realizar la venta final? No puede deshacerse" +
                                    "El jugador " + this.mayor_postor + " deberá abonar " + this.precio_mayor,
                                    "Confirmación de venta", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    
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
                    this.n_mayor_postor = 0;
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
    }
}
