using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Juego_Hotel
{
    class Actualizador
    {
        public String version_actual;
        public Boolean error;
        
        public Actualizador()
        {
            this.version_actual = "1.3";
            this.error = false;
        }
        public Boolean comprobar_actualizacion()
        {
            System.Net.HttpWebRequest peticion = (HttpWebRequest)WebRequest.Create("http://betovserver.no-ip.org/version_hotel.txt");

            String resultado;

            try
            {
                using (HttpWebResponse respuesta = (HttpWebResponse)peticion.GetResponse())
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(respuesta.GetResponseStream());

                    resultado = reader.ReadToEnd();
                }
                resultado = resultado.Replace("\n", "").Replace("\r", "");
                if (resultado == this.version_actual)
                    return false;
                else
                    return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Se produjo un error comprobando la versión. Ve a https://sourceforge.net/projects/hotels-game/ para comprobar la versión manualmente." + 
                    Environment.NewLine + "El error producido fue: " + e.Message);
                this.error = true;
                return false;
            }
        }
    }
}
