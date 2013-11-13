using System;
using System.Net;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    class Actualizador
    {
        public String version_actual;
        public String ultima_version;
        public Boolean error;
        
        public Actualizador()
        {
            Version assembly_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.version_actual = assembly_version.Major.ToString() + '.' + assembly_version.Minor + '.' + assembly_version.Build;
            this.error = false;
        }

        public Boolean comprobar_actualizacion()
        {
            var peticion = (HttpWebRequest)WebRequest.Create("http://betovserver.no-ip.org/version_hotel.txt");

            try
            {
                using (var respuesta = (HttpWebResponse)peticion.GetResponse())
                {
                    var reader = new System.IO.StreamReader(respuesta.GetResponseStream());

                    ultima_version = reader.ReadToEnd();
                }
                ultima_version = ultima_version.Replace("\n", "").Replace("\r", "");
                return ultima_version != this.version_actual;
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Mensajes.mensajeErrorAlComprobarVersion, "https://sourceforge.net/projects/hotels-game/") + 
                    Environment.NewLine + Mensajes.tituloErrorAlComprobarVersion + e.Message);
                this.error = true;
                return false;
            }
        }
    }
}
