using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class PartidaOnline : Form, IReLocalizable
    {
        readonly System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartidaOnline));
        public Online frm_online;
        public Principal interfaz;
        public int id;
        public String creador;
        public String nombre;
        public int num_jugadores;
        public Boolean cerrando_por_desconexion = false;
        public Boolean cargada = false;

        public PartidaOnline(Online frm_online)
        {
            InitializeComponent();
            this.frm_online = frm_online;
        }

        public void desactivar_envio()
        {
            this.bEnviar.Enabled = false;
        }

        private void Actualizar_lista_jugadores(String[] lista)
        {
            this.listaJugadores.BeginUpdate();
            this.listaJugadores.Items.Clear();
            foreach (String nombre_jugador in lista)
                this.listaJugadores.Items.Add(nombre_jugador);
            this.listaJugadores.EndUpdate();
            if ((this.listaJugadores.Items.Count == this.num_jugadores) && (this.creador == this.frm_online.txtLogin.Text))
                this.bIniciar.Enabled = true;
            else
                this.bIniciar.Enabled = false;
        }

        public void rellenar_lista()
        {
            try
            {
                int bytes_recibidos = 0;
                int cuantos = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                int i;
                var lista_jugadores = new String[cuantos];
                for (i = 0; i < cuantos; i++)
                {
                    int long_nombre = frm_online.recibir_int(frm_online.socket, ref bytes_recibidos);
                    lista_jugadores[i] = frm_online.recibir_string(frm_online.socket, long_nombre, ref bytes_recibidos);
                }
                this.BeginInvoke(new Action<String[]>(Actualizar_lista_jugadores), new object[] { lista_jugadores });
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorActualizandoListaJugadores + ex.Message);
            }
        }

        public void Cambiar_Creador(String nuevo_creador)
        {
            this.creador = nuevo_creador;
            this.txtCreador.Text = this.resources.GetString("txtCreador.Text") + this.creador;
            if ((this.listaJugadores.Items.Count == this.num_jugadores) && (this.creador == this.frm_online.txtLogin.Text))
                this.bIniciar.Enabled = true;
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
            this.frm_online.enviar_comando("send_chat_msg", this.id.ToString(), this.mensaje.Text);
            this.mensaje.Text = "";
        }

        private void PartidaOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.cerrando_por_desconexion)
            {
                this.frm_online.salir_de_partida();
                this.frm_online.enviar_comando("leave_game", this.id.ToString());
                this.frm_online.lista_partidas.Remove(this);
            }
            else
                this.interfaz.partida_activa = false;
            if (this.interfaz == null)
                return;
            if (!this.interfaz.IsDisposed)
                this.interfaz.BeginInvoke(new Action(this.interfaz.Close));
        }

        private void Nuevo_mensaje(String msg)
        {
            this.mensajes.AppendText(msg);
        }

        public void nuevo_mensaje(String remitente, String msg)
        {
            this.BeginInvoke(new Action<String>(Nuevo_mensaje), new object[] { remitente + ": " + msg + Environment.NewLine });
        }

        private void PartidaOnline_Shown(object sender, EventArgs e)
        {
            this.txtNombre.Text = this.resources.GetString("txtNombre.Text") + this.nombre;
            this.txtCreador.Text = this.resources.GetString("txtCreador.Text") + this.creador;
            this.txtNJugadores.Text = this.resources.GetString("txtNJugadores.Text") + this.num_jugadores;
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

        public void Iniciar(int numJugadores, String config, int jug_inicial, List<Tuple<String, String>> lista_jugadores, Boolean partidaCargada = false,
            int ultimo_avance_auto = 0, int ultimo_res_dado = 0, List<String> estado_hoteles = null, String nombre_jugador_actual = null)
        {
            var thread_partida = new Thread(Manejar_partida) { CurrentUICulture = Thread.CurrentThread.CurrentUICulture };
            var parametros = new List<Object>
            {
                numJugadores,
                jug_inicial,
                config,
                this.creador,
                lista_jugadores,
                partidaCargada,
                ultimo_avance_auto,
                ultimo_res_dado,
                estado_hoteles,
                nombre_jugador_actual
            };
            thread_partida.Start(parametros);
        }

        void Manejar_partida(object parametros)
        {
            var l_parametros = parametros as List<Object>;
            this.interfaz = new Principal(this.frm_online, this.frm_online.configuracion_local);
            if (l_parametros != null)
            {
                var nJugadores = (int)l_parametros.First();
                l_parametros.RemoveAt(0);
                this.interfaz.juego.jug_inicial = (int)l_parametros.First() + 1;
                l_parametros.RemoveAt(0);
                this.interfaz.juego.n_jugadores = nJugadores;
                this.interfaz.game_id = this.id;
                this.interfaz.online_config = l_parametros.First() as String;
                l_parametros.RemoveAt(0);
                this.interfaz.creador_online = l_parametros.First() as String;
                l_parametros.RemoveAt(0);
                var lista_jugadores = l_parametros.First() as List<Tuple<String, String>>;
                l_parametros.RemoveAt(0);
                this.interfaz.partida_cargada_online = (Boolean)l_parametros.First();
                l_parametros.RemoveAt(0);
                if (this.interfaz.partida_cargada_online)
                {
                    this.interfaz.juego.ultimo_avance_auto = (int)l_parametros.First();
                    l_parametros.RemoveAt(0);
                    this.interfaz.juego.ultimo_res_dado = (int)l_parametros.First();
                    l_parametros.RemoveAt(0);
                    this.interfaz.juego.estado_hoteles_online = l_parametros.First() as List<String>;
                    l_parametros.RemoveAt(0);
                    this.interfaz.juego.nombre_jugador_actual_salvado = l_parametros.First() as String;
                    l_parametros.RemoveAt(0);
                }
                else
                    l_parametros.Clear();
                this.interfaz.juego.lista_jugadores_online = lista_jugadores;
                this.interfaz.nombre_online = this.frm_online.txtLogin.Text;
                if (lista_jugadores != null)
                {
                    switch (nJugadores)
                    {
                        case 4:
                            this.interfaz.nombreJ4.Text = this.interfaz.resources.GetString("nombreJ4.Text") +
                                                          lista_jugadores[3].Item1;
                            goto case 3;
                        case 3:
                            this.interfaz.nombreJ3.Text = this.interfaz.resources.GetString("nombreJ3.Text") +
                                                          lista_jugadores[2].Item1;
                            goto case 2;
                        case 2:
                            this.interfaz.nombreJ2.Text = this.interfaz.resources.GetString("nombreJ2.Text") +
                                                          lista_jugadores[1].Item1;
                            this.interfaz.nombreJ1.Text = this.interfaz.resources.GetString("nombreJ1.Text") +
                                                          lista_jugadores[0].Item1;
                            break;
                    }
                }
            }
            // Configurar jugadores si la partida es cargada
            if (this.cargada)
            {
                this.interfaz.partida_cargada_online = true;
            }
            Application.Run(this.interfaz);
        }

        private void mensaje_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bEnviar.PerformClick();
        }

        public void ReLocalize(System.Globalization.CultureInfo nuevoCulture, System.Globalization.CultureInfo antiguoCulture)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<System.Globalization.CultureInfo, System.Globalization.CultureInfo>(this.ReLocalize), new object[] { nuevoCulture, antiguoCulture });
            else
            {
                Thread.CurrentThread.CurrentUICulture = nuevoCulture;
                resources.ApplyResources(this, "$this");
                foreach (Control c in this.Controls)
                {
                    if (c is GroupBox)
                    {
                        c.Text = resources.GetString(c.Name + ".Text");
                        foreach (Control o in c.Controls)
                        {
                            if (o is Label)
                            {
                                var nombreAntiguo = (String)resources.GetObject(o.Name + ".Text", antiguoCulture);
                                if (nombreAntiguo != null)
                                    o.Text = o.Text.Replace(nombreAntiguo, resources.GetString(o.Name + ".Text"));
                            }
                            else
                                resources.ApplyResources(o, o.Name);
                        }
                    }
                    else if (c is Label)
                    {
                        var nombreAntiguo = (String)resources.GetObject(c.Name + ".Text", antiguoCulture);
                        if (nombreAntiguo == null)
                            continue;
                        if (c.Text != null)
                            c.Text = c.Text.Replace(nombreAntiguo, resources.GetString(c.Name + ".Text"));
                    }
                    else
                        c.Text = resources.GetString(c.Name + ".Text");
                }
            }
        }
    }
}