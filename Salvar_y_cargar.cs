using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Juego_Hotel
{
    class Salvar_y_cargar
    {
        public Juego juego;

        public Salvar_y_cargar(Juego juego)
        {
            this.juego = juego;
        }

        public Boolean Salvar_partida(String ruta)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);
            
            return true;
        }

        public Boolean Cargar_partida(String ruta)
        {
            return true;
        }
    }
}
