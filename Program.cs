using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            if (MessageBox.Show("¿Quieres jugar online?", "¡Bienvenido a Hotel!", MessageBoxButtons.YesNo) == DialogResult.No)
                Application.Run(new Principal(false, null));
            else
                Application.Run(new Online());
        }
    }
}
