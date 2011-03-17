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
        Boolean global;
        Online frm_online;
        public Chat(Boolean global)
        {
            InitializeComponent();
            this.jugadores = new LinkedList<String>();
            this.global = global;
            this.frm_online = null;
        }

        public Chat(Boolean global, Online frm_online)
        {
            InitializeComponent();
            this.jugadores = new LinkedList<String>();
            this.global = global;
            this.frm_online = frm_online;
        }

        public void rellenar_lista()
        {
            try
            {
                if (this.global)
                {
                    frm_online.enviar_int(frm_online.socket, 14);
                    frm_online.enviar_string(frm_online.socket, "get_chat_users");
                    int bytes_recibidos = 0;
                    int long_lista = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                    String lista = frm_online.recibir_string(frm_online.socket, long_lista, ref bytes_recibidos);
                    String[] lista_jugadores = lista.Split('~');
                    this.listaJugadores.BeginUpdate();
                    this.listaJugadores.Items.Clear();
                    foreach (String nombre in lista_jugadores)
                        this.listaJugadores.Items.Add(nombre);
                    this.listaJugadores.EndUpdate();
                }
                else
                {
                    this.listaJugadores.BeginUpdate();
                    this.listaJugadores.Items.Clear();
                    foreach (String nombre in this.jugadores)
                    {
                        this.listaJugadores.Items.Add(nombre);
                    }
                    this.listaJugadores.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error actualizando lista de jugadores: " + ex.Message);
                this.refrescoLista.Stop();
            }
        }

        public void añadir_jugador(String nombre)
        {
            this.jugadores.AddLast(nombre);
            this.rellenar_lista();
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.global)
            {
                frm_online.enviar_int(frm_online.socket, 10);
                frm_online.enviar_string(frm_online.socket, "leave_chat");
                this.frm_online.chat_cerrado();
                this.refrescoLista.Stop();
            }
        }

        private void refrescoLista_Tick(object sender, EventArgs e)
        {
            this.rellenar_lista();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            this.refrescoLista.Start();
        }
    }
}
