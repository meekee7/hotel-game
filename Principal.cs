using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Globalization;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    
    public partial class Principal : Form, IReLocalizable
    {
        public System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principal));
        public Juego juego;
        int[] tiradas_ini;
        Sel_colores frm_colores = new Sel_colores();
        Image posRojo_orig, posAzul_orig, posVerde_orig, posAmarillo_orig, img_entrada;
        Image img_tick_Amarillo, img_tick_Azul, img_tick_Rojo, img_tick_Verde;
        public Boolean online;
        public Online frm_online;
        public int game_id;
        public String nombre_online;
        public String online_config;
        public Boolean partida_cargada_offline, partida_cargada_online, dado_tirado, partida_activa;
        int ancho_ini;
        int alto_ini;
        Point pos_rojo_orig, pos_azul_orig, pos_verde_orig, pos_amarillo_orig, pos_banco_orig, pos_ayto_orig;
        int ancho_coche = 20;
        int alto_coche = 20;
        int ancho_fase = 18;
        int alto_fase = 18;
        public Subastas frm_subasta_en_curso;
        public Hotel hotel_a_subastar_online;
        public int precio_minimo_subasta_online;
        XmlDocument configuracion;
        public Actividad actividad;
        public String creador_online;
        // TODO: Revisar todos los destructores para las pérdidas de memoria

        public Principal(Boolean autostart, Online frm_online, XmlDocument configuracion)
        {
            InitializeComponent();
            this.juego = new Juego();
            this.alto_ini = this.imgTablero.Height;
            this.ancho_ini = this.imgTablero.Width;
            this.partida_cargada_offline = false;
            this.dado_tirado = false;
            this.posRojo.Parent = this.imgTablero;
            this.pos_rojo_orig = this.posRojo.Location;
            this.posRojo.Location = Calcular_Posicion(this.posRojo.Location.X, this.posRojo.Location.Y);
            this.posRojo_orig = (Image)posRojo.Image.Clone();
            this.posAzul.Parent = this.imgTablero;
            this.pos_azul_orig = this.posAzul.Location;
            this.posAzul.Location = Calcular_Posicion(this.posAzul.Location.X, this.posAzul.Location.Y);
            this.posAzul_orig = (Image)posAzul.Image.Clone();
            this.posVerde.Parent = this.imgTablero;
            this.pos_verde_orig = this.posVerde.Location;
            this.posVerde.Location = Calcular_Posicion(this.posVerde.Location.X, this.posVerde.Location.Y);
            this.posVerde_orig = (Image)posVerde.Image.Clone();
            this.posAmarillo.Parent = this.imgTablero;
            this.pos_amarillo_orig = this.posAmarillo.Location;
            this.posAmarillo.Location = Calcular_Posicion(this.posAmarillo.Location.X, this.posAmarillo.Location.Y);
            this.posAmarillo_orig = (Image)posAmarillo.Image.Clone();
            this.img_entrada = (Image) global::Juego_Hotel.Properties.Resources.Entrada.Clone();
            this.img_tick_Amarillo = (Image)global::Juego_Hotel.Properties.Resources.TickAmarillo.Clone();
            this.img_tick_Azul = (Image)global::Juego_Hotel.Properties.Resources.TickAzul.Clone();
            this.img_tick_Rojo = (Image)global::Juego_Hotel.Properties.Resources.TickRojo.Clone();
            this.img_tick_Verde = (Image)global::Juego_Hotel.Properties.Resources.TickVerde.Clone();
            this.img_Banco.Parent = this.imgTablero;
            this.pos_banco_orig = this.img_Banco.Location;
            this.img_Banco.Location = Calcular_Posicion(this.img_Banco.Location.X, this.img_Banco.Location.Y);
            this.img_ayto.Parent = this.imgTablero;
            this.pos_ayto_orig = this.img_ayto.Location;
            this.img_ayto.Location = Calcular_Posicion(this.img_ayto.Location.X, this.img_ayto.Location.Y);
            this.configuracion = configuracion;
            if (frm_online != null)
            {
                this.online = true;
                this.frm_online = frm_online;
                this.actividad = new Actividad();
                this.partida_activa = true;
                XmlDocument fichero_configuracion = new XmlDocument();
                fichero_configuracion.Load("Config.xml");
                XmlNode nodo_Idioma = fichero_configuracion.GetElementsByTagName("language")[0];
                if (nodo_Idioma.ChildNodes[0].FirstChild != null)
                {
                    if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("Spanish"))
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
                    else if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("English"))
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                }
            }
            else
            {
                this.online = false;
                this.frm_online = null;
                this.partida_activa = false;
                this.bCargar.Enabled = true;
            }
            // Rellenar combobox de idiomas
            this.comboBoxIdiomas.Items.Add(new ComboItemImagen(Mensajes.comboBoxIdiomas1, 0));
            this.comboBoxIdiomas.Items.Add(new ComboItemImagen(Mensajes.comboBoxIdiomas2, 1));
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.Equals("es"))
                this.comboBoxIdiomas.SelectedIndex = 0;
            else
                this.comboBoxIdiomas.SelectedIndex = 1;
        }

        private void bIniciar_Click(object sender, EventArgs e)
        {
            // Buscar número de jugadores
            if (this.juego.n_jugadores == 0)
            {
                MessageBox.Show(Mensajes.mensajeSelecJugadores, Mensajes.tituloSelecJugadores);
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
                if ((!this.online) && (!this.partida_cargada_offline)) // Nos viene dado del servidor o por la partida cargada
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
                this.jug_ini.Text = resources.GetString("jug_ini.Text") + Environment.NewLine + Environment.NewLine + this.juego.jug_inicial.ToString();
                this.colorJugIni.Text = this.juego.jugadores[this.juego.jug_inicial - 1].Nombre_color();
                this.juego.jug_actual = this.juego.jug_inicial;
                this.juego.Cambiar_jugador_actual();
                this.Establecer_Turno();
                this.bIniciar.Enabled = false;
                this.bColores.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.grupoNJugadores.Enabled = false;
                this.bCobrarBanca.Enabled = false;
                this.bSalvar.Enabled = true;
                this.posRojo.Visible = true;
                this.posAzul.Visible = true;
                this.posVerde.Visible = true;
                this.posAmarillo.Visible = true;
                this.img_Banco.Visible = true;
                this.img_ayto.Visible = true;
                this.posJ1.Text = resources.GetString("posJ1.Text") + "0 (" + Mensajes.TipoCasillaSalida + ")";
                this.posJ2.Text = resources.GetString("posJ2.Text") + "0 (" + Mensajes.TipoCasillaSalida + ")";
                this.posJ3.Text = resources.GetString("posJ3.Text") + "0 (" + Mensajes.TipoCasillaSalida + ")";
                this.posJ4.Text = resources.GetString("posJ4.Text") + "0 (" + Mensajes.TipoCasillaSalida + ")";
                this.bEntradasJ1.Enabled = false;
                this.bEntradasJ2.Enabled = false;
                this.bEntradasJ3.Enabled = false;
                this.bEntradasJ4.Enabled = false;
                this.bRetirarseJ1.Enabled = false;
                this.bRetirarseJ2.Enabled = false;
                this.bRetirarseJ3.Enabled = false;
                this.bRetirarseJ4.Enabled = false;
                switch (this.juego.jugador_actual.n_jugador)
                {
                    case 0: this.bRetirarseJ1.Enabled = true;
                        break;
                    case 1: this.bRetirarseJ2.Enabled = true;
                        break;
                    case 2: this.bRetirarseJ3.Enabled = true;
                        break;
                    case 3: this.bRetirarseJ4.Enabled = true;
                        break;
                }
                this.colorJ1.Text = resources.GetString("colorJ1.Text") + this.juego.jugadores[0].Nombre_color();
                this.colorJ2.Text = resources.GetString("colorJ2.Text") + this.juego.jugadores[1].Nombre_color();
                this.dineroJ1.Text = resources.GetString("dineroJ1.Text") + this.juego.jugadores[0].dinero_total;
                this.dineroJ2.Text = resources.GetString("dineroJ2.Text") + this.juego.jugadores[1].dinero_total;
                if (this.juego.n_jugadores > 2)
                {
                    this.dineroJ3.Text = resources.GetString("dineroJ3.Text") + this.juego.jugadores[2].dinero_total;
                    this.colorJ3.Text = resources.GetString("colorJ3.Text") + this.juego.jugadores[2].Nombre_color();
                }
                if (this.juego.n_jugadores > 3)
                {
                    this.dineroJ4.Text = resources.GetString("dineroJ4.Text") + this.juego.jugadores[3].dinero_total;
                    this.colorJ4.Text = resources.GetString("colorJ4.Text") + this.juego.jugadores[3].Nombre_color();
                }
                if (!this.online)
                    this.bDado.Enabled = true;
                else
                {
                    this.bDado.Enabled = false;
                    this.controlJ1.Enabled = false;
                    this.controlJ2.Enabled = false;
                    this.controlJ3.Enabled = false;
                    this.controlJ4.Enabled = false;
                    Jugador yo = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == this.nombre_online);
                    switch (yo.n_jugador)
                    {
                        case 0: this.controlJ1.Enabled = true;
                            break;
                        case 1: this.controlJ2.Enabled = true;
                            break;
                        case 2: this.controlJ3.Enabled = true;
                            break;
                        case 3: this.controlJ4.Enabled = true;
                            break;
                    }
                    if (yo.nombre_online != this.creador_online)
                        this.bSalvar.Enabled = false;
                }
                if ((!this.online) || (this.online && (this.nombre_online == this.juego.jugadores[this.juego.jug_inicial - 1].nombre_online)))
                    this.bDado.Enabled = true;
                if (this.partida_cargada_offline || this.partida_cargada_online) // Reajustar posiciones de los jugadores y rellenar datos
                {
                    this.resDado.Text = resources.GetString("resDado.Text") + this.juego.ultimo_res_dado;
                    foreach (Jugador jugador in this.juego.jugadores)
                    {
                        if (jugador.posicion.numero != 0)
                        {
                            Point pos = Calcular_Posicion(this.juego.casillas[jugador.posicion.numero].pos_coche.X, this.juego.casillas[jugador.posicion.numero].pos_coche.Y);
                            switch (jugador.color)
                            {
                                case Tipos.Tcolor.rojo:     this.posRojo.Image = RotateImage(posRojo_orig, jugador.posicion.pos_coche.grados);
                                                            this.posRojo.Location = pos;
                                                            this.posRojo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                                                            break;
                                case Tipos.Tcolor.azul:     this.posAzul.Image = RotateImage(posAzul_orig, jugador.posicion.pos_coche.grados);
                                                            this.posAzul.Location = pos;
                                                            this.posAzul.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                                                            break;
                                case Tipos.Tcolor.verde:    this.posVerde.Image = RotateImage(posVerde_orig, jugador.posicion.pos_coche.grados);
                                                            this.posVerde.Location = pos;
                                                            this.posVerde.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                                                            break;
                                case Tipos.Tcolor.amarillo: this.posAmarillo.Image = RotateImage(posAmarillo_orig, jugador.posicion.pos_coche.grados);
                                                            this.posAmarillo.Location = pos;
                                                            this.posAmarillo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                                                            break;
                            }
                        }
                    }
                    // Poner casilla actual a cada uno
                    switch (this.juego.n_jugadores)
                    {
                        case 4: this.posJ4.Text = resources.GetString("posJ4.Text") + this.juego.jugadores[3].posicion.numero.ToString();
                                goto case 3;
                        case 3: this.posJ3.Text = resources.GetString("posJ3.Text") + this.juego.jugadores[2].posicion.numero.ToString();
                                goto case 2;
                        case 2: this.posJ2.Text = resources.GetString("posJ2.Text") + this.juego.jugadores[1].posicion.numero.ToString();
                                this.posJ1.Text = resources.GetString("posJ1.Text") + this.juego.jugadores[0].posicion.numero.ToString();
                                break;
                    }
                    this.Actualizar_Dinero_Jugadores();
                    // Cargar el estado de los hoteles si es partida online
                    if (this.partida_cargada_online)
                    {
                        foreach (String estado_hotel in this.juego.estado_hoteles_online)
                        {
                            String[] estado = estado_hotel.Split(';');
                            Hotel hotel = this.juego.hoteles.First(x => x.nombre_txt == estado[0]);
                            if (estado[1] != "0")
                            {
                                hotel.dueño = this.juego.jugadores.FirstOrDefault(x => x.n_jugador == Convert.ToInt16(estado[1]) - 1);
                                hotel.dueño.hoteles.AddLast(hotel);
                            }
                            hotel.n_fases_construidas = Convert.ToInt16(estado[2]);
                            hotel.entrada_comprada_ultimo_turno = (estado[3] == "1" ? true : false);
                            hotel.entrada_comprada_ultimo_turno = (estado[4] == "1" ? true : false);
                            if (estado[5] != "")
                            {
                                String[] lista_entradas = estado[5].Split('@');
                                hotel.n_entradas = lista_entradas.Length;
                                foreach (String s_num_casilla in lista_entradas)
                                {
                                    int num_casilla = Convert.ToInt16(s_num_casilla);
                                    if (this.juego.casillas[num_casilla].hotel_der == hotel.nombre)
                                    {
                                        this.juego.casillas[num_casilla].entrada_en_der = true;
                                        this.Dibujar_Entrada(this.juego.casillas[num_casilla], true);
                                    }
                                    else if (this.juego.casillas[num_casilla].hotel_izq == hotel.nombre)
                                    {
                                        this.juego.casillas[num_casilla].entrada_en_izq = true;
                                        this.Dibujar_Entrada(this.juego.casillas[num_casilla], false);
                                    }
                                    hotel.entradas.AddLast(this.juego.casillas[num_casilla]);
                                }
                            }
                        }
                    }
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

        /// <summary>
        /// Method to rotate an Image object. The result can be one of three cases:
        /// - upsizeOk = true: output image will be larger than the input, and no clipping occurs 
        /// - upsizeOk = false & clipOk = true: output same size as input, clipping occurs
        /// - upsizeOk = false & clipOk = false: output same size as input, image reduced, no clipping
        /// 
        /// The background color will be transparent, so the output image will be 32-bit.
        /// 
        /// Note that this method always returns a new Bitmap object, even if rotation is zero - in 
        /// which case the returned object is a clone of the input object. 
        /// </summary>
        /// <param name="inputImage">input Image object, is not modified</param>
        /// <param name="angleDegrees">angle of rotation, in degrees</param>
        /// <param name="upsizeOk">see comments above</param>
        /// <param name="clipOk">see comments above, not used if upsizeOk = true</param>
        /// <returns>new Bitmap object, may be larger than input image</returns>
        public static Bitmap RotateImage(Image inputImage, float angleDegrees, bool upsizeOk = true, bool clipOk = false)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
                return (Bitmap)inputImage.Clone();

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = inputImage.Width;
            int oldHeight = inputImage.Height;
            int newWidth = oldWidth;
            int newHeight = oldHeight;
            float scaleFactor = 1f;

            // If upsizing wanted or clipping not OK calculate the size of the resulting bitmap
            if (upsizeOk || !clipOk)
            {
                double angleRadians = angleDegrees * Math.PI / 180d;

                double cos = Math.Abs(Math.Cos(angleRadians));
                double sin = Math.Abs(Math.Sin(angleRadians));
                newWidth = (int)Math.Round(oldWidth * cos + oldHeight * sin);
                newHeight = (int)Math.Round(oldWidth * sin + oldHeight * cos);
            }

            // If upsizing not wanted and clipping not OK need a scaling factor
            if (!upsizeOk && !clipOk)
            {
                scaleFactor = Math.Min((float)oldWidth / newWidth, (float)oldHeight / newHeight);
                newWidth = oldWidth;
                newHeight = oldHeight;
            }

            // Create the new bitmap object. If background color is transparent it must be 32-bit, 
            //  otherwise 24-bit is good enough.
            Bitmap newBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            newBitmap.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

            // Create the Graphics object that does the work
            using (Graphics graphicsObject = Graphics.FromImage(newBitmap))
            {
                graphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsObject.SmoothingMode = SmoothingMode.HighQuality;

                // Fill in the specified background color if necessary
                graphicsObject.Clear(Color.Transparent);

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                graphicsObject.TranslateTransform(newWidth / 2f, newHeight / 2f);

                if (scaleFactor != 1f)
                    graphicsObject.ScaleTransform(scaleFactor, scaleFactor);

                graphicsObject.RotateTransform(angleDegrees);
                graphicsObject.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result 
                graphicsObject.DrawImage(inputImage, 0, 0);
            }

            return newBitmap;
        }

        public void Establecer_Turno()
        {
            this.ImagenTurnoJ1.Visible = false;
            this.ImagenTurnoJ2.Visible = false;
            this.ImagenTurnoJ3.Visible = false;
            this.ImagenTurnoJ4.Visible = false;
            this.bEntradasJ1.Enabled = false;
            this.bEntradasJ2.Enabled = false;
            this.bEntradasJ3.Enabled = false;
            this.bEntradasJ4.Enabled = false;
            switch (this.juego.jug_actual)
            {
                case 1: this.ImagenTurnoJ1.Visible = true;
                        break;
                case 2: this.ImagenTurnoJ2.Visible = true;
                        break;
                case 3: this.ImagenTurnoJ3.Visible = true;
                        break;
                case 4: this.ImagenTurnoJ4.Visible = true;
                        break;
            }
        }

        public Boolean Todos_Eliminados()
        {
            return this.juego.n_jugadores_activos == 1;
        }

        public void Conexion_perdida()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button) control.Enabled = false;
            }
            this.bReiniciar.Enabled = false;
            this.bSalir.Enabled = true;
            this.controlJ1.Enabled = false;
            this.controlJ2.Enabled = false;
            this.controlJ3.Enabled = false;
            this.controlJ4.Enabled = false;
            this.partida_activa = false;
        }

        public void Finalizar_Partida(Jugador ganador)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button) control.Enabled = false;
            }
            if (!this.online)
                this.bReiniciar.Enabled = true;
            this.bSalir.Enabled = true;
            this.controlJ1.Enabled = false;
            this.controlJ2.Enabled = false;
            this.controlJ3.Enabled = false;
            this.controlJ4.Enabled = false;
            this.partida_activa = false;
            if (ganador != null)
                MessageBox.Show(String.Format(Mensajes.mensajePartidaFinalizada, ganador.Nombre_color()), resources.GetString("tituloHotel"));
            else
                MessageBox.Show(Mensajes.mensajeJugadoresRetidados);
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

        public void Pasar_Turno(String sig_jugador, Boolean automaticamente = false)
        {
            if (Todos_Eliminados())
                Finalizar_Partida(this.juego.jugadores[Sig_jugador_Activo()-1]);
            else if (this.juego.n_jugadores_activos == 0)
                Finalizar_Partida(null);
            else
            {
                if (!this.online)
                    this.juego.jug_actual = Sig_jugador_Activo();
                else
                    this.juego.jug_actual = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == sig_jugador).n_jugador + 1;
                this.Establecer_Turno();
                this.juego.Cambiar_jugador_actual();
                if ((!this.online) || (this.online && (this.nombre_online == this.juego.jugador_actual.nombre_online)))
                    this.bDado.Enabled = true;
                else
                    this.bDado.Enabled = false;
                this.bComprar.Enabled = false;
                this.bConstruir.Enabled = false;
                this.bComprarSuelo.Enabled = false;
                this.bCobrarBanca.Enabled = false;
                this.bRetirarseJ1.Enabled = false;
                this.bRetirarseJ2.Enabled = false;
                this.bRetirarseJ3.Enabled = false;
                this.bRetirarseJ4.Enabled = false;
                switch (this.juego.jugador_actual.n_jugador)
                {
                    case 0: this.bRetirarseJ1.Enabled = true;
                        break;
                    case 1: this.bRetirarseJ2.Enabled = true;
                        break;
                    case 2: this.bRetirarseJ3.Enabled = true;
                        break;
                    case 3: this.bRetirarseJ4.Enabled = true;
                        break;
                }
                this.juego.jugador_actual.pago_ultimo_turno = false;
                this.dado_tirado = false;
                System.Media.SystemSounds.Beep.Play();
                if (this.online && Application.OpenForms.Cast<Form>().Contains(this.actividad))
                    this.actividad.PasarTurno(this.juego.jugador_actual.nombre_online, this.juego.jugador_actual.Nombre_color(), automaticamente);//Actualizando datos de la actividad
                if (automaticamente)
                    this.bTurno.Enabled = false;
            }
            // Cerrar las ventanas que se pudieran quedar abiertas al ser un paso automático
            if (automaticamente)
            {
                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    if ((form is Principal) || (form is Online) || (form is Chat) || (form is PartidaOnline) || (form is Actividad)
                        || (form is Reglas) || (form is ReporteBug) || (form is VerHoteles) || (form is Subastas))
                        continue;
                    else
                    {
                        if (form is PedirPago)
                        {
                            (form as PedirPago).cancelado = true;
                            form.Hide();
                        }
                        else if (form is Construir)
                        {
                            (form as Construir).cancelado = true;
                            form.Close();
                        }
                        else if (form is ComprarHotel)
                        {
                            (form as ComprarHotel).cancelado = true;
                            form.Close();
                        }
                        else
                            form.Close();
                    }
                }
            }
        }

        private Image ObtenerImagenDesdeColor(Tipos.Tcolor color)
        {
            switch (color)
            {
                case Tipos.Tcolor.amarillo: return (Image)global::Juego_Hotel.Properties.Resources.TurnoAmarillo.Clone();
                case Tipos.Tcolor.azul: return (Image)global::Juego_Hotel.Properties.Resources.TurnoAzul.Clone();
                case Tipos.Tcolor.rojo: return (Image)global::Juego_Hotel.Properties.Resources.TurnoRojo.Clone();
                case Tipos.Tcolor.verde: return (Image)global::Juego_Hotel.Properties.Resources.TurnoVerde.Clone();
                default: return null;
            }
        }

        public void Crear_Jugadores()
        {
            if (!this.partida_cargada_offline)
            {
                this.juego.jugadores = new Jugador[this.juego.n_jugadores];
                this.juego.n_jugadores_activos = this.juego.n_jugadores;
                XmlDocument configuracion_online = new XmlDocument();
                XmlNode config_dinero;
                try
                {
                    if (this.online)
                    {
                        configuracion_online.LoadXml(this.online_config);
                        config_dinero = configuracion_online.GetElementsByTagName("money_per_player")[0];
                    }
                    else
                        config_dinero = configuracion.GetElementsByTagName("money_per_player")[0];
                }
                catch
                {
                    if (!this.online)
                    {
                        MessageBox.Show(Mensajes.mensajeErrorCargarConfig, resources.GetString("tituloError"));
                        this.juego.jugadores = null;
                        throw;
                    }
                    else
                    {
                        MessageBox.Show(Mensajes.mensajeErrorCargarConfigServidor, resources.GetString("tituloError"));
                        this.juego.jugadores = null;
                        throw;
                    }
                }
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
                    case 4: if (this.partida_cargada_online)
                                this.juego.jugadores[3] = new Jugador(this.juego.lista_jugadores_online[3].Item1, this.frm_colores.color_j4, this.juego.lista_jugadores_online[3].Item2);
                            else
                            {
                                this.juego.jugadores[3] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j4, 3);
                                if (this.online)
                                    this.juego.jugadores[3].nombre_online = this.juego.lista_jugadores_online[3].Item1;
                            }
                            this.controlJ4.Enabled = true;
                            this.ImagenTurnoJ4.Image = ObtenerImagenDesdeColor(this.juego.jugadores[3].color);
                            goto case 3;
                    case 3: if (this.partida_cargada_online)
                                this.juego.jugadores[2] = new Jugador(this.juego.lista_jugadores_online[2].Item1, this.frm_colores.color_j3, this.juego.lista_jugadores_online[2].Item2);
                            else
                            {
                                this.juego.jugadores[2] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j3, 2);
                                if (this.online)
                                    this.juego.jugadores[2].nombre_online = this.juego.lista_jugadores_online[2].Item1;
                            }
                            this.controlJ3.Enabled = true;
                            this.ImagenTurnoJ3.Image = ObtenerImagenDesdeColor(this.juego.jugadores[2].color);
                            goto case 2;
                    case 2: if (this.partida_cargada_online)
                            {
                                this.juego.jugadores[1] = new Jugador(this.juego.lista_jugadores_online[1].Item1, this.frm_colores.color_j2, this.juego.lista_jugadores_online[1].Item2);
                                this.juego.jugadores[0] = new Jugador(this.juego.lista_jugadores_online[0].Item1, this.frm_colores.color_j1, this.juego.lista_jugadores_online[0].Item2);
                            }
                            else
                            {
                                this.juego.jugadores[1] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j2, 1);
                                this.juego.jugadores[0] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, this.frm_colores.color_j1, 0);
                                if (this.online)
                                {
                                    this.juego.jugadores[1].nombre_online = this.juego.lista_jugadores_online[1].Item1;
                                    this.juego.jugadores[0].nombre_online = this.juego.lista_jugadores_online[0].Item1;
                                }
                            }
                            this.controlJ2.Enabled = true;
                            this.controlJ1.Enabled = true;
                            this.ImagenTurnoJ2.Image = ObtenerImagenDesdeColor(this.juego.jugadores[1].color);
                            this.ImagenTurnoJ1.Image = ObtenerImagenDesdeColor(this.juego.jugadores[0].color);
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

        public void Tirar_Dado(Jugador jugador, Boolean automaticamente = false)
        {
            if (!this.online)
            {
                this.juego.ultimo_res_dado = this.juego.dado.tirar();
                jugador.posicion.ocupada = false; // Desocupamos la casilla
                if ((jugador.posicion.numero + this.juego.ultimo_res_dado) <= 31) // Damos la vuelta al tablero
                    jugador.posicion = this.juego.casillas[jugador.posicion.numero + this.juego.ultimo_res_dado];
                else
                    jugador.posicion = this.juego.casillas[jugador.posicion.numero + this.juego.ultimo_res_dado - 31];
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
            }
            if (this.online && Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.DadoTirado(jugador, this.juego.ultimo_res_dado, automaticamente);//Actualizando datos de la actividad
            this.resDado.Text = resources.GetString("resDado.Text") + this.juego.ultimo_res_dado.ToString();
            // Pintamos el coche en su lugar
            Point posicion = Calcular_Posicion(this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.X, this.juego.casillas[this.juego.jugador_actual.posicion.numero].pos_coche.Y);
            switch (jugador.color)
            {
                case Tipos.Tcolor.rojo:     this.posRojo.Image = RotateImage(posRojo_orig, jugador.posicion.pos_coche.grados);
                                            this.posRojo.Location = posicion;
                                            break;
                case Tipos.Tcolor.azul:     this.posAzul.Image = RotateImage(posAzul_orig, jugador.posicion.pos_coche.grados);
                                            this.posAzul.Location = posicion;
                                            break;
                case Tipos.Tcolor.verde:    this.posVerde.Image = RotateImage(posVerde_orig, jugador.posicion.pos_coche.grados);
                                            this.posVerde.Location = posicion;
                                            break;
                case Tipos.Tcolor.amarillo: this.posAmarillo.Image = RotateImage(posAmarillo_orig, jugador.posicion.pos_coche.grados);
                                            this.posAmarillo.Location = posicion;
                                            break;
            }
            // Poner casilla actual a cada uno
            switch (this.juego.jug_actual)
            {
                case 1: this.posJ1.Text = resources.GetString("posJ1.Text") + jugador.posicion.numero.ToString() + " (" + jugador.posicion.ObtenerTipoTxt() + ")";
                    break;
                case 2: this.posJ2.Text = resources.GetString("posJ2.Text") + jugador.posicion.numero.ToString() + " (" + jugador.posicion.ObtenerTipoTxt() + ")";
                    break;
                case 3: this.posJ3.Text = resources.GetString("posJ3.Text") + jugador.posicion.numero.ToString() + " (" + jugador.posicion.ObtenerTipoTxt() + ")";
                    break;
                case 4: this.posJ4.Text = resources.GetString("posJ4.Text") + jugador.posicion.numero.ToString() + " (" + jugador.posicion.ObtenerTipoTxt() + ")";
                    break;
            }
            // Activar botones según el tipo de casilla
            this.bPedirNochesJ1.Enabled = true;
            this.bPedirNochesJ2.Enabled = true;
            this.bPedirNochesJ3.Enabled = true;
            this.bPedirNochesJ4.Enabled = true;
            switch (this.juego.jugador_actual.n_jugador)
            {
                case 0: this.bPedirNochesJ1.Enabled = false;
                    break;
                case 1: this.bPedirNochesJ2.Enabled = false;
                    break;
                case 2: this.bPedirNochesJ3.Enabled = false;
                    break;
                case 3: this.bPedirNochesJ4.Enabled = false;
                    break;
            }
            if ((!this.online) || (this.online && (this.nombre_online == jugador.nombre_online)))
            {
                jugador.entrada_gratis_usada = false;
                foreach (Hotel hotel in this.juego.hoteles)
                    hotel.entrada_comprada_ultimo_turno = false;
                this.bComprarSuelo.Enabled = true;
                this.bEntradasJ1.Enabled = false;
                this.bEntradasJ2.Enabled = false;
                this.bEntradasJ3.Enabled = false;
                this.bEntradasJ4.Enabled = false;
                if (this.Puede_poner_entradas(this.juego.jugador_actual))
                    this.Activar_Poner_Entradas(this.juego.jug_actual);
                if (this.Puede_Cobrar_Banca(this.juego.jug_actual))
                    this.bCobrarBanca.Enabled = true;
                else
                    this.bCobrarBanca.Enabled = false;
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
                        if (!automaticamente)
                            MessageBox.Show(Mensajes.mensajeCasillaFaseGratis);
                        break;
                    case Tipos.Tcasilla.entrada_gratis: this.bConstruir.Enabled = false;
                        this.bComprar.Enabled = false;
                        this.Activar_Poner_Entradas(this.juego.jug_actual);
                        if (!automaticamente)
                            MessageBox.Show(Mensajes.mensajeCasillaEntradaGratis);
                        break;
                    default: this.bConstruir.Enabled = false;
                        this.bComprar.Enabled = false;
                        break;
                }
                if (this.juego.ultimo_res_dado == 6)
                {
                    if (!automaticamente)
                        MessageBox.Show(Mensajes.mensajeSacadoUnSeis);
                    this.bDado.Enabled = true;
                }
                this.bTurno.Enabled = true;
                this.dado_tirado = true;
            }
        }

        private void bDado_Click(object sender, EventArgs e)
        {
            this.bDado.Enabled = false;
            if (this.online)
                this.frm_online.enviar_comando("roll_dice", this.game_id.ToString());
            else
                this.Tirar_Dado(this.juego.jugador_actual);
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
            DialogResult dr = MessageBox.Show(Mensajes.mensajeReiniciarPartida, resources.GetString("tituloHotel"), MessageBoxButtons.YesNo);
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
            this.bComprar.Enabled = false;
            this.bConstruir.Enabled = false;
            this.bComprarSuelo.Enabled = false;
            this.controlJ1.Enabled = false;
            this.controlJ2.Enabled = false;
            this.controlJ3.Enabled = false;
            this.controlJ4.Enabled = false;
            this.bCobrarBanca.Enabled = false;
            this.bVerHoteles.Enabled = true;
            this.bNormas.Enabled = true;
            this.bReportarBug.Enabled = true;
            this.jug_ini.Text = resources.GetString("jug_ini.Text");
            this.colorJugIni.Text = "";
            this.posJ1.Text = resources.GetString("posJ1.Text");
            this.posJ2.Text = resources.GetString("posJ2.Text");
            this.posJ3.Text = resources.GetString("posJ3.Text");
            this.posJ4.Text = resources.GetString("posJ4.Text");
            this.dineroJ1.Text = resources.GetString("dineroJ1.Text");
            this.dineroJ2.Text = resources.GetString("dineroJ2.Text");
            this.dineroJ3.Text = resources.GetString("dineroJ3.Text");
            this.dineroJ4.Text = resources.GetString("dineroJ4.Text");
            this.colorJ1.Text = resources.GetString("colorJ1.Text");
            this.colorJ2.Text = resources.GetString("colorJ2.Text");
            this.colorJ3.Text = resources.GetString("colorJ3.Text");
            this.colorJ4.Text = resources.GetString("colorJ4.Text");
            this.juego.jugadores = null;
            this.posRojo.Image = posRojo_orig;
            this.posAzul.Image = posAzul_orig;
            this.posAmarillo.Image = posAmarillo_orig;
            this.posVerde.Image = posVerde_orig;
            this.posRojo.Location = Calcular_Posicion(this.pos_rojo_orig.X, this.pos_rojo_orig.Y);
            this.posAzul.Location = Calcular_Posicion(this.pos_azul_orig.X, this.pos_azul_orig.Y);
            this.posVerde.Location = Calcular_Posicion(this.pos_verde_orig.X, this.pos_verde_orig.Y);
            this.posAmarillo.Location = Calcular_Posicion(this.pos_amarillo_orig.X, this.pos_amarillo_orig.Y);
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
            this.partida_cargada_offline = false;
            this.partida_cargada_online = false;
        }

        private void bTurno_Click(object sender, EventArgs e)
        {
            this.bTurno.Enabled = false;
            if (this.online)
                this.frm_online.enviar_comando("turn_pass", this.game_id.ToString());
            else
                this.Pasar_Turno(null);
        }

        private void bColores_Click(object sender, EventArgs e)
        {
            if (this.juego.n_jugadores == 0)
                MessageBox.Show(Mensajes.mensajeNoIndicadoJugadores, resources.GetString("tituloNoIndicadoJugadores"));
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
            ComprarHotel frm_comprar_hotel = new ComprarHotel();
            Jugador jugador = this.juego.jugadores[this.juego.jug_actual - 1];
            nombre_izq = this.juego.jugadores[this.juego.jug_actual - 1].posicion.hotel_izq;
            nombre_der = this.juego.jugadores[this.juego.jug_actual - 1].posicion.hotel_der;
            frm_comprar_hotel.HabilitarControles(this.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre == nombre_izq), this.juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre == nombre_der));
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
                            DialogResult dr = MessageBox.Show(String.Format(Mensajes.mensajeExpropiacionPosible, hotel.nombre, hotel.dueño.Nombre_color()), resources.GetString("tituloExpropiacionPosible"), MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                if (hotel.precio_expropiacion > this.juego.jugador_actual.dinero_total)
                                    MessageBox.Show(Mensajes.mensajeSinFondosParaExpropiacion);
                                else
                                    this.Comprar_hotel(ref hotel, ref jugador, true);
                            }
                        }
                        else
                            MessageBox.Show(Mensajes.mensajeExpropiacionImposible, Mensajes.tituloExpropiacionImposible);
                    }
                    else
                        MessageBox.Show(Mensajes.mensajeImposibleComprarHotelTuyo, Mensajes.tituloImposibleComprarHotelTuyo);
                }
                else // Es posible comprar el hotel
                {
                    if (hotel.precio > jugador.dinero_total)
                        MessageBox.Show(String.Format(Mensajes.mensajeFondosInsuficientesParaComprarHotel, hotel.nombre_txt));
                    else
                    {
                        DialogResult dr = MessageBox.Show(Mensajes.mensajeComprarHotel, Mensajes.tituloComprarHotel, MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                            this.Comprar_hotel(ref hotel, ref jugador, false);
                    }
                }
            }
            frm_comprar_hotel.Close();
        }

        void Comprar_hotel (ref Hotel hotel, ref Jugador jugador, Boolean expropiando)
        {
            // En caso de que el turno se pase automáticamente y quede algun messagebox abierto, no hacer nada
            if (juego.jugador_actual != jugador)
                return;
            int dinero_necesario;
            int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;

            if (expropiando)
                dinero_necesario = hotel.precio_expropiacion;
            else
                dinero_necesario = hotel.precio;

            PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, this.juego.jugador_actual, this, null);
            frm_pago.ShowDialog();
            if (!frm_pago.cancelado)
            {
                if (expropiando)
                {
                    if (!this.online)
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
                        n_5000 = frm_pago.n_5000;
                        n_1000 = frm_pago.n_1000;
                        n_500 = frm_pago.n_500;
                        n_100 = frm_pago.n_100;
                        n_50 = frm_pago.n_50;
                        this.frm_online.enviar_comando("expropriate_hotel", this.game_id.ToString(), hotel.nombre_txt, n_5000.ToString(),
                             n_1000.ToString(), n_500.ToString(), n_100.ToString(), n_50.ToString());
                    }
                }
                else
                {
                    if (!this.online)
                    {
                        jugador.Comprar_Hotel(ref hotel, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
                        if (frm_pago.total_seleccionado > dinero_necesario)
                        {
                            Principal.Calcular_Devolucion((frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                    }
                    else // Todo se hace en el lado del servidor, devolución incluída
                    {
                        n_5000 = frm_pago.n_5000;
                        n_1000 = frm_pago.n_1000;
                        n_500 = frm_pago.n_500;
                        n_100 = frm_pago.n_100;
                        n_50 = frm_pago.n_50;
                        this.frm_online.enviar_comando("buy_hotel", this.game_id.ToString(), hotel.nombre_txt, n_5000.ToString(),
                             n_1000.ToString(), n_500.ToString(), n_100.ToString(), n_50.ToString());
                    }
                }
            }
            frm_pago.Close();
            this.bComprar.Enabled = false;
            this.bComprarSuelo.Enabled = false;
            this.Actualizar_Dinero_Jugadores();
        }

        public void Actualizar_Fases_Nuevo_Dueño_Hotel(Hotel hotel)
        {
            Image imagen_fase;
            switch (hotel.dueño.color)
            {
                case Tipos.Tcolor.amarillo: imagen_fase = (Image)this.img_tick_Amarillo.Clone();
                    break;
                case Tipos.Tcolor.azul: imagen_fase = (Image)this.img_tick_Azul.Clone();
                    break;
                case Tipos.Tcolor.rojo: imagen_fase = (Image)this.img_tick_Rojo.Clone();
                    break;
                case Tipos.Tcolor.verde: imagen_fase = (Image)this.img_tick_Verde.Clone();
                    break;
            }
            Control[] lista_fases = this.Controls.Find("fase", true);
            foreach (Control fase in lista_fases)
            {

            }
        }

        public void Hotel_Comprado(Hotel hotel, Jugador jugador)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.ComprarHotel(jugador.nombre_online, jugador.Nombre_color(), hotel.nombre_txt); //Actualizando datos de la actividad
        }

        public void Hotel_Expropiado(Jugador jugador,Jugador expropiado, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Expropiar_Hotel(jugador, expropiado, hotel);
        }

        public void Tirar_Dado_Construccion(Jugador jugador, Tipos.Resultado_dado_cons resultado)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Tirar_Dado_Construccion(jugador, resultado);
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

        public void Actualizar_Dinero_Jugador(String nombre_jugador, int n_50, int n_100, int n_500, int n_1000, int n_5000)
        {
            // Solo es llamada en modo online
            Jugador jugador = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            jugador.n_billetes_50 = n_50;
            jugador.n_billetes_100 = n_100;
            jugador.n_billetes_500 = n_500;
            jugador.n_billetes_1000 = n_1000;
            jugador.n_billetes_5000 = n_5000;
            jugador.calcular_dinero_total();
            switch (jugador.n_jugador)
            {
                case 0: this.dineroJ1.Text = resources.GetString("dineroJ1.Text") + jugador.dinero_total;
                    break;
                case 1: this.dineroJ2.Text = resources.GetString("dineroJ2.Text") + jugador.dinero_total;
                    break;
                case 2: this.dineroJ3.Text = resources.GetString("dineroJ3.Text") + jugador.dinero_total;
                    break;
                case 3: this.dineroJ4.Text = resources.GetString("dineroJ4.Text") + jugador.dinero_total;
                    break;
            }
        }

        public void Actualizar_Dinero_Jugador_Actual()
        {
            switch (this.juego.jug_actual)
            {
                case 1: this.dineroJ1.Text = resources.GetString("dineroJ1.Text") + this.juego.jugador_actual.dinero_total;
                        break;
                case 2: this.dineroJ2.Text = resources.GetString("dineroJ2.Text") + this.juego.jugador_actual.dinero_total;
                        break;
                case 3: this.dineroJ3.Text = resources.GetString("dineroJ3.Text") + this.juego.jugador_actual.dinero_total;
                        break;
                case 4: this.dineroJ4.Text = resources.GetString("dineroJ4.Text") + this.juego.jugador_actual.dinero_total;
                        break;
            }
        }

        public void Actualizar_Dinero_Jugadores()
        {
            switch (this.juego.n_jugadores)
            {
                case 4: this.dineroJ4.Text = resources.GetString("dineroJ1.Text") + this.juego.jugadores[3].dinero_total;
                        goto case 3;
                case 3: this.dineroJ3.Text = resources.GetString("dineroJ2.Text") + this.juego.jugadores[2].dinero_total;
                        goto case 2;
                case 2: this.dineroJ2.Text = resources.GetString("dineroJ3.Text") + this.juego.jugadores[1].dinero_total;
                        this.dineroJ1.Text = resources.GetString("dineroJ4.Text") + this.juego.jugadores[0].dinero_total;
                        break;
            }
        }

        private void bConstruir_Click(object sender, EventArgs e)
        {
            if (this.juego.jugador_actual.hoteles.Count == 0)
            {
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles);
                return;
            }
            Construir frm_construir = new Construir(ref this.juego, false, this);
            DialogResult dr = frm_construir.ShowDialog();
            if ((dr == DialogResult.OK) || (dr == DialogResult.Abort))
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
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles);
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
            this.bCobrarBanca.Enabled = false;
            if (this.online)
                this.frm_online.enviar_comando("charge_bank", this.game_id.ToString());
            else
            {
                this.juego.jugador_actual.Cobrar_Banco();
                this.Actualizar_Dinero_Jugador_Actual();
            }
        }

        private void bVerHotelesJ1_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[0].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, false);
                frm_ver_hoteles.Show(this);
                this.bVerHotelesJ1.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, resources.GetString("tituloNoEsPosibleMostrarTusHoteles"));
        }

        private void bVerHotelesJ2_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[1].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 1, false);
                frm_ver_hoteles.Show(this);
                this.bVerHotelesJ2.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHotelesJ3_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[2].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 2, false);
                frm_ver_hoteles.Show(this);
                this.bVerHotelesJ3.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHotelesJ4_Click(object sender, EventArgs e)
        {
            if (this.juego.jugadores[3].hoteles.Count != 0)
            {
                VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 3, false);
                frm_ver_hoteles.Show(this);
                this.bVerHotelesJ4.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            VerHoteles frm_ver_hoteles = new VerHoteles(ref this.juego, 0, true);
            frm_ver_hoteles.Show(this);
            this.bVerHoteles.Enabled = false;
        }

        public void ReactivarVerHoteles()
        {
            this.bVerHoteles.Enabled = true;
            if (this.controlJ1.Enabled)
                this.bVerHotelesJ1.Enabled = true;
            if (this.controlJ2.Enabled)
                this.bVerHotelesJ2.Enabled = true;
            if (this.controlJ3.Enabled)
                this.bVerHotelesJ3.Enabled = true;
            if (this.controlJ4.Enabled)
                this.bVerHotelesJ4.Enabled = true;
        }

        public Boolean Puede_poner_entradas(Jugador jugador)
        {
            int pos = jugador.posicion.numero;
            if (pos - this.juego.ultimo_res_dado - this.juego.ultimo_avance_auto < 1) // En caso de pasarse de la vuelta al tablero en la misma tirada que te toca poner entradas
                pos += 31;
            if ((pos >= 27) && ((pos - this.juego.ultimo_res_dado - this.juego.ultimo_avance_auto) < 27))
                return true;
            else
                return false;
        }

        private void Poner_entradas(int num_jugador)
        {
            if (this.juego.jugadores[num_jugador].hoteles.Count != 0)
            {
                PonerEntradas frm_poner_entradas = new PonerEntradas(num_jugador, this);
                frm_poner_entradas.ShowDialog();
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoesPosiblePonerEntradas);
        }

        private void bEntradasJ1_Click(object sender, EventArgs e)
        {
            this.Poner_entradas(0);
        }

        private void bEntradasJ2_Click(object sender, EventArgs e)
        {
            this.Poner_entradas(1);
        }

        private void bEntradasJ3_Click(object sender, EventArgs e)
        {
            this.Poner_entradas(2);
        }

        private void bEntradasJ4_Click(object sender, EventArgs e)
        {
            this.Poner_entradas(3);
        }

        public void Añadir_Entrada(Jugador jugador, int casilla, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Añadir_Entrada(jugador, casilla, hotel);
        }

        public void Añadir_Fase(Jugador jugador, int fase, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Añadir_Fase(jugador, fase, hotel);
        }

        public void Subasta_Iniciada(Jugador jugador, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Iniciar_Subasta(jugador, hotel);
        }

        public void Nueva_Puja(Jugador jugador, int cantidad, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Realizar_Puja(jugador, cantidad, hotel);
        }

        public void Subasta_Terminada(Jugador vendedor, int cantidad, Jugador comprador, String hotel, Boolean automaticamente)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Subasta_Terminada(vendedor, comprador, hotel, cantidad, automaticamente);
        }

        private void bNormas_Click(object sender, EventArgs e)
        {
            Reglas frm_reglas = new Reglas();
            frm_reglas.Show();
        }

        private void Pedir_Noches(int n_jugador)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            // Reiniciar el bucle si la lista de hoteles cambia por una subasta
            Boolean reiniciar;
            int cuantos_hoteles;
            do
            {
                reiniciar = false;
                cuantos_hoteles = this.juego.jugadores[n_jugador].hoteles.Count;
                foreach (Hotel hotel in this.juego.jugadores[n_jugador].hoteles)
                {
                    // Iteramos por las casillas con entrada del hotel
                    foreach (Casilla casilla in hotel.entradas)
                    {
                        if (casilla.ocupada == true)
                        {
                            // Buscar el jugador que esté en la casilla
                            Jugador jugador_pagador = this.juego.jugadores.FirstOrDefault(x => (x.posicion.numero == casilla.numero)
                                && (x.color != this.juego.jugadores[n_jugador].color) && (x.pago_ultimo_turno == false));
                            if (jugador_pagador != null)
                            {
                                // El jugador encontrado debe pagar las noches correspondientes
                                MessageBox.Show(String.Format(Mensajes.mensajeDebePagarNoches, jugador_pagador.Nombre_color(),
                                    hotel.dueño.Nombre_color()), resources.GetString("tituloPagarNoches"), MessageBoxButtons.OK);
                                int num_noches = this.juego.dado.tirar();
                                int dinero_necesario = hotel.Calcular_noches(num_noches);
                                MessageBox.Show(String.Format(Mensajes.mensajeTotalAPagar, num_noches, jugador_pagador.Nombre_color(), dinero_necesario, hotel.dueño.Nombre_color()));
                                PedirPago frm_pago = new PedirPago(dinero_necesario, ref this.juego, jugador_pagador, this, hotel.dueño);
                                frm_pago.ShowDialog();
                                jugador_pagador.pago_ultimo_turno = true;
                                jugador_pagador.Pagar_Noches(ref hotel.dueño, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                                if (frm_pago.total_seleccionado > dinero_necesario)
                                {
                                    int n_5000, n_1000, n_500, n_100, n_50;
                                    Principal.Calcular_Devolucion(ref hotel.dueño, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                                    jugador_pagador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                                }
                                frm_pago.Close();
                                if (this.juego.jugadores[n_jugador].hoteles.Count != cuantos_hoteles)
                                {
                                    reiniciar = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (reiniciar)
                        break;
                }
            }
            while (reiniciar);
            this.Actualizar_Dinero_Jugadores();
        }

        public void Registrar_Pagar_Noches_Online(Jugador jugador_dueño, Jugador jugador_pagador, int cantidad, int noches, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(this.actividad))
                this.actividad.Pedir_Noches_Online(jugador_dueño, jugador_pagador, cantidad, noches, hotel);
        }

        public void Pedir_Noches_Online(Jugador jugador, int cantidad, int noches, String hotel)
        {
            this.bTurno.Enabled = false;
            PedirPago frm_pago = new PedirPago(cantidad, ref this.juego, this.juego.jugador_actual, this, jugador);
            frm_pago.ShowDialog();
            if (!frm_pago.cancelado)
            {
                jugador.pago_ultimo_turno = true;
                int n_5000 = frm_pago.n_5000, n_1000 = frm_pago.n_1000, n_500 = frm_pago.n_500, n_100 = frm_pago.n_100, n_50 = frm_pago.n_50;
                this.frm_online.enviar_comando("pay_nights", this.game_id.ToString(), n_5000.ToString(), n_1000.ToString(), n_500.ToString(), n_100.ToString(), n_50.ToString());
                this.bTurno.Enabled = true;
                this.Registrar_Pagar_Noches_Online(jugador, this.juego.jugador_actual, cantidad, noches, hotel);
            }
            frm_pago.Close();
        }

        private void bPedirNochesJ1_Click(object sender, EventArgs e)
        {
            this.bPedirNochesJ1.Enabled = false;
            if (this.online)
            {
                this.bTurno.Enabled = false;
                this.frm_online.enviar_comando("ask_nights", this.game_id.ToString());
            }
            else
                this.Pedir_Noches(0);
        }

        private void bPedirNochesJ2_Click(object sender, EventArgs e)
        {
            this.bPedirNochesJ2.Enabled = false;
            if (this.online)
            {
                this.bTurno.Enabled = false;
                this.frm_online.enviar_comando("ask_nights", this.game_id.ToString());
            }
            else
                this.Pedir_Noches(1);
        }

        private void bPedirNochesJ3_Click(object sender, EventArgs e)
        {
            this.bPedirNochesJ3.Enabled = false;
            if (this.online)
            {
                this.bTurno.Enabled = false;
                this.frm_online.enviar_comando("ask_nights", this.game_id.ToString());
            }
            else
                this.Pedir_Noches(2);
        }

        private void bPedirNochesJ4_Click(object sender, EventArgs e)
        {
            this.bPedirNochesJ4.Enabled = false;
            if (this.online)
            {
                this.bTurno.Enabled = false;
                this.frm_online.enviar_comando("ask_nights", this.game_id.ToString());
            }
            else
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
            this.bTurno.Enabled = true;
            this.bTurno.PerformClick();
            this.bTurno.Enabled = false;
        }

        public void Desactivar_Controles()
        {
            this.controlJ1.Enabled = false;
            this.controlJ2.Enabled = false;
            this.controlJ3.Enabled = false;
            this.controlJ4.Enabled = false;
            foreach (Control control in this.Controls)
            {
                if (control is Button) control.Enabled = false;
            }
            this.bSalir.Enabled = true;
            return;
        }

        private Boolean Retirarse(int num_jugador)
        {
            if (this.juego.jugadores[num_jugador].Eliminado()) // Ya está eliminado
                return true;
            if (MessageBox.Show(Mensajes.mensajeRetirarse, Mensajes.tituloHotel, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (!this.online)
                {
                    this.juego.Eliminar_Jugador(this.juego.jugadores[num_jugador], null);
                    this.Marcar_Jugador_Eliminado(num_jugador);
                }
                else
                {
                    this.frm_online.enviar_comando("retire", this.game_id.ToString(), "0");
                    this.partida_activa = false;
                    this.Desactivar_Controles();
                }
                return true;
            }
            else
                return false;
        }

        private void bRetirarseJ1_Click(object sender, EventArgs e)
        {
            this.Retirarse(0);
        }

        private void bRetirarseJ2_Click(object sender, EventArgs e)
        {
            this.Retirarse(1);
        }

        private void bRetirarseJ3_Click(object sender, EventArgs e)
        {
            this.Retirarse(2);
        }

        private void bRetirarseJ4_Click(object sender, EventArgs e)
        {
            this.Retirarse(3);
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.partida_activa)
                if (!this.Retirarse(this.juego.jugador_actual.n_jugador))
                    e.Cancel = true;
            this.Guardar_idioma(System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        private void Principal_Shown(object sender, EventArgs e)
        {
            if (this.online)
            { 
                this.bIniciar.PerformClick();
                this.actividad.Show();
            }
        }

        public string InputBox(string prompt, string title, string defaultValue)
        {
            InputBoxDialog ib = new InputBoxDialog();
            ib.FormPrompt = prompt;
            ib.FormCaption = title;
            ib.DefaultValue = defaultValue;
            ib.ShowDialog();
            string s = ib.InputResponse;
            ib.Close();
            return s;
        }

        private void bSalvar_Click(object sender, EventArgs e)
        {
            if (this.dado_tirado)
            {
                MessageBox.Show(Mensajes.mensajeSalvarAntesDeTirar);
                return;
            }
            if (this.online)
            {
                string password = this.InputBox(Mensajes.mensajeInputPasswordPartida, Mensajes.tituloCrearPartida, "");
                this.frm_online.enviar_comando("save_game", this.game_id.ToString(), password);
            }
            else
            {   
                SaveFileDialog dialogo = new SaveFileDialog();
                dialogo.AddExtension = true;
                dialogo.CheckPathExists = true;
                dialogo.DefaultExt = "xml";
                dialogo.SupportMultiDottedExtensions = true;
                dialogo.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
                dialogo.Filter = Mensajes.filtroDialogoCargarGuardarPartida;
                dialogo.Title = Mensajes.tituloDialogoGuardarPartida;
                dialogo.FileName = "Partida Hotel " + System.DateTime.Today.ToShortDateString().Replace('/', '-') + ".xml";
                if (dialogo.ShowDialog() != DialogResult.Cancel)
                {
                    if (dialogo.FileName != "")
                    {
                        Salvar_y_cargar mgr_salvar = new Salvar_y_cargar(ref this.juego);
                        if (mgr_salvar.Salvar_partida(dialogo.FileName) == false)
                            MessageBox.Show(String.Format(Mensajes.mensajeErrorAlSalvar, mgr_salvar.error));
                    }
                }
                dialogo = null;
            }
        }

        private void bCargar_Click(object sender, EventArgs e)
        {
            if (!this.bIniciar.Enabled)
                if (MessageBox.Show(Mensajes.mensajeCargarPartida, Mensajes.tituloCargarPartida, MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.AddExtension = true;
            dialogo.CheckPathExists = true;
            dialogo.DefaultExt = "xml";
            dialogo.SupportMultiDottedExtensions = true;
            String dir_trabajo = System.IO.Directory.GetCurrentDirectory(); // Después de cargar el fichero, el directorio actual se pierde
            dialogo.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            dialogo.Filter = Mensajes.filtroDialogoCargarGuardarPartida;
            dialogo.Title = Mensajes.tituloDialogoCargarPartida;
            dialogo.FileName = "Partida Hotel " + System.DateTime.Today.ToShortDateString().Replace('/', '-') + ".xml";
            if (dialogo.ShowDialog() != DialogResult.Cancel)
            {
                if (dialogo.FileName != "")
                {
                    this.Reiniciar_partida();
                    Salvar_y_cargar mgr_cargar = new Salvar_y_cargar(ref this.juego);
                    if (mgr_cargar.Cargar_partida(dialogo.FileName, this) == false)
                    {
                        MessageBox.Show(String.Format(Mensajes.mensajeErrorAlCargar, Environment.NewLine + mgr_cargar.error));
                        this.Reiniciar_partida();
                    }
                    else
                    {
                        this.partida_cargada_offline = true;
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
                // Creando nuevo PictureBox para meter la imagen de la fase
                PictureBox fase = new PictureBox();
                ((ISupportInitialize)(fase)).BeginInit();
                Tipos.Posicion pos = hotel.posiciones_fases.ToList()[num_fase];
                switch (hotel.dueño.color)
                {
                    case Tipos.Tcolor.amarillo: fase.Image = (Image)this.img_tick_Amarillo.Clone();
                        break;
                    case Tipos.Tcolor.azul: fase.Image = (Image)this.img_tick_Azul.Clone();
                        break;
                    case Tipos.Tcolor.rojo: fase.Image = (Image)this.img_tick_Rojo.Clone();
                        break;
                    case Tipos.Tcolor.verde: fase.Image = (Image)this.img_tick_Verde.Clone();
                        break;
                }
                fase.BackColor = Color.Transparent;
                fase.Location = new Point(pos.X, pos.Y);
                fase.Size = Calcular_Tamaño(ancho_fase, alto_fase);
                fase.SizeMode = PictureBoxSizeMode.StretchImage;
                fase.TabStop = false;
                fase.Name = "fase";
                fase.Tag = new Tuple<String, Hotel>(fase.Location.X.ToString() + "@" + fase.Location.Y.ToString(), hotel);
                fase.Location = Calcular_Posicion(fase.Location.X, fase.Location.Y);
                this.Controls.Add(fase);
                fase.Parent = this.imgTablero;
                ((ISupportInitialize)(fase)).EndInit();
                fase.BringToFront();
            }
        }

        public void Dibujar_Entrada(Casilla casilla, Boolean en_la_derecha)
        {
            // Se repinta el tablero con la nueva imagen encima
            Image tablero = this.imgTablero.Image;
            Graphics g = Graphics.FromImage(tablero);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Image entrada;
            Tipos.Posicion pos;
            if (en_la_derecha)
            {
                entrada = RotateImage(this.img_entrada, casilla.pos_entrada_der.grados);
                pos = casilla.pos_entrada_der;
            }
            else
            {
                entrada = RotateImage(this.img_entrada, casilla.pos_entrada_izq.grados);
                pos = casilla.pos_entrada_izq;
            }
            if (entrada != null)
                g.DrawImage(entrada, pos.X, pos.Y);
            else
                return;
            // Sustitur imagen actual
            this.imgTablero.Image = tablero;
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

        Point Calcular_Posicion(int x, int y)
        {
            x -= this.imgTablero.Left;
            y -= this.imgTablero.Top;
            float desplazamiento_x = ((float)this.imgTablero.Width / (float)this.ancho_ini);
            float desplazamiento_y = ((float)this.imgTablero.Height / (float)this.alto_ini);
            int x2 = Convert.ToInt32((float)x * desplazamiento_x);
            int y2 = Convert.ToInt32((float)y * desplazamiento_y);
            return new Point(x2, y2);
        }

        Size Calcular_Tamaño(int ancho, int alto)
        {
            float factor_x = ((float)this.imgTablero.Width / (float)this.ancho_ini);
            float factor_y = ((float)this.imgTablero.Height / (float)this.alto_ini);
            int ancho2 = Convert.ToInt32((float)ancho * factor_x);
            int alto2 = Convert.ToInt32((float)alto * factor_y);
            return new Size(ancho2, alto2);
        }

        private void Principal_Resize(object sender, EventArgs e)
        {
            // Obtener nuevo tamaño y recolocar todos los picturebox
            if (this.juego == null)
               return; // No se ha inicializado la ventana aun, el constructor no se ha ejecutado
            // Banco y Ayuntamiento
            this.img_Banco.Location = Calcular_Posicion(this.pos_banco_orig.X, this.pos_banco_orig.Y);
            this.img_Banco.Size = Calcular_Tamaño(ancho_fase, alto_fase);
            this.img_ayto.Location = Calcular_Posicion(this.pos_ayto_orig.X, this.pos_ayto_orig.Y);
            this.img_ayto.Size = Calcular_Tamaño(ancho_fase, alto_fase);
            // Fases construidas
            Control[] lista_fases = this.Controls.Find("fase", true);
            String pos;
            int x, y;
            foreach (Control fase in lista_fases)
            {
                pos = ((Tuple<String, Hotel>)fase.Tag).Item1.ToString(); // Uso la propiedad Tag para almacenar la posición original
                x = Convert.ToInt32(pos.Split('@')[0]);
                y = Convert.ToInt32(pos.Split('@')[1]);
                fase.Location = Calcular_Posicion(x, y);
                fase.Size = Calcular_Tamaño(ancho_fase, alto_fase);
            }
            if (this.juego.jugadores == null) // Partida no empezada
            {
                this.posRojo.Location = Calcular_Posicion(this.pos_rojo_orig.X, this.pos_rojo_orig.Y);
                this.posRojo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                this.posAzul.Location = Calcular_Posicion(this.pos_azul_orig.X, this.pos_azul_orig.Y);
                this.posAzul.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                this.posVerde.Location = Calcular_Posicion(this.pos_verde_orig.X, this.pos_verde_orig.Y);
                this.posVerde.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                this.posAmarillo.Location = Calcular_Posicion(this.pos_amarillo_orig.X, this.pos_amarillo_orig.Y);
                this.posAmarillo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
            }
            else
            {
                Jugador jugador = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "rojo");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    this.posRojo.Location = Calcular_Posicion(this.pos_rojo_orig.X, this.pos_rojo_orig.Y);
                else
                    this.posRojo.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                this.posRojo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                jugador = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "azul");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    this.posAzul.Location = Calcular_Posicion(this.pos_azul_orig.X, this.pos_azul_orig.Y);
                else
                    this.posAzul.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                this.posAzul.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                jugador = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "verde");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    this.posVerde.Location = Calcular_Posicion(this.pos_verde_orig.X, this.pos_verde_orig.Y);
                else
                    this.posVerde.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                this.posVerde.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                jugador = this.juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "amarillo");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    this.posAmarillo.Location = Calcular_Posicion(this.pos_amarillo_orig.X, this.pos_amarillo_orig.Y);
                else
                    this.posAmarillo.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                this.posAmarillo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
            }            
        }

        private void Guardar_idioma(CultureInfo culture)
        {
            XmlNode nodo_Idioma;
            XmlDocument fichero_configuracion = new XmlDocument();
            if (this.online)
            {
                fichero_configuracion.Load("Config.xml");
                nodo_Idioma = fichero_configuracion.GetElementsByTagName("language")[0];
            }
            else
                nodo_Idioma = this.configuracion.GetElementsByTagName("language")[0];
            switch (culture.Name)
            {
                case "es": nodo_Idioma.ChildNodes[0].InnerText = "Spanish";
                    break;
                case "en": nodo_Idioma.ChildNodes[0].InnerText = "English";
                    break;
            }
            XmlTextWriter writer = new XmlTextWriter("Config.xml", Encoding.UTF8);
            try
            {
                writer.Formatting = Formatting.Indented;
                if (this.online)
                    fichero_configuracion.Save(writer);
                else
                    this.configuracion.Save(writer);
            }
            catch (Exception e)
            {
                MessageBox.Show(Mensajes.mensajeErrorSalvandoIdioma + ": " + e.Message);
            }
            finally
            {
                writer.Close();
            }
        }

        private void IdiomaElegido(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex.Equals(0))
            {
                Program.ReLocalizeAll(new CultureInfo("es"));
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
            }
            else if (senderComboBox.SelectedIndex.Equals(1))
            {
                Program.ReLocalizeAll(new CultureInfo("en"));
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            this.Guardar_idioma(System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        public void ReLocalize(CultureInfo nuevoCulture, CultureInfo antiguoCulture)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action<System.Globalization.CultureInfo, System.Globalization.CultureInfo>(this.ReLocalize), new object[] { nuevoCulture, antiguoCulture });
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = nuevoCulture;
                resources.ApplyResources(this, "$this");
                foreach (Control c in this.Controls)
                {
                    if (c is GroupBox)
                    {
                        c.Text = resources.GetString(c.Name + ".Text");
                        foreach (Control o in ((GroupBox)c).Controls)
                        {
                            if (o is Label)
                            {
                                switch (o.Name) // Etiquetas con el color
                                {
                                    case "colorJ1": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (this.juego.n_jugadores > 0)
                                                        o.Text += this.juego.jugadores[0].Nombre_color();
                                                    break;
                                    case "colorJ2": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (this.juego.n_jugadores > 1)
                                                        o.Text += this.juego.jugadores[1].Nombre_color();
                                                    break;
                                    case "colorJ3": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (this.juego.n_jugadores > 2)
                                                        o.Text += this.juego.jugadores[2].Nombre_color();
                                                    break;
                                    case "colorJ4": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (this.juego.n_jugadores > 3)
                                                        o.Text += this.juego.jugadores[3].Nombre_color();
                                                    break;
                                    default:    String nombreAntiguo = (String)resources.GetObject(o.Name + ".Text", antiguoCulture);
                                                if (nombreAntiguo != null)
                                                    o.Text = o.Text.Replace(nombreAntiguo, resources.GetString(o.Name + ".Text"));
                                                break;
                                }
                            }
                            else
                                resources.ApplyResources(o, o.Name);
                        }
                    }
                    else if (c is ComboBox)
                    {
                        ((ComboItemImagen)((ComboBox)c).Items[0]).Etiqueta = Mensajes.comboBoxIdiomas1;
                        ((ComboItemImagen)((ComboBox)c).Items[1]).Etiqueta = Mensajes.comboBoxIdiomas2;
                        if (antiguoCulture.Name.Equals("es"))
                            ((ComboBox)c).SelectedIndex = 1;
                        else
                            ((ComboBox)c).SelectedIndex = 0;
                    }
                    else if (c is Label)
                    {
                        String nombreAntiguo = (String)resources.GetObject(c.Name + ".Text", antiguoCulture);
                        if (nombreAntiguo != null)
                            c.Text = c.Text.Replace(nombreAntiguo, resources.GetString(c.Name + ".Text"));
                    }
                    else
                        c.Text = resources.GetString(c.Name + ".Text");
                }
                frm_colores.ReLocalize(nuevoCulture, antiguoCulture);
            }
        }
        
        private void comboBoxIdiomas_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ComboItemImagen item = comboBoxIdiomas.Items[e.Index] as ComboItemImagen;
                e.DrawBackground();
                if (item != null)
                {
                    if (item.ImageIndex >= 0 && item.ImageIndex < imageListIdiomas.Images.Count)
                        e.Graphics.DrawImage(imageListIdiomas.Images[item.ImageIndex], new PointF(e.Bounds.Left, e.Bounds.Top));
                    e.Graphics.DrawString(item.Etiqueta, e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.Left + imageListIdiomas.ImageSize.Width + 1, e.Bounds.Top));
                }
            }
        }

        private void bReportarBug_Click(object sender, EventArgs e)
        {
            ReporteBug reportar_bug = new ReporteBug();
            reportar_bug.ShowDialog();
        }

        public void PermitirPasarTurno()
        {
            this.bTurno.Enabled = true;
        }

        public void Juego_salvado(int id, String quien, String password)
        {
            MessageBox.Show(String.Format(Mensajes.mensajeJuegoSalvado, quien, id, password), Mensajes.tituloHotel);
        }

        public void Juego_no_salvado()
        {
            MessageBox.Show(Mensajes.mensajeJuegoNoSalvado, Mensajes.tituloHotel, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
