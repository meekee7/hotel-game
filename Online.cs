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

namespace Juego_Hotel
{
    public partial class Online : Form
    {
        public Principal interfaz;
        public Socket socket;
        Chat frm_chat_global;
        public LinkedList<Chat> chats_abiertos;
        public LinkedList<PartidaOnline> lista_partidas;
        Thread thread_recepcion;
        Semaphore sem_en_comunicacion;
        Boolean continuar_thread, conectado = false;

        public Online(Principal interfaz)
        {
            InitializeComponent();
            this.interfaz = interfaz;
            this.chats_abiertos = new LinkedList<Chat>();
            this.lista_partidas = new LinkedList<PartidaOnline>();
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
                    MessageBox.Show("Excepción recibiendo datos: " + e.Message);
                }
                return null;
            }
        }

        public int recibir_int(Socket s, ref int bytes_recibidos)
        {
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
                    MessageBox.Show("Excepción recibiendo datos: " + e.Message);
                }
                return 0;
            }
        }

        public int enviar_string(Socket s, String texto)
        {
            try
            {
                return s.Send(Encoding.UTF8.GetBytes(texto));
            }
            catch (Exception e)
            {
                if (this.continuar_thread == true)
                {
                    this.Pulsar_Desconectar();
                    MessageBox.Show("Excepción recibiendo datos: " + e.Message);
                }
                return 0;
            }
        }

        public int enviar_int(Socket s, int num)
        {
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
                    MessageBox.Show("Excepción recibiendo datos: " + e.Message);
                }
                return 0;
            }
        }

        private void bLogin_Click(object sender, EventArgs e)
        {
            if (socket.Connected)
            {
                if (this.txtLogin.Text.Length == 0)
                    MessageBox.Show("El apodo no puede estar vacío");
                else if (this.txtLogin.Text.Length > 20)
                    MessageBox.Show("El apodo no puede superar 20 caracteres");
                else if (this.txtLogin.Text.Contains('~'))
                    MessageBox.Show("El apodo no puede contener el carácter '~'");
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
                            MessageBox.Show("El apodo ya está en uso");
                            this.bDesconectar.PerformClick();
                            this.txtLogin.Enabled = true;
                        }
                        else if (res_login == "login no")
                        {
                            MessageBox.Show("El apodo contiene el carácter '~' y no es válido");
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
                            thread_recepcion.Start();
                            Thread.Sleep(200);
                            this.refrescoListas.Start();
                            this.enviar_comando("get_users");
                            this.enviar_comando("get_games");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error haciendo login: " + ex.Message);
                        this.Pulsar_Desconectar();
                    }
                }
            }
            else
                MessageBox.Show("No estás conectado");
        }

        private void bConectar_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress dir = Dns.GetHostAddresses(this.txtServidor.Text).First(IPAddress => IPAddress.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint Ep = new IPEndPoint(dir, 12345);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(Ep);
                this.bLogin.Enabled = true;
                this.bConectar.Enabled = false;
                this.bDesconectar.Enabled = true;
                this.txtServidor.Enabled = false;
                this.txtLogin.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error conectando: " + ex.Message);
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
            try
            {
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                int bytes_recibidos = 0;
                int long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                String s_lista_jugadores = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                String[] lista_jugadores = s_lista_jugadores.Split('~');
                this.Actualizar_lista_usuarios(lista_jugadores);
                s_lista_jugadores = null;
                lista_jugadores = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo lista de usuarios: " + ex.Message);
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
            try
            {
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                int bytes_recibidos = 0;
                int long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                if (long_cadena == 0)
                {
                    this.Borrar_lista_partidas();
                    return;
                }
                String s_lista_partidas = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                String[] lista_partidas = s_lista_partidas.Split('~');
                this.Actualizar_lista_partidas(lista_partidas);
                s_lista_partidas = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo lista de partidas: " + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        private void Online_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.bDesconectar.PerformClick();
            if (((this.frm_chat_global != null) ||
                (this.chats_abiertos.Count > 0)) &&
                (MessageBox.Show("¿Quieres que se cierre cualquier chat abierto?", "Confirmación para desconectar", MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                if (this.frm_chat_global != null)
                {
                    this.frm_chat_global.Close();
                    this.frm_chat_global = null;
                }
                foreach (Chat chat in this.chats_abiertos)
                    chat.Close();
                this.chats_abiertos.Clear();
            }
            this.interfaz.bOnline.Enabled = true;
        }

        private void bDesconectar_Click(object sender, EventArgs e)
        {
            this.continuar_thread = false;
            this.refrescoListas.Stop();
            if (this.conectado)
                this.enviar_comando("#disconnect#");
            Thread.Sleep(500);
            try
            {
                this.socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desconectar: " + ex.Message);
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
            // Desactivar el botón Enviar de cada chat
            if (this.frm_chat_global != null)
                this.frm_chat_global.desactivar_envio();
            foreach (Chat chat in this.chats_abiertos)
                chat.desactivar_envio();
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
                string nombre = this.InputBox("Nombre de la partida:", "Crear partida", "");
                if (nombre == "")
                    return;
                else if (nombre.Contains('~'))
                {
                    MessageBox.Show("El nombre de la partida no puede contener el carácter '~'");
                    return;
                }
                string s_n_jugadores = this.InputBox("Número de jugadores de la partida:", "Crear partida", "");
                if (s_n_jugadores == "")
                    return;
                int n_jugadores;
                try
                {
                    n_jugadores = Convert.ToInt32(s_n_jugadores);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Datos incorrectos: " + ex.Message);
                    return;
                }
                if (n_jugadores < 2)
                {
                    MessageBox.Show("El número mínimo de jugadores es 2");
                    return;
                }
                if (n_jugadores > 4)
                {
                    MessageBox.Show("El número máximo de jugadores es 4");
                    return;
                }
                this.enviar_comando("create_game", nombre, n_jugadores.ToString());
                this.bUnirse.Enabled = false;
                this.bCrearPartida.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        private void txtServidor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bConectar.PerformClick();
        }

        private void txtLogin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bLogin.PerformClick();
        }

        private void refrescoListas_Tick(object sender, EventArgs e)
        {
            this.enviar_comando("get_users");
            this.enviar_comando("get_games");
        }

        private void bCrearConv_Click(object sender, EventArgs e)
        {
            if ((this.listaUsuarios.SelectedItems.Count < 1) ||
                (this.listaUsuarios.SelectedItems.Contains(this.txtLogin.Text)))
            {
                MessageBox.Show("Has de seleccionar al menos un usuario sin incluir el tuyo");
                return;
            }
            // Este mecanismo funciona así:
            // Se crea una cadena con todos los usuarios seleccionados separados con un '~' para así enviarles la petición
            String lista = this.txtLogin.Text.ToString();
            foreach (Object nombre in this.listaUsuarios.SelectedItems)
            {
                lista += '~' + nombre.ToString();
            }
            this.enviar_comando("create_chat", lista);
        }

        private void bChatGlobal_Click(object sender, EventArgs e)
        {
            this.frm_chat_global = new Chat(true, this);
            this.enviar_comando("join_global_chat");
            this.bChatGlobal.Enabled = false;
            this.enviar_comando("get_global_chat_users");
            frm_chat_global.Show();
        }

        public void chat_global_cerrado()
        {
            this.bChatGlobal.Enabled = true;
            this.frm_chat_global = null;
        }

        delegate void Reactivar_crear_unirse_Callback();

        private void Reactivar_crear_unirse()
        {
            if (this.bUnirse.InvokeRequired || this.bCrearPartida.InvokeRequired)
            {
                Reactivar_crear_unirse_Callback d = new Reactivar_crear_unirse_Callback(Reactivar_crear_unirse);
                this.Invoke(d);
            }
            else
            {
                this.bCrearPartida.Enabled = true;
                this.bUnirse.Enabled = true;
            }
        }

        public void salir_de_partida()
        {
            this.Reactivar_crear_unirse();
        }

        private void Unirse_a_chat()
        {
            int bytes_recibidos = 0;
            int id_chat = recibir_int(this.socket, ref bytes_recibidos);
            int long_creador = recibir_int(this.socket, ref bytes_recibidos);
            String creador = recibir_string(this.socket, long_creador, ref bytes_recibidos);
            if (creador != this.txtLogin.Text)
            {
                if (MessageBox.Show("El jugador " + creador + " quiere que te unas a un chat privado. ¿Deseas hacerlo?", "Nuevo chat", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.enviar_comando("join_chat", id_chat.ToString());
                    List<String> lista_params = new List<String>(2);
                    lista_params.Add(id_chat.ToString());
                    lista_params.Add(creador);
                    Thread thread_chat = new Thread(Manejar_nuevo_chat);
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
                List<String> lista_params = new List<String>(3);
                lista_params.Add(id_chat.ToString());
                lista_params.Add(creador);
                lista_params.Add(nombre);
                Thread thread_partida = new Thread(Manejar_nueva_partida);
                thread_partida.Start(lista_params);
            }
            else
            {
                MessageBox.Show("La partida " + nombre + " está llena");
                this.Activar_bUnirse();
                this.Activar_bCrearPartida();
            }
        }

        private void Manejar_nuevo_chat(object parametros)
        {
            List<String> lista_params = (List<String>) parametros;
            Chat chat = new Chat(false, this);
            chat.id = Convert.ToInt32(lista_params[0]);
            chat.creador = lista_params[1];
            this.chats_abiertos.AddFirst(chat);
            chat.ShowDialog();
        }

        private void Manejar_nueva_partida(object parametros)
        {
            List<String> lista_params = (List<String>)parametros;
            PartidaOnline partida = new PartidaOnline(this);
            partida.id = Convert.ToInt32(lista_params[0]);
            partida.creador = lista_params[1];
            partida.nombre = lista_params[2];
            this.lista_partidas.AddFirst(partida);
            partida.ShowDialog();
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

        private PartidaOnline Buscar_partida(int id)
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
                partida.rellenar_lista();
            }
        }

        private void esperar_comandos()
        {
            String msg = null;
            int bytes_recibidos = 0;
            do
            {
                int long_msg = this.recibir_int(this.socket, ref bytes_recibidos);
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
                msg = null;
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
                this.enviar_int(this.socket, parametro.Length);
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
                MessageBox.Show("Error recibiendo nuevo mensaje de chat global: " + ex.Message);
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
                MessageBox.Show("Error recibiendo nuevo mensaje de chat global: " + ex.Message);
                this.Pulsar_Desconectar();
            }
        }

        delegate void Pulsar_Desconectar_Callback();

        private void Pulsar_Desconectar()
        {
            if (this.bDesconectar.InvokeRequired)
            {
                Pulsar_Desconectar_Callback d = new Pulsar_Desconectar_Callback(Pulsar_Desconectar);
                this.Invoke(d);
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
            this.enviar_comando("join_game", this.listaPartidas.SelectedItem.ToString());
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
    }
}
