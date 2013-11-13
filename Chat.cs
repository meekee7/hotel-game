using System;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class Chat : Form
    {
        readonly Boolean global;
        readonly Online frm_online;
        public int id;
        public String creador;
        public Boolean conectado;

        public Chat(Boolean global, Online frm_online)
        {
            InitializeComponent();
            this.global = global;
            this.frm_online = frm_online;
            this.conectado = true;
        }

        public void desactivar_envio()
        {
            this.bEnviar.Enabled = false;
            this.conectado = false;
        }

        private void Actualizar_lista_jugadores(String[] lista)
        {
            this.listaJugadores.BeginUpdate();
            this.listaJugadores.Items.Clear();
            foreach (String nombre in lista)
                this.listaJugadores.Items.Add(nombre);
            this.listaJugadores.EndUpdate();
        }

        public void rellenar_lista()
        {
            try
            {
                int bytes_recibidos = 0;
                //int long_lista = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                //String lista = frm_online.recibir_string(frm_online.socket, long_lista, ref bytes_recibidos);
                //String[] lista_jugadores = lista.Split('~');
                int cuantos = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                int i;
                var lista_jugadores = new String[cuantos];
                for (i = 0; i < cuantos; i++)
                {
                    int long_nombre = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                    lista_jugadores[i] = frm_online.recibir_string(frm_online.socket, long_nombre, ref bytes_recibidos);
                }
                this.BeginInvoke(new Action<String[]>(Actualizar_lista_jugadores) , new object[] { lista_jugadores });
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorActualizandoListaJugadores + ex.Message);
            }
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.conectado) // not connected
                return;
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

        private void Nuevo_mensaje(String msg)
        {
            this.mensajes.AppendText(msg);
        }

        public void nuevo_mensaje(String remitente, String msg)
        {
            this.BeginInvoke(new Action<String>(Nuevo_mensaje), new object[] { remitente + ": " + msg + Environment.NewLine });
        }

        private void bEnviar_Click(object sender, EventArgs e)
        {
            if (this.mensaje.Text.Trim().Length == 0)
            {
                MessageBox.Show(Mensajes.mensajeNoEnvMensajeVacio);
                return;
            }
            if (this.mensaje.Text.Length > 1024)
            {
                MessageBox.Show(Mensajes.mensajeTamMensajeMax);
                return;
            }
            if (this.global)
                this.frm_online.enviar_comando("send_global_chat_msg", this.mensaje.Text);
            else
                this.frm_online.enviar_comando("send_chat_msg", this.id.ToString(), this.mensaje.Text);
            this.mensaje.Text = "";
        }

        private void mensaje_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bEnviar.PerformClick();
        }

        private void Chat_Shown(object sender, EventArgs e)
        {
            if (this.global)
                this.Text += @" global";
            else
                this.Text += @" " + this.id;
            this.Text += @": " + frm_online.txtLogin.Text;
        }
    }
}