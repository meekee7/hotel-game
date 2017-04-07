using System;
using System.Globalization;
using System.Net;
using System.Net.Cache;
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

        private HttpWebRequest CrearPeticion(String url)
        {
            var peticion = HttpWebRequest.Create(url) as HttpWebRequest;
            // Como los ficheros del servidor pueden cambiar rápidamente y son extremadamente pequeños, no hay problema en desactivar la caché
            peticion.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            return peticion;
        }

        private String LeerRespuestaPeticion(HttpWebRequest peticion)
        {
            using (var respuesta = (HttpWebResponse)peticion.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(respuesta.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public Boolean ComprobarActualizacion()
        {
            try
            {
                ultima_version = LeerRespuestaPeticion(CrearPeticion("http://betovserver.no-ip.org:81/version_hotel.txt")).Replace("\n", "").Replace("\r", "");
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

        public Boolean MostrarNovedades()
        {
            CultureInfo cultura = System.Threading.Thread.CurrentThread.CurrentUICulture;

            try
            {
                String novedades = LeerRespuestaPeticion(CrearPeticion("http://betovserver.no-ip.org:81/hotel_welcome_msg_" + cultura.Name + ".txt"));
                if (!String.IsNullOrEmpty(novedades))
                {
                    MessageBox.Show(novedades, Mensajes.tituloBienvenido);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(Mensajes.mensajeErrorAlObtenerNovedades + Environment.NewLine + Mensajes.tituloErrorAlComprobarVersion + e.Message);
                this.error = true;
                return false;
            }
        }
    }
}
