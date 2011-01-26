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
    public partial class Construir : Form
    {
        Boolean comprando_suelo;
        Juego juego;
        Hotel hotel_seleccionado;
        public Boolean cancelado;
        public int total_a_pagar;

        public Construir(ref Juego juego, Boolean comprando_suelo)
        {
            InitializeComponent();
            this.comprando_suelo = comprando_suelo;
            this.juego = juego;
            this.cancelado = false;
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.cancelado = true;
            this.Hide();
        }

        private void bSelHotel_Click(object sender, EventArgs e)
        {
            LinkedList <Hotel> lista = this.juego.jugador_actual.hoteles;
            if (lista.Count == 0)
            {
                MessageBox.Show("No posees ningún hotel", "No es posible construir");
                this.Hide();
            }
            else
            {
                this.listaHoteles.BeginUpdate();
                this.listaHoteles.Items.Clear();
                foreach (Hotel hotel in lista)
                    this.listaHoteles.Items.Add(hotel.nombre_txt);
                this.listaHoteles.EndUpdate();
                // No se necesita para ComboBox
                //this.listaHoteles.Height = (this.listaHoteles.Items.Count + 1) * this.listaHoteles.ItemHeight;
                this.listaHoteles.Show();
            }
        }

        private void listaHoteles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.hotel_seleccionado = this.juego.hoteles.First(Hotel => Hotel.nombre_txt == this.listaHoteles.SelectedItem.ToString());
            // Tenemos el hotel en cuestión
            this.bPrincipal.Enabled = false;
            this.bAmpli1.Enabled = false;
            this.bAmpli2.Enabled = false;
            this.bAmpli3.Enabled = false;
            this.bAmpli4.Enabled = false;
            this.bSuelo.Enabled = false;
            this.listaHoteles.BringToFront();
            if (!this.hotel_seleccionado.Ampliable())
                MessageBox.Show ("El hotel " + this.hotel_seleccionado.nombre_txt + " está completamente construido", "No es posible construir");
            else
            {
                switch (this.hotel_seleccionado.n_ampliaciones_construidas)
                {
                    case (0): this.bPrincipal.Enabled = true;
                              break;
                    case (1): if (this.hotel_seleccionado.n_ampliaciones_construidas < (this.hotel_seleccionado.n_ampliaciones_max - 1))
                                  this.bAmpli1.Enabled = true;
                              else
                                  this.bSuelo.Enabled = true;
                              break;
                    case (2): if (this.hotel_seleccionado.n_ampliaciones_construidas < (this.hotel_seleccionado.n_ampliaciones_max - 1))
                                  this.bAmpli2.Enabled = true;
                              else
                                  this.bSuelo.Enabled = true;
                              break;
                    case (3): if (this.hotel_seleccionado.n_ampliaciones_construidas < (this.hotel_seleccionado.n_ampliaciones_max - 1))
                                  this.bAmpli3.Enabled = true;
                              else
                                  this.bSuelo.Enabled = true;
                              break;
                    case (4): if (this.hotel_seleccionado.n_ampliaciones_construidas < (this.hotel_seleccionado.n_ampliaciones_max - 1))
                                  this.bAmpli4.Enabled = true;
                              else
                                  this.bSuelo.Enabled = true;
                              break;
                    case (5): this.bSuelo.Enabled = true;
                              break;
                }
            }
            this.listaHoteles.Hide();
            this.bSelHotel.Enabled = true;
            this.HotelSeleccionado.Text = this.hotel_seleccionado.nombre.ToString();
        }

        private void bPrincipal_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
            {
                DialogResult res = MessageBox.Show("Tienes dinero suficiente para pagar el edificio principal\n" +
                                                    "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                                    "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    Dado_construccion dado_cons = new Dado_construccion();
                    dado_cons.ShowDialog();
                    if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                        this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                        this.total_a_pagar = 0;
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                        this.total_a_pagar = -1;
                    dado_cons.Close();
                    if (this.total_a_pagar > 0)
                    {
                        PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado);
                        frm_pago.ShowDialog();
                        if (frm_pago.cancelado)
                        {
                            frm_pago.Close();
                            this.Close();
                            return;
                        }
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.total_a_pagar)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);    
                            this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                    }
                    if (this.total_a_pagar != -1)
                        this.hotel_seleccionado.Ampliar();
                    this.Close();
                }
            }
            else
                MessageBox.Show("No tienes el suficiente dinero para comprar el edificio principal", "No es posible construir");
        }

        private void bAmpli1_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
            {
                DialogResult res = MessageBox.Show("Tienes dinero suficiente para pagar la primera ampliación\n" +
                                                    "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                                    "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    Dado_construccion dado_cons = new Dado_construccion();
                    dado_cons.ShowDialog();
                    if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                        this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                        this.total_a_pagar = 0;
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                        this.total_a_pagar = -1;
                    dado_cons.Close();
                    if (this.total_a_pagar > 0)
                    {
                        PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado);
                        frm_pago.ShowDialog();
                        if (frm_pago.cancelado)
                        {
                            frm_pago.Close();
                            this.Close();
                            return;
                        }
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.total_a_pagar)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                    }
                    if (this.total_a_pagar != -1)
                        this.hotel_seleccionado.Ampliar();
                    this.Close();
                }
            }
            else
                MessageBox.Show("No tienes el suficiente dinero para comprar la primera ampliación", "No es posible construir");
        }

        private void bAmpli2_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
            {
                DialogResult res = MessageBox.Show("Tienes dinero suficiente para pagar la segunda ampliación\n" +
                                                    "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                                    "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    Dado_construccion dado_cons = new Dado_construccion();
                    dado_cons.ShowDialog();
                    if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                        this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                        this.total_a_pagar = 0;
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                        this.total_a_pagar = -1;
                    dado_cons.Close();
                    if (this.total_a_pagar > 0)
                    {
                        PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado);
                        frm_pago.ShowDialog();
                        if (frm_pago.cancelado)
                        {
                            frm_pago.Close();
                            this.Close();
                            return;
                        }
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.total_a_pagar)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                    }
                    if (this.total_a_pagar != -1)
                        this.hotel_seleccionado.Ampliar();
                    this.Close();
                }
            }
            else
                MessageBox.Show("No tienes el suficiente dinero para comprar la segunda ampliación", "No es posible construir");
        }

        private void bAmpli3_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
            {
                DialogResult res = MessageBox.Show("Tienes dinero suficiente para pagar la tercera ampliación\n" +
                                                    "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                                    "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    Dado_construccion dado_cons = new Dado_construccion();
                    dado_cons.ShowDialog();
                    if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                        this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                        this.total_a_pagar = 0;
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                        this.total_a_pagar = -1;
                    dado_cons.Close();
                    if (this.total_a_pagar > 0)
                    {
                        PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado);
                        frm_pago.ShowDialog();
                        if (frm_pago.cancelado)
                        {
                            frm_pago.Close();
                            this.Close();
                            return;
                        }
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.total_a_pagar)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                    }
                    if (this.total_a_pagar != -1)
                        this.hotel_seleccionado.Ampliar();
                    this.Close();
                }
            }
            else
                MessageBox.Show("No tienes el suficiente dinero para comprar la tercera ampliación", "No es posible construir");
        }

        private void bAmpli4_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
            {
                DialogResult res = MessageBox.Show("Tienes dinero suficiente para pagar la cuarta ampliación\n" +
                                                    "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                                    "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    Dado_construccion dado_cons = new Dado_construccion();
                    dado_cons.ShowDialog();
                    if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                        this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                        this.total_a_pagar = 0;
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                        this.total_a_pagar = -1;
                    dado_cons.Close();
                    if (this.total_a_pagar > 0)
                    {
                        PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado);
                        frm_pago.ShowDialog();
                        if (frm_pago.cancelado)
                        {
                            frm_pago.Close();
                            this.Close();
                            return;
                        }
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.total_a_pagar)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                    }
                    if (this.total_a_pagar != -1)
                        this.hotel_seleccionado.Ampliar();
                    this.Close();
                }
            }
            else
                MessageBox.Show("No tienes el suficiente dinero para comprar la cuarta ampliación", "No es posible construir");
        }

        private void bSuelo_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
            {
                DialogResult res = MessageBox.Show("Tienes dinero suficiente para pagar los complejos recreativos\n" +
                                                    "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                                    "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    Dado_construccion dado_cons = new Dado_construccion(); // TODO: quitar esto porque no necesita permiso de construcción
                    dado_cons.ShowDialog();
                    if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                        this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                        this.total_a_pagar = 0;
                    else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                        this.total_a_pagar = -1;
                    dado_cons.Close();
                    if (this.total_a_pagar > 0)
                    {
                        PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado);
                        frm_pago.ShowDialog();
                        if (frm_pago.cancelado)
                        {
                            frm_pago.Close();
                            this.Close();
                            return;
                        }
                        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                        this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > this.total_a_pagar)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                    }
                    if (this.total_a_pagar != -1)
                        this.hotel_seleccionado.Ampliar();
                    this.Close();
                }
            }
            else
                MessageBox.Show("No tienes el suficiente dinero para comprar los complejos recreativos", "No es posible construir");
            this.Hide();
        }
    }
}
