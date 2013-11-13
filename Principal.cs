using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Juego_Hotel.Resources;

namespace Juego_Hotel
{
    public partial class Principal : Form, IReLocalizable
    {
        public ComponentResourceManager resources = new ComponentResourceManager(typeof(Principal));
        public Juego juego;
        int[] tiradas_ini;
        readonly Sel_colores frm_colores = new Sel_colores();
        private readonly Image posRojo_orig, posAzul_orig, posVerde_orig, posAmarillo_orig;
        private readonly Image img_entrada, img_tick_Amarillo, img_tick_Azul, img_tick_Rojo, img_tick_Verde;
        public Boolean online;
        public Online frm_online;
        public int game_id;
        public String nombre_online;
        public String online_config;
        public Boolean partida_cargada_offline, partida_cargada_online, dado_tirado, partida_activa;
        readonly int ancho_ini;
        readonly int alto_ini;
        Point pos_rojo_orig, pos_azul_orig, pos_verde_orig, pos_amarillo_orig, pos_banco_orig, pos_ayto_orig;
        private const int ancho_coche = 20;
        private const int alto_coche = 20;
        private const int ancho_fase = 18;
        private const int alto_fase = 18;
        public Subastas frm_subasta_en_curso;
        public Hotel hotel_a_subastar_online;
        public int precio_minimo_subasta_online;
        readonly XmlDocument configuracion;
        public Actividad actividad;
        public String creador_online;

        public Principal(Online frm_online, XmlDocument configuracion)
        {
            InitializeComponent();
            juego = new Juego();
            alto_ini = imgTablero.Height;
            ancho_ini = imgTablero.Width;
            partida_cargada_offline = false;
            dado_tirado = false;
            posRojo.Parent = imgTablero;
            pos_rojo_orig = posRojo.Location;
            posRojo.Location = Calcular_Posicion(posRojo.Location.X, posRojo.Location.Y);
            posRojo_orig = (Image)posRojo.Image.Clone();
            posAzul.Parent = imgTablero;
            pos_azul_orig = posAzul.Location;
            posAzul.Location = Calcular_Posicion(posAzul.Location.X, posAzul.Location.Y);
            posAzul_orig = (Image)posAzul.Image.Clone();
            posVerde.Parent = imgTablero;
            pos_verde_orig = posVerde.Location;
            posVerde.Location = Calcular_Posicion(posVerde.Location.X, posVerde.Location.Y);
            posVerde_orig = (Image)posVerde.Image.Clone();
            posAmarillo.Parent = imgTablero;
            pos_amarillo_orig = posAmarillo.Location;
            posAmarillo.Location = Calcular_Posicion(posAmarillo.Location.X, posAmarillo.Location.Y);
            posAmarillo_orig = (Image)posAmarillo.Image.Clone();
            img_entrada = (Image) Properties.Resources.Entrada.Clone();
            img_tick_Amarillo = (Image)Properties.Resources.TickAmarillo.Clone();
            img_tick_Azul = (Image)Properties.Resources.TickAzul.Clone();
            img_tick_Rojo = (Image)Properties.Resources.TickRojo.Clone();
            img_tick_Verde = (Image)Properties.Resources.TickVerde.Clone();
            img_Banco.Parent = imgTablero;
            pos_banco_orig = img_Banco.Location;
            img_Banco.Location = Calcular_Posicion(img_Banco.Location.X, img_Banco.Location.Y);
            img_ayto.Parent = imgTablero;
            pos_ayto_orig = img_ayto.Location;
            img_ayto.Location = Calcular_Posicion(img_ayto.Location.X, img_ayto.Location.Y);
            this.configuracion = configuracion;
            if (frm_online != null)
            {
                online = true;
                this.frm_online = frm_online;
                actividad = new Actividad();
                partida_activa = true;
                var fichero_configuracion = new XmlDocument();
                fichero_configuracion.Load("Config.xml");
                XmlNode nodo_Idioma = fichero_configuracion.GetElementsByTagName("language")[0];
                if (nodo_Idioma.ChildNodes[0].FirstChild != null)
                {
                    if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("Spanish"))
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
                    else if (nodo_Idioma.ChildNodes[0].FirstChild.Value.Equals("English"))
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                }
            }
            else
            {
                online = false;
                this.frm_online = null;
                partida_activa = false;
                bCargar.Enabled = true;
            }
            // Rellenar combobox de idiomas
            comboBoxIdiomas.Items.Add(new ComboItemImagen(Mensajes.comboBoxIdiomas1, 0));
            comboBoxIdiomas.Items.Add(new ComboItemImagen(Mensajes.comboBoxIdiomas2, 1));
            comboBoxIdiomas.SelectedIndex = Thread.CurrentThread.CurrentUICulture.Name.Equals("es") ? 0 : 1;
        }

        private void bIniciar_Click(object sender, EventArgs e)
        {
            // Buscar número de jugadores
            if (juego.n_jugadores == 0)
            {
                MessageBox.Show(Mensajes.mensajeSelecJugadores, Mensajes.tituloSelecJugadores);
                return;
            }
            try
            {
                Crear_Jugadores();
            }
            catch
            {
                return; //Se trata en la función Crear_Jugadores
            }
            if ((!online) && (!partida_cargada_offline)) // Nos viene dado del servidor o por la partida cargada
            {
                // Decidir quien empieza
                tiradas_ini = new int[juego.n_jugadores];
                int i;
                for (i = 0; i < juego.n_jugadores; i++)
                {
                    tiradas_ini[i] = juego.dado.tirar();
                }
                // Encontrando el mayor
                int max = 0;
                for (i = 0; i < juego.n_jugadores; i++)
                {
                    if (tiradas_ini[i] <= max)
                        continue;
                    max = tiradas_ini[i];
                    juego.jug_inicial = i;
                }
                juego.jug_inicial++; // Para no comenzar en 0
            }
            jug_ini.Text = resources.GetString("jug_ini.Text") + Environment.NewLine + Environment.NewLine + juego.jug_inicial;
            colorJugIni.Text = juego.jugadores[juego.jug_inicial - 1].Nombre_color();
            juego.jug_actual = juego.jug_inicial;
            juego.Cambiar_jugador_actual();
            Establecer_Turno();
            bIniciar.Enabled = false;
            bColores.Enabled = false;
            bComprarSuelo.Enabled = false;
            grupoNJugadores.Enabled = false;
            bCobrarBanca.Enabled = false;
            bSalvar.Enabled = true;
            posRojo.Visible = true;
            posAzul.Visible = true;
            posVerde.Visible = true;
            posAmarillo.Visible = true;
            img_Banco.Visible = true;
            img_ayto.Visible = true;
            posJ1.Text = resources.GetString("posJ1.Text") + @"0 (" + Mensajes.TipoCasillaSalida + @")";
            posJ2.Text = resources.GetString("posJ2.Text") + @"0 (" + Mensajes.TipoCasillaSalida + @")";
            posJ3.Text = resources.GetString("posJ3.Text") + @"0 (" + Mensajes.TipoCasillaSalida + @")";
            posJ4.Text = resources.GetString("posJ4.Text") + @"0 (" + Mensajes.TipoCasillaSalida + @")";
            bEntradasJ1.Enabled = false;
            bEntradasJ2.Enabled = false;
            bEntradasJ3.Enabled = false;
            bEntradasJ4.Enabled = false;
            bRetirarseJ1.Enabled = false;
            bRetirarseJ2.Enabled = false;
            bRetirarseJ3.Enabled = false;
            bRetirarseJ4.Enabled = false;
            switch (juego.jugador_actual.n_jugador)
            {
                case 0: bRetirarseJ1.Enabled = true;
                    break;
                case 1: bRetirarseJ2.Enabled = true;
                    break;
                case 2: bRetirarseJ3.Enabled = true;
                    break;
                case 3: bRetirarseJ4.Enabled = true;
                    break;
            }
            colorJ1.Text = resources.GetString("colorJ1.Text") + juego.jugadores[0].Nombre_color();
            colorJ2.Text = resources.GetString("colorJ2.Text") + juego.jugadores[1].Nombre_color();
            dineroJ1.Text = resources.GetString("dineroJ1.Text") + juego.jugadores[0].dinero_total;
            dineroJ2.Text = resources.GetString("dineroJ2.Text") + juego.jugadores[1].dinero_total;
            if (juego.n_jugadores > 2)
            {
                dineroJ3.Text = resources.GetString("dineroJ3.Text") + juego.jugadores[2].dinero_total;
                colorJ3.Text = resources.GetString("colorJ3.Text") + juego.jugadores[2].Nombre_color();
            }
            if (juego.n_jugadores > 3)
            {
                dineroJ4.Text = resources.GetString("dineroJ4.Text") + juego.jugadores[3].dinero_total;
                colorJ4.Text = resources.GetString("colorJ4.Text") + juego.jugadores[3].Nombre_color();
            }
            if (!online)
            {
                bDado.Enabled = true;
                bReiniciar.Enabled = true;
            }
            else
            {
                bDado.Enabled = false;
                bVerHoteles.Focus();
                controlJ1.Enabled = false;
                controlJ2.Enabled = false;
                controlJ3.Enabled = false;
                controlJ4.Enabled = false;
                Jugador yo = juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_online);
                if (yo != null)
                {
                    switch (yo.n_jugador)
                    {
                        case 0: controlJ1.Enabled = true;
                            break;
                        case 1: controlJ2.Enabled = true;
                            break;
                        case 2: controlJ3.Enabled = true;
                            break;
                        case 3: controlJ4.Enabled = true;
                            break;
                    }
                    if (yo.nombre_online != creador_online)
                    {
                        bSalvar.Enabled = false;
                        bReiniciar.Enabled = false;
                    }
                }
            }
            if ((!online) || (nombre_online == juego.jugadores[juego.jug_inicial - 1].nombre_online))
            {
                bDado.Enabled = true;
                bDado.Focus();
            }
            if (!partida_cargada_offline && !partida_cargada_online)
                return;
            resDado.Text = resources.GetString("resDado.Text") + juego.ultimo_res_dado;
            foreach (Jugador jugador in juego.jugadores)
            {
                if (jugador.posicion.numero == 0)
                    continue;
                Point pos = Calcular_Posicion(juego.casillas[jugador.posicion.numero].pos_coche.X, juego.casillas[jugador.posicion.numero].pos_coche.Y);
                switch (jugador.color)
                {
                    case Tipos.Tcolor.rojo:     posRojo.Image = RotateImage(posRojo_orig, jugador.posicion.pos_coche.grados);
                        posRojo.Location = pos;
                        posRojo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                        break;
                    case Tipos.Tcolor.azul:     posAzul.Image = RotateImage(posAzul_orig, jugador.posicion.pos_coche.grados);
                        posAzul.Location = pos;
                        posAzul.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                        break;
                    case Tipos.Tcolor.verde:    posVerde.Image = RotateImage(posVerde_orig, jugador.posicion.pos_coche.grados);
                        posVerde.Location = pos;
                        posVerde.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                        break;
                    case Tipos.Tcolor.amarillo: posAmarillo.Image = RotateImage(posAmarillo_orig, jugador.posicion.pos_coche.grados);
                        posAmarillo.Location = pos;
                        posAmarillo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                        break;
                }
            }
            // Poner casilla actual a cada uno
            switch (juego.n_jugadores)
            {
                case 4: posJ4.Text = resources.GetString("posJ4.Text") + juego.jugadores[3].posicion.numero;
                    goto case 3;
                case 3: posJ3.Text = resources.GetString("posJ3.Text") + juego.jugadores[2].posicion.numero;
                    goto case 2;
                case 2: posJ2.Text = resources.GetString("posJ2.Text") + juego.jugadores[1].posicion.numero;
                    posJ1.Text = resources.GetString("posJ1.Text") + juego.jugadores[0].posicion.numero;
                    break;
            }
            Actualizar_Dinero_Jugadores();
            // Cargar el estado de los hoteles si es partida online
            if (partida_cargada_online)
            {
                foreach (String estado_hotel in juego.estado_hoteles_online)
                {
                    String[] estado = estado_hotel.Split(';');
                    Hotel hotel = juego.hoteles.First(x => x.nombre_txt == estado[0]);
                    if (estado[1] != "0")
                    {
                        hotel.dueño = juego.jugadores.FirstOrDefault(x => x.n_jugador == Convert.ToInt16(estado[1]) - 1);
                        hotel.dueño.hoteles.AddLast(hotel);
                    }
                    hotel.n_fases_construidas = Convert.ToInt16(estado[2]);
                    hotel.suelo_comprado = (hotel.n_fases_construidas == hotel.n_fases_max);
                    hotel.entrada_comprada_ultimo_turno = (estado[3] == "1");
                    hotel.entrada_comprada_ultimo_turno = (estado[4] == "1");
                    if (estado[5] == "")
                        continue;
                    String[] lista_entradas = estado[5].Split('@');
                    hotel.n_entradas = lista_entradas.Length;
                    foreach (short num_casilla in lista_entradas.Select(s_num_casilla => Convert.ToInt16(s_num_casilla)))
                    {
                        if (juego.casillas[num_casilla].hotel_der == hotel.nombre)
                        {
                            juego.casillas[num_casilla].entrada_en_der = true;
                            Dibujar_Entrada(juego.casillas[num_casilla], true);
                        }
                        else if (juego.casillas[num_casilla].hotel_izq == hotel.nombre)
                        {
                            juego.casillas[num_casilla].entrada_en_izq = true;
                            Dibujar_Entrada(juego.casillas[num_casilla], false);
                        }
                        hotel.entradas.AddLast(juego.casillas[num_casilla]);
                    }
                }
            }
            // Dibujar todas las fases ya hechas
            foreach (Hotel hotel in juego.hoteles)
            {
                for (int pos_fase_hotel = 0; pos_fase_hotel < hotel.n_fases_construidas; pos_fase_hotel++)
                {
                    Dibujar_Fase(hotel, pos_fase_hotel);
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
            if (Equals(angleDegrees, 0f))
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
            var newBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
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

                if (!Equals(scaleFactor, 1f))
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
            ImagenTurnoJ1.Visible = false;
            ImagenTurnoJ2.Visible = false;
            ImagenTurnoJ3.Visible = false;
            ImagenTurnoJ4.Visible = false;
            bEntradasJ1.Enabled = false;
            bEntradasJ2.Enabled = false;
            bEntradasJ3.Enabled = false;
            bEntradasJ4.Enabled = false;
            switch (juego.jug_actual)
            {
                case 1: ImagenTurnoJ1.Visible = true;
                        break;
                case 2: ImagenTurnoJ2.Visible = true;
                        break;
                case 3: ImagenTurnoJ3.Visible = true;
                        break;
                case 4: ImagenTurnoJ4.Visible = true;
                        break;
            }
        }

        public Boolean Todos_Eliminados()
        {
            return juego.n_jugadores_activos == 1;
        }

        public void Conexion_perdida()
        {
            foreach (Button control in Controls.OfType<Button>())
            {
                control.Enabled = false;
            }
            bReiniciar.Enabled = false;
            bSalir.Enabled = true;
            controlJ1.Enabled = false;
            controlJ2.Enabled = false;
            controlJ3.Enabled = false;
            controlJ4.Enabled = false;
            partida_activa = false;
        }

        public void Finalizar_Partida(Jugador ganador)
        {
            foreach (Button control in Controls.OfType<Button>())
            {
                control.Enabled = false;
            }
            if (!online)
                bReiniciar.Enabled = true;
            bSalir.Enabled = true;
            controlJ1.Enabled = false;
            controlJ2.Enabled = false;
            controlJ3.Enabled = false;
            controlJ4.Enabled = false;
            partida_activa = false;
            if (ganador != null)
                MessageBox.Show(String.Format(Mensajes.mensajePartidaFinalizada, ganador.Nombre_color()), Mensajes.tituloHotel);
            else
                MessageBox.Show(Mensajes.mensajeJugadoresRetidados);
        }

        public int Sig_jugador_Activo()
        {
            int sig_jugador = juego.jug_actual;
            Boolean valido = false;

            while (!valido)
            {
                if (sig_jugador < juego.n_jugadores)
                    sig_jugador = sig_jugador + 1;
                else
                    sig_jugador = 1;
                if (juego.jugadores[sig_jugador-1].Eliminado() == false)
                    valido = true;
            }

            return sig_jugador;
        }

        public void Pasar_Turno(String sig_jugador, Boolean automaticamente = false)
        {
            if (Todos_Eliminados())
                Finalizar_Partida(juego.jugadores[Sig_jugador_Activo()-1]);
            else if (juego.n_jugadores_activos == 0)
                Finalizar_Partida(null);
            else
            {
                if (!online)
                    juego.jug_actual = Sig_jugador_Activo();
                else
                {
                    Jugador jugador = juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == sig_jugador);
                    if (jugador != null)
                        juego.jug_actual = jugador.n_jugador + 1;
                }
                Establecer_Turno();
                juego.Cambiar_jugador_actual();
                if ((!online) || (nombre_online == juego.jugador_actual.nombre_online))
                {
                    bDado.Enabled = true;
                    bDado.Focus();
                }
                else
                {
                    bDado.Enabled = false;
                    bVerHoteles.Focus();
                }
                bComprar.Enabled = false;
                bConstruir.Enabled = false;
                bComprarSuelo.Enabled = false;
                bCobrarBanca.Enabled = false;
                bRetirarseJ1.Enabled = false;
                bRetirarseJ2.Enabled = false;
                bRetirarseJ3.Enabled = false;
                bRetirarseJ4.Enabled = false;
                switch (juego.jugador_actual.n_jugador)
                {
                    case 0: bRetirarseJ1.Enabled = true;
                        break;
                    case 1: bRetirarseJ2.Enabled = true;
                        break;
                    case 2: bRetirarseJ3.Enabled = true;
                        break;
                    case 3: bRetirarseJ4.Enabled = true;
                        break;
                }
                juego.jugador_actual.pago_ultimo_turno = false;
                dado_tirado = false;
                SystemSounds.Beep.Play();
                if (online && Application.OpenForms.Cast<Form>().Contains(actividad))
                    actividad.PasarTurno(juego.jugador_actual.nombre_online, juego.jugador_actual.Nombre_color(), automaticamente);//Actualizando datos de la actividad
                if (automaticamente)
                    bTurno.Enabled = false;
            }
            // Cerrar las ventanas que se pudieran quedar abiertas al ser un paso automático
            if (!automaticamente)
                return;
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                if ((form is Principal) || (form is Online) || (form is Chat) || (form is PartidaOnline) || (form is Actividad)
                    || (form is Reglas) || (form is ReporteBug) || (form is VerHoteles) || (form is Subastas))
                    continue;
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

        private static Image ObtenerImagenDesdeColor(Tipos.Tcolor color)
        {
            switch (color)
            {
                case Tipos.Tcolor.amarillo: return (Image)Properties.Resources.TurnoAmarillo.Clone();
                case Tipos.Tcolor.azul: return (Image)Properties.Resources.TurnoAzul.Clone();
                case Tipos.Tcolor.rojo: return (Image)Properties.Resources.TurnoRojo.Clone();
                case Tipos.Tcolor.verde: return (Image)Properties.Resources.TurnoVerde.Clone();
                default: return null;
            }
        }

        public void Crear_Jugadores()
        {
            if (!partida_cargada_offline)
            {
                juego.jugadores = new Jugador[juego.n_jugadores];
                juego.n_jugadores_activos = juego.n_jugadores;
                var configuracion_online = new XmlDocument();
                XmlNode config_dinero;
                try
                {
                    if (online)
                    {
                        configuracion_online.LoadXml(online_config);
                        config_dinero = configuracion_online.GetElementsByTagName("money_per_player")[0];
                    }
                    else
                        config_dinero = configuracion.GetElementsByTagName("money_per_player")[0];
                }
                catch
                {
                    if (!online)
                    {
                        MessageBox.Show(Mensajes.mensajeErrorCargarConfig, Mensajes.tituloError);
                        juego.jugadores = null;
                        throw;
                    }
                    MessageBox.Show(Mensajes.mensajeErrorCargarConfigServidor, Mensajes.tituloError);
                    juego.jugadores = null;
                    throw;
                }
                XmlNode nodo_cantidades = juego.n_jugadores == 2
                    ? ((XmlElement)config_dinero).GetElementsByTagName("two_players")[0]
                    : ((XmlElement)config_dinero).GetElementsByTagName("three_or_four_players")[0];
                int n_5000 = Convert.ToInt16(nodo_cantidades.ChildNodes[0].FirstChild.Value);
                int n_1000 = Convert.ToInt16(nodo_cantidades.ChildNodes[1].FirstChild.Value);
                int n_500 = Convert.ToInt16(nodo_cantidades.ChildNodes[2].FirstChild.Value);
                int n_100 = Convert.ToInt16(nodo_cantidades.ChildNodes[3].FirstChild.Value);
                int n_50 = Convert.ToInt16(nodo_cantidades.ChildNodes[4].FirstChild.Value);
                switch (juego.n_jugadores)
                {
                    case 4: if (partida_cargada_online)
                                juego.jugadores[3] = new Jugador(juego.lista_jugadores_online[3].Item1, frm_colores.color_j4, juego.lista_jugadores_online[3].Item2);
                            else
                            {
                                juego.jugadores[3] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, frm_colores.color_j4, 3);
                                if (online)
                                    juego.jugadores[3].nombre_online = juego.lista_jugadores_online[3].Item1;
                            }
                            controlJ4.Enabled = true;
                            ImagenTurnoJ4.Image = ObtenerImagenDesdeColor(juego.jugadores[3].color);
                            goto case 3;
                    case 3: if (partida_cargada_online)
                                juego.jugadores[2] = new Jugador(juego.lista_jugadores_online[2].Item1, frm_colores.color_j3, juego.lista_jugadores_online[2].Item2);
                            else
                            {
                                juego.jugadores[2] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, frm_colores.color_j3, 2);
                                if (online)
                                    juego.jugadores[2].nombre_online = juego.lista_jugadores_online[2].Item1;
                            }
                            controlJ3.Enabled = true;
                            ImagenTurnoJ3.Image = ObtenerImagenDesdeColor(juego.jugadores[2].color);
                            goto case 2;
                    case 2: if (partida_cargada_online)
                            {
                                juego.jugadores[1] = new Jugador(juego.lista_jugadores_online[1].Item1, frm_colores.color_j2, juego.lista_jugadores_online[1].Item2);
                                juego.jugadores[0] = new Jugador(juego.lista_jugadores_online[0].Item1, frm_colores.color_j1, juego.lista_jugadores_online[0].Item2);
                            }
                            else
                            {
                                juego.jugadores[1] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, frm_colores.color_j2, 1);
                                juego.jugadores[0] = new Jugador(n_5000, n_1000, n_500, n_100, n_50, frm_colores.color_j1, 0);
                                if (online)
                                {
                                    juego.jugadores[1].nombre_online = juego.lista_jugadores_online[1].Item1;
                                    juego.jugadores[0].nombre_online = juego.lista_jugadores_online[0].Item1;
                                }
                            }
                            controlJ2.Enabled = true;
                            controlJ1.Enabled = true;
                            ImagenTurnoJ2.Image = ObtenerImagenDesdeColor(juego.jugadores[1].color);
                            ImagenTurnoJ1.Image = ObtenerImagenDesdeColor(juego.jugadores[0].color);
                            break;
                }
            }
            else
            {
                switch (juego.n_jugadores)
                {
                    case 4: controlJ4.Enabled = true;
                            goto case 3;
                    case 3: controlJ3.Enabled = true;
                            goto case 2;
                    case 2: controlJ2.Enabled = true;
                            controlJ1.Enabled = true;
                            break;
                }
            }
        }

        public void Tirar_Dado(Jugador jugador, Boolean automaticamente = false)
        {
            if (!online)
            {
                juego.ultimo_res_dado = juego.dado.tirar();
                jugador.posicion.ocupada = false; // Desocupamos la casilla
                jugador.posicion = (jugador.posicion.numero + juego.ultimo_res_dado) <= 31
                    ? juego.casillas[jugador.posicion.numero + juego.ultimo_res_dado]
                    : juego.casillas[jugador.posicion.numero + juego.ultimo_res_dado - 31];
                juego.ultimo_avance_auto = 0;
                while (jugador.posicion.ocupada) // Hay que avanzar una porque está ocupada
                {
                    jugador.posicion = jugador.posicion.numero < 31
                        ? juego.casillas[jugador.posicion.numero + 1]
                        : juego.casillas[1];
                    juego.ultimo_avance_auto++;
                }
                jugador.posicion.ocupada = true; // Ocupamos la casilla
            }
            if (online && Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.DadoTirado(jugador, juego.ultimo_res_dado, automaticamente);//Actualizando datos de la actividad
            resDado.Text = resources.GetString("resDado.Text") + juego.ultimo_res_dado;
            // Pintamos el coche en su lugar
            Point posicion = Calcular_Posicion(juego.casillas[juego.jugador_actual.posicion.numero].pos_coche.X, juego.casillas[juego.jugador_actual.posicion.numero].pos_coche.Y);
            switch (jugador.color)
            {
                case Tipos.Tcolor.rojo:     posRojo.Image = RotateImage(posRojo_orig, jugador.posicion.pos_coche.grados);
                                            posRojo.Location = posicion;
                                            break;
                case Tipos.Tcolor.azul:     posAzul.Image = RotateImage(posAzul_orig, jugador.posicion.pos_coche.grados);
                                            posAzul.Location = posicion;
                                            break;
                case Tipos.Tcolor.verde:    posVerde.Image = RotateImage(posVerde_orig, jugador.posicion.pos_coche.grados);
                                            posVerde.Location = posicion;
                                            break;
                case Tipos.Tcolor.amarillo: posAmarillo.Image = RotateImage(posAmarillo_orig, jugador.posicion.pos_coche.grados);
                                            posAmarillo.Location = posicion;
                                            break;
            }
            // Poner casilla actual a cada uno
            switch (juego.jug_actual)
            {
                case 1: posJ1.Text = resources.GetString("posJ1.Text") + jugador.posicion.numero + @" (" + jugador.posicion.ObtenerTipoTxt() + @")";
                    break;
                case 2: posJ2.Text = resources.GetString("posJ2.Text") + jugador.posicion.numero + @" (" + jugador.posicion.ObtenerTipoTxt() + @")";
                    break;
                case 3: posJ3.Text = resources.GetString("posJ3.Text") + jugador.posicion.numero + @" (" + jugador.posicion.ObtenerTipoTxt() + @")";
                    break;
                case 4: posJ4.Text = resources.GetString("posJ4.Text") + jugador.posicion.numero + @" (" + jugador.posicion.ObtenerTipoTxt() + @")";
                    break;
            }
            // Activar botones según el tipo de casilla
            bPedirNochesJ1.Enabled = true;
            bPedirNochesJ2.Enabled = true;
            bPedirNochesJ3.Enabled = true;
            bPedirNochesJ4.Enabled = true;
            switch (juego.jugador_actual.n_jugador)
            {
                case 0: bPedirNochesJ1.Enabled = false;
                    break;
                case 1: bPedirNochesJ2.Enabled = false;
                    break;
                case 2: bPedirNochesJ3.Enabled = false;
                    break;
                case 3: bPedirNochesJ4.Enabled = false;
                    break;
            }
            if ((online) && (nombre_online != jugador.nombre_online))
                return;
            jugador.entrada_gratis_usada = false;
            foreach (Hotel hotel in juego.hoteles)
                hotel.entrada_comprada_ultimo_turno = false;
            bComprarSuelo.Enabled = true;
            bEntradasJ1.Enabled = false;
            bEntradasJ2.Enabled = false;
            bEntradasJ3.Enabled = false;
            bEntradasJ4.Enabled = false;
            if (Puede_poner_entradas(juego.jugador_actual))
                Activar_Poner_Entradas(juego.jug_actual);
            bCobrarBanca.Enabled = Puede_Cobrar_Banca(juego.jug_actual);
            switch (jugador.posicion.tipo)
            {
                case Tipos.Tcasilla.comprar: bComprar.Enabled = true;
                    bConstruir.Enabled = false;
                    break;
                case Tipos.Tcasilla.construir: bConstruir.Enabled = true;
                    bComprar.Enabled = false;
                    break;
                case Tipos.Tcasilla.fase_gratis: bConstruir.Enabled = true;
                    bComprar.Enabled = false;
                    if (!automaticamente)
                        MessageBox.Show(Mensajes.mensajeCasillaFaseGratis);
                    break;
                case Tipos.Tcasilla.entrada_gratis: bConstruir.Enabled = false;
                    bComprar.Enabled = false;
                    Activar_Poner_Entradas(juego.jug_actual);
                    if (!automaticamente)
                        MessageBox.Show(Mensajes.mensajeCasillaEntradaGratis);
                    break;
                default: bConstruir.Enabled = false;
                    bComprar.Enabled = false;
                    break;
            }
            bTurno.Enabled = true;
            bTurno.Focus();
            if (juego.ultimo_res_dado == 6)
            {
                if (!automaticamente)
                    MessageBox.Show(Mensajes.mensajeSacadoUnSeis);
                bDado.Enabled = true;
                bDado.Focus();
            }
            dado_tirado = true;
        }

        private void bDado_Click(object sender, EventArgs e)
        {
            bDado.Enabled = false;
            if (online)
                frm_online.enviar_comando("roll_dice", game_id.ToString());
            else
                Tirar_Dado(juego.jugador_actual);
        }

        private void Activar_Poner_Entradas(int num_jugador)
        {
            switch (num_jugador)
            {
                case 1: bEntradasJ1.Enabled = true;
                        break;
                case 2: bEntradasJ2.Enabled = true;
                        break;
                case 3: bEntradasJ3.Enabled = true;
                        break;
                case 4: bEntradasJ4.Enabled = true;
                        break;
            }
        }

        private void bReiniciar_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(Mensajes.mensajeReiniciarPartida, Mensajes.tituloHotel, MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                Reiniciar_partida();
        }

        public void Reiniciar_partida()
        {
            bTurno.Enabled = false;
            bDado.Enabled = false;
            bIniciar.Enabled = true;
            bColores.Enabled = true;
            grupoNJugadores.Enabled = true;
            bComprar.Enabled = false;
            bConstruir.Enabled = false;
            bComprarSuelo.Enabled = false;
            controlJ1.Enabled = false;
            controlJ2.Enabled = false;
            controlJ3.Enabled = false;
            controlJ4.Enabled = false;
            bCobrarBanca.Enabled = false;
            bVerHoteles.Enabled = true;
            bNormas.Enabled = true;
            bReportarBug.Enabled = true;
            jug_ini.Text = resources.GetString("jug_ini.Text");
            colorJugIni.Text = "";
            posJ1.Text = resources.GetString("posJ1.Text");
            posJ2.Text = resources.GetString("posJ2.Text");
            posJ3.Text = resources.GetString("posJ3.Text");
            posJ4.Text = resources.GetString("posJ4.Text");
            dineroJ1.Text = resources.GetString("dineroJ1.Text");
            dineroJ2.Text = resources.GetString("dineroJ2.Text");
            dineroJ3.Text = resources.GetString("dineroJ3.Text");
            dineroJ4.Text = resources.GetString("dineroJ4.Text");
            colorJ1.Text = resources.GetString("colorJ1.Text");
            colorJ2.Text = resources.GetString("colorJ2.Text");
            colorJ3.Text = resources.GetString("colorJ3.Text");
            colorJ4.Text = resources.GetString("colorJ4.Text");
            juego.jugadores = null;
            posRojo.Image = posRojo_orig;
            posAzul.Image = posAzul_orig;
            posAmarillo.Image = posAmarillo_orig;
            posVerde.Image = posVerde_orig;
            posRojo.Location = Calcular_Posicion(pos_rojo_orig.X, pos_rojo_orig.Y);
            posAzul.Location = Calcular_Posicion(pos_azul_orig.X, pos_azul_orig.Y);
            posVerde.Location = Calcular_Posicion(pos_verde_orig.X, pos_verde_orig.Y);
            posAmarillo.Location = Calcular_Posicion(pos_amarillo_orig.X, pos_amarillo_orig.Y);
            Control[] lista_entradas = Controls.Find("entrada", true);
            foreach (Control entrada in lista_entradas)
            {
                Controls.Remove(entrada);
                entrada.Dispose();
            }
            Control[] lista_fases = Controls.Find("fase", true);
            foreach (Control fase in lista_fases)
            {
                Controls.Remove(fase);
                fase.Dispose();
            }
            // Redibujar tablero para quitar los suelos
            imgTablero.Image = Properties.Resources.Tablero;
            int num_jugadores = juego.n_jugadores;
            juego = new Juego {n_jugadores = num_jugadores};
            partida_cargada_offline = false;
            partida_cargada_online = false;
        }

        private void bTurno_Click(object sender, EventArgs e)
        {
            bTurno.Enabled = false;
            if (online)
                frm_online.enviar_comando("turn_pass", game_id.ToString());
            else
                Pasar_Turno(null);
        }

        private void bColores_Click(object sender, EventArgs e)
        {
            if (juego.n_jugadores == 0)
                MessageBox.Show(Mensajes.mensajeNoIndicadoJugadores, Mensajes.tituloNoIndicadoJugadores);
            else
            {
                frm_colores.habilitarControles(juego.n_jugadores);
                frm_colores.ShowDialog();
            }
        }

        private void dos_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            juego.n_jugadores = 2;
        }

        private void tres_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            juego.n_jugadores = 3;
        }

        private void cuatro_jugadores_CheckedChanged(object sender, EventArgs e)
        {
            juego.n_jugadores = 4;
        }

        private void bComprar_Click(object sender, EventArgs e)
        {
            var frm_comprar_hotel = new ComprarHotel();
            Jugador jugador = juego.jugadores[juego.jug_actual - 1];
            Tipos.Tnombre_hotel nombre_izq = juego.jugadores[juego.jug_actual - 1].posicion.hotel_izq;
            Tipos.Tnombre_hotel nombre_der = juego.jugadores[juego.jug_actual - 1].posicion.hotel_der;
            frm_comprar_hotel.HabilitarControles(juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre == nombre_izq), juego.hoteles.FirstOrDefault(Hotel => Hotel.nombre == nombre_der));
            frm_comprar_hotel.ShowDialog();
            // Ya ha sido seleccionado cual comprar. Haciendo efectiva la compra
            if (!frm_comprar_hotel.cancelado)
            {
                Hotel hotel = frm_comprar_hotel.comprado_izq
                    ? juego.hoteles[(int) nombre_izq]
                    : juego.hoteles[(int) nombre_der];

                if (hotel.dueño != null) // El hotel tiene dueño
                {
                    if (hotel.dueño != jugador)
                    {
                        if (hotel.n_fases_construidas == 0) // Se puede expropiar
                        {
                            DialogResult dr = MessageBox.Show(String.Format(Mensajes.mensajeExpropiacionPosible, hotel.nombre, hotel.dueño.Nombre_color()), Mensajes.tituloExpropiacionPosible, MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                if (hotel.precio_expropiacion > juego.jugador_actual.dinero_total)
                                    MessageBox.Show(Mensajes.mensajeSinFondosParaExpropiacion);
                                else
                                    Comprar_hotel(ref hotel, ref jugador, true, false);
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
                    if ((frm_comprar_hotel.comprado_con_todo ? hotel.Calcular_precio_con_todo() : hotel.precio) > jugador.dinero_total)
                        MessageBox.Show(String.Format(Mensajes.mensajeFondosInsuficientesParaComprarHotel, hotel.nombre_txt));
                    else
                    {
                        Comprar_hotel(ref hotel, ref jugador, false, frm_comprar_hotel.comprado_con_todo);
                    }
                }
            }
            frm_comprar_hotel.Close();
        }

        void Comprar_hotel (ref Hotel hotel, ref Jugador jugador, Boolean expropiando, Boolean comprar_con_todo)
        {
            // En caso de que el turno se pase automáticamente y quede algun messagebox abierto, no hacer nada
            if (juego.jugador_actual != jugador)
                return;
            int dinero_necesario;

            if (expropiando)
                dinero_necesario = hotel.precio_expropiacion;
            else
            {
                if (comprar_con_todo)
                    dinero_necesario = hotel.Calcular_precio_con_todo();
                else
                {
                    hotel.Limpiar_fases_y_entradas();
                    dinero_necesario = hotel.precio;
                }
            }

            var frm_pago = new PedirPago(dinero_necesario, ref juego, juego.jugador_actual, this, null);
            frm_pago.ShowDialog();
            if (!frm_pago.cancelado)
            {
                int n_5000;
                int n_1000;
                int n_500;
                int n_100;
                int n_50;
                if (expropiando)
                {
                    if (!online)
                    {
                        Jugador dueño_ant = hotel.dueño;
                        dueño_ant.Hotel_Expropiado(ref hotel);
                        jugador.Comprar_Hotel(ref hotel, ref dueño_ant, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
                        if (frm_pago.total_seleccionado > dinero_necesario)
                        {
                            Calcular_Devolucion(ref dueño_ant, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
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
                        frm_online.enviar_comando("expropriate_hotel", game_id.ToString(), hotel.nombre_txt, n_5000.ToString(),
                             n_1000.ToString(), n_500.ToString(), n_100.ToString(), n_50.ToString());
                    }
                }
                else
                {
                    if (!online)
                    {
                        jugador.Comprar_Hotel(ref hotel, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        // Hay que calcular el dinero a devolver y restarlo de la llamada a Comprar_Hotel
                        if (frm_pago.total_seleccionado > dinero_necesario)
                        {
                            Calcular_Devolucion((frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            jugador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        // Dibujar las fases y entradas que ya tuviera
                        if (comprar_con_todo)
                        {
                            Dibujar_Fases_y_Entradas(hotel);
                        }
                    }
                    else // Todo se hace en el lado del servidor, devolución incluída
                    {
                        n_5000 = frm_pago.n_5000;
                        n_1000 = frm_pago.n_1000;
                        n_500 = frm_pago.n_500;
                        n_100 = frm_pago.n_100;
                        n_50 = frm_pago.n_50;
                        frm_online.enviar_comando("buy_hotel", game_id.ToString(), hotel.nombre_txt, n_5000.ToString(),
                            n_1000.ToString(), n_500.ToString(), n_100.ToString(), n_50.ToString(), (comprar_con_todo ? "1" : "0"));
                    }
                }
            }
            frm_pago.Close();
            bComprar.Enabled = false;
            bComprarSuelo.Enabled = false;
            Actualizar_Dinero_Jugadores();
        }

        public void Dibujar_Fases_y_Entradas(Hotel hotel)
        {
            for (int i = 0; i < hotel.n_fases_construidas; i++)
            {
                Dibujar_Fase(hotel, i);
            }
            foreach (Casilla casilla in hotel.entradas)
            {
                Dibujar_Entrada(casilla, casilla.entrada_en_der);
            }
        }

        public void Actualizar_Fases_Nuevo_Dueño_Hotel(Hotel hotel)
        {
            Image imagen_fase = null;
            switch (hotel.dueño.color)
            {
                case Tipos.Tcolor.amarillo: imagen_fase = (Image)img_tick_Amarillo.Clone();
                    break;
                case Tipos.Tcolor.azul: imagen_fase = (Image)img_tick_Azul.Clone();
                    break;
                case Tipos.Tcolor.rojo: imagen_fase = (Image)img_tick_Rojo.Clone();
                    break;
                case Tipos.Tcolor.verde: imagen_fase = (Image)img_tick_Verde.Clone();
                    break;
            }
            List<Control> lista_fases = Controls.Find("fase", true).Where(f =>
            {
                var tuple = f.Tag as Tuple<int, int, Hotel>;
                return tuple != null && tuple.Item3 == hotel;
            }).ToList();
            foreach (var pictureBox in lista_fases.OfType<PictureBox>())
            {
                pictureBox.Image = imagen_fase;
            }
        }

        public void Hotel_Comprado(Hotel hotel, Jugador jugador)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.ComprarHotel(jugador.nombre_online, jugador.Nombre_color(), hotel.nombre_txt); //Actualizando datos de la actividad
        }

        public void Hotel_Expropiado(Jugador jugador,Jugador expropiado, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Expropiar_Hotel(jugador, expropiado, hotel);
        }

        public void Tirar_Dado_Construccion(Jugador jugador, Tipos.Resultado_dado_cons resultado)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Tirar_Dado_Construccion(jugador, resultado);
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

            while (cantidad > 0)
            {
                int n_1000_;
                int n_500_;
                int n_100_;
                int n_50_;
                if (cantidad >= 5000)
                {
                    if (jugador.n_billetes_5000 > 0)
                    {
                        jugador.n_billetes_5000--;
                        n_5000++;
                    }
                    else
                    {
                        int n_5000_;
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
            Jugador jugador = juego.jugadores.FirstOrDefault(Jugador => Jugador.nombre_online == nombre_jugador);
            if (jugador == null)
                return;
            jugador.n_billetes_50 = n_50;
            jugador.n_billetes_100 = n_100;
            jugador.n_billetes_500 = n_500;
            jugador.n_billetes_1000 = n_1000;
            jugador.n_billetes_5000 = n_5000;
            jugador.calcular_dinero_total();
            switch (jugador.n_jugador)
            {
                case 0: dineroJ1.Text = resources.GetString("dineroJ1.Text") + jugador.dinero_total;
                    break;
                case 1: dineroJ2.Text = resources.GetString("dineroJ2.Text") + jugador.dinero_total;
                    break;
                case 2: dineroJ3.Text = resources.GetString("dineroJ3.Text") + jugador.dinero_total;
                    break;
                case 3: dineroJ4.Text = resources.GetString("dineroJ4.Text") + jugador.dinero_total;
                    break;
            }
        }

        public void Actualizar_Dinero_Jugador_Actual()
        {
            switch (juego.jug_actual)
            {
                case 1: dineroJ1.Text = resources.GetString("dineroJ1.Text") + juego.jugador_actual.dinero_total;
                        break;
                case 2: dineroJ2.Text = resources.GetString("dineroJ2.Text") + juego.jugador_actual.dinero_total;
                        break;
                case 3: dineroJ3.Text = resources.GetString("dineroJ3.Text") + juego.jugador_actual.dinero_total;
                        break;
                case 4: dineroJ4.Text = resources.GetString("dineroJ4.Text") + juego.jugador_actual.dinero_total;
                        break;
            }
        }

        public void Actualizar_Dinero_Jugadores()
        {
            switch (juego.n_jugadores)
            {
                case 4: dineroJ4.Text = resources.GetString("dineroJ1.Text") + juego.jugadores[3].dinero_total;
                        goto case 3;
                case 3: dineroJ3.Text = resources.GetString("dineroJ2.Text") + juego.jugadores[2].dinero_total;
                        goto case 2;
                case 2: dineroJ2.Text = resources.GetString("dineroJ3.Text") + juego.jugadores[1].dinero_total;
                        dineroJ1.Text = resources.GetString("dineroJ4.Text") + juego.jugadores[0].dinero_total;
                        break;
            }
        }

        private void bConstruir_Click(object sender, EventArgs e)
        {
            if (juego.jugador_actual.hoteles.Count == 0)
            {
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles);
                return;
            }
            var frm_construir = new Construir(ref juego, false, this);
            DialogResult dr = frm_construir.ShowDialog();
            if ((dr != DialogResult.OK) && (dr != DialogResult.Abort))
                return;
            Actualizar_Dinero_Jugadores();
            bConstruir.Enabled = false;
            bComprarSuelo.Enabled = false;
        }

        private void bComprarSuelo_Click(object sender, EventArgs e)
        {
            if (juego.jugador_actual.hoteles.Count == 0)
            {
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles);
                return;
            }
            var frm_construir = new Construir(ref juego, true, this);
            if (frm_construir.ShowDialog() != DialogResult.OK)
                return;
            Actualizar_Dinero_Jugadores();
            bConstruir.Enabled = false;
            bComprarSuelo.Enabled = false;
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            Close();
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

            switch (keyData)
            {
                case Keys.T:
                    bTurno.PerformClick();
                    break;
                case Keys.E:
                    bDado.PerformClick();
                    break;
                case Keys.C:
                    bCargar.PerformClick();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private Boolean Puede_Cobrar_Banca(int num_jugador)
        {
            if ((juego.n_jugadores != 2) && ((juego.n_jugadores_activos <= 2) || (juego.n_jugadores <= 2)))
                return false;
            int pos = juego.jugadores[num_jugador - 1].posicion.numero;
            return (pos >= 8) && ((pos - juego.ultimo_res_dado - juego.ultimo_avance_auto) < 8);
        }

        private void bCobrarBanca_Click(object sender, EventArgs e)
        {
            bCobrarBanca.Enabled = false;
            if (online)
                frm_online.enviar_comando("charge_bank", game_id.ToString());
            else
            {
                juego.jugador_actual.Cobrar_Banco();
                Actualizar_Dinero_Jugador_Actual();
            }
        }

        private void bVerHotelesJ1_Click(object sender, EventArgs e)
        {
            if (juego.jugadores[0].hoteles.Count != 0)
            {
                var frm_ver_hoteles = new VerHoteles(ref juego, 0, false);
                frm_ver_hoteles.Show(this);
                bVerHotelesJ1.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHotelesJ2_Click(object sender, EventArgs e)
        {
            if (juego.jugadores[1].hoteles.Count != 0)
            {
                var frm_ver_hoteles = new VerHoteles(ref juego, 1, false);
                frm_ver_hoteles.Show(this);
                bVerHotelesJ2.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHotelesJ3_Click(object sender, EventArgs e)
        {
            if (juego.jugadores[2].hoteles.Count != 0)
            {
                var frm_ver_hoteles = new VerHoteles(ref juego, 2, false);
                frm_ver_hoteles.Show(this);
                bVerHotelesJ3.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHotelesJ4_Click(object sender, EventArgs e)
        {
            if (juego.jugadores[3].hoteles.Count != 0)
            {
                var frm_ver_hoteles = new VerHoteles(ref juego, 3, false);
                frm_ver_hoteles.Show(this);
                bVerHotelesJ4.Enabled = false;
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoEsPosibleMostrarTusHoteles);
        }

        private void bVerHoteles_Click(object sender, EventArgs e)
        {
            var frm_ver_hoteles = new VerHoteles(ref juego, 0, true);
            frm_ver_hoteles.Show(this);
            bVerHoteles.Enabled = false;
        }

        public void ReactivarVerHoteles()
        {
            bVerHoteles.Enabled = true;
            if (controlJ1.Enabled)
                bVerHotelesJ1.Enabled = true;
            if (controlJ2.Enabled)
                bVerHotelesJ2.Enabled = true;
            if (controlJ3.Enabled)
                bVerHotelesJ3.Enabled = true;
            if (controlJ4.Enabled)
                bVerHotelesJ4.Enabled = true;
        }

        public Boolean Puede_poner_entradas(Jugador jugador)
        {
            int pos = jugador.posicion.numero;
            if (pos - juego.ultimo_res_dado - juego.ultimo_avance_auto < 1) // En caso de pasarse de la vuelta al tablero en la misma tirada que te toca poner entradas
                pos += 31;
            return (pos >= 27) && ((pos - juego.ultimo_res_dado - juego.ultimo_avance_auto) < 27);
        }

        private void Poner_entradas(int num_jugador)
        {
            if (juego.jugadores[num_jugador].hoteles.Count != 0)
            {
                var frm_poner_entradas = new PonerEntradas(num_jugador, this);
                frm_poner_entradas.ShowDialog();
            }
            else
                MessageBox.Show(Mensajes.mensajeNoPoseesHoteles, Mensajes.tituloNoesPosiblePonerEntradas);
        }

        private void bEntradasJ1_Click(object sender, EventArgs e)
        {
            Poner_entradas(0);
        }

        private void bEntradasJ2_Click(object sender, EventArgs e)
        {
            Poner_entradas(1);
        }

        private void bEntradasJ3_Click(object sender, EventArgs e)
        {
            Poner_entradas(2);
        }

        private void bEntradasJ4_Click(object sender, EventArgs e)
        {
            Poner_entradas(3);
        }

        public void Añadir_Entrada(Jugador jugador, int casilla, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Añadir_Entrada(jugador, casilla, hotel);
        }

        public void Añadir_Fase(Jugador jugador, int fase, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Añadir_Fase(jugador, fase, hotel);
        }

        public void Subasta_Iniciada(Jugador jugador, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Iniciar_Subasta(jugador, hotel);
        }

        public void Nueva_Puja(Jugador jugador, int cantidad, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Realizar_Puja(jugador, cantidad, hotel);
        }

        public void Subasta_Terminada(Jugador vendedor, int cantidad, Jugador comprador, String hotel, Boolean automaticamente)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Subasta_Terminada(vendedor, comprador, hotel, cantidad, automaticamente);
        }

        private void bNormas_Click(object sender, EventArgs e)
        {
            var frm_reglas = new Reglas();
            this.bNormas.Enabled = false;
            frm_reglas.Show(this);
        }

        private void Pedir_Noches(int n_jugador)
        {
            // Hay que buscar si hay alguien en alguna de tus casillas con entrada
            // Reiniciar el bucle si la lista de hoteles cambia por una subasta
            Boolean reiniciar;
            do
            {
                reiniciar = false;
                int cuantos_hoteles = juego.jugadores[n_jugador].hoteles.Count;
                foreach (Hotel hotel in juego.jugadores[n_jugador].hoteles)
                {
                    // Iteramos por las casillas con entrada del hotel
                    foreach (Casilla casilla in hotel.entradas)
                    {
                        if (!casilla.ocupada)
                            continue;
                        // Buscar el jugador que esté en la casilla
                        Jugador jugador_pagador = juego.jugadores.FirstOrDefault(x => (x.posicion.numero == casilla.numero)
                                                                                      && (x.color != juego.jugadores[n_jugador].color) && (x.pago_ultimo_turno == false));
                        if (jugador_pagador == null)
                            continue;
                        // El jugador encontrado debe pagar las noches correspondientes
                        MessageBox.Show(String.Format(Mensajes.mensajeDebePagarNoches, jugador_pagador.Nombre_color(),
                            hotel.dueño.Nombre_color()), Mensajes.tituloPagarNoches, MessageBoxButtons.OK);
                        int num_noches = juego.dado.tirar();
                        int dinero_necesario = hotel.Calcular_noches(num_noches);
                        MessageBox.Show(String.Format(Mensajes.mensajeTotalAPagar, num_noches, jugador_pagador.Nombre_color(), dinero_necesario, hotel.dueño.Nombre_color()));
                        var frm_pago = new PedirPago(dinero_necesario, ref juego, jugador_pagador, this, hotel.dueño);
                        frm_pago.ShowDialog();
                        jugador_pagador.pago_ultimo_turno = true;
                        jugador_pagador.Pagar_Noches(ref hotel.dueño, frm_pago.n_5000, frm_pago.n_1000, frm_pago.n_500, frm_pago.n_100, frm_pago.n_50);
                        if (frm_pago.total_seleccionado > dinero_necesario)
                        {
                            int n_5000, n_1000, n_500, n_100, n_50;
                            Calcular_Devolucion(ref hotel.dueño, (frm_pago.total_seleccionado - dinero_necesario), out n_5000, out n_1000, out n_500, out n_100, out n_50);
                            jugador_pagador.Devolver_cambio(n_5000, n_1000, n_500, n_100, n_50);
                        }
                        frm_pago.Close();
                        if (juego.jugadores[n_jugador].hoteles.Count == cuantos_hoteles)
                            continue;
                        reiniciar = true;
                        break;
                    }
                    if (reiniciar)
                        break;
                }
            }
            while (reiniciar);
            Actualizar_Dinero_Jugadores();
        }

        public void Registrar_Pagar_Noches_Online(Jugador jugador_dueño, Jugador jugador_pagador, int cantidad, int noches, String hotel)
        {
            if (Application.OpenForms.Cast<Form>().Contains(actividad))
                actividad.Pedir_Noches_Online(jugador_dueño, jugador_pagador, cantidad, noches, hotel);
        }

        public void Pedir_Noches_Online(Jugador jugador, int cantidad, int noches, String hotel)
        {
            bTurno.Enabled = false;
            var frm_pago = new PedirPago(cantidad, ref juego, juego.jugador_actual, this, jugador);
            frm_pago.ShowDialog();
            if (!frm_pago.cancelado)
            {
                jugador.pago_ultimo_turno = true;
                int n_5000 = frm_pago.n_5000, n_1000 = frm_pago.n_1000, n_500 = frm_pago.n_500, n_100 = frm_pago.n_100, n_50 = frm_pago.n_50;
                frm_online.enviar_comando("pay_nights", game_id.ToString(), n_5000.ToString(), n_1000.ToString(), n_500.ToString(), n_100.ToString(), n_50.ToString());
                bTurno.Enabled = true;
                Registrar_Pagar_Noches_Online(jugador, juego.jugador_actual, cantidad, noches, hotel);
            }
            frm_pago.Close();
        }

        private void bPedirNochesJ1_Click(object sender, EventArgs e)
        {
            bPedirNochesJ1.Enabled = false;
            if (online)
            {
                bTurno.Enabled = false;
                frm_online.enviar_comando("ask_nights", game_id.ToString());
            }
            else
                Pedir_Noches(0);
        }

        private void bPedirNochesJ2_Click(object sender, EventArgs e)
        {
            bPedirNochesJ2.Enabled = false;
            if (online)
            {
                bTurno.Enabled = false;
                frm_online.enviar_comando("ask_nights", game_id.ToString());
            }
            else
                Pedir_Noches(1);
        }

        private void bPedirNochesJ3_Click(object sender, EventArgs e)
        {
            bPedirNochesJ3.Enabled = false;
            if (online)
            {
                bTurno.Enabled = false;
                frm_online.enviar_comando("ask_nights", game_id.ToString());
            }
            else
                Pedir_Noches(2);
        }

        private void bPedirNochesJ4_Click(object sender, EventArgs e)
        {
            bPedirNochesJ4.Enabled = false;
            if (online)
            {
                bTurno.Enabled = false;
                frm_online.enviar_comando("ask_nights", game_id.ToString());
            }
            else
                Pedir_Noches(3);
        }

        public void Marcar_Jugador_Eliminado(int n_jugador)
        {
            switch (n_jugador)
            {
                case 0: controlJ1.Enabled = false;
                    break;
                case 1: controlJ2.Enabled = false;
                    break;
                case 2: controlJ3.Enabled = false;
                    break;
                case 3: controlJ4.Enabled = false;
                    break;
            }
            bTurno.Enabled = true;
            bTurno.PerformClick();
            bTurno.Enabled = false;
        }

        public void Desactivar_Controles()
        {
            controlJ1.Enabled = false;
            controlJ2.Enabled = false;
            controlJ3.Enabled = false;
            controlJ4.Enabled = false;
            foreach (Button control in Controls.OfType<Button>())
            {
                control.Enabled = false;
            }
            bSalir.Enabled = true;
        }

        public void Limpiar_Fases_y_Entradas_Hoteles_Jugador(Jugador jugador)
        {
            foreach (Control fase in jugador.hoteles.Select(hotel => Controls.Find("fase", true).Where(f =>
            {
                var tuple = f.Tag as Tuple<int, int, Hotel>;
                return tuple != null && tuple.Item3 == hotel;
            }).ToList()).SelectMany(lista_fases => lista_fases))
            {
                Controls.Remove(fase);
                fase.Dispose();
            }
            imgTablero.Image = Properties.Resources.Tablero;
            Image nuevo_tablero = Properties.Resources.Tablero;
            // Las entradas no pueden ser borradas, hay que redibujar el tablero entero con las entradas que queden
            foreach (Hotel hotel in juego.hoteles.Where(h => h.dueño != jugador))
            {
                foreach (Casilla casilla in hotel.entradas)
                {
                    Dibujar_Entrada(casilla, casilla.entrada_en_der, nuevo_tablero);
                }
                if (hotel.suelo_comprado)
                    Dibujar_Suelo(hotel, nuevo_tablero);
            }
            imgTablero.Image = nuevo_tablero;
        }

        private Boolean Retirarse(int num_jugador)
        {
            if (juego.jugadores[num_jugador].Eliminado()) // Ya está eliminado
                return true;
            if (MessageBox.Show(Mensajes.mensajeRetirarse, Mensajes.tituloHotel, MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            if (!online)
            {
                Limpiar_Fases_y_Entradas_Hoteles_Jugador(juego.jugadores[num_jugador]);
                juego.Eliminar_Jugador(juego.jugadores[num_jugador], null);
                Marcar_Jugador_Eliminado(num_jugador);
            }
            else
            {
                frm_online.enviar_comando("retire", game_id.ToString(), "0");
                partida_activa = false;
                Desactivar_Controles();
            }
            return true;
        }

        private void bRetirarseJ1_Click(object sender, EventArgs e)
        {
            Retirarse(0);
        }

        private void bRetirarseJ2_Click(object sender, EventArgs e)
        {
            Retirarse(1);
        }

        private void bRetirarseJ3_Click(object sender, EventArgs e)
        {
            Retirarse(2);
        }

        private void bRetirarseJ4_Click(object sender, EventArgs e)
        {
            Retirarse(3);
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (partida_activa)
                if (!Retirarse(juego.jugador_actual.n_jugador))
                    e.Cancel = true;
            Guardar_idioma(Thread.CurrentThread.CurrentUICulture);
        }

        private void Principal_Shown(object sender, EventArgs e)
        {
            if (!online)
                return;
            bIniciar.PerformClick();
            actividad.Show();
        }

        public string InputBox(string prompt, string title, string defaultValue)
        {
            var ib = new InputBoxDialog
            {
                FormPrompt = prompt,
                FormCaption = title,
                DefaultValue = defaultValue
            };
            ib.ShowDialog();
            string s = ib.InputResponse;
            ib.Close();
            return s;
        }

        private void bSalvar_Click(object sender, EventArgs e)
        {
            if (dado_tirado)
            {
                MessageBox.Show(Mensajes.mensajeSalvarAntesDeTirar);
                return;
            }
            if (online)
            {
                string password = InputBox(Mensajes.mensajeInputPasswordPartida, Mensajes.tituloCrearPartida, "");
                frm_online.enviar_comando("save_game", game_id.ToString(), password);
            }
            else
            {   
                var dialogo = new SaveFileDialog
                {
                    AddExtension = true,
                    CheckPathExists = true,
                    DefaultExt = "xml",
                    SupportMultiDottedExtensions = true,
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = Mensajes.filtroDialogoCargarGuardarPartida,
                    Title = Mensajes.tituloDialogoGuardarPartida,
                    FileName = "Partida Hotel " + DateTime.Today.ToShortDateString().Replace('/', '-') + ".xml"
                };
                if (dialogo.ShowDialog() == DialogResult.Cancel)
                    return;
                if (dialogo.FileName == "")
                    return;
                var mgr_salvar = new Salvar_y_cargar(ref juego);
                if (mgr_salvar.Salvar_partida(dialogo.FileName) == false)
                    MessageBox.Show(String.Format(Mensajes.mensajeErrorAlSalvar, mgr_salvar.error));
            }
        }

        private void bCargar_Click(object sender, EventArgs e)
        {
            if (!bIniciar.Enabled)
                if (MessageBox.Show(Mensajes.mensajeCargarPartida, Mensajes.tituloCargarPartida, MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            var dialogo = new OpenFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "xml",
                SupportMultiDottedExtensions = true
            };
            String dir_trabajo = Directory.GetCurrentDirectory(); // Después de cargar el fichero, el directorio actual se pierde
            dialogo.InitialDirectory = Directory.GetCurrentDirectory();
            dialogo.Filter = Mensajes.filtroDialogoCargarGuardarPartida;
            dialogo.Title = Mensajes.tituloDialogoCargarPartida;
            dialogo.FileName = "Partida Hotel " + DateTime.Today.ToShortDateString().Replace('/', '-') + ".xml";
            if (dialogo.ShowDialog() != DialogResult.Cancel)
            {
                if (dialogo.FileName != "")
                {
                    Reiniciar_partida();
                    var mgr_cargar = new Salvar_y_cargar(ref juego);
                    if (mgr_cargar.Cargar_partida(dialogo.FileName, this) == false)
                    {
                        MessageBox.Show(String.Format(Mensajes.mensajeErrorAlCargar, Environment.NewLine + mgr_cargar.error));
                        Reiniciar_partida();
                    }
                    else
                    {
                        partida_cargada_offline = true;
                        bIniciar.PerformClick();
                    }
                }
            }
            Directory.SetCurrentDirectory(dir_trabajo); // Restaurar el directorio, para poder cargar el Config.xml
        }

        public void Dibujar_Fase(Hotel hotel, int num_fase)
        {
            if (num_fase == hotel.n_fases_max - 1)
                Dibujar_Suelo(hotel);
            else
            {
                // Creando nuevo PictureBox para meter la imagen de la fase
                var fase = new PictureBox();
                ((ISupportInitialize)(fase)).BeginInit();
                Tipos.Posicion pos = hotel.posiciones_fases.ToList()[num_fase];
                switch (hotel.dueño.color)
                {
                    case Tipos.Tcolor.amarillo: fase.Image = (Image)img_tick_Amarillo.Clone();
                        break;
                    case Tipos.Tcolor.azul: fase.Image = (Image)img_tick_Azul.Clone();
                        break;
                    case Tipos.Tcolor.rojo: fase.Image = (Image)img_tick_Rojo.Clone();
                        break;
                    case Tipos.Tcolor.verde: fase.Image = (Image)img_tick_Verde.Clone();
                        break;
                }
                fase.BackColor = Color.Transparent;
                fase.Location = new Point(pos.X, pos.Y);
                fase.Size = Calcular_Tamaño(ancho_fase, alto_fase);
                fase.SizeMode = PictureBoxSizeMode.StretchImage;
                fase.TabStop = false;
                fase.Name = "fase";
                fase.Tag = new Tuple<int, int, Hotel>(fase.Location.X, fase.Location.Y, hotel);
                fase.Location = Calcular_Posicion(fase.Location.X, fase.Location.Y);
                Controls.Add(fase);
                fase.Parent = imgTablero;
                ((ISupportInitialize)(fase)).EndInit();
                fase.BringToFront();
            }
        }

        public void Dibujar_Entrada(Casilla casilla, Boolean en_la_derecha, Image img_tablero = null)
        {
            // Se repinta el tablero con la nueva imagen encima
            // Si no se pasa un tablero por parámetro, usar el actual. Esto sirve para cuando se retira un jugador,
            // poder reusar este método para repintar sobre un tablero sin asignarlo al formulario
            Image tablero = (img_tablero ?? imgTablero.Image);
            Graphics g = Graphics.FromImage(tablero);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Image entrada;
            Tipos.Posicion pos;
            if (en_la_derecha)
            {
                entrada = RotateImage(img_entrada, casilla.pos_entrada_der.grados);
                pos = casilla.pos_entrada_der;
            }
            else
            {
                entrada = RotateImage(img_entrada, casilla.pos_entrada_izq.grados);
                pos = casilla.pos_entrada_izq;
            }
            if (entrada != null)
                g.DrawImage(entrada, pos.X, pos.Y);
            else
                return;
            // Sustitur imagen actual
            if (img_tablero == null)
                imgTablero.Image = tablero;
        }

        public void Dibujar_Suelo(Hotel hotel, Image img_tablero = null)
        {
            // Se repinta el tablero con la nueva imagen encima
            // Si no se pasa un tablero por parámetro, usar el actual. Esto sirve para cuando se retira un jugador,
            // poder reusar este método para repintar sobre un tablero sin asignarlo al formulario
            Image tablero = (img_tablero ?? imgTablero.Image);
            Graphics g = Graphics.FromImage(tablero);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Image suelo;
            switch (hotel.nombre)
            {
                case Tipos.Tnombre_hotel.Boomerang: suelo = Properties.Resources.Suelo_Boomerang;
                                                    break;
                case Tipos.Tnombre_hotel.Fujiyama:  suelo = Properties.Resources.Suelo_Fujiyama;
                                                    break;
                case Tipos.Tnombre_hotel.Letoile:   suelo = Properties.Resources.Suelo_Letoile;
                                                    break;
                case Tipos.Tnombre_hotel.President: suelo = Properties.Resources.Suelo_President;
                                                    break;
                case Tipos.Tnombre_hotel.Royal:     suelo = Properties.Resources.Suelo_Royal;
                                                    break;
                case Tipos.Tnombre_hotel.Safari:    suelo = Properties.Resources.Suelo_Safari;
                                                    break;
                case Tipos.Tnombre_hotel.Taj_Mahal: suelo = Properties.Resources.Suelo_TajMahal;
                                                    break;
                case Tipos.Tnombre_hotel.Waikiki:   suelo = Properties.Resources.Suelo_Waikiki;
                                                    break;
                default:                            suelo = null;
                                                    break;
            }
            Tipos.Posicion pos = hotel.posiciones_fases.ToList()[hotel.n_fases_max - 1];
            if (suelo != null) // Por si acaso alguna cosa rara
                g.DrawImage(suelo, pos.X, pos.Y);
            else
                return;
            // Sustitur imagen actual
            if (img_tablero == null)
                imgTablero.Image = tablero;
        }

        Point Calcular_Posicion(int x, int y)
        {
            x -= imgTablero.Left;
            y -= imgTablero.Top;
            float desplazamiento_x = (imgTablero.Width / (float)ancho_ini);
            float desplazamiento_y = (imgTablero.Height / (float)alto_ini);
            int x2 = Convert.ToInt32(x * desplazamiento_x);
            int y2 = Convert.ToInt32(y * desplazamiento_y);
            return new Point(x2, y2);
        }

        Size Calcular_Tamaño(int ancho, int alto)
        {
            float factor_x = (imgTablero.Width / (float)ancho_ini);
            float factor_y = (imgTablero.Height / (float)alto_ini);
            int ancho2 = Convert.ToInt32(ancho * factor_x);
            int alto2 = Convert.ToInt32(alto * factor_y);
            return new Size(ancho2, alto2);
        }

        private void Principal_Resize(object sender, EventArgs e)
        {
            // Obtener nuevo tamaño y recolocar todos los picturebox
            if (juego == null)
               return; // No se ha inicializado la ventana aun, el constructor no se ha ejecutado
            // Banco y Ayuntamiento
            img_Banco.Location = Calcular_Posicion(pos_banco_orig.X, pos_banco_orig.Y);
            img_Banco.Size = Calcular_Tamaño(ancho_fase, alto_fase);
            img_ayto.Location = Calcular_Posicion(pos_ayto_orig.X, pos_ayto_orig.Y);
            img_ayto.Size = Calcular_Tamaño(ancho_fase, alto_fase);
            // Fases construidas
            Control[] lista_fases = Controls.Find("fase", true);
            foreach (Control fase in lista_fases)
            {
                var fase_tag = fase.Tag as Tuple<int, int, Hotel>; // Uso la propiedad Tag para almacenar la posición original
                if (fase_tag != null)
                {
                    int x = Convert.ToInt32(fase_tag.Item1);
                    int y = Convert.ToInt32(fase_tag.Item2);
                    fase.Location = Calcular_Posicion(x, y);
                }
                fase.Size = Calcular_Tamaño(ancho_fase, alto_fase);
            }
            if (juego.jugadores == null) // Partida no empezada
            {
                posRojo.Location = Calcular_Posicion(pos_rojo_orig.X, pos_rojo_orig.Y);
                posRojo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                posAzul.Location = Calcular_Posicion(pos_azul_orig.X, pos_azul_orig.Y);
                posAzul.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                posVerde.Location = Calcular_Posicion(pos_verde_orig.X, pos_verde_orig.Y);
                posVerde.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                posAmarillo.Location = Calcular_Posicion(pos_amarillo_orig.X, pos_amarillo_orig.Y);
                posAmarillo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
            }
            else
            {
                Jugador jugador = juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "rojo");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    posRojo.Location = Calcular_Posicion(pos_rojo_orig.X, pos_rojo_orig.Y);
                else
                    posRojo.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                posRojo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                jugador = juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "azul");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    posAzul.Location = Calcular_Posicion(pos_azul_orig.X, pos_azul_orig.Y);
                else
                    posAzul.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                posAzul.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                jugador = juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "verde");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    posVerde.Location = Calcular_Posicion(pos_verde_orig.X, pos_verde_orig.Y);
                else
                    posVerde.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                posVerde.Size = Calcular_Tamaño(ancho_coche, alto_coche);
                jugador = juego.jugadores.FirstOrDefault(Jugador => Jugador.Nombre_color() == "amarillo");
                if ((jugador == null) || (jugador.posicion.tipo == Tipos.Tcasilla.salida))
                    posAmarillo.Location = Calcular_Posicion(pos_amarillo_orig.X, pos_amarillo_orig.Y);
                else
                    posAmarillo.Location = Calcular_Posicion(jugador.posicion.pos_coche.X, jugador.posicion.pos_coche.Y);
                posAmarillo.Size = Calcular_Tamaño(ancho_coche, alto_coche);
            }            
        }

        private void Guardar_idioma(CultureInfo culture)
        {
            XmlNode nodo_Idioma;
            var fichero_configuracion = new XmlDocument();
            if (online)
            {
                fichero_configuracion.Load("Config.xml");
                nodo_Idioma = fichero_configuracion.GetElementsByTagName("language")[0];
            }
            else
                nodo_Idioma = configuracion.GetElementsByTagName("language")[0];
            switch (culture.Name)
            {
                case "es": nodo_Idioma.ChildNodes[0].InnerText = "Spanish";
                    break;
                case "en": nodo_Idioma.ChildNodes[0].InnerText = "English";
                    break;
            }
            var writer = new XmlTextWriter("Config.xml", Encoding.UTF8);
            try
            {
                writer.Formatting = Formatting.Indented;
                if (online)
                    fichero_configuracion.Save(writer);
                else
                    configuracion.Save(writer);
            }
            catch (Exception e)
            {
                MessageBox.Show(Mensajes.mensajeErrorSalvandoIdioma + @": " + e.Message);
            }
            finally
            {
                writer.Close();
            }
        }

        private void IdiomaElegido(object sender, EventArgs e)
        {
            var senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex.Equals(0))
            {
                Program.ReLocalizeAll(new CultureInfo("es"));
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
            }
            else if (senderComboBox.SelectedIndex.Equals(1))
            {
                Program.ReLocalizeAll(new CultureInfo("en"));
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            Guardar_idioma(Thread.CurrentThread.CurrentUICulture);
        }

        public void ReLocalize(CultureInfo nuevoCulture, CultureInfo antiguoCulture)
        {
            if (InvokeRequired)
                BeginInvoke(new Action<CultureInfo, CultureInfo>(ReLocalize), new object[] { nuevoCulture, antiguoCulture });
            else
            {
                Thread.CurrentThread.CurrentUICulture = nuevoCulture;
                resources.ApplyResources(this, "$this");
                foreach (Control c in Controls)
                {
                    if (c is GroupBox)
                    {
                        c.Text = resources.GetString(c.Name + ".Text");
                        foreach (Control o in c.Controls)
                        {
                            if (o is Label)
                            {
                                switch (o.Name) // Etiquetas con el color
                                {
                                    case "colorJ1": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (juego.n_jugadores > 0)
                                                        o.Text += juego.jugadores[0].Nombre_color();
                                                    break;
                                    case "colorJ2": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (juego.n_jugadores > 1)
                                                        o.Text += juego.jugadores[1].Nombre_color();
                                                    break;
                                    case "colorJ3": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (juego.n_jugadores > 2)
                                                        o.Text += juego.jugadores[2].Nombre_color();
                                                    break;
                                    case "colorJ4": o.Text = resources.GetString(o.Name + ".Text");
                                                    if (juego.n_jugadores > 3)
                                                        o.Text += juego.jugadores[3].Nombre_color();
                                                    break;
                                    default:        var nombreAntiguo = (String)resources.GetObject(o.Name + ".Text", antiguoCulture);
                                                    if (nombreAntiguo != null)
                                                        if (o.Text != null)
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
                        ((ComboItemImagen)(c as ComboBox).Items[0]).Etiqueta = Mensajes.comboBoxIdiomas1;
                        ((ComboItemImagen)(c as ComboBox).Items[1]).Etiqueta = Mensajes.comboBoxIdiomas2;
                        (c as ComboBox).SelectedIndex = antiguoCulture.Name.Equals("es") ? 1 : 0;
                    }
                    else if (c is Label)
                    {
                        var nombreAntiguo = (String)resources.GetObject(c.Name + ".Text", antiguoCulture);
                        if (nombreAntiguo == null)
                            continue;
                        if (c.Text != null)
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
            if (e.Index == -1)
                return;
            var item = comboBoxIdiomas.Items[e.Index] as ComboItemImagen;
            e.DrawBackground();
            if (item == null)
                return;
            if (item.ImageIndex >= 0 && item.ImageIndex < imageListIdiomas.Images.Count)
                e.Graphics.DrawImage(imageListIdiomas.Images[item.ImageIndex], new PointF(e.Bounds.Left, e.Bounds.Top));
            e.Graphics.DrawString(item.Etiqueta, e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.Left + imageListIdiomas.ImageSize.Width + 1, e.Bounds.Top));
        }

        private void bReportarBug_Click(object sender, EventArgs e)
        {
            var reportar_bug = new ReporteBug();
            reportar_bug.ShowDialog();
        }

        public void PermitirPasarTurno()
        {
            bTurno.Enabled = true;
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