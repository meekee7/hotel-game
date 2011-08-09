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

        delegate void Actualizar_lista_jugadores_global_Callback(String[] lista);

        private void Actualizar_lista_jugadores(String[] lista)
        {
            if (this.listaJugadores.InvokeRequired)
            {
                Actualizar_lista_jugadores_global_Callback d = new Actualizar_lista_jugadores_global_Callback(Actualizar_lista_jugadores);
                this.Invoke(d, new object[] { lista });
            }
            else
            {
                this.listaJugadores.BeginUpdate();
                this.listaJugadores.Items.Clear();
                foreach (String nombre in lista)
                    this.listaJugadores.Items.Add(nombre);
                this.listaJugadores.EndUpdate();
            }
        }

        delegate void Actualizar_lista_jugadores_Callback(LinkedList<String> lista);

        private void Actualizar_lista_jugadores(LinkedList<String> lista)
        {
            if (this.listaJugadores.InvokeRequired)
            {
                Actualizar_lista_jugadores_Callback d = new Actualizar_lista_jugadores_Callback(Actualizar_lista_jugadores);
                this.Invoke(d, new object[] { lista });
            }
            else
            {
                this.listaJugadores.BeginUpdate();
                this.listaJugadores.Items.Clear();
                foreach (String nombre in lista)
                    this.listaJugadores.Items.Add(nombre);
                this.listaJugadores.EndUpdate();
            }
        }

        public void rellenar_lista()
        {
            try
            {
                if (this.global)
                {
                    int bytes_recibidos = 0;
                    int long_lista = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                    String lista = frm_online.recibir_string(frm_online.socket, long_lista, ref bytes_recibidos);
                    String[] lista_jugadores = lista.Split('~');
                    this.Actualizar_lista_jugadores(lista_jugadores);
                }
                else
                {
                    this.Actualizar_lista_jugadores(this.jugadores);
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
        }

        private void refrescoLista_Tick(object sender, EventArgs e)
        {
            this.frm_online.enviar_comando("get_chat_users", true);
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            if (this.global)
                this.refrescoLista.Start();
        }

        delegate void Nuevo_mensaje_Callback(String msg);

        private void Nuevo_mensaje(String msg)
        {
            if (this.mensajes.InvokeRequired)
            {
                Nuevo_mensaje_Callback d = new Nuevo_mensaje_Callback(Nuevo_mensaje);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                this.mensajes.AppendText(msg);
            }
        }

        public void nuevo_mensaje(String remitente, String msg)
        {
            this.Nuevo_mensaje(remitente + ": " + msg + Environment.NewLine);
        }

        private void bEnviar_Click(object sender, EventArgs e)
        {
            if (this.mensaje.Text.Trim().Length == 0)
            {
                MessageBox.Show("No puedes enviar un mensaje vacío");
                return;
            }
            if (this.global)
            {
                this.frm_online.enviar_comando("send_global_msg", false, this.mensaje.Text);
            }
            this.mensaje.Text = "";
        }

        private void mensaje_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bEnviar.PerformClick();
        }
    }
}
