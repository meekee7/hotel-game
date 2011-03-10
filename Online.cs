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
        Principal interfaz;
        Socket socket;
        public Online(Principal interfaz)
        {
            InitializeComponent();
            this.interfaz = interfaz;
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
                    socket.Send(Encoding.UTF8.GetBytes(this.txtLogin.Text.ToCharArray()));
                    byte[] data = new byte[20];
                    int bytes_recibidos = socket.Receive(data);
                    if (System.Text.Encoding.UTF8.GetString(data, 0, bytes_recibidos) == "username in use")
                    {
                        MessageBox.Show("El apodo ya está en uso");
                    }
                    else
                    {
                        this.Rellenar_lista_usuarios();
                        this.Rellenar_lista_partidas();
                        this.bLogin.Enabled = false;
                        this.bCrearPartida.Enabled = true;
                        this.bChatear.Enabled = true;
                    }
                    data = null;
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
                socket.Send(Encoding.UTF8.GetBytes("get_users"));
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                byte[] b_long_cadena = new byte[4];
                socket.Receive(b_long_cadena);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(b_long_cadena);
                int long_cadena = BitConverter.ToInt32(b_long_cadena, 0);
                byte[] b_lista_jugadores = new byte[long_cadena];
                int bytes_recibidos;
                bytes_recibidos = socket.Receive(b_lista_jugadores);
                string s_lista_jugadores = Encoding.UTF8.GetString(b_lista_jugadores, 0, bytes_recibidos);
                string[] lista_jugadores = s_lista_jugadores.Split('~');
                this.listaUsuarios.BeginUpdate();
                this.listaUsuarios.Items.Clear();
                foreach (string nombre in lista_jugadores)
                    this.listaUsuarios.Items.Add(nombre);
                this.listaUsuarios.EndUpdate();
                b_long_cadena = null;
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
                this.socket.Send(Encoding.UTF8.GetBytes("get_games"));
                // Primero se recibe la longitud de la cadena de usuarios aplanada
                byte[] b_long_cadena = new byte[4];
                socket.Receive(b_long_cadena);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(b_long_cadena);
                int long_cadena = BitConverter.ToInt32(b_long_cadena, 0);
                if (long_cadena == 0)
                {
                    b_long_cadena = null;
                    return;
                }
                byte[] b_lista_partidas = new byte[long_cadena];
                int bytes_recibidos;
                bytes_recibidos = socket.Receive(b_lista_partidas);
                string s_lista_partidas = Encoding.UTF8.GetString(b_lista_partidas, 0, bytes_recibidos);
                string[] lista_partidas = s_lista_partidas.Split('~');
                this.listaPartidas.BeginUpdate();
                this.listaPartidas.Items.Clear();
                foreach (string nombre in lista_partidas)
                    this.listaPartidas.Items.Add(nombre);
                this.listaPartidas.EndUpdate();
                b_lista_partidas = null;
                s_lista_partidas = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo lista de partidas: " + ex.Message);
                this.listaPartidas.Items.Clear();
                this.bDesconectar.PerformClick();
            }
        }

        private void Online_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.bDesconectar.PerformClick();
        }

        private void bDesconectar_Click(object sender, EventArgs e)
        {
            if (this.socket.Connected)
            {
                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al desconectar: " + ex.Message);
                    this.socket.Close();
                    this.socket = null;
                    this.bConectar.Enabled = true;
                    this.bDesconectar.Enabled = false;
                }
                this.socket.Close();
                this.socket = null;
                this.bConectar.Enabled = true;
                this.bDesconectar.Enabled = false;
                this.bCrearPartida.Enabled = false;
                this.bChatear.Enabled = false;
            }
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
                string s_n_jugadores = this.InputBox("Número de jugadores de la partida:", "Crear partida", "");
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
                MessageBox.Show(this.socket.Send(Encoding.UTF8.GetBytes("create_game")).ToString());
                MessageBox.Show(this.socket.Send(Encoding.UTF8.GetBytes(nombre.ToCharArray())).ToString());
                byte[] b_n_jugadores = BitConverter.GetBytes(n_jugadores);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(b_n_jugadores);
                MessageBox.Show(this.socket.Send(b_n_jugadores).ToString());
                this.Rellenar_lista_partidas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
                this.bDesconectar.PerformClick();
            }
        }
    }
}
