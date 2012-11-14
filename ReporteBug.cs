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
using Juego_Hotel.Resources;

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
            if (this.textBoxEmail.Text.ToString().Trim() == "")
            {
                MessageBox.Show(Mensajes.mensajeErrorBugReportEmailVacio);
                return;
            }
            if (this.TextoBug.Text.ToString().Trim() == "")
            {
                MessageBox.Show(Mensajes.mensajeErrorBugReportTextoVacio);
                return;
            }
            String direccion = "http://betovserver.no-ip.org/hotel_bug_report.php?email=" + this.textBoxEmail.Text.ToString() + "&lang=" + System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() + "&text=" + this.TextoBug.Text.ToString();
            Stream response = WebRequest.Create(direccion).GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(response);
            MessageBox.Show(reader.ReadToEnd());
            this.Close();
        }
    }
}
