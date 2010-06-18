using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Juego_Hotel
{
    public partial class Principal : Form
    {
        public Juego juego;
        short[] tiradas_ini;
        Sel_colores frm_colores = new Sel_colores();
        Image posRojo_orig, posAzul_orig, posVerde_orig, posAmarillo_orig;

        public Principal()
        {
            InitializeComponent();
            this.juego = new Juego();
            posRojo_orig = (Image) posRojo.Image.Clone();
            posAzul_orig = (Image) posAzul.Image.Clone();
            posVerde_orig = (Image) posVerde.Image.Clone();
            posAmarillo_orig = (Image) posAmarillo.Image.Clone();
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
                this.tiradas_ini = new short [this.juego.n_jugadores];
                short i;
                for (i = 0 ; i < this.juego.n_jugadores ; i++)
                {
                    this.tiradas_ini[i] = this.juego.dado.tirar();
                }
                // Encontrando el mayor
                short max = 0;
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
                this.dineroJ1.Text = "Dinero: " + this.juego.jugadores[0].dinero_total;
                this.dineroJ2.Text = "Dinero: " + this.juego.jugadores[1].dinero_total;
                if (this.juego.n_jugadores > 2)
                    this.dineroJ3.Text = "Dinero: " + this.juego.jugadores[2].dinero_total;
                if (this.juego.n_jugadores > 3)
                    this.dineroJ4.Text = "Dinero: " + this.juego.jugadores[3].dinero_total;
                this.bDado.Enabled = true;
            }
        }

        public Bitmap RotarImagen(Image imagen, float angulo)
        {
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

        public void Pasar_turno()
        {
            if (this.juego.jug_actual < this.juego.n_jugadores)
                this.juego.jug_actual++;
            else
                this.juego.jug_actual = 1;
            this.Establecer_Te_Toca();
            this.juego.Cambiar_jugador_actual();
            this.bDado.Enabled = true;
            this.bTurno.Enabled = false;
            this.bComprar.Enabled = false;
            this.bConstruir.Enabled = false;
            this.bComprarSuelo.Enabled = false;
        }

        public void Crear_Jugadores()
        {
            this.juego.jugadores = new Jugador[this.juego.n_jugadores];
            // Primero a mirar qué color eligió cada jugador
            switch (this.juego.n_jugadores)
            {
                case 4: this.juego.jugadores[3] = new Jugador(3, 3, 3, 3, 3, this.frm_colores.color_j4);
                        this.controlJ4.Enabled = true;
                        goto case 3;
                case 3: this.juego.jugadores[2] = new Jugador(3, 3, 3, 3, 3, this.frm_colores.color_j3);
                        this.controlJ3.Enabled = true;
                        goto case 2;
                case 2: this.juego.jugadores[1] = new Jugador(3, 3, 3, 3, 3, this.frm_colores.color_j2);
                        this.juego.jugadores[0] = new Jugador(3, 3, 3, 3, 3, this.frm_colores.color_j1);
                        this.controlJ2.Enabled = true;
                        this.controlJ1.Enabled = true;
                        break;
            }
        }

        private void bDado_Click(object sender, EventArgs e)
        {
            this.juego.ultimo_res_dado = this.juego.dado.tirar();
            this.resDado.Text = "Dado: " + this.juego.ultimo_res_dado.ToString();
            this.bDado.Enabled = false;
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
                case Tipos.Tcolor.rojo: this.posRojo.Image = RotarImagen(posRojo_orig, jugador.posicion.grados);
                                        this.posRojo.Location =
                                            new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].posX,
                                                      this.juego.casillas[this.juego.jugador_actual.posicion.numero].posY);
                                        break;
                case Tipos.Tcolor.azul: this.posAzul.Image = RotarImagen(posAzul_orig, jugador.posicion.grados);
                                        this.posAzul.Location =
                                                                new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].posX,
                                                                          this.juego.casillas[this.juego.jugador_actual.posicion.numero].posY);
                                        break;
                case Tipos.Tcolor.verde: this.posVerde.Image = RotarImagen(posVerde_orig, jugador.posicion.grados);
                                         this.posVerde.Location =
                                                                new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].posX,
                                                                          this.juego.casillas[this.juego.jugador_actual.posicion.numero].posY);
                                        break;
                case Tipos.Tcolor.amarillo: this.posAmarillo.Image = RotarImagen(posAmarillo_orig, jugador.posicion.grados);
                                            this.posAmarillo.Location =
                                                                new Point(this.juego.casillas[this.juego.jugador_actual.posicion.numero].posX,
                                                                          this.juego.casillas[this.juego.jugador_actual.posicion.numero].posY);
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
                this.posJ1.Text = "Casilla:";
                this.posJ2.Text = "Casilla:";
                this.posJ3.Text = "Casilla:";
                this.posJ4.Text = "Casilla:";
                this.dineroJ1.Text = "Dinero:";
                this.dineroJ2.Text = "Dinero:";
                this.dineroJ3.Text = "Dinero:";
                this.dineroJ4.Text = "Dinero:";
                this.juego.jugadores = null;
                this.posRojo.Image = posRojo_orig;
                this.posAzul.Image = posAzul_orig;
                this.posAmarillo.Image = posAmarillo_orig;
                this.posVerde.Image = posVerde_orig;
                this.posRojo.Location = new Point(40, 322);
                this.posAzul.Location = new Point(60, 322);
                this.posVerde.Location = new Point(80, 322);
                this.posAmarillo.Location = new Point(100, 322);
            }
        }

        private void bDadoCons_Click(object sender, EventArgs e)
        {
            Dado_construccion.Caras res;
            res = this.juego.dado_cons.tirar();
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
            // Ya ha sido seleccionado cual comprar, haciendo efectiva la compra
            if (!frm_comprar_hotel.cancelado)
            {
                Hotel hotel;
                if (frm_comprar_hotel.comprado_izq)
                    hotel = this.juego.hoteles[(short) nombre_izq];
                else
                    hotel = this.juego.hoteles[(short) nombre_der];

                if (hotel.dueño != null) // El hotel tiene dueño
                {
                    if (hotel.dueño != jugador)
                    {
                        if (hotel.n_ampliaciones_construidas == 0) // Se puede expropiar
                        {
                            String texto = "¿Quieres expropiar el hotel " + hotel.nombre + " al jugador "
                                + hotel.dueño.color.ToString() + "?";
                            DialogResult dr = MessageBox.Show(texto, "Expropiación posible", MessageBoxButtons.YesNo);
                            switch (dr)
                            {
                                case DialogResult.Yes: this.Comprar_hotel(ref hotel, ref jugador, true);
                                    break;
                                case DialogResult.No:
                                    break;
                            }
                        }
                        else
                            MessageBox.Show("El hotel ya tiene dueño y no es expropiable", "Expropiación imposible");
                    }
                    else
                        MessageBox.Show("Ya posees este hotel", "No es posible realizar la compra");
                }
                else // Es posible comprar el hotel
                    this.Comprar_hotel(ref hotel, ref jugador, false);
            }
            frm_comprar_hotel.Close();
            this.Actualizar_Dinero_Jugador();
        }

        void Comprar_hotel (ref Hotel hotel, ref Jugador jugador, Boolean expropiando)
        {
            int dinero_necesario;
            short n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;

            if (expropiando)
                    dinero_necesario = hotel.precio + hotel.precio_expropiacion;
            else
                dinero_necesario = hotel.precio;

            PedirPago frm_pago = new PedirPago (dinero_necesario, ref this.juego);
            frm_pago.ShowDialog();
            // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
            if (frm_pago.total_seleccionado > dinero_necesario)
            {
                this.Calcular_Devolucion(ref frm_pago, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
            }
            // Falta quitarle el hotel al dueño original
            if (expropiando)
                hotel.dueño.Hotel_Expropiado(ref hotel);
            jugador.Comprar_Hotel(ref hotel, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
            jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
            frm_pago.Close();
            this.bComprar.Enabled = false;
            this.Actualizar_Dinero_Jugador();
        }

        void Calcular_Devolucion (ref PedirPago frm_pago, int cantidad, out short n_5000, out short n_1000, out short n_500, out short n_100, out short n_50)
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

        void Actualizar_Dinero_Jugador()
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

        private void bConstruir_Click(object sender, EventArgs e)
        {
            Construir frm_construir = new Construir(ref this.juego, false);
            frm_construir.ShowDialog();
            if (!frm_construir.cancelado)
            {
                // Toca cobrar la construcción
                int dinero_necesario = frm_construir.total_a_pagar;
                PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego);
                frm_pago.ShowDialog();
                // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
                short n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
                if (frm_pago.total_seleccionado > dinero_necesario)
                {
                    this.Calcular_Devolucion(ref frm_pago, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                }
                // Falta quitarle el hotel al dueño original
                this.juego.jugador_actual.Pagar_Ampliacion(frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                this.juego.jugador_actual.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                frm_pago.Close();
                this.Actualizar_Dinero_Jugador();
                this.bConstruir.Enabled = false;
            }
        }

        private void bComprarSuelo_Click(object sender, EventArgs e)
        {
            Construir frm_construir = new Construir(ref this.juego, true);
            frm_construir.ShowDialog();
            this.Actualizar_Dinero_Jugador();
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
            int pos = this.juego.jugador_actual.posicion.numero;
            if ((pos >= 8) && ((pos - this.juego.ultimo_res_dado) < 8))
            {
                this.juego.jugador_actual.Cobrar_Banco();
                this.Actualizar_Dinero_Jugador();
            }
            else
                MessageBox.Show("No puedes cobrar si no acabas de pasar por la línea del banco", "Hotel");
        }
    }
}
