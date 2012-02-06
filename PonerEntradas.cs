using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Juego_Hotel
{
    public partial class PonerEntradas : Form
    {
        Juego juego;
        Hotel hotel_seleccionado;
        Principal interfaz;
        Jugador jugador;
        Boolean entrada_gratis;

        public PonerEntradas(ref Juego juego, int jugador, Principal interfaz)
        {
            InitializeComponent();
            this.juego = juego;
            this.jugador = this.juego.jugadores[jugador];
            Hotel[] lista = new Hotel[this.jugador.hoteles.Count];
            this.jugador.hoteles.CopyTo(lista, 0);
            this.Rellenar_lista(ref lista);
            this.interfaz = interfaz;
            if (this.jugador.posicion.tipo == Tipos.Tcasilla.entrada_gratis)
            {
                if (this.jugador.entrada_gratis_usada)
                    this.listaHoteles.Enabled = false;
                else
                {
                    if (this.interfaz.Puede_poner_entradas(this.jugador))
                        MessageBox.Show("La primera entrada que selecciones será la gratuíta debido a la casilla en la que estás. Las consecutivas serán según el método normal (una por hotel)");
                    this.entrada_gratis = true;
                }
            }
            else
                this.entrada_gratis = false;
        }

        void Rellenar_lista(ref Hotel[] lista)
        {
            // Ya se ha comprobado que la lista tiene hoteles
            this.listaHoteles.BeginUpdate();
            this.listaHoteles.Items.Clear();
            foreach (Hotel hotel in lista)
            {
                if (!hotel.entrada_comprada_ultimo_turno)
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
                MessageBox.Show("El hotel " + this.hotel_seleccionado.nombre_txt + " no tiene fases construídas y por tanto no se le pueden añadir entradas aun");
                return;
            }
            if (this.hotel_seleccionado.precio_entrada > this.jugador.dinero_total)
            {
                MessageBox.Show("No tienes dinero suficiente para pagar una entrada del hotel " + this.hotel_seleccionado.nombre_txt);
                return;
            }
            // Hay que obtener todas las casillas de un Hotel, buscando el hotel entre todas las casillas
            LinkedList<Casilla> casillas_del_hotel = new LinkedList<Casilla>();
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
                    MessageBox.Show("Esta casilla está ocupada y no puedes añadir una entrada ahora mismo en ella");
                else if (this.juego.casillas[n_casilla].entrada_en_izq == true || this.juego.casillas[n_casilla].entrada_en_der == true)
                {
                    String nombre_hotel;
                    if (this.juego.casillas[n_casilla].entrada_en_izq)
                        nombre_hotel = this.juego.casillas[n_casilla].hotel_izq.ToString();
                    else
                        nombre_hotel = this.juego.casillas[n_casilla].hotel_der.ToString();
                    MessageBox.Show("Esta casilla ya tiene una entrada comprada para el hotel " + nombre_hotel);
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
            this.listaCasillas.SelectedItem = null;
            this.listaCasillas.Enabled = true;
            this.listaHoteles.Enabled = true;
        }

        private void bComprar_Click(object sender, EventArgs e)
        {
            int sel_n_5000 = 0, sel_n_1000 = 0, sel_n_500 = 0, sel_n_100 = 0, sel_n_50 = 0;
            if (this.entrada_gratis == true)
            {
                MessageBox.Show("Estás en una casilla de tipo Entrada Gratis. ¡Disfrútala!");
                this.entrada_gratis = false;
                this.jugador.entrada_gratis_usada = true;
                if (this.interfaz.Puede_poner_entradas(jugador))
                    this.bUnaMas.Enabled = true;
                else
                    this.bUnaMas.Enabled = false; // Sólo se permite una por ser la casilla especial
                this.hotel_seleccionado.entrada_comprada_ultimo_turno = false; // No cuenta para el turno
            }
            else
            {
                if (MessageBox.Show("Si aceptas comprar la entrada, estás obligado a pagarla. ¿Deseas realizar la compra?", "Comprar entrada", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                PedirPago frm_pago = new PedirPago(this.hotel_seleccionado.precio_entrada, ref this.juego, this.juego.jugador_actual, this.interfaz, null);
                frm_pago.ShowDialog();
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
            // Desactivar el hotel de la lista para no comprar más entradas en este turno
            this.listaHoteles.Items.Remove(this.listaHoteles.SelectedItem);
            this.listaHoteles.SelectedIndex = -1;
            this.listaHoteles.Refresh();
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
        }
    }
}
