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
    public partial class PedirPago : Form
    {
        public int n_50, n_100, n_500, n_1000, n_5000;
        public int dinero_necesario, total_seleccionado;
        public Boolean cancelado;
        Juego juego;

        public PedirPago(int dinero_necesario, ref Juego juego)
        {
            InitializeComponent();
            this.juego = juego;
            this.dinero_necesario = dinero_necesario;
            Inicializar();
        }

        public void Inicializar()
        {
            this.n_50 = 0;
            this.n_100 = 0;
            this.n_500 = 0;
            this.n_1000 = 0;
            this.n_5000 = 0;
            this.n50j.Text = "Tienes: " + this.juego.jugador_actual.n_billetes_50.ToString();
            this.n100j.Text = "Tienes: " + this.juego.jugador_actual.n_billetes_100.ToString();
            this.n500j.Text = "Tienes: " + this.juego.jugador_actual.n_billetes_500.ToString();
            this.n1000j.Text = "Tienes: " + this.juego.jugador_actual.n_billetes_1000.ToString();
            this.n5000j.Text = "Tienes: " + this.juego.jugador_actual.n_billetes_5000.ToString();
            this.n50.Text = "Usas: 0";
            this.n100.Text = "Usas: 0";
            this.n500.Text = "Usas: 0";
            this.n1000.Text = "Usas: 0";
            this.n5000.Text = "Usas: 0";
            this.total.Text = "Total: 0";
            this.total_seleccionado = 0;
            this.necesario.Text = "Necesario: " + this.dinero_necesario;
        }

        private void img50_Click(object sender, EventArgs e)
        {
            if (this.n_50 < this.juego.jugador_actual.n_billetes_50)
            {
                this.n_50++;
                this.n50.Text = "Usas: " + this.n_50.ToString();
                this.Calcular_total();
            }
            else
            {
                MessageBox.Show("No tienes tantos billetes de 50", "Imposible incrementar");
            }
        }

        private void img100_Click(object sender, EventArgs e)
        {
            if (this.n_100 < this.juego.jugador_actual.n_billetes_100)
            {
                this.n_100++;
                this.n100.Text = "Usas: " + this.n_100.ToString();
                this.Calcular_total();
            }
            else
            {
                MessageBox.Show("No tienes tantos billetes de 100", "Imposible incrementar");
            }
        }

        private void img500_Click(object sender, EventArgs e)
        {
            if (this.n_500 < this.juego.jugador_actual.n_billetes_500)
            {
                this.n_500++;
                this.n500.Text = "Usas: " + this.n_500.ToString();
                this.Calcular_total();
            }
            else
            {
                MessageBox.Show("No tienes tantos billetes de 500", "Imposible incrementar");
            }
        }

        private void img1000_Click(object sender, EventArgs e)
        {
            if (this.n_1000 < this.juego.jugador_actual.n_billetes_1000)
            {
                this.n_1000++;
                this.n1000.Text = "Usas: " + this.n_1000.ToString();
                this.Calcular_total();
            }
            else
            {
                MessageBox.Show("No tienes tantos billetes de 1000", "Imposible incrementar");
            }
        }

        private void img5000_Click(object sender, EventArgs e)
        {
            if (this.n_5000 < this.juego.jugador_actual.n_billetes_5000)
            {
                this.n_5000++;
                this.n5000.Text = "Usas: " + this.n_5000.ToString();
                this.Calcular_total();
            }
            else
            {
                MessageBox.Show("No tienes tantos billetes de 5000", "Imposible incrementar");
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            if (this.total_seleccionado < this.dinero_necesario)
                MessageBox.Show("No se ha indicado el dinero mínimo necesario", "Dinero insuficiente");
            else
            {
                this.cancelado = false;
                this.Hide();
            }
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.cancelado = true;
            this.Hide();
        }

        void Calcular_total()
        {
            this.total_seleccionado = (this.n_50 * 50) + (this.n_100 * 100) + (this.n_500 * 500) +
                (this.n_1000 * 1000) + (this.n_5000 * 5000);
            this.total.Text = "Total: " + this.total_seleccionado.ToString();
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            this.Inicializar();
        }

        private void bSubastar_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.hoteles.Count != 0)
            {
                Subastas frm_subastas = new Subastas(ref this.juego);
                frm_subastas.ShowDialog();
                // Refrescar valores después de la subasta
            }
            else
            {
                if (this.juego.jugador_actual.dinero_total < this.dinero_necesario)
                {
                    MessageBox.Show("No tienes propiedades para subastar ni fondos suficientes para pagar. Quedas eliminado de la partida", "Oh-Oh");
                }
                else
                {
                    MessageBox.Show("No tienes propiedades para subastar", "Subastas");
                }
            }
        }
    }
}