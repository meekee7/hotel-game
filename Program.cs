using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
            if (MessageBox.Show("¿Quieres comprobar si existe una versión del juego más actualizada?", "¡Bienvenido a Hotel!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Actualizador actualizador = new Actualizador();
                if (actualizador.comprobar_actualizacion())
                {
                    if (MessageBox.Show("Existe una versión diferente (" + actualizador.ultima_version + ") que la que estás usando (" + actualizador.version_actual + "). ¿Quieres acceder a la página del proyecto en SourceForge?\nEl juego se cerrará.",
                        "¡Bienvenido a Hotel!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("https://sourceforge.net/projects/hotels-game/files");
                        return;
                    }
                }
                else
                {
                    if (!actualizador.error)
                        MessageBox.Show("No existe una versión más nueva :)");
                }
                actualizador = null;
            }
            if (MessageBox.Show("¿Quieres jugar online?", "¡Bienvenido a Hotel!", MessageBoxButtons.YesNo) == DialogResult.No)
                Application.Run(new Principal(false, null));
            else
                Application.Run(new Online());
        }
    }
}
