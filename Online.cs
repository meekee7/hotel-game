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

namespace Juego_Hotel
{
    public partial class Online : Form
    {
        public Principal interfaz;
        public Socket socket;

        public Online(Principal interfaz)
        {
            InitializeComponent();
            this.interfaz = interfaz;
        }

        public String recibir_string(Socket s, int longitud, ref int bytes_recibidos)
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

        public int recibir_int(Socket s, ref int bytes_recibidos)
        {
            byte[] b_int = new byte[4];
            bytes_recibidos = s.Receive(b_int);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b_int);
            return BitConverter.ToInt32(b_int, 0);
        }

        public int enviar_string(Socket s, String texto)
        {
            return s.Send(Encoding.UTF8.GetBytes(texto));
        }

        public int enviar_int(Socket s, int num)
        {
            byte[] b_int = BitConverter.GetBytes(num);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b_int);
            return s.Send(b_int);
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
                            this.Rellenar_lista_usuarios();
                            this.Rellenar_lista_partidas();
                            this.bLogin.Enabled = false;
                            this.bCrearPartida.Enabled = true;
                            this.bCrearConv.Enabled = true;
                            this.txtLogin.Enabled = false;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error conectando: " + ex.Message);
            }
        }

        private void Rellenar_lista_usuarios()
        {
            try
            {
                this.enviar_int(this.socket, 9);
                this.enviar_string(this.socket, "get_users");
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                int bytes_recibidos = 0;
                int long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                String s_lista_jugadores = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                String[] lista_jugadores = s_lista_jugadores.Split('~');
                this.listaUsuarios.BeginUpdate();
                this.listaUsuarios.Items.Clear();
                foreach (String nombre in lista_jugadores)
                    this.listaUsuarios.Items.Add(nombre);
                this.listaUsuarios.EndUpdate();
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

        private void Rellenar_lista_partidas()
        {
            try
            {
                this.enviar_int(this.socket, 9);
                this.enviar_string(this.socket, "get_games");
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                int bytes_recibidos = 0;
                int long_cadena = this.recibir_int(this.socket, ref bytes_recibidos);
                if (long_cadena == 0)
                {
                    this.listaPartidas.Items.Clear();
                    return;
                }
                String s_lista_partidas = this.recibir_string(this.socket, long_cadena, ref bytes_recibidos);
                String[] lista_partidas = s_lista_partidas.Split('~');
                this.listaPartidas.BeginUpdate();
                this.listaPartidas.Items.Clear();
                foreach (String nombre in lista_partidas)
                    this.listaPartidas.Items.Add(nombre);
                this.listaPartidas.EndUpdate();
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
            this.refrescoListas.Stop();
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
                this.Rellenar_lista_partidas();
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
            this.Rellenar_lista_usuarios();
            this.Rellenar_lista_partidas();
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
        }

        private void bChatGlobal_Click(object sender, EventArgs e)
        {
            Chat frm_chat = new Chat(true, this);
            enviar_int(this.socket, 9);
            enviar_string(this.socket, "join_chat");
            this.bChatGlobal.Enabled = false;
            frm_chat.rellenar_lista();
            frm_chat.Show();
        }

        public void chat_cerrado()
        {
            this.bChatGlobal.Enabled = true;
        }
    }
}
