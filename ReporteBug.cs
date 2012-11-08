using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Juego_Hotel
{
    public partial class ReporteBug : Form
    {
        public ReporteBug()
        {
            InitializeComponent();
        }

        private void bEnviar_Click(object sender, EventArgs e)
        {
            String direccion = "http://betovserver.no-ip.org/hotel_bug_report.php?lang=" + System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() + "&text=" + this.TextoBug.Text.ToString();
            Stream response = WebRequest.Create(direccion).GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(response);
            MessageBox.Show(reader.ReadToEnd());
            this.Close();
        }
    }
}
