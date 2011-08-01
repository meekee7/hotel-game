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
        LinkedList<Chat> chats_abiertos;
        Thread thread_recepcion;
        bool continuar_thread;
        //static readonly object en_comunicacion = new object();
        //Mutex mut_en_comunicacion;
        Semaphore sem_en_comunicacion;
        //static bool en_comunicacion;

        public Online(Principal interfaz)
        {
            InitializeComponent();
            this.interfaz = interfaz;
            this.chats_abiertos = new LinkedList<Chat>();
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
                MessageBox.Show("Excepción recibiendo datos: " + e.Message);
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
                MessageBox.Show("Excepción recibiendo datos: " + e.Message);
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
                MessageBox.Show("Excepción recibiendo datos: " + e.Message);
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
                MessageBox.Show("Excepción recibiendo datos: " + e.Message);
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
                        else
                        {
                            this.bLogin.Enabled = false;
                            this.bCrearPartida.Enabled = true;
                            this.bCrearConv.Enabled = true;
                            this.txtLogin.Enabled = false;
                            //this.mut_en_comunicacion = new Mutex();
                            this.sem_en_comunicacion = new Semaphore(1, 1);
                            thread_recepcion = new Thread(esperar_mensajes);
                            this.continuar_thread = true;
                            thread_recepcion.Start();
                            Thread.Sleep(200);
                            this.refrescoListas.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error haciendo login: " + ex.Message);
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
                this.txtLogin.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error conectando: " + ex.Message);
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
                this.listaUsuarios.Items.Clear();
                this.bDesconectar.PerformClick();
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
                this.listaPartidas.Items.Clear();
                this.listaUsuarios.Items.Clear();
                this.bDesconectar.PerformClick();
            }
        }

        private void Online_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.bDesconectar.PerformClick();
        }

        private void bDesconectar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Quieres que se cierre cualquier chat abierto?", "Confirmación para desconectar") == DialogResult.Yes)
            {
                this.frm_chat_global.Close();
                foreach (Chat chat in this.chats_abiertos)
                    chat.Close();
                this.chats_abiertos.Clear();
            }
            else // Desactivar el botón Enviar de cada chat
            {
                if (this.frm_chat_global != null)
                    this.frm_chat_global.desactivar_envio();
                foreach (Chat chat in this.chats_abiertos)
                    chat.desactivar_envio();
            }
            this.refrescoListas.Stop();
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
            this.bConectar.Enabled = true;
            this.bLogin.Enabled = false;
            this.bDesconectar.Enabled = false;
            this.bCrearPartida.Enabled = false;
            this.bCrearConv.Enabled = false;
            this.bChatGlobal.Enabled = false;
            this.txtLogin.Enabled = true;
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
                this.enviar_int(this.socket, 11);
                this.enviar_string(this.socket, "create_game");
                this.enviar_int(this.socket, nombre.Length);
                this.enviar_string(this.socket, nombre);
                this.enviar_int(this.socket, n_jugadores);
                //this.Rellenar_lista_partidas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
                this.bDesconectar.PerformClick();
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
            Chat frm_chat = new Chat(false);
            if ((this.listaUsuarios.SelectedItems.Count < 1) ||
                ((this.listaUsuarios.SelectedItems.Count == 1) && (this.listaUsuarios.SelectedItem.ToString() == this.txtLogin.Text)))
            {
                MessageBox.Show("Has de seleccionar al menos un usuario diferente del tuyo");
                return;
            }
            frm_chat.añadir_jugador(this.txtLogin.Text.ToString());
            foreach (Object nombre in this.listaUsuarios.SelectedItems)
            {
                if (nombre.ToString() != this.txtLogin.Text)
                    frm_chat.añadir_jugador(nombre.ToString());
            }
            frm_chat.Show();
            this.chats_abiertos.AddFirst(frm_chat);
        }

        private void bChatGlobal_Click(object sender, EventArgs e)
        {
            this.frm_chat_global = new Chat(true, this);
            enviar_int(this.socket, 9);
            enviar_string(this.socket, "join_chat");
            this.bChatGlobal.Enabled = false;
            frm_chat_global.rellenar_lista();
            frm_chat_global.Show();
        }

        public void chat_global_cerrado()
        {
            this.bChatGlobal.Enabled = true;
        }

        private void esperar_mensajes()
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
            thread_envio_comando.Start(lista_parametros);
        }

        private void enviar_comando_t(object lista_parametros)
        {
            this.sem_en_comunicacion.WaitOne();
            List<String> lista = (List<String>)lista_parametros;
            String comando = lista[0];
            this.enviar_int(this.socket, comando.Length);
            this.enviar_string(this.socket,comando);
        }
    }
}
