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
    public partial class Subastas : Form
    {
        public int n_5000;
        public int n_1000;
        public int n_500;
        public int n_100;
        public int n_50;
        Juego juego;
        public int n_mayor_postor; // nº de jugador
        public int n_precio_mayor = 0;
        public Hotel hotel_seleccionado;
        private Principal interfaz;
        Boolean online;
        int precio_minimo;

        public Subastas(ref Juego juego, Principal interfaz, Boolean online)
        {
            InitializeComponent();
            this.n_5000 = 0;
            this.n_1000 = 0;
            this.n_500 = 0;
            this.n_100 = 0;
            this.n_50 = 0;
            this.juego = juego;
            this.interfaz = interfaz;
            this.online = online;
            this.Rellenar_Lista_Hoteles(this.online);
            if (this.online) // Sólo activar los botones de puja si no eres el creador
            {
                if (this.juego.jugador_actual.nombre_online != this.interfaz.nombre_online)
                {
                    this.listaHoteles.Enabled = false;
                    this.bSubastar.Enabled = false;
                    this.bVender.Enabled = false;
                    this.cantidad.Enabled = true;
                    this.bCerrar.Enabled = false;
                    switch (this.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == this.interfaz.nombre_online).n_jugador)
                    {
                        case 0: this.bJ1.Enabled = true;
                            break;
                        case 1: this.bJ2.Enabled = true;
                            break;
                        case 2: this.bJ3.Enabled = true;
                             break;
                        case 3: this.bJ4.Enabled = true;
                            break;
                    }
                }
            }
        }

        public void Establecer_Precio_Minimo(int precio)
        {
            this.precio_minimo = precio;
            this.precioMinimo.Text = precio.ToString();
        }

        private void Rellenar_Lista_Hoteles(Boolean online)
        {
            if ((!online) || (this.online && (this.juego.jugador_actual.nombre_online == this.interfaz.nombre_online)))
            {
                LinkedList<Hotel> lista = this.juego.jugador_actual.hoteles;
                this.listaHoteles.BeginUpdate();
                this.listaHoteles.Items.Clear();
                if (lista.Count > 0)
                {
                    foreach (Hotel hotel in lista)
                        this.listaHoteles.Items.Add(hotel.nombre_txt);
                    this.listaHoteles.Enabled = true;
                }
                else
                    this.listaHoteles.Enabled = false;
                this.listaHoteles.EndUpdate();
                this.listaHoteles.SelectedIndex = -1;
            }
            else
            {
                this.listaHoteles.BeginUpdate();
                this.listaHoteles.Enabled = false;
                this.listaHoteles.Items.Clear();
                this.listaHoteles.Items.Add(this.interfaz.hotel_a_subastar_online.nombre_txt);
                this.listaHoteles.EndUpdate();
                this.listaHoteles.SelectedIndex = 0;
            }
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.bSubastar.Enabled = true;
            this.hotel_seleccionado = this.juego.hoteles.First(Hotel => Hotel.nombre_txt == this.listaHoteles.SelectedItem.ToString());
        }

        private void bSubastar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Mensajes.mensajeConfirmacionSubasta, Mensajes.tituloConfirmacionSubasta, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (MessageBox.Show(Mensajes.mensajePrecioMinimoEnSubasta, Mensajes.tituloSubastas, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    this.Establecer_Precio_Minimo(this.hotel_seleccionado.Calcular_precio_minimo());
                else
                    this.Establecer_Precio_Minimo(50);
                this.cantidad.Enabled = true;
                this.bVender.Enabled = true;
                this.bCerrar.Enabled = false;
                this.listaHoteles.Enabled = false;
                if (!this.online)
                {
                    if (this.juego.jug_actual != 1)
                        bJ1.Enabled = true;
                    else
                        bJ1.Enabled = false;
                    if (this.juego.jug_actual != 2)
                        bJ2.Enabled = true;
                    else
                        bJ2.Enabled = false;
                    if (this.juego.n_jugadores > 2)
                    {
                        if (this.juego.jug_actual != 3)
                            bJ3.Enabled = true;
                        else
                            bJ3.Enabled = false;
                    }
                    else
                        bJ3.Enabled = false;
                    if (this.juego.n_jugadores > 3)
                    {
                        if (this.juego.jug_actual != 4)
                            bJ4.Enabled = true;
                        else
                            bJ4.Enabled = false;
                    }
                    else
                        bJ4.Enabled = false;
                }
                else
                {
                    // En la ventana del creador de la subasta no se le permite pujar
                    this.bJ1.Enabled = false;
                    this.bJ2.Enabled = false;
                    this.bJ3.Enabled = false;
                    this.bJ4.Enabled = false;
                    this.cantidad.Enabled = false;
                    int id = this.interfaz.game_id;
                    this.interfaz.frm_online.enviar_comando("auction_start", id.ToString(),
                        this.hotel_seleccionado.nombre_txt.ToString(), this.precio_minimo.ToString());
                }
                this.bSubastar.Enabled = false;
            }
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, true);
            frm_ver_hoteles.Show(this);
            this.bVerHoteles.Enabled = false;
        }

        private void bVender_Click(object sender, EventArgs e)
        {
            int game_id;
            if (this.precio_mayor.Text.ToString().Trim() == "")
                    MessageBox.Show(Mensajes.mensajeNoPujadoTodavia);
            else
            {
                if (MessageBox.Show(String.Format(Mensajes.mensajeConfirmarVentaSubasta, this.mayor_postor.Text, this.precio_mayor.Text),
                    Mensajes.tituloConfirmarVentaSubasta, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    
                    if (!this.interfaz.online)
                    {
                        PedirPago frm_pago = new PedirPago(this.n_precio_mayor, ref this.juego, true, this.juego.jugadores[n_mayor_postor], this.interfaz); // Subasta deshabilitada al estar ya en una subasta
                        frm_pago.ShowDialog();
                        Jugador dueño_ant = hotel_seleccionado.dueño;
                        dueño_ant.Hotel_Expropiado(ref hotel_seleccionado);
                        this.juego.jugadores[n_mayor_postor].Comprar_Hotel(ref hotel_seleccionado, ref dueño_ant, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.n_precio_mayor)
                        {
                            int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                            Principal.Calcular_Devolucion(ref dueño_ant, (frm_pago.total_seleccionado - this.n_precio_mayor), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            this.juego.jugadores[n_mayor_postor].Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                        this.Rellenar_Lista_Hoteles(this.online);
                        this.listaHoteles.Enabled = true;
                        this.bCerrar.Enabled = true;
                        this.interfaz.Actualizar_Fases_Nuevo_Dueño_Hotel(hotel_seleccionado);
                    }
                    else
                    {
                        game_id = this.interfaz.game_id;
                        this.interfaz.frm_online.enviar_comando("auction_sell", game_id.ToString());
                        this.listaHoteles.Enabled = false; ; // No se permite subastar de nuevo hasta que no llegue el pago
                    }
                    this.bJ1.Enabled = false;
                    this.bJ2.Enabled = false;
                    this.bJ3.Enabled = false;
                    this.bJ4.Enabled = false;
                    this.bVender.Enabled = false;
                    this.bSubastar.Enabled = false;
                }
            }
        }

        private void Pujar(int n_jugador)
        {
            String cantidad = this.cantidad.Text.ToString().Trim();
            int cantidad_num;
            if (cantidad == "" || cantidad == "0")
                MessageBox.Show(Mensajes.mensajeErrorPujaCantidadInvalida);
            else if (int.TryParse(cantidad, out cantidad_num) == false)
                MessageBox.Show(Mensajes.mensajeErrorPujaNoNumEntero);
            else if (cantidad_num < 0)
                MessageBox.Show(Mensajes.mensajeErrorPujaNoNegativo);
            else if (cantidad_num <= n_precio_mayor)
                MessageBox.Show(Mensajes.mensajeErrorPujaMenorMax);
            else if (cantidad_num > this.juego.jugadores[n_jugador].dinero_total)
                MessageBox.Show(Mensajes.mensajeErrorPujaNoSuficienteDinero + this.juego.jugadores[n_jugador].Nombre_color());
            else if (cantidad_num % 50 != 0)
                MessageBox.Show(Mensajes.mensajeErrorPujaNoMultiplo50);
            else if (cantidad_num < this.precio_minimo)
                MessageBox.Show(String.Format(Mensajes.mensajeErrorPujaNoPrecioMinimo, this.precio_minimo));
            else
            {
                if (MessageBox.Show(Mensajes.mensajeConfirmarPuja, Mensajes.tituloConfirmarPuja, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (this.interfaz.online)
                    {
                        int id = this.interfaz.game_id;
                        this.interfaz.frm_online.enviar_comando("auction_bid", id.ToString(), cantidad_num.ToString());
                    }
                    else
                    {
                        this.n_precio_mayor = cantidad_num;
                        this.precio_mayor.Text = cantidad_num.ToString();
                        this.mayor_postor.Text = Mensajes.textoAbreviaturaJugador + (n_jugador + 1) + " - " + this.juego.jugadores[n_jugador].Nombre_color();
                        this.n_mayor_postor = n_jugador;
                    }
                }
            }
        }

        private void bJ1_Click(object sender, EventArgs e)
        {
            this.Pujar(0);
        }

        private void bJ2_Click(object sender, EventArgs e)
        {
            this.Pujar(1);
        }

        private void bJ3_Click(object sender, EventArgs e)
        {
            this.Pujar(2);
        }

        private void bJ4_Click(object sender, EventArgs e)
        {
            this.Pujar(3);
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        public void Nueva_puja(Jugador jugador, int cantidad)
        {
            this.n_precio_mayor = cantidad;
            this.precio_mayor.Text = cantidad.ToString();
            this.mayor_postor.Text = Mensajes.textoAbreviaturaJugador + (jugador.n_jugador + 1) + " - " + jugador.Nombre_color();
            this.n_mayor_postor = jugador.n_jugador;
        }

        public void Subasta_vendida()
        {
            this.listaHoteles.Enabled = false;
            this.bVender.Enabled = false;
            this.bJ1.Enabled = false;
            this.bJ2.Enabled = false;
            this.bJ3.Enabled = false;
            this.bJ4.Enabled = false;
            this.cantidad.Enabled = false;
        }

        public void Subasta_terminada()
        {
            this.bJ1.Enabled = false;
            this.bJ2.Enabled = false;
            this.bJ3.Enabled = false;
            this.bJ4.Enabled = false;
            this.bVender.Enabled = false;
            this.cantidad.Enabled = false;
            this.bCerrar.Enabled = true;
            // Permitir que el creador de la subasta (el jugador actual) haga otra nueva
            if (this.juego.jugador_actual.nombre_online == this.interfaz.nombre_online)
                this.Rellenar_Lista_Hoteles(this.online);
            else
            {
                this.listaHoteles.BeginUpdate();
                this.listaHoteles.Items.Clear();
                this.listaHoteles.Enabled = false;
                this.listaHoteles.EndUpdate();
            }
        }

        public void Subasta_anulada()
        {
            this.DialogResult = DialogResult.Abort;
            // Ocultar la ventana si somos el jugador actual, puesto que fue abierta desde el diálogo PedirPago, pero si no lo soy, cerrar directamente para que no se quede en memoria
            if (this.juego.jugador_actual.nombre_online == this.interfaz.nombre_online)
                this.Hide();
            else
                this.Close();
        }
    }
}
