using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
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
            if (this.textBoxEmail.Text.Trim() == "")
            {
                MessageBox.Show(Mensajes.mensajeErrorBugReportEmailVacio);
                return;
            }
            if (this.TextoBug.Text.Trim() == "")
            {
                MessageBox.Show(Mensajes.mensajeErrorBugReportTextoVacio);
                return;
            }
            String direccion = "http://betovserver.no-ip.org/hotel_bug_report.php?email=" + this.textBoxEmail.Text + "&lang=" + System.Threading.Thread.CurrentThread.CurrentUICulture + "&text=" + this.TextoBug.Text;
            Stream response = WebRequest.Create(direccion).GetResponse().GetResponseStream();
            var reader = new StreamReader(response);
            MessageBox.Show(reader.ReadToEnd());
            this.Close();
        }
    }
}