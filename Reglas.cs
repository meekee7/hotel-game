using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Juego_Hotel
{
    public partial class Reglas : Form, IReLocalizable
    {
        readonly System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Reglas));

        public Reglas()
        {
            InitializeComponent();
            if (Thread.CurrentThread.CurrentUICulture.Name == "fr")
                this.img_reglas.Image = Properties.Resources.Reglas_fr;
            else if (Thread.CurrentThread.CurrentUICulture.Name == "es")
                this.img_reglas.Image = Properties.Resources.Reglas_es;
            else
                this.img_reglas.Image = Properties.Resources.Reglas_es;
        }

        public void ReLocalize(CultureInfo nuevoCulture, CultureInfo antiguoCulture)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<System.Globalization.CultureInfo, System.Globalization.CultureInfo>(this.ReLocalize), new object[] { nuevoCulture, antiguoCulture });
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = nuevoCulture;
                resources.ApplyResources(this, "$this");
                foreach (Control c in this.Controls)
                {
                    if (c is GroupBox)
                    {
                        c.Text = resources.GetString(c.Name + ".Text");
                        foreach (Control o in c.Controls)
                        {
                            if (o is Label)
                            {
                                var nombreAntiguo = (String)resources.GetObject(o.Name + ".Text", antiguoCulture);
                                if (nombreAntiguo != null)
                                    o.Text = o.Text.Replace(nombreAntiguo, resources.GetString(o.Name + ".Text"));
                            }
                            else
                                resources.ApplyResources(o, o.Name);
                        }
                    }
                    else if (c is Label)
                    {
                        var nombreAntiguo = (String)resources.GetObject(c.Name + ".Text", antiguoCulture);
                        if (nombreAntiguo == null)
                            continue;
                        if (c.Text != null)
                            c.Text = c.Text.Replace(nombreAntiguo, resources.GetString(c.Name + ".Text"));
                    }
                    else
                        c.Text = resources.GetString(c.Name + ".Text");
                }

                if (nuevoCulture.Name == "fr")
                    this.img_reglas.Image = Properties.Resources.Reglas_fr;
                else if (nuevoCulture.Name == "es")
                    this.img_reglas.Image = Properties.Resources.Reglas_es;
                else
                    this.img_reglas.Image = Properties.Resources.Reglas_es;
            }
        }

        private void Reglas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Owner == null)
                return;
            if (Owner is Principal)
                (Owner as Principal).bNormas.Enabled = true;
        }
    }
}
