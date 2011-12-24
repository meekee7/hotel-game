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
        Image posRojo_orig, posAzul_orig, posVerde_orig, posAmarillo_orig, img_entrada, img_tick;
        public Boolean online;
        Online frm_online;
        public int game_id;
        public String online_config;
        public Boolean partida_cargada, dado_tirado;
        // TODO: Revisar todos los destructores para las pérdidas de memoria

        public Principal(Boolean autostart, Online frm_online)
        {
            InitializeComponent();
            this.juego = new Juego();
            this.partida_cargada = false;
            this.dado_tirado = false;
            this.posRojo_orig = (Image)posRojo.Image.Clone();
            this.posAzul_orig = (Image)posAzul.Image.Clone();
            this.posVerde_orig = (Image)posVerde.Image.Clone();
            this.posAmarillo_orig = (Image)posAmarillo.Image.Clone();
            this.img_entrada = (Image) global::Juego_Hotel.Properties.Resources.Entrada.Clone();
            this.img_tick = (Image)global::Juego_Hotel.Properties.Resources.green_tick.Clone();
            if (frm_online != null)
            {
                this.online = true;
                this.frm_online = frm_online;
            }
            else
            {
                this.online = false;
                this.frm_online = null;
            }
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
                try
                {
                    this.Crear_Jugadores();
                }
                catch
                {
                    return; //Se trata en la función Crear_Jugadores
                }
                if ((!this.online) || (!this.partida_cargada)) // Nos viene dado del servidor o por la partida cargada
                {
                    // Decidir quien empieza
                    this.tiradas_ini = new int[this.juego.n_jugadores];
                    int i;
                    for (i = 0; i < this.juego.n_jugadores; i++)
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
                }
                this.jug_ini.Text = "Jugador inicial:" + Environment.NewLine + Environment.NewLine + this.juego.jug_inicial.ToString();
                this.colorJugIni.Text = this.juego.jugadores[this.juego.jug_inicial - 1].color.ToString();
                this.juego.jug_actual = this.juego.jug_inicial;
                this.juego.Cambiar_jugador_actual();
                this.Establecer_Turno();
                this.bIniciar.Enabled = false;
                this.bColores.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.grupoNJugadores.Enabled = false;
                this.bCobrarBanca.Enabled = false;
                this.posJ1.Text = "Casilla: 0";
                this.posJ2.Text = "Casilla: 0";
                this.posJ3.Text = "Casilla: 0";
                this.posJ4.Text = "Casilla: 0";
                this.bEntradasJ1.Enabled = false;
                this.bEntradasJ2.Enabled = false;
                this.bEntradasJ3.Enabled = false;
                this.bEntradasJ4.Enabled = false;
                this.bPedirNochesJ1.Enabled = false;
                this.bPedirNochesJ2.Enabled = false;
                this.bPedirNochesJ3.Enabled = false;
                this.bPedirNochesJ4.Enabled = false;
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
                this.bSalvar.Enabled = true;
                if (this.partida_cargada) // Reajustar posiciones de los jugadores y rellenar datos
                {
                    this.resDado.Text = "Dado: " + this.juego.ultimo_res_dado;
                    foreach (Jugador jugador in this.juego.jugadores)
                    {
                        if (jugador.posicion.numero != 0)
                        {
                            switch (jugador.color)
                            {
                                case Tipos.Tcolor.rojo: this.posRojo.Image = RotarImagen(posRojo_orig, jugador.posicion.pos_coche.grados);
                                    this.posRojo.Location = new Point(this.juego.casillas[jugador.posicion.numero].pos_coche.X,
                                                                      this.juego.casillas[jugador.posicion.numero].pos_coche.Y);
                                    break;
                                case Tipos.Tcolor.azul: this.posAzul.Image = RotarImagen(posAzul_orig, jugador.posicion.pos_coche.grados);
                                    this.posAzul.Location = new Point(this.juego.casillas[jugador.posicion.numero].pos_coche.X,
                                                                      this.juego.casillas[jugador.posicion.numero].pos_coche.Y);
                                    break;
                                case Tipos.Tcolor.verde: this.posVerde.Image = RotarImagen(posVerde_orig, jugador.posicion.pos_coche.grados);
                                    this.posVerde.Location = new Point(this.juego.casillas[jugador.posicion.numero].pos_coche.X,
                                                                       this.juego.casillas[jugador.posicion.numero].pos_coche.Y);
                                    break;
                                case Tipos.Tcolor.amarillo: this.posAmarillo.Image = RotarImagen(posAmarillo_orig, jugador.posicion.pos_coche.grados);
                                    this.posAmarillo.Location = new Point(this.juego.casillas[jugador.posicion.numero].pos_coche.X,
                                                                          this.juego.casillas[jugador.posicion.numero].pos_coche.Y);
                                    break;
                            }
                        }
                    }
                    // Poner casilla actual a cada uno
                    switch (this.juego.n_jugadores)
                    {
                        case 4: this.posJ4.Text = "Casilla: " + this.juego.jugadores[3].posicion.numero.ToString();
                                goto case 3;
                        case 3: this.posJ3.Text = "Casilla: " + this.juego.jugadores[2].posicion.numero.ToString();
                                goto case 2;
                        case 2: this.posJ2.Text = "Casilla: " + this.juego.jugadores[1].posicion.numero.ToString();
                                this.posJ1.Text = "Casilla: " + this.juego.jugadores[0].posicion.numero.ToString();
                                break;
                    }
                    this.Actualizar_Dinero_Jugadores();
                    // Dibujar todas las fases ya hechas
                    int pos_fase_hotel;
                    foreach (Hotel hotel in this.juego.hoteles)
                    {
                        for (pos_fase_hotel = 0; pos_fase_hotel < hotel.n_fases_construidas; pos_fase_hotel++)
                        {
                            this.Dibujar_Fase(hotel, pos_fase_hotel);
                        }
                    }
                }
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

        public void Establecer_Turno()
        {
            this.turnoJ1.Text = "";
            this.turnoJ2.Text = "";
            this.turnoJ3.Text = "";
            this.turnoJ4.Text = "";
            this.bEntradasJ1.Enabled = false;
            this.bEntradasJ2.Enabled = false;
            this.bEntradasJ3.Enabled = false;
            this.bEntradasJ4.Enabled = false;
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
            if (ganador != null)
                MessageBox.Show("Partida finalizada. Ha ganado el jugador " + ganador.color.ToString(), "Hotel");
            else
                MessageBox.Show("Partida finalizada porque todos los jugadores se han retirado");
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
            else if (this.juego.n_jugadores_activos == 0)
                Finalizar_Partida(null);
            else
            {
                this.juego.jug_actual = Sig_jugador_Activo();
                this.Establecer_Turno();
                this.juego.Cambiar_jugador_actual();
                this.bDado.Enabled = true;
                this.bTurno.Enabled = false;
                this.bComprar.Enabled = false;
                this.bConstruir.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.bCobrarBanca.Enabled = false;
                this.bPedirNochesJ1.Enabled = false;
                this.bPedirNochesJ2.Enabled = false;
                this.bPedirNochesJ3.Enabled = false;
                this.bPedirNochesJ4.Enabled = false;
                this.juego.jugador_actual.pago_ultimo_turno = false;
                this.dado_tirado = false;
                foreach (Hotel hotel in this.juego.hoteles)
                    hotel.entrada_comprada_ultimo_turno = false;
            }
        }

        public void Crear_Jugadores()
        {
            if (!this.partida_cargada)
            {
                this.juego.jugadores = new Jugador[this.juego.n_jugadores];
                this.juego.n_jugadores_activos = this.juego.n_jugadores;
                XmlDocument configuracion = new XmlDocument();
                try
                {
                    if (this.online)
                        configuracion.LoadXml(this.online_config);
                    else
                        configuracion.Load("Config.xml");
                }
                catch
                {
                    MessageBox.Show("No se puede cargar el fichero de configuración Config.xml", "Error");
                    this.juego.jugador_actual = null;
                    throw;
                }
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
                    case 4: this.juego.jugadores[3] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j4, 3);
                            this.controlJ4.Enabled = true;
                            goto case 3;
                    case 3: this.juego.jugadores[2] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j3, 2);
                            this.controlJ3.Enabled = true;
                            goto case 2;
                    case 2: this.juego.jugadores[1] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j2, 1);
                            this.juego.jugadores[0] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j1, 0);
                            this.controlJ2.Enabled = true;
                            this.controlJ1.Enabled = true;
                            break;
                }
            }
            else
            {
                switch (this.juego.n_jugadores)
                {
                    case 4: this.controlJ4.Enabled = true;
                            goto case 3;
                    case 3: this.controlJ3.Enabled = true;
                            goto case 2;
                    case 2: this.controlJ2.Enabled = true;
                            this.controlJ1.Enabled = true;
                            break;
                }
            }
        }

        public void Tirar_dado()
        {
            if (!this.online)
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
            this.juego.ultimo_avance_auto = 0;
            while (jugador.posicion.ocupada) // Hay que avanzar una porque está ocupada
            {
                if (jugador.posicion.numero < 31) // Proteger la vuelta al tablero
                    jugador.posicion = this.juego.casillas[jugador.posicion.numero + 1];
                else
                    jugador.posicion = this.juego.casillas[1];
                this.juego.ultimo_avance_auto++;
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
            this.bEntradasJ1.Enabled = false;
            this.bEntradasJ2.Enabled = false;
            this.bEntradasJ3.Enabled = false;
            this.bEntradasJ4.Enabled = false;
            if (this.Puede_poner_entradas(this.juego.jugador_actual))
                this.Activar_Poner_Entradas(this.juego.jug_actual);
            this.bPedirNochesJ1.Enabled = true;
            this.bPedirNochesJ2.Enabled = true;
            this.bPedirNochesJ3.Enabled = true;
            this.bPedirNochesJ4.Enabled = true;
            if (this.Puede_Cobrar_Banca(this.juego.jug_actual))
                this.bCobrarBanca.Enabled = true;
            switch (jugador.posicion.tipo)
            {
                case Tipos.Tcasilla.comprar: this.bComprar.Enabled = true;
                    this.bConstruir.Enabled = false;
                    break;
                case Tipos.Tcasilla.construir: this.bConstruir.Enabled = true;
                    this.bComprar.Enabled = false;
                    break;
                case Tipos.Tcasilla.fase_gratis: this.bConstruir.Enabled = true;
                    this.bComprar.Enabled = false;
                    MessageBox.Show("Has caído en una casilla de tipo Fase Gratis, ¡aprovecha!");
                    break;
                case Tipos.Tcasilla.entrada_gratis: this.bConstruir.Enabled = false;
                    this.bComprar.Enabled = false;
                    this.Activar_Poner_Entradas(this.juego.jug_actual);
                    MessageBox.Show("Has caído en una casilla de tipo Entrada Gratis, ¡aprovecha!");
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
            this.bTurno.Enabled = true;
            this.dado_tirado = true;
        }

        private void bDado_Click(object sender, EventArgs e)
        {
            if (this.online)
                this.frm_online.enviar_comando("roll_dice", this.game_id.ToString(), this.frm_online.txtLogin.Text);
            else
                this.Tirar_dado();
        }

        private void Activar_Poner_Entradas(int num_jugador)
        {
            switch (num_jugador)
            {
                case 1: this.bEntradasJ1.Enabled = true;
                        break;
                case 2: this.bEntradasJ2.Enabled = true;
                        break;
                case 3: this.bEntradasJ3.Enabled = true;
                        break;
                case 4: this.bEntradasJ4.Enabled = true;
                        break;
            }
        }

        private void bReiniciar_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("¿Estás seguro de que quieres reiniciar la partida?", "Hotel", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                Reiniciar_partida();
        }

        public void Reiniciar_partida()
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
            this.jug_ini.Text = "Jugador inicial:";
            this.colorJugIni.Text = "";
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
            Control[] lista_fases = this.Controls.Find("fase", true);
            foreach (Control fase in lista_fases)
            {
                this.Controls.Remove(fase);
                fase.Dispose();
            }
            // Redibujar tablero para quitar los suelos
            this.imgTablero.Image = global::Juego_Hotel.Properties.Resources.Tablero;
            int num_jugadores = this.juego.n_jugadores;
            this.juego = new Juego();
            this.juego.n_jugadores = num_jugadores;
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
                        if (hotel.n_fases_construidas == 0) // Se puede expropiar
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
            //this.Actualizar_Dinero_Jugador_Actual();
        }

        void Comprar_hotel (ref Hotel hotel, ref Jugador jugador, Boolean expropiando)
        {
            int dinero_necesario;
            int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;

            if (expropiando)
                dinero_necesario = hotel.precio_expropiacion;
            else
                dinero_necesario = hotel.precio;

            PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, this.juego.jugador_actual, this, null);
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
            this.bComprarSuelo.Enabled = false;
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
            if (this.juego.jugador_actual.hoteles.Count == 0)
            {
                MessageBox.Show("No posees ningún hotel");
                return;
            }
            Construir frm_construir = new Construir(ref this.juego, false, this);
            if (frm_construir.ShowDialog() == DialogResult.OK)
            {
                this.Actualizar_Dinero_Jugadores();
                this.bConstruir.Enabled = false;
                this.bComprarSuelo.Enabled = false;
            }
        }

        private void bComprarSuelo_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.hoteles.Count == 0)
            {
                MessageBox.Show("No posees ningún hotel");
                return;
            }
            Construir frm_construir = new Construir(ref this.juego, true, this);
            if (frm_construir.ShowDialog() == DialogResult.OK)
            {
                this.Actualizar_Dinero_Jugadores();
                this.bConstruir.Enabled = false;
                this.bComprarSuelo.Enabled = false;
            }
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /* Ejemplo de captura de "Ctrl+Alt+O"
            if ((int)keyData == (int)Keys.O + (int)Keys.Control + (int)Keys.Alt)
            {
                // do whatever, return true to show keystroke processed, false to allow further processing
                MessageBox.Show("Hey");
                return true;
            }*/

            if (keyData == Keys.T)
                this.bTurno.PerformClick();
            else if (keyData == Keys.E)
                this.bDado.PerformClick();
            else if (keyData == Keys.C)
                this.bCargar.PerformClick();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private Boolean Puede_Cobrar_Banca(int num_jugador)
        {
            if ((this.juego.n_jugadores == 2) || ((this.juego.n_jugadores_activos > 2) && (this.juego.n_jugadores > 2)))
            {
                int pos = this.juego.jugadores[num_jugador - 1].posicion.numero;
                if ((pos >= 8) && ((pos - this.juego.ultimo_res_dado - this.juego.ultimo_avance_auto) < 8))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private void bCobrarBanca_Click(object sender, EventArgs e)
        {
            this.juego.jugador_actual.Cobrar_Banco();
            this.Actualizar_Dinero_Jugador_Actual();
            this.bCobrarBanca.Enabled = false;
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

        private Boolean Puede_poner_entradas(Jugador jugador)
        {
            int pos = jugador.posicion.numero;
            if ((pos >= 27) && ((pos - this.juego.ultimo_res_dado - this.juego.ultimo_avance_auto) < 27))
                return true;
            else
                return false;
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

        private void Pedir_Noches(int n_jugador)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            foreach (Hotel hotel in this.juego.jugadores[n_jugador].hoteles)
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
                                if ((jugador.color != this.juego.jugadores[n_jugador].color) && (jugador.posicion.numero == casilla.numero) && (jugador.pago_ultimo_turno == false))
                                {
                                    // El jugador encontrado debe pagar las noches correspondientes
                                    MessageBox.Show("El jugador " + jugador.color + " debe pagar las noches al jugador " +
                                        hotel.dueño.color + ". Pulsa OK para lanzar el dado.", "Pagar noches", MessageBoxButtons.OK);
                                    int num_noches = this.juego.dado.tirar();
                                    int dinero_necesario = hotel.Calcular_noches(num_noches);
                                    MessageBox.Show("Has sacado un " + num_noches + ", por lo que el jugador " + jugador.color + " debe abonar " + dinero_necesario + " al jugador " + hotel.dueño.color);
                                    PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, jugador, this, hotel.dueño);
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

        private void bPedirNochesJ1_Click(object sender, EventArgs e)
        {
            this.Pedir_Noches(0);
        }

        private void bPedirNochesJ2_Click(object sender, EventArgs e)
        {
            this.Pedir_Noches(1);
        }

        private void bPedirNochesJ3_Click(object sender, EventArgs e)
        {
            this.Pedir_Noches(2);
        }

        private void bPedirNochesJ4_Click(object sender, EventArgs e)
        {
            this.Pedir_Noches(3);
        }

        public void Marcar_Jugador_Eliminado(int n_jugador)
        {
            switch (n_jugador)
            {
                case 0: this.controlJ1.Enabled = false;
                    break;
                case 1: this.controlJ2.Enabled = false;
                    break;
                case 2: this.controlJ3.Enabled = false;
                    break;
                case 3: this.controlJ4.Enabled = false;
                    break;
            }
            if (n_jugador + 1 == this.juego.jug_actual)
            {
                this.bTurno.Enabled = true;
                this.bTurno.PerformClick();
                this.bTurno.Enabled = false;
            }
        }

        private void bRetirarseJ1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de que quieres retirarte?", "Hotel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.juego.Eliminar_Jugador(this.juego.jugadores[0], null);
                this.Marcar_Jugador_Eliminado(0);
            }
        }

        private void bRetirarseJ2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de que quieres retirarte?", "Hotel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.juego.Eliminar_Jugador(this.juego.jugadores[1], null);
                this.Marcar_Jugador_Eliminado(1);
            }
        }

        private void bRetirarseJ3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de que quieres retirarte?", "Hotel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.juego.Eliminar_Jugador(this.juego.jugadores[2], null);
                this.Marcar_Jugador_Eliminado(2);
            }
        }

        private void bRetirarseJ4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de que quieres retirarte?", "Hotel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.juego.Eliminar_Jugador(this.juego.jugadores[3], null);
                this.Marcar_Jugador_Eliminado(3);
            }
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: Salir de la partida si estás en modo online
            /*if (this.frm_online != null)
                this.frm_online.Close();*/
        }

        private void Principal_Shown(object sender, EventArgs e)
        {
            if (this.online)
                this.bIniciar.PerformClick();
        }

        private void bSalvar_Click(object sender, EventArgs e)
        {
            if (this.dado_tirado)
            {
                MessageBox.Show("Sólo se puede salvar antes de tirar el dado");
                return;
            }
            SaveFileDialog dialogo = new SaveFileDialog();
            dialogo.AddExtension = true;
            dialogo.CheckPathExists = true;
            dialogo.DefaultExt = "xml";
            dialogo.SupportMultiDottedExtensions = true;
            dialogo.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            dialogo.Filter = "Partida de Hotel|*.xml";
            dialogo.Title = "Salvar partida en curso";
            dialogo.FileName = "Partida Hotel " + System.DateTime.Today.ToShortDateString().Replace('/', '-') + ".xml";
            if (dialogo.ShowDialog() != DialogResult.Cancel)
            {
                if (dialogo.FileName != "")
                {
                    Salvar_y_cargar mgr_salvar = new Salvar_y_cargar(ref this.juego);
                    if (mgr_salvar.Salvar_partida(dialogo.FileName) == false)
                        MessageBox.Show("Error salvando la partida: " + mgr_salvar.error);
                }
            }
            dialogo = null;
        }

        private void bCargar_Click(object sender, EventArgs e)
        {
            if (!this.bIniciar.Enabled)
                if (MessageBox.Show("La partida actual se perderá si continúas, tanto si la carga es exitosa como no, ¿Quieres proceder?", "Cargar partida", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.AddExtension = true;
            dialogo.CheckPathExists = true;
            dialogo.DefaultExt = "xml";
            dialogo.SupportMultiDottedExtensions = true;
            String dir_trabajo = System.IO.Directory.GetCurrentDirectory(); // Después de cargar el fichero, el directorio actual se pierde
            dialogo.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            dialogo.Filter = "Partida de Hotel|*.xml";
            dialogo.Title = "Cargar una partida";
            dialogo.FileName = "Partida Hotel " + System.DateTime.Today.ToShortDateString().Replace('/', '-') + ".xml";
            if (dialogo.ShowDialog() != DialogResult.Cancel)
            {
                if (dialogo.FileName != "")
                {
                    this.Reiniciar_partida();
                    Salvar_y_cargar mgr_cargar = new Salvar_y_cargar(ref this.juego);
                    if (mgr_cargar.Cargar_partida(dialogo.FileName, this) == false)
                    {
                        MessageBox.Show("Error cargando la partida:" + Environment.NewLine + mgr_cargar.error);
                        this.Reiniciar_partida();
                    }
                    else
                    {
                        this.partida_cargada = true;
                        this.bIniciar.PerformClick();
                    }
                }
            }
            dialogo = null;
            System.IO.Directory.SetCurrentDirectory(dir_trabajo); // Restaurar el directorio, para poder cargar el Config.xml
        }

        public void Dibujar_Fase(Hotel hotel, int num_fase)
        {
            if (num_fase == hotel.n_fases_max - 1)
                this.Dibujar_Suelo(hotel);
            else
            {
                // Creando nuevo PictureBox para meter la imagen de la entrada
                PictureBox fase = new PictureBox();
                ((ISupportInitialize)(fase)).BeginInit();
                Tipos.Posicion pos = hotel.posiciones_fases.ToList()[num_fase];
                fase.Image = RotarImagen(this.img_tick, pos.grados);
                fase.Location = new Point(pos.X, pos.Y);
                fase.Size = new Size(18, 18);
                fase.SizeMode = PictureBoxSizeMode.StretchImage;
                fase.TabStop = false;
                fase.Name = "fase";
                this.Controls.Add(fase);
                ((ISupportInitialize)(fase)).EndInit();
                fase.BringToFront();
            }
        }

        public void Dibujar_Suelo(Hotel hotel)
        {
            // Se repinta el tablero con la nueva imagen encima
            Image tablero = this.imgTablero.Image;
            Graphics g = Graphics.FromImage(tablero);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image suelo;
            Tipos.Posicion pos;
            switch (hotel.nombre)
            {
                case Tipos.Tnombre_hotel.Boomerang: suelo = global::Juego_Hotel.Properties.Resources.Suelo_Boomerang;
                                                    break;
                case Tipos.Tnombre_hotel.Fujiyama:  suelo = global::Juego_Hotel.Properties.Resources.Suelo_Fujiyama;
                                                    break;
                case Tipos.Tnombre_hotel.Letoile:   suelo = global::Juego_Hotel.Properties.Resources.Suelo_Letoile;
                                                    break;
                case Tipos.Tnombre_hotel.President: suelo = global::Juego_Hotel.Properties.Resources.Suelo_President;
                                                    break;
                case Tipos.Tnombre_hotel.Royal:     suelo = global::Juego_Hotel.Properties.Resources.Suelo_Royal;
                                                    break;
                case Tipos.Tnombre_hotel.Safari:    suelo = global::Juego_Hotel.Properties.Resources.Suelo_Safari;
                                                    break;
                case Tipos.Tnombre_hotel.Taj_Mahal: suelo = global::Juego_Hotel.Properties.Resources.Suelo_TajMahal;
                                                    break;
                case Tipos.Tnombre_hotel.Waikiki:   suelo = global::Juego_Hotel.Properties.Resources.Suelo_Waikiki;
                                                    break;
                default:                            suelo = null;
                                                    break;
            }
            pos = hotel.posiciones_fases.ToList()[hotel.n_fases_max - 1];
            if (suelo != null) // Por si acaso alguna cosa rara
                g.DrawImage(suelo, pos.X, pos.Y);
            else
                return;
            // Sustitur imagen actual
            this.imgTablero.Image = tablero;
        }
    }
}