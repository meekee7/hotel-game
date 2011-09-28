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
    public partial class PartidaOnline : Form
    {
        Online frm_online;
        public int id;
        public String creador;
        public String nombre;

        public PartidaOnline(Online frm_online)
        {
            InitializeComponent();
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
                if ((this.listaJugadores.Items.Count >= 2) && (this.creador == this.frm_online.txtLogin.Text))
                    this.bIniciar.Enabled = true;
                else
                    this.bIniciar.Enabled = false;
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
            }
        }

        private void bEnviar_Click(object sender, EventArgs e)
        {
            if (this.mensaje.Text.Trim().Length == 0)
            {
                MessageBox.Show("No puedes enviar un mensaje vacío");
                return;
            }
            this.frm_online.enviar_comando("send_chat_msg", this.id.ToString(), this.mensaje.Text);
            this.mensaje.Text = "";
        }

        private void PartidaOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.bAbandonar.PerformClick();
            //this.frm_online.salir_de_partida();
        }

        delegate void bAbandonar_Callback();

        public void Cerrar()
        {
            if (this.bAbandonar.InvokeRequired)
            {
                bAbandonar_Callback d = new bAbandonar_Callback(Cerrar);
                this.Invoke(d);
            }
            else
            {
                this.bAbandonar.PerformClick();
            }
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

        private void PartidaOnline_Shown(object sender, EventArgs e)
        {
            this.frm_online.enviar_comando("get_chat_users", this.id.ToString());
        }

        private void bAbandonar_Click(object sender, EventArgs e)
        {
            //TODO: Faltarían acciones para cuando dejas una partida, por ahora solo se envía el comando
            this.frm_online.enviar_comando("leave_game", this.id.ToString());
            this.frm_online.lista_partidas.Remove(this);
            this.bAbandonar.Enabled = false;
            this.Close();
        }            

        private void bDado_Click(object sender, EventArgs e)
        {
            this.frm_online.enviar_comando("roll_dice", this.id.ToString());
        }

        private void bIniciar_Click(object sender, EventArgs e)
        {
            this.frm_online.enviar_comando("start_game", this.id.ToString());
        }

        public void Iniciar()
        {
            this.frm_online.interfaz.online = true;
            this.frm_online.interfaz.Iniciar();
        }

        private void mensaje_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bEnviar.PerformClick();
        }
    }
}
