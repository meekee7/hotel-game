using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
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
            var configuracion = new XmlDocument();
            configuracion.Load("Config.xml");
            XmlNode nodo_Idioma = configuracion.GetElementsByTagName("language")[0];
            if (nodo_Idioma.ChildNodes[0].FirstChild == null)
            {
                MessageBox.Show(Mensajes.mensajeIdiomaPorDefecto, Mensajes.tituloBienvenido);
                if ((System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "es-ES") || (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2) == "es"))
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
                else if ((System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "fr-FR") || (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2) == "fr"))
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
                else
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            else if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("Spanish"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
            }
            else if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("English"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            else if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("French"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (MessageBox.Show(Mensajes.mensajeActualizar, Mensajes.tituloBienvenido, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var actualizador = new Actualizador();
                if (actualizador.ComprobarActualizacion())
                {
                    if (MessageBox.Show(String.Format(Mensajes.mensajeNuevaVersion, actualizador.ultima_version, actualizador.version_actual),
                        Mensajes.tituloBienvenido, MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                    System.Diagnostics.Process.Start("https://sourceforge.net/projects/hotels-game/files");
                    return;
                }
                if (!actualizador.error)
                    MessageBox.Show(Mensajes.mensajeNoNuevaVersion);

				actualizador.MostrarNovedades();
            }
            if (MessageBox.Show(Mensajes.mensajePreguntarSiOnline, Mensajes.tituloBienvenido, MessageBoxButtons.YesNo) == DialogResult.No)
                Application.Run(new Principal(null, configuracion));
            else
                Application.Run(new Online(configuracion));
        }

        internal static void ReLocalizeAll(CultureInfo nuevoCulture)
        {
            CultureInfo antiguoCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = nuevoCulture;
            foreach (IReLocalizable f in Application.OpenForms.OfType<IReLocalizable>())
            {
                f.ReLocalize(nuevoCulture, antiguoCulture);
            }   
        }
    }
}