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
                        this.Rellenar_lista_usuarios();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo lista de usuarios: " + ex.Message);
            }
        }

        private void Online_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.socket.Connected)
                this.socket.Close();
        }
    }
}
