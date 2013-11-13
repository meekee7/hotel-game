using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class Construir : Form
    {
        readonly Boolean comprando_suelo;
        Juego juego;
        Hotel hotel_seleccionado;
        public Boolean cancelado;
        public Boolean fase_gratis;
        public int total_a_pagar;
        int num_5000, num_1000, num_500, num_100, num_50;
        readonly Principal interfaz;

        public Construir(ref Juego juego, Boolean comprando_suelo, Principal interfaz)
        {
            InitializeComponent();
            this.comprando_suelo = comprando_suelo;
            this.juego = juego;
            this.cancelado = false;
            this.interfaz = interfaz;
            this.total_a_pagar = 0;
            this.fase_gratis = this.juego.jugador_actual.posicion.tipo == Tipos.Tcasilla.fase_gratis;
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
            this.PrecioPpal.Text = String.Empty;
            this.PrecioAmp1.Text = String.Empty;
            this.PrecioAmp2.Text = String.Empty;
            this.PrecioAmp3.Text = String.Empty;
            this.PrecioAmp4.Text = String.Empty;
            this.PrecioSuelo.Text = String.Empty;
            this.listaHoteles.BringToFront();
            if (!this.hotel_seleccionado.Ampliable())
                MessageBox.Show (String.Format(Mensajes.mensajeHotelContruidoEntero,this.hotel_seleccionado.nombre_txt), Mensajes.tituloNoEsPosibleConstruir);
            else if (this.comprando_suelo)
            {
                if (this.hotel_seleccionado.n_fases_construidas == this.hotel_seleccionado.n_fases_max - 1)
                    this.bSuelo.Enabled = true;
                else
                    MessageBox.Show(String.Format(Mensajes.mensajeHotelFasesIncompletas,this.hotel_seleccionado.nombre_txt, Mensajes.tituloNoSePuedeComparSuelo));
            }
            else
            {
                switch (this.hotel_seleccionado.n_fases_construidas)
                {
                    case (0):
                        this.bPrincipal.Enabled = true;
                        this.PrecioPpal.Text = this.hotel_seleccionado.Precio_Ampliacion(0).ToString();
                        break;
                    case (1):
                        if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                        {
                            this.bAmpli1.Enabled = true;
                            this.PrecioAmp1.Text = this.hotel_seleccionado.Precio_Ampliacion(1).ToString();
                        }
                        else
                        {
                            this.bSuelo.Enabled = true;
                            this.PrecioSuelo.Text = this.hotel_seleccionado.Precio_Suelo().ToString();
                        }
                        break;
                    case (2):
                        if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                        {
                            this.bAmpli2.Enabled = true;
                            this.PrecioAmp2.Text = this.hotel_seleccionado.Precio_Ampliacion(2).ToString();
                        }
                        else
                        {
                            this.bSuelo.Enabled = true;
                            this.PrecioSuelo.Text = this.hotel_seleccionado.Precio_Suelo().ToString();
                        }
                        break;
                    case (3):
                        if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                        {
                            this.bAmpli3.Enabled = true;
                            this.PrecioAmp3.Text = this.hotel_seleccionado.Precio_Ampliacion(3).ToString();
                        }
                        else
                        {
                            this.bSuelo.Enabled = true;
                            this.PrecioSuelo.Text = this.hotel_seleccionado.Precio_Suelo().ToString();
                        }
                        break;
                    case (4):
                        if (this.hotel_seleccionado.n_fases_construidas < (this.hotel_seleccionado.n_fases_max - 1))
                        {
                            this.bAmpli4.Enabled = true;
                            this.PrecioAmp4.Text = this.hotel_seleccionado.Precio_Ampliacion(4).ToString();
                        }
                        else
                        {
                            this.bSuelo.Enabled = true;
                            this.PrecioSuelo.Text = this.hotel_seleccionado.Precio_Suelo().ToString();
                        }
                        break;
                    case (5):
                        this.bSuelo.Enabled = true;
                        this.PrecioSuelo.Text = this.hotel_seleccionado.Precio_Suelo().ToString();
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
                case 1: fase = Mensajes.textoEdificioPrincipal;
                    break;
                case 2: fase = Mensajes.textoPrimeraAmpliacion;
                    break;
                case 3: fase = Mensajes.textoSegundaAmpliacion;
                    break;
                case 4: fase = Mensajes.textoTerceraAmpliacion;
                    break;
                case 5: fase = Mensajes.textoCuartaAmpliacion;
                    break;
                case 6: fase = Mensajes.textoComplejosRecreativos;
                    break;
            }
            if (this.fase_gratis)
            {
                MessageBox.Show(Mensajes.mensajeEstasEnCasillaTipoFaseGratis);
                if (!this.interfaz.online)
                    this.hotel_seleccionado.Ampliar();
                this.Close();
            }
            else
            {
                if (this.juego.jugador_actual.dinero_total >= this.hotel_seleccionado.Precio_Sig_Ampliacion())
                {
                    DialogResult res;
                    if ((num_fase != 6) && (!this.comprando_suelo))
                       res = MessageBox.Show(String.Format(Mensajes.mensajeSuficienteDineroParaFase,fase), Mensajes.tituloConfirmacionDeConstruccion, MessageBoxButtons.YesNo);
                    else
                       res = MessageBox.Show(Mensajes.mensajeSufucienteDineroParaComplejos, Mensajes.tituloConfirmacionDeConstruccion, MessageBoxButtons.YesNo);
                    if (res != DialogResult.Yes)
                        return;
                    if ((num_fase != 6) && (!this.comprando_suelo))
                    {
                        var dado_cons = new Dado_construccion(this.interfaz.frm_online, this.interfaz.game_id, hotel_seleccionado);
                        dado_cons.ShowDialog(); // El resultado será comprobado por el servidor
                        switch (dado_cons.resultado)
                        {
                            case Tipos.Resultado_dado_cons.Permitido:
                                this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                                break;
                            case Tipos.Resultado_dado_cons.Doble:
                                this.total_a_pagar = (this.hotel_seleccionado.Precio_Sig_Ampliacion() * 2);
                                break;
                            case Tipos.Resultado_dado_cons.Gratis:
                                this.total_a_pagar = 0;
                                break;
                            case Tipos.Resultado_dado_cons.Denegado:
                                this.total_a_pagar = -1;
                                this.cancelado = true;
                                this.DialogResult = DialogResult.Abort;
                                break;
                        }
                        dado_cons.Close();
                    }
                    else
                        this.total_a_pagar = this.hotel_seleccionado.Precio_Sig_Ampliacion();
                    if (this.total_a_pagar > 0)
                    {
                        var frm_pago = new PedirPago(this.total_a_pagar, ref this.juego, ref this.hotel_seleccionado, this.juego.jugador_actual, this.interfaz);
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
                                this.juego.jugador_actual.Pagar_Ampliacion_o_Entrada(this.num_5000, this.num_1000, this.num_500, this.num_100, this.num_50);
                                if (frm_pago.total_seleccionado > this.total_a_pagar)
                                {
                                    int n_5000;
                                    int n_1000;
                                    int n_500;
                                    int n_100;
                                    int n_50;
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
                else
                    MessageBox.Show(String.Format(Mensajes.mensajeSinDineroParaComprar, fase), Mensajes.tituloNoEsPosibleConstruir);
            }
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            var frm_ver_hoteles = new VerHoteles(ref this.juego, this.juego.jug_actual - 1, false);
            frm_ver_hoteles.Show(this);
            this.bVerHoteles.Enabled = false;
        }

        private void Construir_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.cancelado)
                return;
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
