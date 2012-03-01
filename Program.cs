using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]

        static void Main ()
        {
            XmlDocument configuracion = new XmlDocument();
            configuracion.Load("Config.xml");
            XmlNode nodo_Idioma = configuracion.GetElementsByTagName("language")[0];
            if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("Spanish"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
            }
            else if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("English"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (MessageBox.Show(Mensajes.mensajeActualizar, Mensajes.tituloBienvenido, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Actualizador actualizador = new Actualizador();
                if (actualizador.comprobar_actualizacion())
                {
                    if (MessageBox.Show(String.Format(Mensajes.mensajeNuevaVersion, actualizador.ultima_version, actualizador.version_actual),
                        Mensajes.tituloBienvenido, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://sourceforge.net/projects/hotels-game/files");
                        return;
                    }
                }
                else
                {
                    if (!actualizador.error)
                        MessageBox.Show(Mensajes.mensajeNoNuevaVersion);
                }
                actualizador = null;
            }
            if (MessageBox.Show(Mensajes.mensajePreguntarSiOnline, Mensajes.tituloBienvenido, MessageBoxButtons.YesNo) == DialogResult.No)
                Application.Run(new Principal(false, null));
            else
                Application.Run(new Online());
        }

        internal static void ReLocalizeAll(System.Globalization.CultureInfo NewCulture)
        {
            CultureInfo antiguoCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = NewCulture;
            foreach (Form f in Application.OpenForms)
            {
                if (f is IReLocalizable)
                    ((IReLocalizable)f).ReLocalize(antiguoCulture);
            }   
        }
    }
}
