using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class Actividad : Form
    {
        public Actividad()
        {
            InitializeComponent();
        }

        public void DadoTirado(Jugador jugador, int resultado)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MADadoTirado, jugador.nombre_online, jugador.color.ToString(), resultado.ToString()) + Environment.NewLine);
        }

        public void PasarTurno(String jugador, String color)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAPasarTurno, jugador, color) + Environment.NewLine);
        }

        public void ComprarHotel(String jugador, String color, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAComprarHotel, jugador, color, hotel) + Environment.NewLine);
        }

        public void Tirar_Dado_Construccion(Jugador jugador, Tipos.Resultado_dado_cons resultado)
        {
            switch (resultado)
            {
                case (Tipos.Resultado_dado_cons.Permitido):
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionOk, jugador.nombre_online, jugador.color) + Environment.NewLine);
                    break;
                case (Tipos.Resultado_dado_cons.Gratis): 
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionGratis , jugador.nombre_online, jugador.color) + Environment.NewLine);
                    break;
                case (Tipos.Resultado_dado_cons.Doble):
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionDobleCoste, jugador.nombre_online, jugador.color) + Environment.NewLine);
                    break;
                case (Tipos.Resultado_dado_cons.Denegado): 
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionKo, jugador.nombre_online, jugador.color) + Environment.NewLine);
                    break;
            }
        }

        public void Pedir_Noches_Online(Jugador jugador, Jugador jugadorD, int cantidad, int noches, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAPedirNochesOnline, jugador.nombre_online, jugador.color, jugadorD.nombre_online, jugadorD.color, cantidad, noches, hotel) + Environment.NewLine);
        }
    }
}
