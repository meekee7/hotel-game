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
    public partial class Construir : Form
    {
        Boolean comprando_suelo;
        Juego juego;
        Hotel hotel_seleccionado;
        public Boolean cancelado;
        public Boolean fase_gratis;
        public int total_a_pagar;
        int num_5000, num_1000, num_500, num_100, num_50;
        Principal interfaz;

        public Construir(ref Juego juego, Boolean comprando_suelo, Principal interfaz)
        {
            InitializeComponent();
            this.comprando_suelo = comprando_suelo;
            this.juego = juego;
            this.cancelado = false;
            this.interfaz = interfaz;
            this.total_a_pagar = 0;
            if (this.juego.jugador_actual.posicion.tipo == Tipos.Tcasilla.fase_gratis)
                this.fase_gratis = true;
            else
                this.fase_gratis = false;
        }

        private void bCancelar_Click(object sender, EventArgs e)
        {
            this.cancelado = true;
            this.DialogResult = DialogResult.Cancel;
            this.total_a_pagar = 0;
            this.Hide();
        }

        private void bSelHotel_Click(object sender, EventArgs e)
        {
            LinkedList <Hotel> lista = this.juego.jugador_actual.hoteles;
            if (lista.Count == 0)
            {
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleConstruir);
                this.cancelado = true;
                this.DialogResult = DialogResult.Cancel;
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
                MessageBox.Show (String.Format(Mensajes.mensajeHotelContruidoEntero,this.hotel_seleccionado.nombre_txt), Mensajes.tituloNoEsPosibleConstruir);
            else if (this.comprando_suelo)
            {
                if (this.hotel_seleccionado.n_fases_construidas == this.hotel_seleccionado.n_fases_max - 1)
                    this.bSuelo.Enabled = true;
                else
                    MessageBox.Show("El hotel " + this.hotel_seleccionado.nombre_txt + " aun no tiene todas las fases hechas", "No se puede comprar el suelo");
            }
            else
            {
                switch (this.hotel_seleccionado.n_fases_construidas)
                {
                    case (0): this.bPrincipal.Enabled = true;
                        break;
                    case (1): if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                            this.bAmpli1.Enabled = true;
                        else
                            this.bSuelo.Enabled = true;
                        break;
                    case (2): if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                            this.bAmpli2.Enabled = true;
                        else
                            this.bSuelo.Enabled = true;
                        break;
                    case (3): if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                            this.bAmpli3.Enabled = true;
                        else
                            this.bSuelo.Enabled = true;
                        break;
                    case (4): if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
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
            this.HotelSeleccionado.Text = this.hotel_seleccionado.nombre_txt;
        }

        private void bPrincipal_Click(object sender, EventArgs e)
        {
            this.construir(1);
        }

        private void bAmpli1_Click(object sender, EventArgs e)
        {
            this.construir(2);
        }

        private void bAmpli2_Click(object sender, EventArgs e)
        {
            this.construir(3);
        }

        private void bAmpli3_Click(object sender, EventArgs e)
        {
            this.construir(4);
        }

        private void bAmpli4_Click(object sender, EventArgs e)
        {
            this.construir(5);
        }

        private void bSuelo_Click(object sender, EventArgs e)
        {
            this.construir(6);
        }

        private void construir(int num_fase)
        {
            String fase = "";
            switch (num_fase)
            {
                case 1: fase = "el edificio principal";
                    break;
                case 2: fase = "la primera ampliación";
                    break;
                case 3: fase = "la segunda ampliación";
                    break;
                case 4: fase = "la tercera ampliación";
                    break;
                case 5: fase = "la cuarta ampliación";
                    break;
                case 6: fase = "los complejos recreativos";
                    break;
            }
            if (this.fase_gratis)
            {
                MessageBox.Show("Estás en una casilla de tipo Fase Gratis. ¡Disfrútala!");
                if (!this.interfaz.online)
                    this.hotel_seleccionado.Ampliar();
                this.Close();
            }
            else
            {
                if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
                {
                    DialogResult res = new DialogResult();
                    if ((num_fase != 6) && (!this.comprando_suelo))
                       res = MessageBox.Show("Tienes dinero suficiente para pagar " + fase + "\n" +
                                             "Si decides continuar, el resultado del dado ha de ser cumplido obligatoriamente\n" +
                                             "¿Deseas tirar el dado?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                    else
                       res = MessageBox.Show("Tienes dinero suficiente para pagar los complejos recreativos.\n" +
                                              "No se necesita el permiso para construirlos. ¿Deseas continuar?", "Confirmación de construcción", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        if ((num_fase != 6) && (!this.comprando_suelo))
                        {
                            Dado_construccion dado_cons = new Dado_construccion(this.interfaz.frm_online, this.interfaz.game_id);
                            dado_cons.ShowDialog(); // El resultado será comprobado por el servidor
                            if (dado_cons.resultado == Tipos.Resultado_dado_cons.Permitido)
                                this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                            else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Doble)
                                this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                            else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Gratis)
                                this.total_a_pagar = 0;
                            else if (dado_cons.resultado == Tipos.Resultado_dado_cons.Denegado)
                            {
                                this.total_a_pagar = -1;
                                this.cancelado = true;
                                this.DialogResult = DialogResult.Abort;
                            }
                            dado_cons.Close();
                        }
                        else
                            this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                        if (this.total_a_pagar > 0)
                        {
                            PedirPago frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado, this.juego.jugador_actual, this.interfaz);
                            frm_pago.ShowDialog();
                            if (!frm_pago.cancelado) // Sólo se puede cancelar si el jugador ha subastado el hotel en cuestión o si ha sido eliminado
                            {
                                this.num_5000 = frm_pago.n_5000;
                                this.num_1000 = frm_pago.n_1000;
                                this.num_500 = frm_pago.n_500;
                                this.num_100 = frm_pago.n_100;
                                this.num_50 = frm_pago.n_50;
                                if (!this.interfaz.online)
                                {
                                    int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                                    this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(this.num_5000, this.num_1000, this.num_500, this.num_100, this.num_50);
                                    if (frm_pago.total_seleccionado > this.total_a_pagar)
                                    {
                                        Principal.Calcular_Devolucion((frm_pago.total_seleccionado - this.total_a_pagar), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                                        this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                                    }
                                }
                            }
                            else
                                this.cancelado = true;
                            frm_pago.Close();
                        }
                        if ((this.total_a_pagar != -1) && (!this.interfaz.online))
                                this.hotel_seleccionado.Ampliar();
                        this.Close();
                    }
                }
                else
                    MessageBox.Show("No tienes el dinero suficiente para comprar " + fase, "No es posible construir");
            }
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, this.juego.jug_actual - 1, false);
            frm_ver_hoteles.Show();
        }

        private void Construir_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.cancelado) // Pintar la fase en el tablero y enviar el comando
            {
                if (this.interfaz.online)
                {
                    int game_id = this.interfaz.game_id;
                    if (this.fase_gratis) // Se diferencian 3 tipos para detectar hacks
                        this.interfaz.frm_online.enviar_comando("build_phase", game_id.ToString(), this.hotel_seleccionado.nombre_txt, "0");
                    else if (this.total_a_pagar == 0)
                        this.interfaz.frm_online.enviar_comando("build_phase", game_id.ToString(), this.hotel_seleccionado.nombre_txt, "1");
                    else
                        this.interfaz.frm_online.enviar_comando("build_phase", game_id.ToString(), this.hotel_seleccionado.nombre_txt, "2", this.num_5000.ToString(),
                        this.num_1000.ToString(), this.num_500.ToString(), this.num_100.ToString(), this.num_50.ToString());
                }
                else
                    this.interfaz.Dibujar_Fase(this.hotel_seleccionado, this.hotel_seleccionado.n_fases_construidas - 1);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
