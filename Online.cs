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

        private void button1_Click(object sender, EventArgs e)
        {
            if (socket.Connected)
            {
                socket.Send(Encoding.UTF8.GetBytes(this.textBox1.Text.ToCharArray()));
            }
            else
                MessageBox.Show("No estás conectado");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress dir = Dns.GetHostAddresses("localhost").First(IPAddress => IPAddress.AddressFamily == AddressFamily.InterNetwork);
                IPEndPoint Ep = new IPEndPoint(dir, 12345);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(Ep);
                this.bLogin.Enabled = true;
            }
            catch (SocketException ex)
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
