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
    public partial class ComprarHotel : Form
    {
        public Boolean cancelado = false;
        public Boolean comprado_izq = false;
        public Boolean comprado_der = false;
        public Boolean comprado_con_todo = false;
        private Hotel hotel_izq, hotel_der;

        public ComprarHotel()
        {
            InitializeComponent();
        }

        public void HabilitarControles(Hotel hotel_izq, Hotel hotel_der)
        {
            this.hotel_izq = hotel_izq;
            this.hotel_der = hotel_der;
            if (hotel_izq != null)
            {
                this.nombreIzq.Text = hotel_izq.nombre_txt;
                this.precioIzq.Text = hotel_izq.precio.ToString();
                this.bIzq.Enabled = true;
            }
            else
            {
                this.nombreIzq.Text = "";
                this.precioIzq.Text = "";
                this.bIzq.Enabled = false;
            }
            if (hotel_der != null)
            {
                this.nombreDer.Text = hotel_der.nombre_txt;
                this.precioDer.Text = hotel_der.precio.ToString();
                this.bDer.Enabled = true;
            }
            else
            {
                this.nombreDer.Text = "";
                this.precioDer.Text = "";
                this.bDer.Enabled = false;
            }
        }

        private void bIzq_Click(object sender, EventArgs e)
        {
            // Si el hotel perteneció a alguien que se retiró. Dar la opción de comprarlo entero
            if (hotel_izq.n_fases_construidas > 0)
            {
                if (MessageBox.Show(String.Format(Mensajes.mensajeComprarHotelConFasesYEntradas, hotel_izq.Calcular_precio_con_todo()),
                    Mensajes.tituloComprarHotel, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    comprado_con_todo = true;
                }
            }
            this.cancelado = false;
            this.comprado_izq = true;
            this.comprado_der = false;
            this.Hide();
        }

        private void bDer_Click(object sender, EventArgs e)
        {
            // Si el hotel perteneció a alguien que se retiró. Dar la opción de comprarlo entero
            if (hotel_der.n_fases_construidas > 0)
            {
                if (MessageBox.Show(String.Format(Mensajes.mensajeComprarHotelConFasesYEntradas, hotel_der.Calcular_precio_con_todo()),
                    Mensajes.tituloComprarHotel, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    comprado_con_todo = true;
                }
            }
            this.cancelado = false;
            this.comprado_izq = false;
            this.comprado_der = true;
            this.Hide();
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.cancelado = true;
            this.comprado_izq = false;
            this.comprado_der = false;
            this.Hide();
        }
    }
}
