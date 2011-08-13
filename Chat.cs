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
        Boolean global;
        Online frm_online;
        public int id;
        public String creador;

        public Chat(Boolean global, Online frm_online)
        {
            InitializeComponent();
            this.global = global;
            this.frm_online = frm_online;
        }

        public void desactivar_envio()
        {
            this.bEnviar.Enabled = false;
        }

        delegate void Actualizar_lista_jugadores_Callback(String[] lista);

        private void Actualizar_lista_jugadores(String[] lista)
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
                int bytes_recibidos = 0;
                int long_lista = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                String lista = frm_online.recibir_string(frm_online.socket, long_lista, ref bytes_recibidos);
                String[] lista_jugadores = lista.Split('~');
                this.Actualizar_lista_jugadores(lista_jugadores);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error actualizando lista de jugadores: " + ex.Message);
                this.refrescoLista.Stop();
            }
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.refrescoLista.Stop();
            if (this.global)
            {
                this.frm_online.enviar_comando("leave_global_chat");
                this.frm_online.chat_global_cerrado();
            }
            else
            {
                this.frm_online.enviar_comando("leave_chat", this.id.ToString());
                this.frm_online.chats_abiertos.Remove(this);
            }
        }

        private void refrescoLista_Tick(object sender, EventArgs e)
        {
            if (this.global)
                this.frm_online.enviar_comando("get_global_chat_users");
            else
                this.frm_online.enviar_comando("get_chat_users", this.id.ToString());
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
                this.frm_online.enviar_comando("send_global_msg", this.mensaje.Text);
            }
            this.mensaje.Text = "";
        }

        private void mensaje_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bEnviar.PerformClick();
        }

        private void Chat_Shown(object sender, EventArgs e)
        {
            this.refrescoLista.Start();
        }
    }
}
