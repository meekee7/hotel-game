using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class Online : Form
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Online));
        public Socket socket;
        Chat frm_chat_global;
        public LinkedList<Chat> chats_abiertos;
        public LinkedList<PartidaOnline> lista_partidas;
        Thread thread_recepcion;
        Semaphore sem_en_comunicacion;
        Boolean continuar_thread, conectado = false, cerrando = false;
        public Tipos.Resultado_dado_cons ultimo_res_dado_cons;
        public XmlDocument configuracion_local;

        public Online(XmlDocument configuracion)
        {
            InitializeComponent();
            this.chats_abiertos = new LinkedList<Chat>();
            this.lista_partidas = new LinkedList<PartidaOnline>();
            this.configuracion_local = configuracion;
        }

        private void Online_Load(object sender, EventArgs e)
        {
            this.bChatGlobal.Enabled = false;
            this.bDesconectar.Enabled = false;
            this.bCrearConv.Enabled = false;
            this.bLogin.Enabled = false;
            this.bCrearPartida.Enabled = false;
        }

        public String recibir_string(Socket s, int longitud, ref int bytes_recibidos)
        {
            if (this.socket == null)
                return "";
            if (!this.socket.Connected)
            {
                MessageBox.Show(Mensajes.mensajeNoConectado);
                return "";
            }
            try
            {
                byte[] data = new byte[longitud];
                bytes_recibidos = s.Receive(data);
                String s_data;
                if (bytes_recibidos > 0)
                    s_data = System.Text.Encoding.UTF8.GetString(data, 0, bytes_recibidos);
                else
                    s_data = "";
                data = null;
                return s_data;
            }
            catch (Exception e)
            {
                if (this.continuar_thread == true)
                {
                    this.Pulsar_Desconectar();
                    MessageBox.Show(Mensajes.mensajeExcepcionRecibiendoDatos + e.Message);
                }
                return null;
            }
        }

        public int recibir_int(Socket s, ref int bytes_recibidos)
        {
            if (this.socket == null)
                return 0;
            if (!this.socket.Connected)
            {
                MessageBox.Show(Mensajes.mensajeNoConectado);
                return 0;
            }
            try
            {
                byte[] b_int = new byte[4];
                bytes_recibidos = s.Receive(b_int);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(b_int);
                return BitConverter.ToInt32(b_int, 0);
            }
            catch (Exception e)
            {
                if (this.continuar_thread == true)
                {
                    this.Pulsar_Desconectar();
                    MessageBox.Show(Mensajes.mensajeExcepcionRecibiendoDatos + e.Message);
                }
                return 0;
            }
        }

        public int enviar_string(Socket s, String texto)
        {
            if (this.socket == null)
                return 0;
            if (!this.socket.Connected)
            {
                MessageBox.Show(Mensajes.mensajeNoConectado);
                return 0;
            }
            try
            {
                byte[] texto_bytes = Encoding.UTF8.GetBytes(texto);
                return s.Send(texto_bytes);
            }
            catch (Exception e)
            {
                if (this.continuar_thread == true)
                {
                    this.Pulsar_Desconectar();
                    MessageBox.Show(Mensajes.mensajeExcepcionRecibiendoDatos + e.Message);
                }
                return 0;
            }
        }

        public int enviar_int(Socket s, int num)
        {
            if (this.socket == null)
                return 0;
            if (!this.socket.Connected)
            {
                MessageBox.Show(Mensajes.mensajeNoConectado);
                return 0;
            }
            try
            {
                byte[] b_int = BitConverter.GetBytes(num);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(b_int);
                return s.Send(b_int);
            }
            catch (Exception e)
            {
                if (this.continuar_thread == true)
                {
                    this.Pulsar_Desconectar();
                    MessageBox.Show(Mensajes.mensajeExcepcionRecibiendoDatos + e.Message);
                }
                return 0;
            }
        }

        private void bLogin_Click(object sender, EventArgs e)
        {
            if (socket.Connected)
            {
                if (this.txtLogin.Text.Length == 0)
                    MessageBox.Show(Mensajes.mensajeApodoVacio);
                else if (this.txtLogin.Text.Length > 20)
                    MessageBox.Show(Mensajes.mensajeApodoDemasiadoLargo);
                else
                {
                    try
                    {
                        enviar_int(this.socket, Encoding.UTF8.GetBytes(this.txtLogin.Text).Length);
                        enviar_string(this.socket, this.txtLogin.Text);
                        int bytes_recibidos = 0;
                        String res_login = recibir_string(this.socket, 8, ref bytes_recibidos);
                        if (res_login == "login ko")
                        {
                            MessageBox.Show(Mensajes.mensajeApodoEnUso);
                            this.bDesconectar.PerformClick();
                            this.txtLogin.Enabled = true;
                        }
                        else if (res_login == "login no")
                        {
                            MessageBox.Show(Mensajes.mensajeErrorTamañoApodo);
                            this.bDesconectar.PerformClick();
                            this.txtLogin.Enabled = true;
                        }
                        else
                        {
                            this.conectado = true;
                            this.bLogin.Enabled = false;
                            this.bCrearPartida.Enabled = true;
                            this.bCrearConv.Enabled = true;
                            this.txtLogin.Enabled = false;
                            this.bChatGlobal.Enabled = true;
                            this.sem_en_comunicacion = new Semaphore(1, 1);
                            thread_recepcion = new Thread(esperar_comandos);
                            this.continuar_thread = true;
                            thread_recepcion.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                            thread_recepcion.Start();
                            this.Text += ": " + this.txtLogin.Text;
                            this.enviar_comando("get_games");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Mensajes.mensajeErrorLogin + ex.Message);
                        this.Pulsar_Desconectar();
                    }
                }
            }
            else
                MessageBox.Show(Mensajes.mensajeNoConectado);
        }

        private void bConectar_Click(object sender, EventArgs e)
        {
            try
            {
                String servidor;
                int puerto;
                if (this.checkSrvOficial.Checked)
                {
                    servidor = "betovserver.no-ip.org";
                    puerto = 12345;
                }
                else
                {
                    servidor = this.txtServidor.Text.Trim();
                    if (this.txtPuerto.Text.Trim() == "")
                        puerto = 12345;
                    else
                    {
                        Boolean res_puerto = int.TryParse(this.txtPuerto.Text.Trim(), out puerto);
                        if (res_puerto == false)
                        {
                            MessageBox.Show(Mensajes.mensajeErrorPuertoNoNumerico);
                            return;
                        }
                        else if (puerto > 65535)
                        {
                            MessageBox.Show(Mensajes.mensajeErrorPuertoAlto);
                            return;
                        }
                        else if (puerto <= 0)
                        {
                            MessageBox.Show(Mensajes.mensajeErrorPuertoBajo);
                            return;
                        }
                    }
                }
                IPAddress dir = Dns.GetHostAddresses(servidor).First(IPAddress => IPAddress.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint Ep = new IPEndPoint(dir, puerto);
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.Connect(Ep);
                this.bDesconectar.Enabled = true;
                // Comprobar versión correcta:
                Actualizador actualizador = new Actualizador();
                this.enviar_int(this.socket, Encoding.UTF8.GetBytes(actualizador.version_actual).Length);
                this.enviar_string(this.socket, actualizador.version_actual);
                int bytes_recibidos = 0;
                int long_res_version = this.recibir_int(this.socket, ref bytes_recibidos);
                String res_version = this.recibir_string(this.socket, long_res_version, ref bytes_recibidos);
                actualizador = null;
                if (res_version != "ok")
                {
                    MessageBox.Show(String.Format(Mensajes.mensajeVersionNoActualizada, res_version));
                    this.Pulsar_Desconectar();
                    return;
                }
                this.bLogin.Enabled = true;
                this.bConectar.Enabled = false;
                this.txtServidor.Enabled = false;
                this.txtPuerto.Enabled = false;
                this.txtLogin.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorConectando + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        delegate void Actualizar_lista_usuarios_Callback(String[] lista);

        private void Actualizar_lista_usuarios(String[] lista)
        {
            if (this.listaUsuarios.InvokeRequired)
            {
                Actualizar_lista_usuarios_Callback d = new Actualizar_lista_usuarios_Callback(Actualizar_lista_usuarios);
                this.Invoke(d, new object[] { lista });
            }
            else
            {
                this.listaUsuarios.BeginUpdate();
                this.listaUsuarios.Items.Clear();
                foreach (String nombre in lista)
                    this.listaUsuarios.Items.Add(nombre);
                this.listaUsuarios.EndUpdate();
            }
        }

        private void Rellenar_lista_usuarios()
        {
            if (this.cerrando)
                return;
            try
            {
                // Primero se recibe la cantidad de usuarios online
                int bytes_recibidos = 0;
                int cuantos = this.recibir_int(this.socket, ref bytes_recibidos);
                int i;
                int long_nombre;
                String[] lista_jugadores = new String[cuantos];
                for (i = 0; i < cuantos; i++)
                {
                    long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
                    lista_jugadores[i] = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
                }
                this.Actualizar_lista_usuarios(lista_jugadores);
                lista_jugadores = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorObteniendoListaUsuarios + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        delegate void Actualizar_lista_partidas_Callback(String[] lista);

        private void Actualizar_lista_partidas(String[] lista)
        {
            if (this.listaPartidas.InvokeRequired)
            {
                Actualizar_lista_partidas_Callback d = new Actualizar_lista_partidas_Callback(Actualizar_lista_partidas);
                this.Invoke(d, new object[] { lista });
            }
            else
            {
                this.listaPartidas.BeginUpdate();
                this.listaPartidas.Items.Clear();
                foreach (String nombre in lista)
                    this.listaPartidas.Items.Add(nombre);
                this.listaPartidas.EndUpdate();
                this.bUnirse.Enabled = false;
            }
        }

        delegate void Borrar_lista_partidas_Callback();

        private void Borrar_lista_partidas()
        {
            if (this.listaPartidas.InvokeRequired)
            {
                Borrar_lista_partidas_Callback d = new Borrar_lista_partidas_Callback(Borrar_lista_partidas);
                this.Invoke(d);
            }
            else
            {
                this.listaPartidas.Items.Clear();
                this.bUnirse.Enabled = false;
            }
        }

        private void Rellenar_lista_partidas()
        {
            if (this.cerrando)
                return;
            try
            {
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                int bytes_recibidos = 0;
                int cuantas = this.recibir_int(this.socket, ref bytes_recibidos);
                if (cuantas == 0)
                {
                    this.Borrar_lista_partidas();
                    return;
                }
                int i, long_nombre, capacidad, n_jugadores_dentro;
                Boolean empezada, finalizada;
                String nombre, estado;
                String[] lista_partidas = new String[cuantas];
                for (i = 0; i < cuantas; i++)
                {
                    long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
                    nombre = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
                    capacidad = this.recibir_int(this.socket, ref bytes_recibidos);
                    n_jugadores_dentro = this.recibir_int(this.socket, ref bytes_recibidos);
                    empezada = Convert.ToBoolean(this.recibir_int(this.socket, ref bytes_recibidos));
                    finalizada = Convert.ToBoolean(this.recibir_int(this.socket, ref bytes_recibidos));
                    if (!empezada && !finalizada)
                    {
                        if (n_jugadores_dentro == capacidad)
                            estado = Mensajes.textoEstadoLlena;
                        else
                            estado = Mensajes.textoEstadoDisponible;
                    }
                    else if (empezada && !finalizada)
                        estado = Mensajes.textoEstadoEmpezada;
                    else
                        estado = Mensajes.textoEstadoFinalizada;
                    if (n_jugadores_dentro == 1)
                        lista_partidas[i] = String.Format(Mensajes.textoPartidaSingular, nombre, capacidad, estado);
                    else
                        lista_partidas[i] = String.Format(Mensajes.textoPartidaPlural, nombre, n_jugadores_dentro, capacidad, estado);
                }
                this.Actualizar_lista_partidas(lista_partidas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorObteniendoListaPartidas + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        delegate void Cerrar_Chat_Callback(Chat chat);

        private void Cerrar_Chat(Chat chat)
        {
            if (chat.InvokeRequired)
            {
                Cerrar_Chat_Callback d = new Cerrar_Chat_Callback(Cerrar_Chat);
                chat.Invoke(d, new object[] {chat});
            }
            else
            {
                chat.Close();
            }
        }

        delegate void Cerrar_Partida_Callback(PartidaOnline partida);

        private void Cerrar_Partida(PartidaOnline partida)
        {
            if (partida.InvokeRequired)
            {
                Cerrar_Partida_Callback d = new Cerrar_Partida_Callback(Cerrar_Partida);
                partida.Invoke(d, new object[] { partida });
            }
            else
            {
                partida.cerrando_por_desconexion = true;
                partida.Close();
            }
        }

        private void Online_FormClosing(object sender, FormClosingEventArgs e)
        {
            int i;
            this.cerrando = true;
            if (((this.frm_chat_global != null) ||
                (this.chats_abiertos.Count > 0)) &&
                (MessageBox.Show(Mensajes.mensajeCerrarChatsAbiertos, Mensajes.tituloConfirmacionDesconectar, MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                if (this.frm_chat_global != null)
                {
                    this.frm_chat_global.conectado = false;
                    this.frm_chat_global.Close();
                    this.frm_chat_global = null;
                }
                for (i = this.chats_abiertos.Count - 1; i >= 0; i--)
                {
                    this.chats_abiertos.ToArray()[i].conectado = false;
                    this.Cerrar_Chat(this.chats_abiertos.ToArray()[i]);
                }
                this.chats_abiertos.Clear();
            }
            else
            {
                if (this.frm_chat_global != null)
                    this.frm_chat_global.conectado = false;
                for (i = this.chats_abiertos.Count - 1; i >= 0; i--)
                    this.chats_abiertos.ToArray()[i].conectado = false;
            }
            if ((this.lista_partidas.Count > 0) &&
                (MessageBox.Show(Mensajes.mensajeCerrarPartidasAbiertas, Mensajes.tituloConfirmacionDesconectar, MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                for (i = this.lista_partidas.Count - 1; i >= 0; i--)
                    this.Cerrar_Partida(this.lista_partidas.ToArray()[i]);
                this.lista_partidas.Clear();
            }
            else
            {
                for (i = this.lista_partidas.Count - 1; i >= 0; i--)
                {
                    this.lista_partidas.ToArray()[i].cerrando_por_desconexion = true;
                }
            }
            this.bDesconectar.PerformClick();
        }

        delegate void desactivar_envio_Callback();

        private void bDesconectar_Click(object sender, EventArgs e)
        {
            this.continuar_thread = false;
            if (this.conectado)
                this.enviar_comando("#disconnect#");
            Thread.Sleep(500);
            try
            {
                this.socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorDesconectar + ex.Message);
            }
            this.socket.Close();
            this.socket = null;
            this.conectado = false;
            this.bConectar.Enabled = true;
            this.bLogin.Enabled = false;
            this.bDesconectar.Enabled = false;
            this.bCrearPartida.Enabled = false;
            this.bCrearConv.Enabled = false;
            this.bChatGlobal.Enabled = false;
            this.txtLogin.Enabled = true;
            this.txtServidor.Enabled = true;
            this.txtPuerto.Enabled = true;
            this.Text = "Online";
            // Desactivar el botón Enviar de cada chat
            if (this.frm_chat_global != null)
                this.frm_chat_global.desactivar_envio();
            foreach (Chat chat in this.chats_abiertos)
                chat.BeginInvoke(new desactivar_envio_Callback(chat.desactivar_envio));
                //chat.desactivar_envio();
            foreach (PartidaOnline partida in this.lista_partidas)
                partida.interfaz.BeginInvoke(new Conexion_perdida_Callback(partida.interfaz.Conexion_perdida));
        }

        public string InputBox(string prompt, string title, string defaultValue)
        {
            InputBoxDialog ib = new InputBoxDialog();
            ib.FormPrompt = prompt;
            ib.FormCaption = title;
            ib.DefaultValue = defaultValue;
            ib.ShowDialog();
            string s = ib.InputResponse;
            ib.Close();
            return s;
        }

        private void bCrearPartida_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = this.InputBox(Mensajes.mensajeInputNombrePartida, Mensajes.tituloCrearPartida, "");
                if (nombre == "")
                {
                    MessageBox.Show(Mensajes.mensajeNombrePartidaEnBlanco, Mensajes.tituloCrearPartida);
                    return;
                }
                else if (nombre.Contains('~'))
                {
                    MessageBox.Show(Mensajes.mensajeNombrePartidaInvalido);
                    return;
                }
                string s_n_jugadores = this.InputBox(Mensajes.mensajeInputNumJugadoresPartida, Mensajes.tituloCrearPartida, "2");
                if (s_n_jugadores == "")
                {
                    MessageBox.Show(Mensajes.mensajeNumJugadoresEnBlanco, Mensajes.tituloCrearPartida);
                    return;
                }
                int n_jugadores;
                try
                {
                    n_jugadores = Convert.ToInt32(s_n_jugadores);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Mensajes.mensajeDatosIncorrectos + ex.Message);
                    return;
                }
                if (n_jugadores < 2)
                {
                    MessageBox.Show(Mensajes.mensajeNumMinimoJugadores);
                    return;
                }
                if (n_jugadores > 4)
                {
                    MessageBox.Show(Mensajes.mensajeNumMaximoJugadores);
                    return;
                }
                this.enviar_comando("create_game", nombre, n_jugadores.ToString());
                this.bUnirse.Enabled = false;
                this.bCrearPartida.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorConexion + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        private void txtServidor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bConectar.PerformClick();
        }

        private void txtPuerto_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bConectar.PerformClick();
        }

        private void txtLogin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bLogin.PerformClick();
        }

        private void bCrearConv_Click(object sender, EventArgs e)
        {
            if ((this.listaUsuarios.SelectedItems.Count < 1) ||
                (this.listaUsuarios.SelectedItems.Contains(this.txtLogin.Text)))
            {
                MessageBox.Show(Mensajes.mensajeSeleccionarUsuarios);
                return;
            }
            // Este mecanismo funciona así:
            // Se envía la longitud de la lista de usuarios y luego la lista para así enviarles la petición
            String[] parametros = new String[this.listaUsuarios.SelectedItems.Count + 1];
            int i = 1;
            parametros[0] = this.listaUsuarios.SelectedItems.Count.ToString();
            foreach (Object nombre in this.listaUsuarios.SelectedItems)
            {
                parametros[i] = nombre.ToString();
                i++;
            }
            this.enviar_comando("create_chat", parametros);
        }

        private void bChatGlobal_Click(object sender, EventArgs e)
        {
            this.frm_chat_global = new Chat(true, this);
            this.enviar_comando("join_global_chat");
            this.bChatGlobal.Enabled = false;
            frm_chat_global.Show();
        }

        public void chat_global_cerrado()
        {
            this.bChatGlobal.Enabled = true;
            this.frm_chat_global = null;
        }

        delegate void Reactivar_crear_Callback();

        private void Reactivar_crear()
        {
            if (this.bCrearPartida.InvokeRequired)
            {
                Reactivar_crear_Callback d = new Reactivar_crear_Callback(Reactivar_crear);
                this.Invoke(d);
            }
            else
            {
                this.bCrearPartida.Enabled = true;
            }
        }

        public void salir_de_partida()
        {
            this.Reactivar_crear();
        }

        private void Unirse_a_chat()
        {
            int bytes_recibidos = 0;
            int id_chat = recibir_int(this.socket, ref bytes_recibidos);
            int long_creador = recibir_int(this.socket, ref bytes_recibidos);
            String creador = recibir_string(this.socket, long_creador, ref bytes_recibidos);
            if (creador != this.txtLogin.Text)
            {
                if (MessageBox.Show(String.Format(Mensajes.mensajeUnirseAChat, creador), Mensajes.tituloNuevoChat, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.enviar_comando("join_chat", id_chat.ToString());
                    List<String> lista_params = new List<String>(2);
                    lista_params.Add(id_chat.ToString());
                    lista_params.Add(creador);
                    Thread thread_chat = new Thread(Manejar_nuevo_chat);
                    thread_chat.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                    thread_chat.Start(lista_params);
                }
            }
            else
            {
                this.enviar_comando("join_chat", id_chat.ToString());
                List<String> lista_params = new List<String>(2);
                lista_params.Add(id_chat.ToString());
                lista_params.Add(creador);
                Thread thread_chat = new Thread(Manejar_nuevo_chat);
                thread_chat.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                thread_chat.Start(lista_params);
            }
        }

        private void Unirse_a_partida(Boolean ok)
        {
            int bytes_recibidos = 0;
            int long_nombre = recibir_int(this.socket, ref bytes_recibidos);
            String nombre = recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            if (ok)
            {
                int id_chat = recibir_int(this.socket, ref bytes_recibidos);
                int long_creador = recibir_int(this.socket, ref bytes_recibidos);
                String creador = recibir_string(this.socket, long_creador, ref bytes_recibidos);
                int num_jugadores = recibir_int(this.socket, ref bytes_recibidos);
                List<String> lista_params = new List<String>(3);
                lista_params.Add(id_chat.ToString());
                lista_params.Add(creador);
                lista_params.Add(nombre);
                lista_params.Add(num_jugadores.ToString());
                Thread thread_partida = new Thread(Manejar_nueva_partida);
                thread_partida.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                thread_partida.Start(lista_params);
            }
            else
            {
                MessageBox.Show(String.Format(Mensajes.mensajePartidaLlena, nombre));
                this.Activar_bUnirse();
                this.Activar_bCrearPartida();
            }
        }

        private void Partida_empezada_o_terminada(Boolean empezada)
        {
            int bytes_recibidos = 0;
            int long_nombre = recibir_int(this.socket, ref bytes_recibidos);
            String nombre = recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            if (empezada)
            {
                MessageBox.Show(String.Format(Mensajes.mensajePartidaYaEmpezada, nombre));
                this.Activar_bUnirse();
                this.Activar_bCrearPartida();
            }
            else
            {
                MessageBox.Show(String.Format(Mensajes.mensajePartidaYaTerminada, nombre));
                this.Activar_bUnirse();
                this.Activar_bCrearPartida();
            }
        }

        private void No_unirse_a_partida()
        {
            int bytes_recibidos = 0;
            int long_nombre = recibir_int(this.socket, ref bytes_recibidos);
            String nombre = recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            MessageBox.Show(Mensajes.mensajeYaDentroPartida + nombre);
            this.Activar_bUnirse();
            this.Activar_bCrearPartida();
        }

        private void Manejar_nuevo_chat(object parametros)
        {
            List<String> lista_params = (List<String>) parametros;
            Chat chat = new Chat(false, this);
            chat.id = Convert.ToInt32(lista_params[0]);
            chat.creador = lista_params[1];
            this.chats_abiertos.AddFirst(chat);
            //chat.ShowDialog();
            Application.Run(chat);
        }

        private void Manejar_nueva_partida(object parametros)
        {
            List<String> lista_params = (List<String>)parametros;
            PartidaOnline partida = new PartidaOnline(this);
            partida.id = Convert.ToInt32(lista_params[0]);
            partida.creador = lista_params[1];
            partida.nombre = lista_params[2];
            partida.num_jugadores = Convert.ToInt32(lista_params[3]);
            this.lista_partidas.AddFirst(partida);
            //partida.ShowDialog();
            Application.Run(partida);
        }

        private Chat Buscar_chat(int id)
        {
            foreach (Chat chat in chats_abiertos)
            {
                if (chat.id == id)
                    return chat;
            }
            return null;
        }

        public PartidaOnline Buscar_partida(int id)
        {
            foreach (PartidaOnline partida in lista_partidas)
            {
                if (partida.id == id)
                    return partida;
            }
            return null;
        }

        private void rellenar_lista_chat()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            Chat chat = Buscar_chat(id);
            if (chat != null)
                chat.rellenar_lista();
            else //Es el chat de una partida creada
            {
                PartidaOnline partida = Buscar_partida(id);
                // El comando puede llegar desfasado y no existir la partida ya
                if (partida != null)
                    partida.rellenar_lista();
            }
        }

        private void esperar_comandos()
        {
            String msg = null;
            int bytes_recibidos = 0;
            int long_msg;
            do
            {
                long_msg = this.recibir_int(this.socket, ref bytes_recibidos);
                this.sem_en_comunicacion.WaitOne(10000);
                msg = this.recibir_string(this.socket, long_msg, ref bytes_recibidos);
                if (msg == "#disconnect#")
                    this.continuar_thread = false;
                else if (msg == "player_list")
                    this.Rellenar_lista_usuarios();
                else if (msg == "game_list")
                    this.Rellenar_lista_partidas();
                else if (msg == "global_chat_userlist")
                    this.frm_chat_global.rellenar_lista();
                else if (msg == "chat_userlist")
                    this.rellenar_lista_chat();
                else if (msg == "new_global_chat_msg")
                    this.Nuevo_mensaje_chat_global();
                else if (msg == "new_chat_msg")
                    this.Nuevo_mensaje_chat();
                else if (msg == "ask_join_chat")
                    this.Unirse_a_chat();
                else if (msg == "joined_game")
                    this.Unirse_a_partida(true);
                else if (msg == "cant_join_game_full")
                    this.Unirse_a_partida(false);
                else if (msg == "cant_join_game_started")
                    this.Partida_empezada_o_terminada(true);
                else if (msg == "cant_join_game_ended")
                    this.Partida_empezada_o_terminada(false);
                else if (msg == "cant_join_already_joined")
                    this.No_unirse_a_partida();
                else if (msg == "rolled_dice")
                    this.Dado_tirado();
                else if (msg == "rolled_construction_dice")
                    this.Dado_construccion_tirado();
                else if (msg == "game_started")
                    this.Iniciar_partida();
                else if (msg == "turn_passed")
                    this.Pasar_Turno();
                else if (msg == "update_player_money")
                    this.Actualizar_dinero_jugador();
                else if (msg == "hotel_purchased")
                    this.Hotel_comprado();
                else if (msg == "hotel_expropriated")
                    this.Hotel_expropiado();
                else if (msg == "phase_built")
                    this.Fase_construida();
                else if (msg == "entrance_added")
                    this.Entrada_añadida();
                else if (msg == "player_retired")
                    this.Jugador_retirado(false);
                else if (msg == "player_kicked")
                    this.Jugador_retirado(true);
                else if (msg == "game_ended")
                    this.Juego_terminado();
                else if (msg == "ask_pay_nights")
                    this.Pagar_noches();
                else if (msg == "auction_started")
                    this.Subasta_iniciada();
                else if (msg == "auction_bid_placed")
                    this.Nueva_puja();
                else if (msg == "auction_sold")
                    this.Subasta_vendida();
                else if (msg == "auction_ended")
                    this.Subasta_terminada();
                else if (msg == "allow_pass_turn")
                    this.PermitirPasarTurno();
                else
                {
                    if (msg == "")
                        MessageBox.Show(Mensajes.mensajeProblemaConexionConServidor);
                    else
                        MessageBox.Show(Mensajes.mensajeComandoDesconocido + ((msg != null) ? msg : ""));
                    this.conectado = false;
                    this.Finalizar_todas_las_partidas();
                    this.Pulsar_Desconectar();
                }
                msg = null;
                this.sem_en_comunicacion.Release();
            }
            while (this.continuar_thread);
        }

        public void enviar_comando(String comando, params String[] parametros)
        {
            List<String> lista_parametros = new List<String>();
            lista_parametros.Add(comando);
            foreach (String parametro in parametros)
                lista_parametros.Add(parametro);
            Thread thread_envio_comando = new Thread(enviar_comando_t);
            thread_envio_comando.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            thread_envio_comando.Start(lista_parametros);
        }

        private void enviar_comando_t(object lista_parametros)
        {
            this.sem_en_comunicacion.WaitOne(10000);
            List<String> lista = (List<String>)lista_parametros;
            String comando = lista[0];
            this.enviar_int(this.socket, comando.Length);
            this.enviar_string(this.socket, comando);
            // Comprobación de existencia de parámetros.
            // El protocolo es este: Si existen parámetros, se supone que son cadenas para enviar
            // Para cada cadena, primero se envía su longitud y luego la cadena en sí
            // Nos quitamos el primer elemento, que es el comando en sí
            // El bucle solo entrará si hay parámetros
            lista.RemoveAt(0);
            foreach (String parametro in lista)
            {
                this.enviar_int(this.socket, Encoding.UTF8.GetByteCount(parametro));
                this.enviar_string(this.socket, parametro);
            }
            this.sem_en_comunicacion.Release();
        }

        private void Nuevo_mensaje_chat_global()
        {
            try
            {
                // Primero se recibe la longitud de la cadena con el remitente
                int bytes_recibidos = 0;
                int long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                // Después se recibe la cadena con el remitente
                String remitente = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                // Después se recibe la longitud de la cadena con el mensaje
                long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                // Después se recibe el mensaje
                String msg = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                this.frm_chat_global.nuevo_mensaje(remitente, msg);
                remitente = null;
                msg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorRecibiendoChatGlobal + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        private void Nuevo_mensaje_chat()
        {
            try
            {
                // Primero se recibe el id del chat y después la longitud de la cadena con el remitente
                int bytes_recibidos = 0;
                int id = this.recibir_int(this.socket, ref bytes_recibidos);
                int long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                // Después se recibe la cadena con el remitente
                String remitente = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                // Después se recibe la longitud de la cadena con el mensaje
                long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                // Después se recibe el mensaje
                String msg = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                Chat chat = Buscar_chat(id);
                if (chat != null)
                    chat.nuevo_mensaje(remitente, msg);
                else //Es el chat de una partida creada
                {
                    PartidaOnline partida = Buscar_partida(id);
                    partida.nuevo_mensaje(remitente, msg);
                }
                remitente = null;
                msg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Mensajes.mensajeErrorRecibiendoChat + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        delegate void Pulsar_Desconectar_Callback();

        private void Pulsar_Desconectar()
        {
            if (this.bDesconectar.InvokeRequired)
            {
                this.BeginInvoke(new Pulsar_Desconectar_Callback(Pulsar_Desconectar));
            }
            else
            {
                this.bDesconectar.PerformClick();
            }
        }

        delegate void Activar_bUnirse_Callback();

        private void Activar_bUnirse()
        {
            if (this.bUnirse.InvokeRequired)
            {
                Activar_bUnirse_Callback d = new Activar_bUnirse_Callback(Activar_bUnirse);
                this.Invoke(d);
            }
            else
            {
                this.bUnirse.Enabled = true;
            }
        }

        delegate void Activar_bCrearPartida_Callback();

        private void Activar_bCrearPartida()
        {
            if (this.bUnirse.InvokeRequired)
            {
                Activar_bCrearPartida_Callback d = new Activar_bCrearPartida_Callback(Activar_bCrearPartida);
                this.Invoke(d);
            }
            else
            {
                this.bCrearPartida.Enabled = true;
            }
        }

        private void bUnirse_Click(object sender, EventArgs e)
        {
            String linea = this.listaPartidas.SelectedItem.ToString();
            this.enviar_comando("join_game", linea.Substring(0, linea.IndexOf(" (")));
            this.bUnirse.Enabled = false;
            this.bCrearPartida.Enabled = false;
        }

        private void listaPartidas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listaPartidas.SelectedIndex < 0)
                this.bUnirse.Enabled = false;
            else
                this.bUnirse.Enabled = true;
        }

        private void listaPartidas_DoubleClick(object sender, EventArgs e)
        {
            this.bUnirse.PerformClick();
        }

        delegate void Tirar_Dado_Callback(Jugador jugador);

        private void Dado_tirado()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int res_dado = this.recibir_int(this.socket, ref bytes_recibidos);
            int auto_avance = this.recibir_int(this.socket, ref bytes_recibidos);
            int pos = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            partida.interfaz.juego.ultimo_res_dado = res_dado;
            partida.interfaz.juego.ultimo_avance_auto = auto_avance;
            jugador.posicion.ocupada = false;
            jugador.posicion = partida.interfaz.juego.casillas[pos];
            jugador.posicion.ocupada = true;
            partida.interfaz.BeginInvoke(new Tirar_Dado_Callback(partida.interfaz.Tirar_dado), new object[] { jugador });
        }

        private void Dado_construccion_tirado()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            this.ultimo_res_dado_cons = (Tipos.Resultado_dado_cons) this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            partida.interfaz.juego.sem_dado_cons.Release();
            Thread.Sleep(200);
            partida.interfaz.juego.sem_dado_cons.Close();
            partida.interfaz.juego.sem_dado_cons = new Semaphore(0, 1);
        }

        private void Iniciar_partida()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int num_jugadores = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_config = this.recibir_int(this.socket, ref bytes_recibidos);
            String config = this.recibir_string(this.socket, long_config, ref bytes_recibidos);
            int jug_inicial = this.recibir_int(this.socket, ref bytes_recibidos);
            int cuantos = this.recibir_int(this.socket, ref bytes_recibidos);
            int i;
            int long_nombre;
            String[] lista_jugadores = new String[cuantos];
            for (i = 0; i < cuantos; i++)
            {
                long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
                lista_jugadores[i] = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            }
            this.Buscar_partida(id).Iniciar(num_jugadores, config, jug_inicial, lista_jugadores);
        }

        delegate void Pasar_Turno_Callback(String jugador);

        private void Pasar_Turno()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String sig_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            partida.interfaz.BeginInvoke(new Pasar_Turno_Callback(partida.interfaz.Pasar_turno), new object[] { sig_jugador });
        }

        delegate void Actualizar_Dinero_Jugador_Callback(String jugador, int n_50, int n_100, int n_500, int n_1000, int n_5000);

        private void Actualizar_dinero_jugador()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            int n_50 = this.recibir_int(this.socket, ref bytes_recibidos);
            int n_100 = this.recibir_int(this.socket, ref bytes_recibidos);
            int n_500 = this.recibir_int(this.socket, ref bytes_recibidos);
            int n_1000 = this.recibir_int(this.socket, ref bytes_recibidos);
            int n_5000 = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            partida.interfaz.BeginInvoke(new Actualizar_Dinero_Jugador_Callback(partida.interfaz.Actualizar_Dinero_Jugador), jugador, n_50, n_100, n_500, n_1000, n_5000);
        }

        private void Hotel_comprado()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_hotel = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Hotel hotel = partida.interfaz.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre_txt == nombre_hotel);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            hotel.dueño = jugador;
            jugador.hoteles.AddLast(hotel);
            jugador.n_hoteles++;
        }

        private void Hotel_expropiado()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_hotel = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Hotel hotel = partida.interfaz.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre_txt == nombre_hotel);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            hotel.dueño.Hotel_Expropiado(ref hotel);
            hotel.dueño = jugador;
            jugador.hoteles.AddLast(hotel);
            jugador.n_hoteles++;
        }

        delegate void Dibujar_Fase_Callback(Hotel hotel, int n_50);

        private void Fase_construida()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_hotel = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Hotel hotel = partida.interfaz.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre_txt == nombre_hotel);
            hotel.Ampliar();
            partida.interfaz.BeginInvoke(new Dibujar_Fase_Callback(partida.interfaz.Dibujar_Fase), hotel, hotel.n_fases_construidas - 1);
        }

        delegate void Dibujar_Entrada_Callback(Casilla casilla, Boolean en_la_derecha);

        private void Entrada_añadida()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_hotel = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            int casilla = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Hotel hotel = partida.interfaz.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre_txt == nombre_hotel);
            if (partida.interfaz.juego.casillas[casilla].hotel_der == hotel.nombre)
            {
                partida.interfaz.juego.casillas[casilla].entrada_en_der = true;
                partida.interfaz.BeginInvoke(new Dibujar_Entrada_Callback(partida.interfaz.Dibujar_Entrada), partida.interfaz.juego.casillas[casilla], true);
            }
            else
            {
                partida.interfaz.juego.casillas[casilla].entrada_en_izq = true;
                partida.interfaz.BeginInvoke(new Dibujar_Entrada_Callback(partida.interfaz.Dibujar_Entrada), partida.interfaz.juego.casillas[casilla], false);
            }
            hotel.n_entradas++;
            hotel.entradas.AddLast(partida.interfaz.juego.casillas[casilla]);
        }

        delegate void Marcar_Jugador_Eliminado_Callback(int jugador);

        private void Jugador_retirado(Boolean expulsado)
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            if (partida.interfaz == null) // La partida no fue iniciada
                return;
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            if (jugador.Eliminado())
                return;
            partida.interfaz.juego.Eliminar_Jugador(jugador, null);
            if (jugador.nombre_online != partida.interfaz.nombre_online)
            {
                String color = jugador.Nombre_color();
                if (expulsado)
                    MessageBox.Show(String.Format(Mensajes.mensajeJugadorExpulsadoTrampas, Char.ToUpper(color[0]) + color.Substring(1), jugador.nombre_online));
                else
                    MessageBox.Show(String.Format(Mensajes.mensajeJugadorRetirado, Char.ToUpper(color[0]) + color.Substring(1), jugador.nombre_online));
            }
            partida.interfaz.BeginInvoke(new Marcar_Jugador_Eliminado_Callback(partida.interfaz.Marcar_Jugador_Eliminado), jugador.n_jugador);
        }

        delegate void Finalizar_Partida_Callback(Jugador jugador);

        private void Juego_terminado()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            if (!jugador.Eliminado())
                partida.interfaz.BeginInvoke(new Finalizar_Partida_Callback(partida.interfaz.Finalizar_Partida), jugador);
        }

        delegate void Pedir_Noches_Online_Callback(Jugador jugador, int cantidad);

        private void Pagar_noches()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            int cantidad = this.recibir_int(this.socket, ref bytes_recibidos);
            int noches = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            MessageBox.Show(String.Format(Mensajes.mensajePagarNoches, noches, cantidad, jugador.Nombre_color(), jugador.nombre_online));
            partida.interfaz.BeginInvoke(new Pedir_Noches_Online_Callback(partida.interfaz.Pedir_Noches_Online), jugador, cantidad);
        }

        delegate void Conexion_perdida_Callback();

        private void Finalizar_todas_las_partidas()
        {
            foreach (PartidaOnline partida in this.lista_partidas)
                if (partida.interfaz != null)
                    partida.interfaz.BeginInvoke(new Conexion_perdida_Callback(partida.interfaz.Conexion_perdida));
        }

        private void Subasta_iniciada()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_hotel = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            partida.interfaz.hotel_a_subastar_online = partida.interfaz.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre_txt == nombre_hotel);
            if (partida.interfaz.juego.jugador_actual.nombre_online != partida.interfaz.nombre_online) // El jugador actual ya tiene la ventana abierta
            {
                Thread thread_manejar_subasta = new Thread(Manejar_subasta);
                thread_manejar_subasta.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                thread_manejar_subasta.Start(partida);
            }
        }

        private void Manejar_subasta(object parametro)
        {
            PartidaOnline partida = (PartidaOnline)parametro;
            Juego juego = partida.interfaz.juego;
            partida.interfaz.frm_subasta_en_curso = new Subastas(ref juego, partida.interfaz, true);
            //partida.interfaz.frm_subasta_en_curso.ShowDialog();
            Application.Run(partida.interfaz.frm_subasta_en_curso);
        }

        delegate void Nueva_puja_Callback(Jugador jugador, int cantidad);

        private void Nueva_puja()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int long_nombre = this.recibir_int(this.socket, ref bytes_recibidos);
            String nombre_jugador = this.recibir_string(this.socket, long_nombre, ref bytes_recibidos);
            int cantidad = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            partida.interfaz.frm_subasta_en_curso.BeginInvoke(new Nueva_puja_Callback(partida.interfaz.frm_subasta_en_curso.Nueva_puja), jugador, cantidad);
        }

        delegate void Subasta_vendida_Callback();

        private void Subasta_vendida()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            int cantidad = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            Jugador jugador = partida.interfaz.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == this.txtLogin.Text);
            if (jugador.n_jugador == partida.interfaz.frm_subasta_en_curso.n_mayor_postor)
            {
                Juego juego = partida.interfaz.juego;
                PedirPago frm_pago = new PedirPago(cantidad, ref juego, true, jugador, partida.interfaz);
                frm_pago.ShowDialog();
                int n_5000 = frm_pago.n_5000, n_1000 = frm_pago.n_1000, n_500 = frm_pago.n_500, n_100 = frm_pago.n_100, n_50 = frm_pago.n_50;
                this.enviar_comando("auction_pay", id.ToString(), n_5000.ToString(), n_1000.ToString(),
                    n_500.ToString(), n_100.ToString(), n_50.ToString());
                frm_pago.Close();
            }
            else
            {
                partida.interfaz.frm_subasta_en_curso.BeginInvoke(new Subasta_vendida_Callback(partida.interfaz.frm_subasta_en_curso.Subasta_vendida));
                MessageBox.Show(String.Format(Mensajes.mensajeHotelVendido, partida.interfaz.juego.jugadores[partida.interfaz.frm_subasta_en_curso.n_mayor_postor].Nombre_color(), 
                    partida.interfaz.juego.jugadores[partida.interfaz.frm_subasta_en_curso.n_mayor_postor].nombre_online, cantidad));
            }
        }

        delegate void Subasta_terminada_Callback();

        private void Subasta_terminada()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            //MessageBox.Show("");
            partida.interfaz.frm_subasta_en_curso.BeginInvoke(new Subasta_terminada_Callback(partida.interfaz.frm_subasta_en_curso.Subasta_terminada));
        }

        private void checkSrvOficial_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkSrvOficial.Checked)
            {
                this.txtServidor.Enabled = false;
                this.txtPuerto.Enabled = false;
            }
            else
            {
                this.txtServidor.Enabled = true;
                this.txtPuerto.Enabled = true;
            }
        }

        delegate void PermitirPasarTurno_Callback();

        private void PermitirPasarTurno()
        {
            int bytes_recibidos = 0;
            int id = this.recibir_int(this.socket, ref bytes_recibidos);
            PartidaOnline partida = this.Buscar_partida(id);
            partida.interfaz.BeginInvoke(new PermitirPasarTurno_Callback(partida.interfaz.PermitirPasarTurno));
        }

        delegate void ReLocalize_Callback(System.Globalization.CultureInfo nuevoCulture, System.Globalization.CultureInfo antiguoCulture);

        public void ReLocalize(System.Globalization.CultureInfo nuevoCulture, System.Globalization.CultureInfo antiguoCulture)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new ReLocalize_Callback(this.ReLocalize), new object[] { nuevoCulture, antiguoCulture });
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = nuevoCulture;
                resources.ApplyResources(this, "$this");
                foreach (Control c in this.Controls)
                {
                    if (c is GroupBox)
                    {
                        c.Text = resources.GetString(c.Name + ".Text");
                        foreach (Control o in ((GroupBox)c).Controls)
                        {
                            if (o is Label)
                            {
                                String nombreAntiguo = (String)resources.GetObject(o.Name + ".Text", antiguoCulture);
                                if (nombreAntiguo != null)
                                    o.Text = o.Text.Replace(nombreAntiguo, resources.GetString(o.Name + ".Text"));
                            }
                            else
                                resources.ApplyResources(o, o.Name);
                        }
                    }
                    else if (c is Label)
                    {
                        String nombreAntiguo = (String)resources.GetObject(c.Name + ".Text", antiguoCulture);
                        if (nombreAntiguo != null)
                            c.Text = c.Text.Replace(nombreAntiguo, resources.GetString(c.Name + ".Text"));
                    }
                    else
                        c.Text = resources.GetString(c.Name + ".Text");
                }
            }
        }
    }
}