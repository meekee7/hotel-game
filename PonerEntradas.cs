using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class PonerEntradas : Form
    {
        Juego juego;
        Hotel hotel_seleccionado;
        readonly Principal interfaz;
        readonly Jugador jugador;
        Boolean entrada_gratis;

        public PonerEntradas(int jugador, Principal interfaz)
        {
            InitializeComponent();
            this.juego = interfaz.juego;
            this.jugador = this.juego.jugadores[jugador];
            this.Rellenar_lista();
            this.interfaz = interfaz;
            if (this.jugador.posicion.tipo == Tipos.Tcasilla.entrada_gratis)
            {
                if (this.jugador.entrada_gratis_usada)
                {
                    if (!this.interfaz.Puede_poner_entradas(this.jugador))
                        this.listaHoteles.Enabled = false;
                }
                else
                {
                    if (this.interfaz.Puede_poner_entradas(this.jugador))
                        MessageBox.Show(Mensajes.mensajePrimeraEntradaGratis);
                    this.entrada_gratis = true;
                }
            }
            else
                this.entrada_gratis = false;
        }

        void Rellenar_lista()
        {
            // Ya se ha comprobado que la lista tiene hoteles
            this.listaHoteles.BeginUpdate();
            this.listaHoteles.Items.Clear();
            foreach (Hotel hotel in this.jugador.hoteles.Where(hotel => !hotel.entrada_comprada_ultimo_turno))
            {
                this.listaHoteles.Items.Add(hotel.nombre_txt);
            }
            this.listaHoteles.EndUpdate();
            // No se necesita para ComboBox
            //this.listaHoteles.Height = (this.listaHoteles.Items.Count + 1) * this.listaHoteles.ItemHeight;
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.hotel_seleccionado = this.juego.hoteles.First(Hotel => Hotel.nombre_txt == this.listaHoteles.SelectedItem.ToString());
            if (this.hotel_seleccionado.n_fases_construidas == 0)
            {
                MessageBox.Show(String.Format(Mensajes.mensajeNohayFasesConstruidasParaPonerEntradas,this.hotel_seleccionado.nombre_txt));
                return;
            }
            if (!this.entrada_gratis && (this.hotel_seleccionado.precio_entrada > this.jugador.dinero_total))
            {
                MessageBox.Show(Mensajes.mensajeSinDineroParaComprarEntradas + this.hotel_seleccionado.nombre_txt);
                return;
            }
            // Hay que obtener todas las casillas de un Hotel, buscando el hotel entre todas las casillas
            var casillas_del_hotel = new LinkedList<Casilla>();
            int i;
            for (i = 0; i < 32; i++)
            {
                if ((this.juego.casillas[i].hotel_izq == this.hotel_seleccionado.nombre) ||
                    (this.juego.casillas[i].hotel_der == this.hotel_seleccionado.nombre))
                    casillas_del_hotel.AddLast(this.juego.casillas[i]);
            }
            this.listaCasillas.BeginUpdate();
            this.listaCasillas.Items.Clear();
            foreach (Casilla casilla in casillas_del_hotel)
                this.listaCasillas.Items.Add(casilla.numero);
            this.listaCasillas.EndUpdate();
            this.listaCasillas.Enabled = true;
        }

        private void listaCasillas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listaCasillas.SelectedItem != null)
            {
                int n_casilla = Convert.ToInt16(this.listaCasillas.SelectedItem);
                this.bComprar.Enabled = false;
                if (this.juego.casillas[n_casilla].ocupada)
                    MessageBox.Show(Mensajes.mensajeCasillaOcupada);
                else if (this.juego.casillas[n_casilla].entrada_en_izq || this.juego.casillas[n_casilla].entrada_en_der)
                {
                    string nombre_hotel = this.juego.casillas[n_casilla].entrada_en_izq
                        ? this.juego.casillas[n_casilla].hotel_izq.ToString()
                        : this.juego.casillas[n_casilla].hotel_der.ToString();
                    MessageBox.Show(Mensajes.mensajeCasillaYaPoseeEntrada + nombre_hotel);
                }
                else
                    this.bComprar.Enabled = true;
            }
            this.bUnaMas.Enabled = false;
        }

        private void bUnaMas_Click(object sender, EventArgs e)
        {
            this.bComprar.Enabled = false;
            this.bUnaMas.Enabled = false;
            this.listaCasillas.Items.Clear();
            this.listaCasillas.Enabled = false;
            this.listaHoteles.Enabled = true;
        }

        private void bComprar_Click(object sender, EventArgs e)
        {
            int sel_n_5000 = 0, sel_n_1000 = 0, sel_n_500 = 0, sel_n_100 = 0, sel_n_50 = 0;
            if (this.entrada_gratis)
            {
                MessageBox.Show(Mensajes.mensajeEstasEnCasillaTipoEntradaGratis);
                this.jugador.entrada_gratis_usada = true;
                this.bUnaMas.Enabled = this.interfaz.Puede_poner_entradas(jugador);
                this.hotel_seleccionado.entrada_comprada_ultimo_turno = false; // No cuenta para el turno
            }
            else
            {
                if (MessageBox.Show(Mensajes.mensajeComprarEntradas, Mensajes.tituloComprarEntrada, MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                // En caso de que el turno se pase automáticamente, no hacer nada
                if (this.juego.jugador_actual != this.jugador)
                    return;
                int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                var frm_pago = new PedirPago(this.hotel_seleccionado.precio_entrada, ref this.juego, this.juego.jugador_actual, this.interfaz, null);
                frm_pago.ShowDialog();
                if (frm_pago.cancelado)
                {
                    frm_pago.Close();
                    return;
                }
                sel_n_5000 = frm_pago.n_5000;
                sel_n_1000 = frm_pago.n_1000;
                sel_n_500 = frm_pago.n_500;
                sel_n_100 = frm_pago.n_100;
                sel_n_50 = frm_pago.n_50;
                frm_pago.Close();
                if (!this.interfaz.online)
                {
                    if (frm_pago.total_seleccionado > this.hotel_seleccionado.precio_entrada)
                    {
                        Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.hotel_seleccionado.precio_entrada), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                    }
                    this.jugador.Pagar_Ampliacion_o_Entrada(sel_n_5000, sel_n_1000, sel_n_500, sel_n_100, sel_n_50);
                    this.jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                    this.interfaz.Actualizar_Dinero_Jugadores();
                }
                this.bUnaMas.Enabled = true;
                this.hotel_seleccionado.entrada_comprada_ultimo_turno = true;
            }
            this.bComprar.Enabled = false;
            this.listaCasillas.Enabled = false;
            this.listaHoteles.Enabled = false;
            // Hay que saber en que lado de la casilla se pone la entrada
            int n_casilla = Convert.ToInt16(this.listaCasillas.SelectedItem);
            if (!this.interfaz.online)
            {
                if (this.juego.casillas[n_casilla].hotel_der == this.hotel_seleccionado.nombre)
                {
                    this.juego.casillas[n_casilla].entrada_en_der = true;
                    this.interfaz.Dibujar_Entrada(this.juego.casillas[n_casilla], true);
                }
                else
                {
                    this.juego.casillas[n_casilla].entrada_en_izq = true;
                    this.interfaz.Dibujar_Entrada(this.juego.casillas[n_casilla], false);
                }
                this.hotel_seleccionado.n_entradas++;
                this.hotel_seleccionado.entradas.AddLast(this.juego.casillas[n_casilla]);
            }
            this.Rellenar_lista();
            // Limpiar lista de casillas disponibles
            this.listaCasillas.Items.Clear();
            this.listaCasillas.SelectedIndex = -1;
            this.listaCasillas.Refresh();
            if (this.interfaz.online)
            {
                int game_id = this.interfaz.game_id;
                if (this.entrada_gratis)
                    this.interfaz.frm_online.enviar_comando("buy_entrance", game_id.ToString(), this.hotel_seleccionado.nombre_txt, n_casilla.ToString(), "0");
                else
                    this.interfaz.frm_online.enviar_comando("buy_entrance", game_id.ToString(), this.hotel_seleccionado.nombre_txt, n_casilla.ToString(), "1",
                        sel_n_5000.ToString(), sel_n_1000.ToString(), sel_n_500.ToString(), sel_n_100.ToString(), sel_n_50.ToString());
            }
            this.entrada_gratis = false; // Si no se ha usado, es porque ya estaba a falso, se puede poner a falso seguro
        }
    }
}