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
            XmlNode declaracion = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaracion);
            XmlNode raiz = doc.CreateElement("partida");
            doc.AppendChild(raiz);
            XmlNode nodo = doc.CreateElement("fecha");
            nodo.AppendChild(doc.CreateTextNode(System.DateTime.Now.ToString()));
            raiz.AppendChild(nodo);
            nodo = doc.CreateElement("online");
            nodo.AppendChild(doc.CreateTextNode("no"));
            raiz.AppendChild(nodo);
            nodo = doc.CreateElement("num_jugadores");
            nodo.AppendChild(doc.CreateTextNode(this.juego.n_jugadores.ToString()));
            raiz.AppendChild(nodo);

            // Estado de la partida
            XmlNode nodoEstadoPartida = doc.CreateElement("estado_partida");
            nodo = doc.CreateElement("num_jugadores_activos");
            nodo.AppendChild(doc.CreateTextNode(this.juego.n_jugadores_activos.ToString()));
            nodoEstadoPartida.AppendChild(nodo);
            raiz.AppendChild(nodoEstadoPartida);
            doc.Save(ruta);
            return true;
        }

        public Boolean Cargar_partida(String ruta)
        {
            return true;
        }
    }
}
