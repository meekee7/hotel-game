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
        public Online frm_online;
        public Principal interfaz;
        public int id;
        public String creador;
        public String nombre;
        public int num_jugadores;
        public Boolean cerrando_por_desconexion = false;

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
                this.BeginInvoke(d, new object[] { lista });
            }
            else
            {
                this.listaJugadores.BeginUpdate();
                this.listaJugadores.Items.Clear();
                foreach (String nombre in lista)
                    this.listaJugadores.Items.Add(nombre);
                this.listaJugadores.EndUpdate();
                if ((this.listaJugadores.Items.Count == this.num_jugadores) && (this.creador == this.frm_online.txtLogin.Text))
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
                int cuantos = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                int i;
                int long_nombre;
                String[] lista_jugadores = new String[cuantos];
                for (i = 0; i < cuantos; i++)
                {
                    long_nombre = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                    lista_jugadores[i] = frm_online.recibir_string(frm_online.socket, long_nombre, ref bytes_recibidos);
                }
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
            if (this.mensaje.Text.Length > 1024)
            {
                MessageBox.Show("No puedes enviar un mensaje de más de 1024 caracteres");
                return;
            }
            this.frm_online.enviar_comando("send_chat_msg", this.id.ToString(), this.mensaje.Text);
            this.mensaje.Text = "";
        }

        delegate void Cerrar_partida_Callback();

        private void PartidaOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.cerrando_por_desconexion)
            {
                this.frm_online.salir_de_partida();
                this.frm_online.enviar_comando("leave_game", this.id.ToString());
                this.frm_online.lista_partidas.Remove(this);
            }
            if (this.interfaz != null)
            {
                //this.interfaz.partida_activa = false;
                this.interfaz.BeginInvoke(new Cerrar_partida_Callback(this.interfaz.Close));
            }
        }

        delegate void Nuevo_mensaje_Callback(String msg);

        private void Nuevo_mensaje(String msg)
        {
            if (this.mensajes.InvokeRequired)
            {
                Nuevo_mensaje_Callback d = new Nuevo_mensaje_Callback(Nuevo_mensaje);
                this.BeginInvoke(d, new object[] { msg });
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
            this.txtNombre.Text = "Nombre: " + this.nombre;
            this.txtCreador.Text = "Creador: " + this.creador;
            this.txtNJugadores.Text = "Número de jugadores: " + this.num_jugadores.ToString();
            this.mensaje.Focus();
            this.frm_online.enviar_comando("get_chat_users", this.id.ToString());
        }

        private void bAbandonar_Click(object sender, EventArgs e)
        {
            //TODO: Faltarían acciones para cuando dejas una partida, por ahora solo se envía el comando
            this.Close();
        }

        private void bIniciar_Click(object sender, EventArgs e)
        {
            this.bIniciar.Enabled = false;
            this.frm_online.enviar_comando("start_game", this.id.ToString());
        }

        public void Iniciar(int num_jugadores, String config, int jug_inicial, String[] lista_nombres)
        {
            Thread thread_partida = new Thread(manejar_partida);
            LinkedList<String> parametros = new LinkedList<String>();
            parametros.AddLast(num_jugadores.ToString());
            parametros.AddLast(jug_inicial.ToString());
            parametros.AddLast(config);
            parametros.AddLast(lista_nombres.Length.ToString());
            foreach (String nombre in lista_nombres)
                parametros.AddLast(nombre);

            thread_partida.Start(parametros);
        }

        void manejar_partida(object parametros)
        {
            LinkedList<String> l_parametros = (LinkedList<String>)parametros;
            this.interfaz = new Principal(true, this.frm_online);
            int num_jugadores = Convert.ToInt32((l_parametros.First.Value));
            l_parametros.RemoveFirst();
            this.interfaz.juego.jug_inicial = Convert.ToInt32((l_parametros.First.Value)) + 1;
            l_parametros.RemoveFirst();
            this.interfaz.juego.n_jugadores = num_jugadores;
            this.interfaz.game_id = this.id;
            this.interfaz.online_config = l_parametros.First.Value;
            l_parametros.RemoveFirst();
            int cuantos = Convert.ToInt32(l_parametros.First.Value);
            l_parametros.RemoveFirst();
            String[] lista_jugadores = new String[cuantos];
            int i;
            for (i = 0; i < cuantos; i++)
            {
                lista_jugadores[i] = l_parametros.First.Value;
                l_parametros.RemoveFirst();
            }
            this.interfaz.juego.lista_jugadores_online = lista_jugadores;
            this.interfaz.nombre_online = this.frm_online.txtLogin.Text;
            switch (num_jugadores)
            {
                case 4: this.interfaz.nombreJ4.Text = "Nombre: " + lista_jugadores[3];
                        goto case 3;
                case 3: this.interfaz.nombreJ3.Text = "Nombre: " + lista_jugadores[2];
                        goto case 2;
                case 2: this.interfaz.nombreJ2.Text = "Nombre: " + lista_jugadores[1];
                        this.interfaz.nombreJ1.Text = "Nombre: " + lista_jugadores[0];
                        break;
            }
            //this.interfaz.ShowDialog();
            Application.Run(this.interfaz);
        }

        private void mensaje_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bEnviar.PerformClick();
        }
    }
}
