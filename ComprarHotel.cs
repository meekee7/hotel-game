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
    public partial class ComprarHotel : Form
    {
        public Boolean cancelado = false;
        public Boolean comprado_izq = false;
        public Boolean comprado_der = false;

        public ComprarHotel()
        {
            InitializeComponent();
        }

        public void HabilitarControles(Tipos.Tnombre_hotel nombre_izq, Tipos.Tnombre_hotel nombre_der)
        {
            if (nombre_izq != Tipos.Tnombre_hotel.Ninguno)
            {
                this.nombreIzq.Text = nombre_izq.ToString();
                this.bIzq.Enabled = true;
            }
            else
            {
                this.nombreIzq.Text = "";
                this.bIzq.Enabled = false;
            }
            if (nombre_der != Tipos.Tnombre_hotel.Ninguno)
            {
                this.nombreDer.Text = nombre_der.ToString();
                this.bDer.Enabled = true;
            }
            else
            {
                this.nombreDer.Text = "";
                this.bDer.Enabled = false;
            }
        }

        private void bIzq_Click(object sender, EventArgs e)
        {
            this.cancelado = false;
            this.comprado_izq = true;
            this.comprado_der = false;
            this.Hide();
        }

        private void bDer_Click(object sender, EventArgs e)
        {
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
