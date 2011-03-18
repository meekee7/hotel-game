using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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

        public void desactivar_envio()
        {
            this.bEnviar.Enabled = false;
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
                this.frm_online.chat_global_cerrado();
                this.refrescoLista.Stop();
            }
            MessageBox.Show("Cerrando chat");
        }

        private void refrescoLista_Tick(object sender, EventArgs e)
        {
            this.rellenar_lista();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            this.refrescoLista.Start();
            Thread thread_recepcion = new Thread(esperar_mensajes);
        }

        private void esperar_mensajes()
        {
            String msg;
            do
            {
                int bytes_recibidos = 0;
                int long_msg = this.frm_online.recibir_int(this.frm_online.socket, ref bytes_recibidos);
                msg = this.frm_online.recibir_string(this.frm_online.socket, long_msg, ref bytes_recibidos);
                this.mensajes.AppendText(msg + Environment.NewLine);
            }
            while (msg != "##desconectar##");
        }

        private void bEnviar_Click(object sender, EventArgs e)
        {
            if (this.global)
            {
                this.frm_online.enviar_int(this.frm_online.socket, 15);
                this.frm_online.enviar_string(this.frm_online.socket, "send_global_msg");
                this.frm_online.enviar_int(this.frm_online.socket, this.mensaje.Text.Length);
                this.frm_online.enviar_string(this.frm_online.socket, this.mensaje.Text);
            }
        }
    }
}
