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
    public partial class Sel_colores : Form
    {

        public Tipos.Tcolor color_j1, color_j2, color_j3, color_j4;
        short n_jugadores;

        public Sel_colores()
        {
            InitializeComponent();
            // Colores por defecto
            this.color_j1 = Tipos.Tcolor.rojo;
            this.color_j2 = Tipos.Tcolor.azul;
            this.color_j3 = Tipos.Tcolor.verde;
            this.color_j4 = Tipos.Tcolor.amarillo;
        }

        public void habilitarControles(short n_jugadores)
        {
            this.n_jugadores = n_jugadores;
            switch (this.n_jugadores)
            {
                case 2: j3Rojo.Enabled = false;
                    j3Azul.Enabled = false;
                    j3Verde.Enabled = false;
                    j3Amarillo.Enabled = false;
                    j4Rojo.Enabled = false;
                    j4Azul.Enabled = false;
                    j4Verde.Enabled = false;
                    j4Amarillo.Enabled = false;
                    break;
                case 3: j3Rojo.Enabled = true;
                    j3Azul.Enabled = true;
                    j3Verde.Enabled = true;
                    j3Amarillo.Enabled = true;
                    j4Rojo.Enabled = false;
                    j4Azul.Enabled = false;
                    j4Verde.Enabled = false;
                    j4Amarillo.Enabled = false;
                    break;
                case 4: j3Rojo.Enabled = true;
                    j3Azul.Enabled = true;
                    j3Verde.Enabled = true;
                    j3Amarillo.Enabled = true;
                    j4Rojo.Enabled = true;
                    j4Azul.Enabled = true;
                    j4Verde.Enabled = true;
                    j4Amarillo.Enabled = true;
                    break;
            }
        }

        private void j1Rojo_CheckedChanged(object sender, EventArgs e)
        {
            if (j1Rojo.Checked)
            {
                this.color_j1 = Tipos.Tcolor.rojo;
                // Si ponemos J1 a rojo, no puede haber otro a rojo
                if (j2Rojo.Checked)
                    j2Rojo.Checked = false;
                if (j3Rojo.Checked)
                    j3Rojo.Checked = false;
                if (j4Rojo.Checked)
                    j4Rojo.Checked = false;
            }
        }

        private void j2Rojo_CheckedChanged(object sender, EventArgs e)
        {
            if (j2Rojo.Checked)
            {
                this.color_j2 = Tipos.Tcolor.rojo;
                // Si ponemos J2 a rojo, no puede haber otro a rojo
                if (j1Rojo.Checked)
                    j1Rojo.Checked = false;
                if (j3Rojo.Checked)
                    j3Rojo.Checked = false;
                if (j4Rojo.Checked)
                    j4Rojo.Checked = false;
            }
        }

        private void j3Rojo_CheckedChanged(object sender, EventArgs e)
        {
            if (j3Rojo.Checked)
            {
                this.color_j3 = Tipos.Tcolor.rojo;
                // Si ponemos J3 a rojo, no puede haber otro a rojo
                if (j1Rojo.Checked)
                    j1Rojo.Checked = false;
                if (j2Rojo.Checked)
                    j2Rojo.Checked = false;
                if (j4Rojo.Checked)
                    j4Rojo.Checked = false;
            }
        }

        private void j4Rojo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void j1Azul_CheckedChanged(object sender, EventArgs e)
        {
            if (j1Azul.Checked)
            {
                this.color_j1 = Tipos.Tcolor.azul;
                // Si ponemos J1 a azul, no puede haber otro a azul
                if (j2Azul.Checked)
                    j2Azul.Checked = false;
                if (j3Azul.Checked)
                    j3Azul.Checked = false;
                if (j4Azul.Checked)
                    j4Azul.Checked = false;
            }
        }

        private void j2Azul_CheckedChanged(object sender, EventArgs e)
        {
            if (j2Azul.Checked)
            {
                this.color_j2 = Tipos.Tcolor.azul;
                // Si ponemos J2 a azul, no puede haber otro a azul
                if (j1Azul.Checked)
                    j1Azul.Checked = false;
                if (j3Azul.Checked)
                    j3Azul.Checked = false;
                if (j4Azul.Checked)
                    j4Azul.Checked = false;
            }
        }

        private void j3Azul_CheckedChanged(object sender, EventArgs e)
        {
            if (j3Azul.Checked)
            {
                this.color_j3 = Tipos.Tcolor.azul;
                // Si ponemos J3 a azul, no puede haber otro a azul
                if (j1Azul.Checked)
                    j1Azul.Checked = false;
                if (j2Azul.Checked)
                    j2Azul.Checked = false;
                if (j4Azul.Checked)
                    j4Azul.Checked = false;
            }
        }

        private void j4Azul_CheckedChanged(object sender, EventArgs e)
        {
            if (j4Azul.Checked)
            {
                this.color_j4 = Tipos.Tcolor.azul;
                // Si ponemos J4 a azul, no puede haber otro a azul
                if (j1Azul.Checked)
                    j1Azul.Checked = false;
                if (j2Azul.Checked)
                    j2Azul.Checked = false;
                if (j3Azul.Checked)
                    j3Azul.Checked = false;
            }
        }

        private void j1Verde_CheckedChanged(object sender, EventArgs e)
        {
            if (j1Verde.Checked)
            {
                this.color_j1 = Tipos.Tcolor.verde;
                // Si ponemos J1 a verde, no puede haber otro a verde
                if (j2Verde.Checked)
                    j2Verde.Checked = false;
                if (j3Verde.Checked)
                    j3Verde.Checked = false;
                if (j4Verde.Checked)
                    j4Verde.Checked = false;
            }
        }

        private void j2Verde_CheckedChanged(object sender, EventArgs e)
        {
            if (j2Verde.Checked)
            {
                this.color_j2 = Tipos.Tcolor.verde;
                // Si ponemos J2 a verde, no puede haber otro a verde
                if (j1Verde.Checked)
                    j1Verde.Checked = false;
                if (j3Verde.Checked)
                    j3Verde.Checked = false;
                if (j4Verde.Checked)
                    j4Verde.Checked = false;
            }
        }

        private void j3Verde_CheckedChanged(object sender, EventArgs e)
        {
            if (j3Verde.Checked)
            {
                this.color_j3 = Tipos.Tcolor.verde;
                // Si ponemos J3 a verde, no puede haber otro a verde
                if (j1Verde.Checked)
                    j1Verde.Checked = false;
                if (j2Verde.Checked)
                    j2Verde.Checked = false;
                if (j4Verde.Checked)
                    j4Verde.Checked = false;
            }
        }

        private void j4Verde_CheckedChanged(object sender, EventArgs e)
        {
            if (j4Verde.Checked)
            {
                this.color_j4 = Tipos.Tcolor.verde;
                // Si ponemos J4 a verde, no puede haber otro a verde
                if (j1Verde.Checked)
                    j1Verde.Checked = false;
                if (j2Verde.Checked)
                    j2Verde.Checked = false;
                if (j3Verde.Checked)
                    j3Verde.Checked = false;
            }
        }

        private void j1Amarillo_CheckedChanged(object sender, EventArgs e)
        {
            if (j1Amarillo.Checked)
            {
                this.color_j1 = Tipos.Tcolor.amarillo;
                // Si ponemos J1 a amarillo, no puede haber otro a amarillo
                if (j2Amarillo.Checked)
                    j2Amarillo.Checked = false;
                if (j3Amarillo.Checked)
                    j3Amarillo.Checked = false;
                if (j4Amarillo.Checked)
                    j4Amarillo.Checked = false;
            }
        }

        private void j2Amarillo_CheckedChanged(object sender, EventArgs e)
        {
            if (j2Amarillo.Checked)
            {
                this.color_j2 = Tipos.Tcolor.amarillo;
                // Si ponemos J2 a amarillo, no puede haber otro a amarillo
                if (j1Amarillo.Checked)
                    j1Amarillo.Checked = false;
                if (j3Amarillo.Checked)
                    j3Amarillo.Checked = false;
                if (j4Amarillo.Checked)
                    j4Amarillo.Checked = false;
            }
        }

        private void j3Amarillo_CheckedChanged(object sender, EventArgs e)
        {
            if (j3Amarillo.Checked)
            {
                this.color_j3 = Tipos.Tcolor.amarillo;
                // Si ponemos J3 a amarillo, no puede haber otro a amarillo
                if (j1Amarillo.Checked)
                    j1Amarillo.Checked = false;
                if (j2Amarillo.Checked)
                    j2Amarillo.Checked = false;
                if (j4Amarillo.Checked)
                    j4Amarillo.Checked = false;
            }
        }

        private void j4Amarillo_CheckedChanged(object sender, EventArgs e)
        {
            if (j4Amarillo.Checked)
            {
                this.color_j4 = Tipos.Tcolor.amarillo;
                // Si ponemos J4 a amarillo, no puede haber otro a amarillo
                if (j1Amarillo.Checked)
                    j1Amarillo.Checked = false;
                if (j2Amarillo.Checked)
                    j2Amarillo.Checked = false;
                if (j3Amarillo.Checked)
                    j3Amarillo.Checked = false;
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            // Comprobación para ver si algún jugador no tiene color asociado
            if (!j1Rojo.Checked && !j1Verde.Checked && !j1Amarillo.Checked && !j1Azul.Checked)
                MessageBox.Show("El jugador 1 no tiene color asociado");
            else if (!j2Rojo.Checked && !j2Verde.Checked && !j2Amarillo.Checked && !j2Azul.Checked)
                MessageBox.Show("El jugador 2 no tiene color asociado");
            else if (!j3Rojo.Checked && !j3Verde.Checked && !j3Amarillo.Checked && !j3Azul.Checked)
                if (this.n_jugadores >= 3)
                    MessageBox.Show("El jugador 3 no tiene color asociado");
                else
                    this.Hide();
            else if (!j4Rojo.Checked && !j4Verde.Checked && !j4Amarillo.Checked && !j4Azul.Checked)
                if (this.n_jugadores >= 4)
                    MessageBox.Show("El jugador 4 no tiene color asociado");
                else
                    this.Hide();
            else
                this.Hide();
        }
    }
}
