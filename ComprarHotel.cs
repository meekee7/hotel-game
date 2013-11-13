using System;
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

        public void HabilitarControles(Hotel hotelIzq, Hotel hotelDer)
        {
            this.hotel_izq = hotelIzq;
            this.hotel_der = hotelDer;
            if (hotelIzq != null)
            {
                this.nombreIzq.Text = hotelIzq.nombre_txt;
                this.precioIzq.Text = hotelIzq.precio.ToString();
                this.bIzq.Enabled = true;
            }
            else
            {
                this.nombreIzq.Text = "";
                this.precioIzq.Text = "";
                this.bIzq.Enabled = false;
            }
            if (hotelDer != null)
            {
                this.nombreDer.Text = hotelDer.nombre_txt;
                this.precioDer.Text = hotelDer.precio.ToString();
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
