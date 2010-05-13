using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using System.Xml;

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
            /*XmlDocument xDoc = new XmlDocument();
            xDoc.Load ("../../Opciones.xml");
            XmlNodeList personas = xDoc.GetElementsByTagName("personas");
            XmlNodeList lista = ((XmlElement)personas[0]).GetElementsByTagName("persona");*/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Principal());
        }
    }
}
