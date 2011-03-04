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
            catch (SocketException ex)
            {
                MessageBox.Show("Error conectando: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Error conectando: " + ex.Message);
            }
        }

        private void Online_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.socket.Connected)
                this.socket.Close();
        }
    }
}
