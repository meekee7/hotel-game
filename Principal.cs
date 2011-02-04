using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;

namespace Juego_Hotel
{
    public partial class Principal : Form
    {
        public Juego juego;
        int[] tiradas_ini;
        Sel_colores frm_colores = new Sel_colores();
        Image posRojo_orig, posAzul_orig, posVerde_orig, posAmarillo_orig, img_entrada;
        // TODO: Revisar todos los destructores para las pérdidas de memoria

        public Principal()
        {
            InitializeComponent();
            this.juego = new Juego();
            posRojo_orig = (Image) posRojo.Image.Clone();
            posAzul_orig = (Image) posAzul.Image.Clone();
            posVerde_orig = (Image) posVerde.Image.Clone();
            posAmarillo_orig = (Image) posAmarillo.Image.Clone();
            this.img_entrada = (Image) global::Juego_Hotel.Properties.Resources.Entrada.Clone();
        }

        private void bIniciar_Click(object sender, EventArgs e)
        {
            // Buscar número de jugadores
            if (this.juego.n_jugadores == 0)
            {
                MessageBox.Show("Seleccione el nº de jugadores", "No es posible iniciar");
                return;
            }
            else
            {
                this.Crear_Jugadores();
                // Decidir quien empieza
                this.tiradas_ini = new int [this.juego.n_jugadores];
                int i;
                for (i = 0 ; i < this.juego.n_jugadores ; i++)
                {
                    this.tiradas_ini[i] = this.juego.dado.tirar();
                }
                // Encontrando el mayor
                int max = 0;
                for (i = 0; i < this.juego.n_jugadores; i++)
                {
                    if (this.tiradas_ini[i] > max)
                    {
                        max = this.tiradas_ini[i];
                        this.juego.jug_inicial = i;
                    }
                }
                this.juego.jug_inicial++; // Para no comenzar en 0
                this.jug_ini.Text = "Jugador inicial: " + this.juego.jug_inicial.ToString();
                this.colorJugIni.Text = this.juego.jugadores[this.juego.jug_inicial - 1].color.ToString();
                this.juego.jug_actual = this.juego.jug_inicial;
                this.juego.Cambiar_jugador_actual();
                this.Establecer_Te_Toca();
                this.bIniciar.Enabled = false;
                this.bColores.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.grupoNJugadores.Enabled = false;
                this.posJ1.Text = "Casilla: 0";
                this.posJ2.Text = "Casilla: 0";
                this.posJ3.Text = "Casilla: 0";
                this.posJ4.Text = "Casilla: 0";
                this.colorJ1.Text = "Color: " + this.juego.jugadores[0].color.ToString();
                this.colorJ2.Text = "Color: " + this.juego.jugadores[1].color.ToString();
                this.dineroJ1.Text = "Dinero: " + this.juego.jugadores[0].dinero_total;
                this.dineroJ2.Text = "Dinero: " + this.juego.jugadores[1].dinero_total;
                if (this.juego.n_jugadores > 2)
                {
                    this.dineroJ3.Text = "Dinero: " + this.juego.jugadores[2].dinero_total;
                    this.colorJ3.Text = "Color: " + this.juego.jugadores[2].color.ToString();
                }
                if (this.juego.n_jugadores > 3)
                {
                    this.dineroJ4.Text = "Dinero: " + this.juego.jugadores[3].dinero_total;
                    this.colorJ4.Text = "Color: " + this.juego.jugadores[3].color.ToString();
                }
                this.bDado.Enabled = true;
                this.bCobrarBanca.Enabled = true;
            }
        }

        public Bitmap RotarImagen(Image imagen, float angulo)
        {
            if (angulo == 0)
                return (Bitmap) imagen;
            PointF offset = new PointF((float)imagen.Width / 2, (float)imagen.Height / 2); // El centro de la imagen
            if (imagen == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(imagen.Width, imagen.Height);
            rotatedBmp.SetResolution(imagen.HorizontalResolution, imagen.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angulo);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(imagen, new PointF(0, 0));

            return rotatedBmp;
        }

        public void Establecer_Te_Toca()
        {
            this.turnoJ1.Text = "";
            this.turnoJ2.Text = "";
            this.turnoJ3.Text = "";
            this.turnoJ4.Text = "";
            switch (this.juego.jug_actual)
            {
                case 1: this.turnoJ1.Text = "Te toca";
                    break;
                case 2: this.turnoJ2.Text = "Te toca";
                    break;
                case 3: this.turnoJ3.Text = "Te toca";
                    break;
                case 4: this.turnoJ4.Text = "Te toca";
                    break;
            }
        }

        public Boolean Todos_Eliminados()
        {
            return this.juego.n_jugadores_activos == 1;
        }

        public void Finalizar_Partida(Jugador ganador)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button) control.Enabled = false;
            }
            this.bReiniciar.Enabled = true;
            this.bSalir.Enabled = true;
            this.controlJ1.Enabled = false;
            this.controlJ2.Enabled = false;
            this.controlJ3.Enabled = false;
            this.controlJ4.Enabled = false;
            MessageBox.Show("Partida finalizada. Ha ganado el jugador " + ganador.color.ToString(), "Hotel");
        }

        public int Sig_jugador_Activo()
        {
            int sig_jugador = this.juego.jug_actual;
            Boolean valido = false;

            while (!valido)
            {
                if (sig_jugador < this.juego.n_jugadores)
                    sig_jugador = sig_jugador + 1;
                else
                    sig_jugador = 1;
                if (this.juego.jugadores[sig_jugador-1].Eliminado() == false)
                    valido = true;
            }

            return sig_jugador;
        }

        public void Pasar_turno()
        {
            if (Todos_Eliminados())
                Finalizar_Partida(this.juego.jugadores[Sig_jugador_Activo()-1]);
            else
            {
                this.juego.jug_actual = Sig_jugador_Activo();
                this.Establecer_Te_Toca();
                this.juego.Cambiar_jugador_actual();
                this.bDado.Enabled = true;
                this.bTurno.Enabled = false;
                this.bComprar.Enabled = false;
                this.bConstruir.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.juego.jugador_actual.pago_ultimo_turno = false;
            }
        }

        public void Crear_Jugadores()
        {
            this.juego.jugadores = new Jugador[this.juego.n_jugadores];
            this.juego.n_jugadores_activos = this.juego.n_jugadores;
            XmlDocument configuracion = new XmlDocument();
            configuracion.Load ("../../Config.xml");
            XmlNode config_dinero = configuracion.GetElementsByTagName("money_per_player")[0];
            XmlNode nodo_cantidades;
            if (this.juego.n_jugadores == 2)
                nodo_cantidades = ((XmlElement)config_dinero).GetElementsByTagName("two_players")[0];
            else
                nodo_cantidades = ((XmlElement)config_dinero).GetElementsByTagName("three_or_four_players")[0];
            int n_5000, n_1000, n_500, n_100, n_50;
            n_5000 = Convert.ToInt16(nodo_cantidades.ChildNodes[0].FirstChild.Value);
            n_1000 = Convert.ToInt16(nodo_cantidades.ChildNodes[1].FirstChild.Value);
            n_500 = Convert.ToInt16(nodo_cantidades.ChildNodes[2].FirstChild.Value);
            n_100 = Convert.ToInt16(nodo_cantidades.ChildNodes[3].FirstChild.Value);
            n_50 = Convert.ToInt16(nodo_cantidades.ChildNodes[4].FirstChild.Value);
            switch (this.juego.n_jugadores)
            {
                case 4: this.juego.jugadores[3] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j4);
                        this.controlJ4.Enabled = true;
                        goto case 3;
                case 3: this.juego.jugadores[2] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j3);
                        this.controlJ3.Enabled = true;
                        goto case 2;
                case 2: this.juego.jugadores[1] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j2);
                        this.juego.jugadores[0] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j1);
                        this.controlJ2.Enabled = true;
                        this.controlJ1.Enabled = true;
                        break;
            }
        }

        private void bDado_Click(object sender, EventArgs e)
        {
            this.juego.ultimo_res_dado = this.juego.dado.tirar();
            this.resDado.Text = "Dado: " + this.juego.ultimo_res_dado.ToString();
            this.bComprarSuelo.Enabled = true;
            Jugador jugador = this.juego.jugador_actual;
            jugador.posicion.ocupada = false; // Desocupamos la casilla
            if ((jugador.posicion.numero + this.juego.ultimo_res_dado) <= 31) // Damos la vuelta al tablero
            {
                jugador.posicion = this.juego.casillas[jugador.posicion.numero + this.juego.ultimo_res_dado];
            }
            else
            {
                jugador.posicion = this.juego.casillas[jugador.posicion.numero + this.juego.ultimo_res_dado - 31];
            }
            while (jugador.posicion.ocupada) // Hay que avanzar una porque está ocupada
            {
                jugador.posicion = this.juego.casillas[jugador.posicion.numero + 1];
            }
            jugador.posicion.ocupada = true; // Ocupamos la casilla
            // Pintamos el coche en su lugar
            switch (this.juego.jugador_actual.color)
            {
                case Tipos.Tcolor.rojo: this.posRojo.Image = RotarImagen(posRojo_orig, jugador.posicion.pos_coche.grados);
                                        this.posRojo.Location =
                                            new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.X,
                                                      this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.Y);
                                        break;
                case Tipos.Tcolor.azul: this.posAzul.Image = RotarImagen(posAzul_orig, jugador.posicion.pos_coche.grados);
                                        this.posAzul.Location =
                                                                new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.X,
                                                                          this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.Y);
                                        break;
                case Tipos.Tcolor.verde: this.posVerde.Image = RotarImagen(posVerde_orig, jugador.posicion.pos_coche.grados);
                                         this.posVerde.Location =
                                                                new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.X,
                                                                          this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.Y);
                                        break;
                case Tipos.Tcolor.amarillo: this.posAmarillo.Image = RotarImagen(posAmarillo_orig, jugador.posicion.pos_coche.grados);
                                            this.posAmarillo.Location =
                                                                new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.X,
                                                                          this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.Y);
                                        break;
            }
            this.bTurno.Enabled = true;
            // Poner casilla actual a cada uno
            switch (this.juego.jug_actual)
            {
                case 1: this.posJ1.Text = "Casilla: " + jugador.posicion.numero.ToString();
                        break;
                case 2: this.posJ2.Text = "Casilla: " + jugador.posicion.numero.ToString();
                        break;
                case 3: this.posJ3.Text = "Casilla: " + jugador.posicion.numero.ToString();
                        break;
                case 4: this.posJ4.Text = "Casilla: " + jugador.posicion.numero.ToString();
                        break;
            }
            // Activar botones según el tipo de casilla
            switch (jugador.posicion.tipo)
            {
                case Tipos.Tcasilla.comprar: this.bComprar.Enabled = true;
                                             break;
                case Tipos.Tcasilla.construir: this.bConstruir.Enabled = true;
                                               break;
                case Tipos.Tcasilla.fase_gratis: this.bConstruir.Enabled = true;
                                                 break;
                default: this.bConstruir.Enabled = false;
                         this.bComprar.Enabled = false;
                         break;
            }
            if (this.juego.ultimo_res_dado == 6)
            {
                MessageBox.Show("Has sacado un 6. Puedes volver a tirar");
                this.bDado.Enabled = true;
            }
            else
                this.bDado.Enabled = false;
        }

        private void bReiniciar_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("¿Estás seguro de que quieres reiniciar la partida?", "Hotel", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                this.bTurno.Enabled = false;
                this.bDado.Enabled = false;
                this.bIniciar.Enabled = true;
                this.bColores.Enabled = true;
                this.grupoNJugadores.Enabled = true;
                this.turnoJ1.Text = "";
                this.turnoJ2.Text = "";
                this.turnoJ3.Text = "";
                this.turnoJ4.Text = "";
                this.bComprar.Enabled = false;
                this.bConstruir.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.controlJ1.Enabled = false;
                this.controlJ2.Enabled = false;
                this.controlJ3.Enabled = false;
                this.controlJ4.Enabled = false;
                this.bCobrarBanca.Enabled = false;
                this.posJ1.Text = "Casilla:";
                this.posJ2.Text = "Casilla:";
                this.posJ3.Text = "Casilla:";
                this.posJ4.Text = "Casilla:";
                this.dineroJ1.Text = "Dinero:";
                this.dineroJ2.Text = "Dinero:";
                this.dineroJ3.Text = "Dinero:";
                this.dineroJ4.Text = "Dinero:";
                this.colorJ1.Text = "Color:";
                this.colorJ2.Text = "Color:";
                this.colorJ3.Text = "Color:";
                this.colorJ4.Text = "Color:";
                this.juego.jugadores = null;
                this.posRojo.Image = posRojo_orig;
                this.posAzul.Image = posAzul_orig;
                this.posAmarillo.Image = posAmarillo_orig;
                this.posVerde.Image = posVerde_orig;
                this.posRojo.Location = new Point(40, 322);
                this.posAzul.Location = new Point(60, 322);
                this.posVerde.Location = new Point(80, 322);
                this.posAmarillo.Location = new Point(100, 322);
                Control[] lista_entradas = this.Controls.Find("entrada", true);
                foreach (Control entrada in lista_entradas)
                {
                    this.Controls.Remove(entrada);
                    entrada.Dispose();
                }
                int num_jugadores = this.juego.n_jugadores;
                this.juego = new Juego();
                this.juego.n_jugadores = num_jugadores;
            }
        }

        private void bTurno_Click(object sender, EventArgs e)
        {
            this.Pasar_turno();
        }

        private void bColores_Click(object sender, EventArgs e)
        {
            if (this.juego.n_jugadores == 0)
                MessageBox.Show("No se ha indicado aun el número de jugadores", "Selección de colores");
            else
            {
                this.frm_colores.habilitarControles(this.juego.n_jugadores);
                this.frm_colores.ShowDialog();
            }
        }

        private void dos_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            this.juego.n_jugadores = 2;
        }

        private void tres_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            this.juego.n_jugadores = 3;
        }

        private void cuatro_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            this.juego.n_jugadores = 4;
        }

        private void bComprar_Click(object sender, EventArgs e)
        {
            Tipos.Tnombre_hotel nombre_izq;
            Tipos.Tnombre_hotel nombre_der;
            ComprarHotel frm_comprar_hotel = new ComprarHotel(ref juego);
            Jugador jugador = this.juego.jugadores[this.juego.jug_actual - 1];
            nombre_izq = this.juego.jugadores[this.juego.jug_actual - 1].posicion.hotel_izq;
            nombre_der = this.juego.jugadores[this.juego.jug_actual - 1].posicion.hotel_der;
            frm_comprar_hotel.HabilitarControles (nombre_izq, nombre_der);
            frm_comprar_hotel.ShowDialog();
            // Ya ha sido seleccionado cual comprar. Haciendo efectiva la compra
            if (!frm_comprar_hotel.cancelado)
            {
                Hotel hotel;
                if (frm_comprar_hotel.comprado_izq)
                    hotel = this.juego.hoteles[(int) nombre_izq];
                else
                    hotel = this.juego.hoteles[(int) nombre_der];

                if (hotel.dueño != null) // El hotel tiene dueño
                {
                    if (hotel.dueño != jugador)
                    {
                        if (hotel.n_ampliaciones_construidas == 0) // Se puede expropiar
                        {
                            String texto = "¿Quieres expropiar el hotel " + hotel.nombre + " al jugador "
                                + hotel.dueño.color.ToString() + "?";
                            DialogResult dr = MessageBox.Show(texto, "Expropiación posible", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                if (hotel.precio_expropiacion > this.juego.jugador_actual.dinero_total)
                                    MessageBox.Show("No tienes suficientes fondos para realizar la expropiación");
                                else
                                    this.Comprar_hotel(ref hotel, ref jugador, true);
                            }
                        }
                        else
                            MessageBox.Show("El hotel ya tiene dueño y no es expropiable", "Expropiación imposible");
                    }
                    else
                        MessageBox.Show("Ya posees este hotel", "No es posible realizar la compra");
                }
                else // Es posible comprar el hotel
                {
                    if (hotel.precio > jugador.dinero_total)
                        MessageBox.Show("No tienes suficientes fondos para realizar la compra del hotel " + hotel.nombre_txt);
                    else
                    {
                        DialogResult dr = MessageBox.Show("Puedes comprar el hotel. Si decides continuar, estás obligado a pagarlo\n" +
                                                          "¿Deseas realizar la compra?", "Comprar hotel", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                            this.Comprar_hotel(ref hotel, ref jugador, false);
                    }
                }
            }
            frm_comprar_hotel.Close();
            this.Actualizar_Dinero_Jugador_Actual();
        }

        void Comprar_hotel (ref Hotel hotel, ref Jugador jugador, Boolean expropiando)
        {
            int dinero_necesario;
            int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;

            if (expropiando)
                dinero_necesario = hotel.precio_expropiacion;
            else
                dinero_necesario = hotel.precio;

            PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, this.juego.jugador_actual);
            frm_pago.ShowDialog();
            if (expropiando)
            {
                Jugador dueño_ant = hotel.dueño;
                dueño_ant.Hotel_Expropiado(ref hotel);
                jugador.Comprar_Hotel(ref hotel, ref dueño_ant, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
                if (frm_pago.total_seleccionado > dinero_necesario)
                {
                    Principal.Calcular_Devolucion(ref dueño_ant, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                    jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                }
            }
            else
            {
                jugador.Comprar_Hotel(ref hotel, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
                if (frm_pago.total_seleccionado > dinero_necesario)
                {
                    Principal.Calcular_Devolucion((frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                    jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                }
            }
            frm_pago.Close();
            this.bComprar.Enabled = false;
            this.Actualizar_Dinero_Jugadores();
        }

        public static void Calcular_Devolucion (int cantidad, out int n_5000, out int n_1000, out int n_500, out int n_100, out int n_50)
        {
            n_5000 = 0;
            n_1000 = 0;
            n_500 = 0;
            n_100 = 0;
            n_50 = 0;

            while (cantidad > 0)
            {
                if (cantidad >= 5000)
                {
                    cantidad -= 5000;
                    n_5000++;
                }
                else if (cantidad >= 1000)
                {
                    cantidad -= 1000;
                    n_1000++;
                }
                else if (cantidad >= 500)
                {
                    cantidad -= 500;
                    n_500++;
                }
                else if (cantidad >= 100)
                {
                    cantidad -= 100;
                    n_100++;
                }
                else if (cantidad >= 50)
                {
                    cantidad -= 50;
                    n_50++;
                }
            }
        }

        public static void Calcular_Devolucion(ref Jugador jugador, int cantidad, out int n_5000, out int n_1000, out int n_500, out int n_100, out int n_50)
        {
            // Al restar la devolución a un jugador, hay que haber ingresado los fondos previamente por si acaso el jugador
            // no tiene fondos suficientes para devolver antes de haber recibido el cobro
            // En el caso de que no tenga cambio justo, el sistema automáticamente obtendrá los billetes necesarios para que así sea, cambiando billetes con la banca
            n_5000 = 0;
            n_1000 = 0;
            n_500 = 0;
            n_100 = 0;
            n_50 = 0;
            int n_5000_, n_1000_, n_500_, n_100_, n_50_;

            while (cantidad > 0)
            {
                if (cantidad >= 5000)
                {
                    if (jugador.n_billetes_5000 > 0)
                    {
                        jugador.n_billetes_5000--;
                        n_5000++;
                    }
                    else
                    {
                        jugador.Quitar_5000_sin_tener_b5000(out n_5000_, out n_1000_, out n_500_, out n_100_, out n_50_);
                        n_5000 += n_5000_;
                        n_1000 += n_1000_;
                        n_500 += n_500_;
                        n_100 += n_100_;
                        n_50 += n_50_;
                    }
                    cantidad -= 5000;
                }
                else if (cantidad >= 1000)
                {
                    if (jugador.n_billetes_1000 > 0)
                    {
                        jugador.n_billetes_1000--;
                        n_1000++;
                    }
                    else
                    {
                        jugador.Quitar_1000_sin_tener_b1000(out n_1000_, out n_500_, out n_100_, out n_50_);
                        n_1000 += n_1000_;
                        n_500 += n_500_;
                        n_100 += n_100_;
                        n_50 += n_50_;
                    }
                    cantidad -= 1000;
                }
                else if (cantidad >= 500)
                {
                    if (jugador.n_billetes_500 > 0)
                    {
                        jugador.n_billetes_500--;
                        n_500++;
                    }
                    else
                    {
                        jugador.Quitar_500_sin_tener_b500(out n_500_, out n_100_, out n_50_);
                        n_500 += n_500_;
                        n_100 += n_100_;
                        n_50 += n_50_;
                    }
                    cantidad -= 500;
                }
                else if (cantidad >= 100)
                {
                    if (jugador.n_billetes_100 > 0)
                    {
                        jugador.n_billetes_100--;
                        n_100++;
                    }
                    else
                    {
                        jugador.Quitar_100_sin_tener_b100(out n_100_, out n_50_);
                        n_100 += n_100_;
                        n_50 += n_50_;
                    }
                    cantidad -= 100;
                }
                else if (cantidad >= 50)
                {
                    if (jugador.n_billetes_50 > 0)
                    {
                        jugador.n_billetes_50--;
                        n_50++;
                    }
                    else
                    {
                        jugador.Quitar_50_sin_tener_b50(out n_50_);
                        n_50 += n_50_;
                    }
                    cantidad -= 50;
                }
            }
            jugador.calcular_dinero_total();
        }

        public void Actualizar_Dinero_Jugador_Actual()
        {
            switch (this.juego.jug_actual)
            {
                case 1: this.dineroJ1.Text = "Dinero: " + this.juego.jugador_actual.dinero_total;
                        break;
                case 2: this.dineroJ2.Text = "Dinero: " + this.juego.jugador_actual.dinero_total;
                        break;
                case 3: this.dineroJ3.Text = "Dinero: " + this.juego.jugador_actual.dinero_total;
                        break;
                case 4: this.dineroJ4.Text = "Dinero: " + this.juego.jugador_actual.dinero_total;
                        break;
            }
        }

        public void Actualizar_Dinero_Jugadores()
        {
            switch (this.juego.n_jugadores)
            {
                case 4: this.dineroJ4.Text = "Dinero: " + this.juego.jugadores[3].dinero_total;
                        goto case 3;
                case 3: this.dineroJ3.Text = "Dinero: " + this.juego.jugadores[2].dinero_total;
                        goto case 2;
                case 2: this.dineroJ2.Text = "Dinero: " + this.juego.jugadores[1].dinero_total;
                        this.dineroJ1.Text = "Dinero: " + this.juego.jugadores[0].dinero_total;
                        break;
            }
        }

        private void bConstruir_Click(object sender, EventArgs e)
        {
            Construir frm_construir = new Construir(ref this.juego, false);
            frm_construir.ShowDialog();
            this.Actualizar_Dinero_Jugadores();
            this.bConstruir.Enabled = false;
        }

        private void bComprarSuelo_Click(object sender, EventArgs e)
        {
            Construir frm_construir = new Construir(ref this.juego, true);
            frm_construir.ShowDialog();
            this.Actualizar_Dinero_Jugadores();
            this.bComprarSuelo.Enabled = false;
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Principal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 't') // Pasar turno
            {
                this.bTurno_Click(sender, e);
            }
            else if (e.KeyChar == 'd') // Tirar dado
            {
                this.bDado_Click(sender, e);
            }
            e.Handled = true;
        }

        private void bCobrarBanca_Click(object sender, EventArgs e)
        {
            if ((this.juego.n_jugadores == 2) || ((this.juego.n_jugadores_activos > 2) && (this.juego.n_jugadores > 2)))
            {
                int pos = this.juego.jugador_actual.posicion.numero;
                if ((pos >= 8) && ((pos - this.juego.ultimo_res_dado) < 8))
                {
                    this.juego.jugador_actual.Cobrar_Banco();
                    this.Actualizar_Dinero_Jugador_Actual();
                }
                else
                    MessageBox.Show("No puedes cobrar si no acabas de pasar por la línea del banco");
            }
            else
                MessageBox.Show("No se puede cobrar de la banca cuando solo quedan dos jugadores");
        }

        private void bVerHotelesJ1_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[0].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, false);
                frm_ver_hoteles.Show();
            }
            else
                MessageBox.Show ("No posees ningún hotel", "No es posible mostrar tus hoteles");
        }

        private void bVerHotelesJ2_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[1].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 1, false);
                frm_ver_hoteles.Show();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible mostrar tus hoteles");
        }

        private void bVerHotelesJ3_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[2].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 2, false);
                frm_ver_hoteles.Show();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible mostrar tus hoteles");
        }

        private void bVerHotelesJ4_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[3].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 3, false);
                frm_ver_hoteles.Show();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible mostrar tus hoteles");
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, true);
            frm_ver_hoteles.Show();
        }

        private void bEntradasJ1_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[0].hoteles.Count != 0)
            {
                PonerEntradas frm_poner_entradas = new PonerEntradas(ref this.juego, 0, this);
                frm_poner_entradas.ShowDialog();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible poner entradas");
        }

        private void bEntradasJ2_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[1].hoteles.Count != 0)
            {
                PonerEntradas frm_poner_entradas = new PonerEntradas(ref this.juego, 1, this);
                frm_poner_entradas.ShowDialog();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible poner entradas");
        }

        private void bEntradasJ3_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[2].hoteles.Count != 0)
            {
                PonerEntradas frm_poner_entradas = new PonerEntradas(ref this.juego, 2, this);
                frm_poner_entradas.ShowDialog();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible poner entradas");
        }

        private void bEntradasJ4_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[3].hoteles.Count != 0)
            {
                PonerEntradas frm_poner_entradas = new PonerEntradas(ref this.juego, 3, this);
                frm_poner_entradas.ShowDialog();
            }
            else
                MessageBox.Show("No posees ningún hotel", "No es posible poner entradas");
        }

        public void Dibujar_Entrada(ref Casilla casilla, Boolean en_la_derecha)
        {
            // Creando nuevo PictureBox para meter la imagen de la entrada
            PictureBox entrada = new PictureBox();
            ((ISupportInitialize)(entrada)).BeginInit();
            if (en_la_derecha)
            {
                entrada.Image = RotarImagen(this.img_entrada, casilla.pos_entrada_der.grados);
                entrada.Location = new Point(casilla.pos_entrada_der.X, casilla.pos_entrada_der.Y);
            }
            else
            {
                entrada.Image = RotarImagen(this.img_entrada, casilla.pos_entrada_izq.grados);
                entrada.Location = new Point(casilla.pos_entrada_izq.X, casilla.pos_entrada_izq.Y);
            }
            entrada.Size = new Size(15, 15);
            entrada.SizeMode = PictureBoxSizeMode.StretchImage;
            entrada.TabStop = false;
            entrada.Name = "entrada";
            this.Controls.Add(entrada);
            ((ISupportInitialize)(entrada)).EndInit();
            entrada.BringToFront();
        }

        private void bNormas_Click(object sender, EventArgs e)
        {
            Reglas frm_reglas = new Reglas();
            frm_reglas.Show();
        }

        private void bPedirNochesJ1_Click(object sender, EventArgs e)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            foreach (Hotel hotel in this.juego.jugadores[0].hoteles)
            {
                // Hay que obtener todas las casillas de un Hotel, buscando el hotel entre todas las casillas
                Casilla casilla;
                for (int i = 0; i < 32; i++)
                {
                    casilla = this.juego.casillas[i];
                    if ((casilla.hotel_izq == hotel.nombre) || (casilla.hotel_der == hotel.nombre))
                    {
                        if (casilla.ocupada == true)
                        {
                            // Buscar el jugador que esté en la casilla
                            foreach (Jugador jugador in this.juego.jugadores)
                            {
                                if ((jugador.color != this.juego.jugadores[0].color) && (jugador.posicion.numero == casilla.numero) && (jugador.pago_ultimo_turno == false))
                                {
                                    // El jugador encontrado debe pagar las noches correspondientes
                                    MessageBox.Show("El jugador " + jugador.color + " debe pagar las noches al jugador " +
                                        this.juego.jugadores[0].color + ". Pulsa OK para lanzar el dado.", "Pagar noches", MessageBoxButtons.OK);
                                    int num_noches = this.juego.dado.tirar();
                                    int dinero_necesario = hotel.Calcular_noches(num_noches);
                                    MessageBox.Show("Has sacado un " + num_noches + ", por lo que el jugador " + jugador.color + " debe abonar " + dinero_necesario + " al jugador " + this.juego.jugadores[0].color);
                                    PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, jugador);
                                    frm_pago.ShowDialog();
                                    jugador.pago_ultimo_turno = true;
                                    jugador.Pagar_Noches(ref hotel.dueño, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                                    if (frm_pago.total_seleccionado > dinero_necesario)
                                    {
                                        int n_5000, n_1000, n_500, n_100, n_50;
                                        Principal.Calcular_Devolucion(ref hotel.dueño, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                                        jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                                    }
                                    frm_pago.Close();
                                }
                            }
                        }
                    }
                }
            }
            this.Actualizar_Dinero_Jugadores();
        }

        private void bPedirNochesJ2_Click(object sender, EventArgs e)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            foreach (Hotel hotel in this.juego.jugadores[1].hoteles)
            {
                // Hay que obtener todas las casillas de un Hotel, buscando el hotel entre todas las casillas
                Casilla casilla;
                for (int i = 0; i < 32; i++)
                {
                    casilla = this.juego.casillas[i];
                    if ((casilla.hotel_izq == hotel.nombre) || (casilla.hotel_der == hotel.nombre))
                    {
                        if (casilla.ocupada == true)
                        {
                            // Buscar el jugador que esté en la casilla
                            foreach (Jugador jugador in this.juego.jugadores)
                            {
                                if ((jugador.color != this.juego.jugadores[1].color) && (jugador.posicion.numero == casilla.numero) && (jugador.pago_ultimo_turno == false))
                                {
                                    // El jugador encontrado debe pagar las noches correspondientes
                                    MessageBox.Show("El jugador " + jugador.color + " debe pagar las noches al jugador " +
                                        this.juego.jugadores[1].color + ". Pulsa OK para lanzar el dado.", "Pagar noches", MessageBoxButtons.OK);
                                    int num_noches = this.juego.dado.tirar();
                                    int dinero_necesario = hotel.Calcular_noches(num_noches);
                                    MessageBox.Show("Has sacado un " + num_noches + ", por lo que el jugador " + jugador.color + " debe abonar " + dinero_necesario + " al jugador " + this.juego.jugadores[1].color);
                                    PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, jugador);
                                    frm_pago.ShowDialog();
                                    jugador.pago_ultimo_turno = true;
                                    jugador.Pagar_Noches(ref hotel.dueño, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                                    if (frm_pago.total_seleccionado > dinero_necesario)
                                    {
                                        int n_5000, n_1000, n_500, n_100, n_50;
                                        Principal.Calcular_Devolucion(ref hotel.dueño, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                                        jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                                    }
                                    frm_pago.Close();
                                }
                            }
                        }
                    }
                }
            }
            this.Actualizar_Dinero_Jugadores();
        }

        private void bPedirNochesJ3_Click(object sender, EventArgs e)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            foreach (Hotel hotel in this.juego.jugadores[2].hoteles)
            {
                // Hay que obtener todas las casillas de un Hotel, buscando el hotel entre todas las casillas
                Casilla casilla;
                for (int i = 0; i < 32; i++)
                {
                    casilla = this.juego.casillas[i];
                    if ((casilla.hotel_izq == hotel.nombre) || (casilla.hotel_der == hotel.nombre))
                    {
                        if (casilla.ocupada == true)
                        {
                            // Buscar el jugador que esté en la casilla
                            foreach (Jugador jugador in this.juego.jugadores)
                            {
                                if ((jugador.color != this.juego.jugadores[2].color) && (jugador.posicion.numero == casilla.numero) && (jugador.pago_ultimo_turno == false))
                                {
                                    // El jugador encontrado debe pagar las noches correspondientes
                                    MessageBox.Show("El jugador " + jugador.color + " debe pagar las noches al jugador " +
                                        this.juego.jugadores[2].color + ". Pulsa OK para lanzar el dado.", "Pagar noches", MessageBoxButtons.OK);
                                    int num_noches = this.juego.dado.tirar();
                                    int dinero_necesario = hotel.Calcular_noches(num_noches);
                                    MessageBox.Show("Has sacado un " + num_noches + ", por lo que el jugador " + jugador.color + " debe abonar " + dinero_necesario + " al jugador " + this.juego.jugadores[2].color);
                                    PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, jugador);
                                    frm_pago.ShowDialog();
                                    jugador.pago_ultimo_turno = true;
                                    jugador.Pagar_Noches(ref hotel.dueño, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                                    if (frm_pago.total_seleccionado > dinero_necesario)
                                    {
                                        int n_5000, n_1000, n_500, n_100, n_50;
                                        Principal.Calcular_Devolucion(ref hotel.dueño, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                                        jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                                    }
                                    frm_pago.Close();
                                }
                            }
                        }
                    }
                }
            }
            this.Actualizar_Dinero_Jugadores();
        }

        private void bPedirNochesJ4_Click(object sender, EventArgs e)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            foreach (Hotel hotel in this.juego.jugadores[3].hoteles)
            {
                // Hay que obtener todas las casillas de un Hotel, buscando el hotel entre todas las casillas
                Casilla casilla;
                for (int i = 0; i < 32; i++)
                {
                    casilla = this.juego.casillas[i];
                    if ((casilla.hotel_izq == hotel.nombre) || (casilla.hotel_der == hotel.nombre))
                    {
                        if (casilla.ocupada == true)
                        {
                            // Buscar el jugador que esté en la casilla
                            foreach (Jugador jugador in this.juego.jugadores)
                            {
                                if ((jugador.color != this.juego.jugadores[3].color) && (jugador.posicion.numero == casilla.numero) && (jugador.pago_ultimo_turno == false))
                                {
                                    // El jugador encontrado debe pagar las noches correspondientes
                                    MessageBox.Show("El jugador " + jugador.color + " debe pagar las noches al jugador " +
                                        this.juego.jugadores[3].color + ". Pulsa OK para lanzar el dado.", "Pagar noches", MessageBoxButtons.OK);
                                    int num_noches = this.juego.dado.tirar();
                                    int dinero_necesario = hotel.Calcular_noches(num_noches);
                                    MessageBox.Show("Has sacado un " + num_noches + ", por lo que el jugador " + jugador.color + " debe abonar " + dinero_necesario + " al jugador " + this.juego.jugadores[3].color);
                                    PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, jugador);
                                    frm_pago.ShowDialog();
                                    jugador.pago_ultimo_turno = true;
                                    jugador.Pagar_Noches(ref hotel.dueño, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                                    if (frm_pago.total_seleccionado > dinero_necesario)
                                    {
                                        int n_5000, n_1000, n_500, n_100, n_50;
                                        Principal.Calcular_Devolucion(ref hotel.dueño, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                                        jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                                    }
                                    frm_pago.Close();
                                }
                            }
                        }
                    }
                }
            }
            this.Actualizar_Dinero_Jugadores();
        }
    }
}