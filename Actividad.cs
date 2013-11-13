using System;
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

        public void DadoTirado(Jugador jugador, int resultado, Boolean automaticamente)
        {
            this.mensajes.AppendText(String.Format((automaticamente ? Mensajes.MADadoTiradoAuto: Mensajes.MADadoTirado),
                jugador.nombre_online, jugador.Nombre_color(), resultado) + Environment.NewLine);
        }

        public void PasarTurno(String jugador, String color, Boolean automaticamente)
        {
            this.mensajes.AppendText(String.Format((automaticamente ? Mensajes.MAPasarTurnoAuto : Mensajes.MAPasarTurno), jugador, color) + Environment.NewLine);
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
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionOk, jugador.nombre_online, jugador.Nombre_color()) + Environment.NewLine);
                    break;
                case (Tipos.Resultado_dado_cons.Gratis):
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionGratis, jugador.nombre_online, jugador.Nombre_color()) + Environment.NewLine);
                    break;
                case (Tipos.Resultado_dado_cons.Doble):
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionDobleCoste, jugador.nombre_online, jugador.Nombre_color()) + Environment.NewLine);
                    break;
                case (Tipos.Resultado_dado_cons.Denegado):
                    this.mensajes.AppendText(String.Format(Mensajes.MATirarDadoConstruccionKo, jugador.nombre_online, jugador.Nombre_color()) + Environment.NewLine);
                    break;
            }
        }

        public void Pedir_Noches_Online(Jugador jugador, Jugador jugadorD, int cantidad, int noches, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAPedirNochesOnline, jugador.nombre_online, jugador.Nombre_color(), jugadorD.nombre_online,
                jugadorD.Nombre_color(), cantidad, noches, hotel) + Environment.NewLine);
        }

        public void Expropiar_Hotel(Jugador expropiador, Jugador expropiado, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAExpropiarHotel, expropiador.nombre_online, expropiador.Nombre_color(), expropiado.nombre_online,
                expropiado.Nombre_color(), hotel) + Environment.NewLine);
        }

        public void Añadir_Entrada(Jugador jugador, int entrada, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAEntradaAñadida, jugador.nombre_online, jugador.Nombre_color(), entrada, hotel) + Environment.NewLine);
        }

        public void Añadir_Fase(Jugador jugador, int fase, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAFaseAñadida, jugador.nombre_online, jugador.Nombre_color(), fase, hotel) + Environment.NewLine);
        }

        public void Iniciar_Subasta(Jugador jugador, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MAIniciarSubasta, jugador.nombre_online, jugador.Nombre_color(), hotel) + Environment.NewLine);
        }

        public void Realizar_Puja(Jugador jugador, int cantidad, String hotel)
        {
            this.mensajes.AppendText(String.Format(Mensajes.MANuevaPuja, jugador.nombre_online, jugador.Nombre_color(), cantidad, hotel) + Environment.NewLine);
        }

        public void Subasta_Terminada(Jugador vendedor, Jugador comprador, String hotel, int cantidad, Boolean automaticamente)
        {
            this.mensajes.AppendText(String.Format((automaticamente ? Mensajes.MASubastaTerminadaAuto : Mensajes.MASubastaTerminada),
                vendedor.nombre_online, vendedor.Nombre_color(), hotel, comprador.nombre_online, comprador.Nombre_color(), cantidad) + Environment.NewLine);
        }
    }
}
