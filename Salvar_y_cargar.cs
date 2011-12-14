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
        public String error;

        public Salvar_y_cargar(Juego juego)
        {
            this.juego = juego;
        }

        public Boolean Salvar_partida(String ruta)
        {
            try
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
                nodo = doc.CreateElement("jugador_inicial");
                nodo.AppendChild(doc.CreateTextNode(this.juego.jug_inicial.ToString()));
                nodoEstadoPartida.AppendChild(nodo);
                nodo = doc.CreateElement("jugador_actual");
                nodo.AppendChild(doc.CreateTextNode(this.juego.jug_actual.ToString()));
                nodoEstadoPartida.AppendChild(nodo);
                nodo = doc.CreateElement("ultimo_res_dado");
                nodo.AppendChild(doc.CreateTextNode(this.juego.ultimo_res_dado.ToString()));
                nodoEstadoPartida.AppendChild(nodo);
                nodo = doc.CreateElement("ultimo_avance_auto");
                nodo.AppendChild(doc.CreateTextNode(this.juego.ultimo_avance_auto.ToString()));
                nodoEstadoPartida.AppendChild(nodo);
                raiz.AppendChild(nodoEstadoPartida);

                // Estado de los hoteles
                XmlNode nodoEstadoHoteles = doc.CreateElement("estado_hoteles");
                XmlNode nodoEstadoHotel;
                foreach (Hotel hotel in juego.hoteles)
                {
                    nodoEstadoHotel = doc.CreateElement("estado_hotel");
                    nodo = doc.CreateElement("nombre");
                    nodo.AppendChild(doc.CreateTextNode(hotel.nombre_txt));
                    nodoEstadoHotel.AppendChild(nodo);
                    nodo = doc.CreateElement("num_amplis_construidas");
                    nodo.AppendChild(doc.CreateTextNode(hotel.n_ampliaciones_construidas.ToString()));
                    nodoEstadoHotel.AppendChild(nodo);
                    nodo = doc.CreateElement("entrada_comprada_ultimo_turno");
                    if (hotel.entrada_comprada_ultimo_turno)
                        nodo.AppendChild(doc.CreateTextNode("si"));
                    else
                        nodo.AppendChild(doc.CreateTextNode("no"));
                    nodoEstadoHotel.AppendChild(nodo);
                    nodo = doc.CreateElement("num_entradas");
                    nodo.AppendChild(doc.CreateTextNode(hotel.n_entradas.ToString()));
                    nodoEstadoHotel.AppendChild(nodo);
                    nodo = doc.CreateElement("suelo_comprado");
                    if (hotel.suelo_comprado)
                        nodo.AppendChild(doc.CreateTextNode("si"));
                    else
                        nodo.AppendChild(doc.CreateTextNode("no"));
                    nodoEstadoHotel.AppendChild(nodo);
                    nodo = doc.CreateElement("posiciones_de_entradas");
                    // Buscar las casillas del hotel actual
                    String lista_entradas = "";
                    foreach (Casilla casilla in hotel.entradas)
                    {
                        lista_entradas += '@' + casilla.numero.ToString();
                    }
                    if (lista_entradas.Length > 0)
                    {
                        lista_entradas = lista_entradas.Remove(0, 1); // Quitar la primera arroba
                        nodo.AppendChild(doc.CreateTextNode(lista_entradas));
                    }
                    nodoEstadoHotel.AppendChild(nodo);
                    nodoEstadoHoteles.AppendChild(nodoEstadoHotel);
                }
                raiz.AppendChild(nodoEstadoHoteles);

                // Estado de los jugadores
                XmlNode nodoEstadoJugadores = doc.CreateElement("estado_jugadores");
                XmlNode nodoEstadoJugador;
                foreach (Jugador jugador in juego.jugadores)
                {
                    nodoEstadoJugador = doc.CreateElement("estado_jugador");
                    nodo = doc.CreateElement("num_jugador");
                    nodo.AppendChild(doc.CreateTextNode(jugador.n_jugador.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("posicion");
                    nodo.AppendChild(doc.CreateTextNode(jugador.posicion.numero.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("color");
                    nodo.AppendChild(doc.CreateTextNode(jugador.color.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("pago_ultimo_turno");
                    if (jugador.pago_ultimo_turno)
                        nodo.AppendChild(doc.CreateTextNode("si"));
                    else
                        nodo.AppendChild(doc.CreateTextNode("no"));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("eliminado");
                    if (jugador.eliminado)
                        nodo.AppendChild(doc.CreateTextNode("si"));
                    else
                        nodo.AppendChild(doc.CreateTextNode("no"));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("n_billetes_50");
                    nodo.AppendChild(doc.CreateTextNode(jugador.n_billetes_50.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("n_billetes_100");
                    nodo.AppendChild(doc.CreateTextNode(jugador.n_billetes_100.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("n_billetes_500");
                    nodo.AppendChild(doc.CreateTextNode(jugador.n_billetes_500.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("n_billetes_1000");
                    nodo.AppendChild(doc.CreateTextNode(jugador.n_billetes_1000.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("n_billetes_5000");
                    nodo.AppendChild(doc.CreateTextNode(jugador.n_billetes_5000.ToString()));
                    nodoEstadoJugador.AppendChild(nodo);
                    nodo = doc.CreateElement("hoteles_poseidos");
                    // Recorrer los hoteles del jugador
                    String lista_hoteles = "";
                    foreach (Hotel hotel in jugador.hoteles)
                    {
                        lista_hoteles += '@' + hotel.nombre_txt;
                    }
                    if (lista_hoteles.Length > 0)
                    {
                        lista_hoteles = lista_hoteles.Remove(0, 1); // Quitar la primera arroba
                        nodo.AppendChild(doc.CreateTextNode(lista_hoteles));
                    }
                    nodoEstadoJugador.AppendChild(nodo);
                    nodoEstadoJugadores.AppendChild(nodoEstadoJugador);
                }
                raiz.AppendChild(nodoEstadoJugadores);

                doc.Save(ruta);
                return true;
            }
            catch (Exception e)
            {
                this.error = e.Message;
                return false;
            }
        }

        public Boolean Cargar_partida(String ruta, Principal interfaz)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ruta);

                // Estado de la partida
                XmlElement nodoEstadoPartida = (XmlElement) doc.GetElementsByTagName("estado_partida")[0];
                juego.n_jugadores_activos = Convert.ToInt16(nodoEstadoPartida.GetElementsByTagName("num_jugadores_activos")[0].FirstChild.Value);
                juego.jug_inicial = Convert.ToInt16(nodoEstadoPartida.GetElementsByTagName("jugador_inicial")[0].FirstChild.Value);
                juego.jug_actual = Convert.ToInt16(nodoEstadoPartida.GetElementsByTagName("jugador_actual")[0].FirstChild.Value);
                juego.ultimo_res_dado = Convert.ToInt16(nodoEstadoPartida.GetElementsByTagName("ultimo_res_dado")[0].FirstChild.Value);
                juego.ultimo_avance_auto = Convert.ToInt16(nodoEstadoPartida.GetElementsByTagName("ultimo_avance_auto")[0].FirstChild.Value);

                // Estado de los hoteles
                XmlNodeList nodoEstadoHoteles = doc.GetElementsByTagName("estado_hotel");
                String[] lista_entradas;
                foreach (XmlNode nodoHotel in nodoEstadoHoteles)
                {
                    XmlElement elemHotel = (XmlElement)nodoHotel;
                    String nombre_hotel = elemHotel.GetElementsByTagName("nombre")[0].FirstChild.Value;
                    Hotel hotel = this.juego.hoteles.First(delegate(Hotel h) { return h.nombre_txt == nombre_hotel; });
                    // Ya hemos emparejado el Hotel con su nombre
                    hotel.n_ampliaciones_construidas = Convert.ToInt16(elemHotel.GetElementsByTagName("num_amplis_construidas")[0].FirstChild.Value);
                    hotel.entrada_comprada_ultimo_turno = Convert.ToBoolean(elemHotel.GetElementsByTagName("entrada_comprada_ultimo_turno")[0].FirstChild.Value.Replace("si", "true").Replace("no", "false"));
                    hotel.n_ampliaciones_construidas = Convert.ToInt16(elemHotel.GetElementsByTagName("num_entradas")[0].FirstChild.Value);
                    hotel.suelo_comprado = Convert.ToBoolean(elemHotel.GetElementsByTagName("suelo_comprado")[0].FirstChild.Value.Replace("si", "true").Replace("no", "false"));
                    XmlNode nodoPosicionesEntradas = elemHotel.GetElementsByTagName("posiciones_de_entradas")[0];
                    if (nodoPosicionesEntradas.HasChildNodes)
                    {
                        lista_entradas = nodoPosicionesEntradas.FirstChild.Value.Split('@');
                        hotel.n_entradas = lista_entradas.Length;
                        foreach (String s_num_casilla in lista_entradas)
                        {
                            int num_casilla = Convert.ToInt16(s_num_casilla);
                            if (this.juego.casillas[num_casilla].hotel_der == hotel.nombre)
                            {
                                this.juego.casillas[num_casilla].entrada_en_der = true;
                                interfaz.Dibujar_Entrada(ref this.juego.casillas[num_casilla], true);
                            }
                            else if (this.juego.casillas[num_casilla].hotel_izq == hotel.nombre)
                            {
                                this.juego.casillas[num_casilla].entrada_en_izq = true;
                                interfaz.Dibujar_Entrada(ref this.juego.casillas[num_casilla], false);
                            }
                            else // Nº de casilla incorrecta, fichero modificado
                            {
                                this.error = "El fichero de la partida ha sido modificado incorrectamente: El hotel " +
                                    hotel.nombre_txt + " tiene unas entradas asignadas que no pertenecen al hotel";
                                return false;
                            }
                            hotel.entradas.AddLast(this.juego.casillas[num_casilla]);
                        }
                    }
                }

                // Estado de los hoteles
                XmlNodeList nodoEstadoJugadores = doc.GetElementsByTagName("estado_hotel");
                String[] lista_entradas;
                foreach (XmlNode nodoHotel in nodoEstadoHoteles)
                {
                return true;
            }
            catch (Exception e)
            {
                this.error = e.Message;
                return false;
            }
        }
    }
}
