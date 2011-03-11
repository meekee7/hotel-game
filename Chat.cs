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
    public partial class Chat : Form
    {
        LinkedList<String> jugadores;
        public Chat()
        {
            InitializeComponent();
            this.jugadores = new LinkedList<String>();
        }

        public void rellenar_lista()
        {
            this.listaJugadores.BeginUpdate();
            this.listaJugadores.Items.Clear();
            foreach (String nombre in this.jugadores)
            {
                this.listaJugadores.Items.Add(nombre);
            }
            this.listaJugadores.EndUpdate();
        }

        public void añadir_jugador(String nombre)
        {
            this.jugadores.AddLast(nombre);
            this.rellenar_lista();
        }
    }
}
