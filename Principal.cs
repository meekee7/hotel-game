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
    public partial class Principal : Form
    {
        Juego juego;
        short[] tiradas_ini;
        public Principal()
        {
            InitializeComponent();
            this.juego = new Juego();
        }

        private void bIniciar_Click(object sender, EventArgs e)
        {
            // Buscar número de jugadores
            if (this.juego.n_jugadores == 0)
            {
                MessageBox.Show("Seleccione el nº de jugadores", "No es posible iniciar");
                this.juego = null;
                return;
            }
            else
            {
                this.Crear_Jugadores();
                // Decidir quien empieza
                this.tiradas_ini = new short [this.juego.n_jugadores];
                short i;
                for (i = 0 ; i < this.juego.n_jugadores ; i++)
                {
                    this.tiradas_ini[i] = this.juego.dado.tirar();
                }
                // Encontrando el mayor
                short max = 0;
                for (i = 0; i < this.juego.n_jugadores; i++)
                {
                    if (this.tiradas_ini[i] > max)
                    {
                        max = this.tiradas_ini[i];
                        this.juego.jug_inicial = i;
                    }
                }
                this.jug_ini.Text = "Jugador inicial: " + (this.juego.jug_inicial + 1).ToString();
            }
        }

        public void Crear_Jugadores()
        {
            this.juego.jugadores = new Jugador[this.juego.n_jugadores];
            // Primero a mirar qué color eligió cada jugador
            switch (this.juego.n_jugadores)
            {
                case 4: this.juego.jugadores[3] = new Jugador(3, 3, 3, 3, 3, this.sel_colores.color_j4);
                        goto case 3;
                case 3: this.juego.jugadores[2] = new Jugador(3, 3, 3, 3, 3, this.sel_colores.color_j3);
                        goto case 2;
                case 2: this.juego.jugadores[1] = new Jugador(3, 3, 3, 3, 3, this.sel_colores.color_j2);
                        this.juego.jugadores[0] = new Jugador(3, 3, 3, 3, 3, this.sel_colores.color_j1);
                        break;
            }
        }

        private void bColores_Click(object sender, EventArgs e)
        {
            this.sel_colores.habilitarControles(this.juego.n_jugadores);
            this.sel_colores.Show();
        }

        private void dos_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            this.juego.n_jugadores = 2;
        }

        private void tres_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            this.juego.n_jugadores = 3;
        }

        private void cuatro_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            this.juego.n_jugadores = 4;
        }

        private void bDado_Click(object sender, EventArgs e)
        {
            this.resDado.Text = "Dado: " + this.juego.dado.tirar().ToString();
        }
    }
}
