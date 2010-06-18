namespace Juego_Hotel
{
    partial class Principal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTablero = new System.Windows.Forms.Label();
            this.imgTablero = new System.Windows.Forms.PictureBox();
            this.bIniciar = new System.Windows.Forms.Button();
            this.dos_jugadores = new System.Windows.Forms.RadioButton();
            this.tres_jugadores = new System.Windows.Forms.RadioButton();
            this.cuatro_jugadores = new System.Windows.Forms.RadioButton();
            this.grupoNJugadores = new System.Windows.Forms.GroupBox();
            this.bColores = new System.Windows.Forms.Button();
            this.bDado = new System.Windows.Forms.Button();
            this.resDado = new System.Windows.Forms.Label();
            this.jug_ini = new System.Windows.Forms.Label();
            this.bTurno = new System.Windows.Forms.Button();
            this.bReiniciar = new System.Windows.Forms.Button();
            this.colorJugIni = new System.Windows.Forms.Label();
            this.bDadoCons = new System.Windows.Forms.Button();
            this.controlJ1 = new System.Windows.Forms.GroupBox();
            this.bPedirNochesJ1 = new System.Windows.Forms.Button();
            this.bEntradasJ1 = new System.Windows.Forms.Button();
            this.dineroJ1 = new System.Windows.Forms.Label();
            this.bVerHotelesJ1 = new System.Windows.Forms.Button();
            this.posJ1 = new System.Windows.Forms.Label();
            this.turnoJ1 = new System.Windows.Forms.Label();
            this.bVerHotelesJ2 = new System.Windows.Forms.Button();
            this.controlJ2 = new System.Windows.Forms.GroupBox();
            this.bPedirNochesJ2 = new System.Windows.Forms.Button();
            this.bEntradasJ2 = new System.Windows.Forms.Button();
            this.dineroJ2 = new System.Windows.Forms.Label();
            this.posJ2 = new System.Windows.Forms.Label();
            this.turnoJ2 = new System.Windows.Forms.Label();
            this.bVerHotelesJ3 = new System.Windows.Forms.Button();
            this.controlJ3 = new System.Windows.Forms.GroupBox();
            this.bPedirNochesJ3 = new System.Windows.Forms.Button();
            this.bEntradasJ3 = new System.Windows.Forms.Button();
            this.dineroJ3 = new System.Windows.Forms.Label();
            this.posJ3 = new System.Windows.Forms.Label();
            this.turnoJ3 = new System.Windows.Forms.Label();
            this.bVerHotelesJ4 = new System.Windows.Forms.Button();
            this.controlJ4 = new System.Windows.Forms.GroupBox();
            this.bPedirNochesJ4 = new System.Windows.Forms.Button();
            this.bEntradasJ4 = new System.Windows.Forms.Button();
            this.dineroJ4 = new System.Windows.Forms.Label();
            this.posJ4 = new System.Windows.Forms.Label();
            this.turnoJ4 = new System.Windows.Forms.Label();
            this.bComprar = new System.Windows.Forms.Button();
            this.bConstruir = new System.Windows.Forms.Button();
            this.bVerHoteles = new System.Windows.Forms.Button();
            this.posRojo = new System.Windows.Forms.PictureBox();
            this.posAzul = new System.Windows.Forms.PictureBox();
            this.posAmarillo = new System.Windows.Forms.PictureBox();
            this.posVerde = new System.Windows.Forms.PictureBox();
            this.bComprarSuelo = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.bCobrarBanca = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgTablero)).BeginInit();
            this.grupoNJugadores.SuspendLayout();
            this.controlJ1.SuspendLayout();
            this.controlJ2.SuspendLayout();
            this.controlJ3.SuspendLayout();
            this.controlJ4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posRojo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posAzul)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posAmarillo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posVerde)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTablero
            // 
            this.labelTablero.AutoSize = true;
            this.labelTablero.Location = new System.Drawing.Point(10, 9);
            this.labelTablero.Name = "labelTablero";
            this.labelTablero.Size = new System.Drawing.Size(43, 13);
            this.labelTablero.TabIndex = 0;
            this.labelTablero.Text = "Tablero";
            // 
            // imgTablero
            // 
            this.imgTablero.Image = global::Juego_Hotel.Properties.Resources.Tablero;
            this.imgTablero.Location = new System.Drawing.Point(13, 30);
            this.imgTablero.Name = "imgTablero";
            this.imgTablero.Size = new System.Drawing.Size(661, 477);
            this.imgTablero.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgTablero.TabIndex = 1;
            this.imgTablero.TabStop = false;
            // 
            // bIniciar
            // 
            this.bIniciar.Location = new System.Drawing.Point(708, 57);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(83, 23);
            this.bIniciar.TabIndex = 2;
            this.bIniciar.Text = "Iniciar";
            this.bIniciar.UseVisualStyleBackColor = true;
            this.bIniciar.Click += new System.EventHandler(this.bIniciar_Click);
            // 
            // dos_jugadores
            // 
            this.dos_jugadores.AutoSize = true;
            this.dos_jugadores.Location = new System.Drawing.Point(13, 19);
            this.dos_jugadores.Name = "dos_jugadores";
            this.dos_jugadores.Size = new System.Drawing.Size(83, 17);
            this.dos_jugadores.TabIndex = 3;
            this.dos_jugadores.TabStop = true;
            this.dos_jugadores.Text = "2 Jugadores";
            this.dos_jugadores.UseVisualStyleBackColor = true;
            this.dos_jugadores.CheckedChanged += new System.EventHandler(this.dos_jugadores_CheckedChanged);
            // 
            // tres_jugadores
            // 
            this.tres_jugadores.AutoSize = true;
            this.tres_jugadores.Location = new System.Drawing.Point(13, 42);
            this.tres_jugadores.Name = "tres_jugadores";
            this.tres_jugadores.Size = new System.Drawing.Size(83, 17);
            this.tres_jugadores.TabIndex = 4;
            this.tres_jugadores.TabStop = true;
            this.tres_jugadores.Text = "3 Jugadores";
            this.tres_jugadores.UseVisualStyleBackColor = true;
            this.tres_jugadores.CheckedChanged += new System.EventHandler(this.tres_jugadores_CheckedChanged);
            // 
            // cuatro_jugadores
            // 
            this.cuatro_jugadores.AutoSize = true;
            this.cuatro_jugadores.Location = new System.Drawing.Point(13, 65);
            this.cuatro_jugadores.Name = "cuatro_jugadores";
            this.cuatro_jugadores.Size = new System.Drawing.Size(83, 17);
            this.cuatro_jugadores.TabIndex = 5;
            this.cuatro_jugadores.TabStop = true;
            this.cuatro_jugadores.Text = "4 Jugadores";
            this.cuatro_jugadores.UseVisualStyleBackColor = true;
            this.cuatro_jugadores.CheckedChanged += new System.EventHandler(this.cuatro_jugadores_CheckedChanged);
            // 
            // grupoNJugadores
            // 
            this.grupoNJugadores.Controls.Add(this.dos_jugadores);
            this.grupoNJugadores.Controls.Add(this.tres_jugadores);
            this.grupoNJugadores.Controls.Add(this.cuatro_jugadores);
            this.grupoNJugadores.Location = new System.Drawing.Point(695, 88);
            this.grupoNJugadores.Name = "grupoNJugadores";
            this.grupoNJugadores.Size = new System.Drawing.Size(107, 92);
            this.grupoNJugadores.TabIndex = 6;
            this.grupoNJugadores.TabStop = false;
            this.grupoNJugadores.Text = "Nº de jugadores";
            // 
            // bColores
            // 
            this.bColores.Location = new System.Drawing.Point(708, 193);
            this.bColores.Name = "bColores";
            this.bColores.Size = new System.Drawing.Size(83, 23);
            this.bColores.TabIndex = 8;
            this.bColores.Text = "Colores";
            this.bColores.UseVisualStyleBackColor = true;
            this.bColores.Click += new System.EventHandler(this.bColores_Click);
            // 
            // bDado
            // 
            this.bDado.Enabled = false;
            this.bDado.Location = new System.Drawing.Point(708, 238);
            this.bDado.Name = "bDado";
            this.bDado.Size = new System.Drawing.Size(85, 23);
            this.bDado.TabIndex = 9;
            this.bDado.Text = "Tirar dado";
            this.bDado.UseVisualStyleBackColor = true;
            this.bDado.Click += new System.EventHandler(this.bDado_Click);
            // 
            // resDado
            // 
            this.resDado.AutoSize = true;
            this.resDado.Location = new System.Drawing.Point(705, 312);
            this.resDado.Name = "resDado";
            this.resDado.Size = new System.Drawing.Size(39, 13);
            this.resDado.TabIndex = 10;
            this.resDado.Text = "Dado: ";
            // 
            // jug_ini
            // 
            this.jug_ini.AutoSize = true;
            this.jug_ini.Location = new System.Drawing.Point(705, 340);
            this.jug_ini.Name = "jug_ini";
            this.jug_ini.Size = new System.Drawing.Size(80, 13);
            this.jug_ini.TabIndex = 11;
            this.jug_ini.Text = "Jugador inicial: ";
            // 
            // bTurno
            // 
            this.bTurno.Enabled = false;
            this.bTurno.Location = new System.Drawing.Point(708, 267);
            this.bTurno.Name = "bTurno";
            this.bTurno.Size = new System.Drawing.Size(83, 23);
            this.bTurno.TabIndex = 12;
            this.bTurno.Text = "Pasar turno";
            this.bTurno.UseVisualStyleBackColor = true;
            this.bTurno.Click += new System.EventHandler(this.bTurno_Click);
            // 
            // bReiniciar
            // 
            this.bReiniciar.Location = new System.Drawing.Point(708, 416);
            this.bReiniciar.Name = "bReiniciar";
            this.bReiniciar.Size = new System.Drawing.Size(83, 23);
            this.bReiniciar.TabIndex = 14;
            this.bReiniciar.Text = "Reinciar";
            this.bReiniciar.UseVisualStyleBackColor = true;
            this.bReiniciar.Click += new System.EventHandler(this.bReiniciar_Click);
            // 
            // colorJugIni
            // 
            this.colorJugIni.AutoSize = true;
            this.colorJugIni.Location = new System.Drawing.Point(793, 340);
            this.colorJugIni.Name = "colorJugIni";
            this.colorJugIni.Size = new System.Drawing.Size(0, 13);
            this.colorJugIni.TabIndex = 15;
            // 
            // bDadoCons
            // 
            this.bDadoCons.Enabled = false;
            this.bDadoCons.Location = new System.Drawing.Point(708, 367);
            this.bDadoCons.Name = "bDadoCons";
            this.bDadoCons.Size = new System.Drawing.Size(83, 35);
            this.bDadoCons.TabIndex = 18;
            this.bDadoCons.Text = "Tirar dado construcción";
            this.bDadoCons.UseVisualStyleBackColor = true;
            this.bDadoCons.Click += new System.EventHandler(this.bDadoCons_Click);
            // 
            // controlJ1
            // 
            this.controlJ1.Controls.Add(this.bPedirNochesJ1);
            this.controlJ1.Controls.Add(this.bEntradasJ1);
            this.controlJ1.Controls.Add(this.dineroJ1);
            this.controlJ1.Controls.Add(this.bVerHotelesJ1);
            this.controlJ1.Controls.Add(this.posJ1);
            this.controlJ1.Enabled = false;
            this.controlJ1.Location = new System.Drawing.Point(13, 531);
            this.controlJ1.Name = "controlJ1";
            this.controlJ1.Size = new System.Drawing.Size(150, 100);
            this.controlJ1.TabIndex = 19;
            this.controlJ1.TabStop = false;
            this.controlJ1.Text = "Jugador 1";
            // 
            // bPedirNochesJ1
            // 
            this.bPedirNochesJ1.Location = new System.Drawing.Point(87, 11);
            this.bPedirNochesJ1.Name = "bPedirNochesJ1";
            this.bPedirNochesJ1.Size = new System.Drawing.Size(57, 47);
            this.bPedirNochesJ1.TabIndex = 27;
            this.bPedirNochesJ1.Text = "Pedir abonar noches";
            this.bPedirNochesJ1.UseVisualStyleBackColor = true;
            // 
            // bEntradasJ1
            // 
            this.bEntradasJ1.Location = new System.Drawing.Point(87, 60);
            this.bEntradasJ1.Name = "bEntradasJ1";
            this.bEntradasJ1.Size = new System.Drawing.Size(57, 34);
            this.bEntradasJ1.TabIndex = 4;
            this.bEntradasJ1.Text = "Poner entrada";
            this.bEntradasJ1.UseVisualStyleBackColor = true;
            // 
            // dineroJ1
            // 
            this.dineroJ1.AutoSize = true;
            this.dineroJ1.Location = new System.Drawing.Point(7, 45);
            this.dineroJ1.Name = "dineroJ1";
            this.dineroJ1.Size = new System.Drawing.Size(41, 13);
            this.dineroJ1.TabIndex = 2;
            this.dineroJ1.Text = "Dinero:";
            // 
            // bVerHotelesJ1
            // 
            this.bVerHotelesJ1.Location = new System.Drawing.Point(10, 71);
            this.bVerHotelesJ1.Name = "bVerHotelesJ1";
            this.bVerHotelesJ1.Size = new System.Drawing.Size(70, 23);
            this.bVerHotelesJ1.TabIndex = 1;
            this.bVerHotelesJ1.Text = "Ver hoteles";
            this.bVerHotelesJ1.UseVisualStyleBackColor = true;
            // 
            // posJ1
            // 
            this.posJ1.AutoSize = true;
            this.posJ1.Location = new System.Drawing.Point(7, 22);
            this.posJ1.Name = "posJ1";
            this.posJ1.Size = new System.Drawing.Size(40, 13);
            this.posJ1.TabIndex = 0;
            this.posJ1.Text = "Casilla:";
            // 
            // turnoJ1
            // 
            this.turnoJ1.AutoSize = true;
            this.turnoJ1.Location = new System.Drawing.Point(97, 515);
            this.turnoJ1.Name = "turnoJ1";
            this.turnoJ1.Size = new System.Drawing.Size(0, 13);
            this.turnoJ1.TabIndex = 3;
            // 
            // bVerHotelesJ2
            // 
            this.bVerHotelesJ2.Location = new System.Drawing.Point(10, 71);
            this.bVerHotelesJ2.Name = "bVerHotelesJ2";
            this.bVerHotelesJ2.Size = new System.Drawing.Size(70, 23);
            this.bVerHotelesJ2.TabIndex = 1;
            this.bVerHotelesJ2.Text = "Ver hoteles";
            this.bVerHotelesJ2.UseVisualStyleBackColor = true;
            // 
            // controlJ2
            // 
            this.controlJ2.Controls.Add(this.bPedirNochesJ2);
            this.controlJ2.Controls.Add(this.bEntradasJ2);
            this.controlJ2.Controls.Add(this.dineroJ2);
            this.controlJ2.Controls.Add(this.bVerHotelesJ2);
            this.controlJ2.Controls.Add(this.posJ2);
            this.controlJ2.Enabled = false;
            this.controlJ2.Location = new System.Drawing.Point(183, 531);
            this.controlJ2.Name = "controlJ2";
            this.controlJ2.Size = new System.Drawing.Size(150, 100);
            this.controlJ2.TabIndex = 20;
            this.controlJ2.TabStop = false;
            this.controlJ2.Text = "Jugador 2";
            // 
            // bPedirNochesJ2
            // 
            this.bPedirNochesJ2.Location = new System.Drawing.Point(87, 11);
            this.bPedirNochesJ2.Name = "bPedirNochesJ2";
            this.bPedirNochesJ2.Size = new System.Drawing.Size(57, 47);
            this.bPedirNochesJ2.TabIndex = 28;
            this.bPedirNochesJ2.Text = "Pedir abonar noches";
            this.bPedirNochesJ2.UseVisualStyleBackColor = true;
            // 
            // bEntradasJ2
            // 
            this.bEntradasJ2.Location = new System.Drawing.Point(87, 60);
            this.bEntradasJ2.Name = "bEntradasJ2";
            this.bEntradasJ2.Size = new System.Drawing.Size(57, 34);
            this.bEntradasJ2.TabIndex = 5;
            this.bEntradasJ2.Text = "Poner entrada";
            this.bEntradasJ2.UseVisualStyleBackColor = true;
            // 
            // dineroJ2
            // 
            this.dineroJ2.AutoSize = true;
            this.dineroJ2.Location = new System.Drawing.Point(7, 45);
            this.dineroJ2.Name = "dineroJ2";
            this.dineroJ2.Size = new System.Drawing.Size(41, 13);
            this.dineroJ2.TabIndex = 2;
            this.dineroJ2.Text = "Dinero:";
            // 
            // posJ2
            // 
            this.posJ2.AutoSize = true;
            this.posJ2.Location = new System.Drawing.Point(7, 22);
            this.posJ2.Name = "posJ2";
            this.posJ2.Size = new System.Drawing.Size(40, 13);
            this.posJ2.TabIndex = 0;
            this.posJ2.Text = "Casilla:";
            // 
            // turnoJ2
            // 
            this.turnoJ2.AutoSize = true;
            this.turnoJ2.Location = new System.Drawing.Point(267, 515);
            this.turnoJ2.Name = "turnoJ2";
            this.turnoJ2.Size = new System.Drawing.Size(0, 13);
            this.turnoJ2.TabIndex = 4;
            // 
            // bVerHotelesJ3
            // 
            this.bVerHotelesJ3.Location = new System.Drawing.Point(10, 71);
            this.bVerHotelesJ3.Name = "bVerHotelesJ3";
            this.bVerHotelesJ3.Size = new System.Drawing.Size(70, 23);
            this.bVerHotelesJ3.TabIndex = 1;
            this.bVerHotelesJ3.Text = "Ver hoteles";
            this.bVerHotelesJ3.UseVisualStyleBackColor = true;
            // 
            // controlJ3
            // 
            this.controlJ3.Controls.Add(this.bPedirNochesJ3);
            this.controlJ3.Controls.Add(this.bEntradasJ3);
            this.controlJ3.Controls.Add(this.dineroJ3);
            this.controlJ3.Controls.Add(this.bVerHotelesJ3);
            this.controlJ3.Controls.Add(this.posJ3);
            this.controlJ3.Enabled = false;
            this.controlJ3.Location = new System.Drawing.Point(354, 531);
            this.controlJ3.Name = "controlJ3";
            this.controlJ3.Size = new System.Drawing.Size(150, 100);
            this.controlJ3.TabIndex = 21;
            this.controlJ3.TabStop = false;
            this.controlJ3.Text = "Jugador 3";
            // 
            // bPedirNochesJ3
            // 
            this.bPedirNochesJ3.Location = new System.Drawing.Point(86, 11);
            this.bPedirNochesJ3.Name = "bPedirNochesJ3";
            this.bPedirNochesJ3.Size = new System.Drawing.Size(57, 47);
            this.bPedirNochesJ3.TabIndex = 32;
            this.bPedirNochesJ3.Text = "Pedir abonar noches";
            this.bPedirNochesJ3.UseVisualStyleBackColor = true;
            // 
            // bEntradasJ3
            // 
            this.bEntradasJ3.Location = new System.Drawing.Point(86, 60);
            this.bEntradasJ3.Name = "bEntradasJ3";
            this.bEntradasJ3.Size = new System.Drawing.Size(57, 34);
            this.bEntradasJ3.TabIndex = 25;
            this.bEntradasJ3.Text = "Poner entrada";
            this.bEntradasJ3.UseVisualStyleBackColor = true;
            // 
            // dineroJ3
            // 
            this.dineroJ3.AutoSize = true;
            this.dineroJ3.Location = new System.Drawing.Point(7, 45);
            this.dineroJ3.Name = "dineroJ3";
            this.dineroJ3.Size = new System.Drawing.Size(41, 13);
            this.dineroJ3.TabIndex = 2;
            this.dineroJ3.Text = "Dinero:";
            // 
            // posJ3
            // 
            this.posJ3.AutoSize = true;
            this.posJ3.Location = new System.Drawing.Point(7, 22);
            this.posJ3.Name = "posJ3";
            this.posJ3.Size = new System.Drawing.Size(40, 13);
            this.posJ3.TabIndex = 0;
            this.posJ3.Text = "Casilla:";
            // 
            // turnoJ3
            // 
            this.turnoJ3.AutoSize = true;
            this.turnoJ3.Location = new System.Drawing.Point(437, 515);
            this.turnoJ3.Name = "turnoJ3";
            this.turnoJ3.Size = new System.Drawing.Size(0, 13);
            this.turnoJ3.TabIndex = 5;
            // 
            // bVerHotelesJ4
            // 
            this.bVerHotelesJ4.Location = new System.Drawing.Point(10, 71);
            this.bVerHotelesJ4.Name = "bVerHotelesJ4";
            this.bVerHotelesJ4.Size = new System.Drawing.Size(70, 23);
            this.bVerHotelesJ4.TabIndex = 1;
            this.bVerHotelesJ4.Text = "Ver hoteles";
            this.bVerHotelesJ4.UseVisualStyleBackColor = true;
            // 
            // controlJ4
            // 
            this.controlJ4.Controls.Add(this.bPedirNochesJ4);
            this.controlJ4.Controls.Add(this.bEntradasJ4);
            this.controlJ4.Controls.Add(this.dineroJ4);
            this.controlJ4.Controls.Add(this.bVerHotelesJ4);
            this.controlJ4.Controls.Add(this.posJ4);
            this.controlJ4.Enabled = false;
            this.controlJ4.Location = new System.Drawing.Point(524, 531);
            this.controlJ4.Name = "controlJ4";
            this.controlJ4.Size = new System.Drawing.Size(150, 100);
            this.controlJ4.TabIndex = 22;
            this.controlJ4.TabStop = false;
            this.controlJ4.Text = "Jugador 4";
            // 
            // bPedirNochesJ4
            // 
            this.bPedirNochesJ4.Location = new System.Drawing.Point(87, 11);
            this.bPedirNochesJ4.Name = "bPedirNochesJ4";
            this.bPedirNochesJ4.Size = new System.Drawing.Size(57, 47);
            this.bPedirNochesJ4.TabIndex = 32;
            this.bPedirNochesJ4.Text = "Pedir abonar noches";
            this.bPedirNochesJ4.UseVisualStyleBackColor = true;
            // 
            // bEntradasJ4
            // 
            this.bEntradasJ4.Location = new System.Drawing.Point(87, 60);
            this.bEntradasJ4.Name = "bEntradasJ4";
            this.bEntradasJ4.Size = new System.Drawing.Size(57, 34);
            this.bEntradasJ4.TabIndex = 25;
            this.bEntradasJ4.Text = "Poner entrada";
            this.bEntradasJ4.UseVisualStyleBackColor = true;
            // 
            // dineroJ4
            // 
            this.dineroJ4.AutoSize = true;
            this.dineroJ4.Location = new System.Drawing.Point(7, 45);
            this.dineroJ4.Name = "dineroJ4";
            this.dineroJ4.Size = new System.Drawing.Size(41, 13);
            this.dineroJ4.TabIndex = 2;
            this.dineroJ4.Text = "Dinero:";
            // 
            // posJ4
            // 
            this.posJ4.AutoSize = true;
            this.posJ4.Location = new System.Drawing.Point(7, 22);
            this.posJ4.Name = "posJ4";
            this.posJ4.Size = new System.Drawing.Size(40, 13);
            this.posJ4.TabIndex = 0;
            this.posJ4.Text = "Casilla:";
            // 
            // turnoJ4
            // 
            this.turnoJ4.AutoSize = true;
            this.turnoJ4.Location = new System.Drawing.Point(608, 515);
            this.turnoJ4.Name = "turnoJ4";
            this.turnoJ4.Size = new System.Drawing.Size(0, 13);
            this.turnoJ4.TabIndex = 6;
            // 
            // bComprar
            // 
            this.bComprar.Enabled = false;
            this.bComprar.Location = new System.Drawing.Point(708, 515);
            this.bComprar.Name = "bComprar";
            this.bComprar.Size = new System.Drawing.Size(83, 23);
            this.bComprar.TabIndex = 23;
            this.bComprar.Text = "Comprar";
            this.bComprar.UseVisualStyleBackColor = true;
            this.bComprar.Click += new System.EventHandler(this.bComprar_Click);
            // 
            // bConstruir
            // 
            this.bConstruir.Enabled = false;
            this.bConstruir.Location = new System.Drawing.Point(708, 544);
            this.bConstruir.Name = "bConstruir";
            this.bConstruir.Size = new System.Drawing.Size(83, 23);
            this.bConstruir.TabIndex = 24;
            this.bConstruir.Text = "Construir";
            this.bConstruir.UseVisualStyleBackColor = true;
            this.bConstruir.Click += new System.EventHandler(this.bConstruir_Click);
            // 
            // bVerHoteles
            // 
            this.bVerHoteles.Location = new System.Drawing.Point(708, 486);
            this.bVerHoteles.Name = "bVerHoteles";
            this.bVerHoteles.Size = new System.Drawing.Size(83, 23);
            this.bVerHoteles.TabIndex = 25;
            this.bVerHoteles.Text = "Ver hoteles";
            this.bVerHoteles.UseVisualStyleBackColor = true;
            // 
            // posRojo
            // 
            this.posRojo.Image = global::Juego_Hotel.Properties.Resources.Fleha_roja;
            this.posRojo.Location = new System.Drawing.Point(40, 322);
            this.posRojo.Name = "posRojo";
            this.posRojo.Size = new System.Drawing.Size(20, 20);
            this.posRojo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.posRojo.TabIndex = 26;
            this.posRojo.TabStop = false;
            // 
            // posAzul
            // 
            this.posAzul.Image = global::Juego_Hotel.Properties.Resources.Fleha_azul;
            this.posAzul.Location = new System.Drawing.Point(60, 322);
            this.posAzul.Name = "posAzul";
            this.posAzul.Size = new System.Drawing.Size(20, 20);
            this.posAzul.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.posAzul.TabIndex = 27;
            this.posAzul.TabStop = false;
            // 
            // posAmarillo
            // 
            this.posAmarillo.Image = global::Juego_Hotel.Properties.Resources.Fleha_amarilla;
            this.posAmarillo.Location = new System.Drawing.Point(100, 322);
            this.posAmarillo.Name = "posAmarillo";
            this.posAmarillo.Size = new System.Drawing.Size(20, 20);
            this.posAmarillo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.posAmarillo.TabIndex = 28;
            this.posAmarillo.TabStop = false;
            // 
            // posVerde
            // 
            this.posVerde.Image = global::Juego_Hotel.Properties.Resources.Fleha_verde;
            this.posVerde.Location = new System.Drawing.Point(80, 322);
            this.posVerde.Name = "posVerde";
            this.posVerde.Size = new System.Drawing.Size(20, 20);
            this.posVerde.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.posVerde.TabIndex = 29;
            this.posVerde.TabStop = false;
            // 
            // bComprarSuelo
            // 
            this.bComprarSuelo.Enabled = false;
            this.bComprarSuelo.Location = new System.Drawing.Point(708, 573);
            this.bComprarSuelo.Name = "bComprarSuelo";
            this.bComprarSuelo.Size = new System.Drawing.Size(85, 23);
            this.bComprarSuelo.TabIndex = 30;
            this.bComprarSuelo.Text = "Comprar suelo";
            this.bComprarSuelo.UseVisualStyleBackColor = true;
            this.bComprarSuelo.Click += new System.EventHandler(this.bComprarSuelo_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(708, 602);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(83, 23);
            this.bSalir.TabIndex = 31;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // bCobrarBanca
            // 
            this.bCobrarBanca.Location = new System.Drawing.Point(708, 457);
            this.bCobrarBanca.Name = "bCobrarBanca";
            this.bCobrarBanca.Size = new System.Drawing.Size(83, 23);
            this.bCobrarBanca.TabIndex = 32;
            this.bCobrarBanca.Text = "Cobrar banca";
            this.bCobrarBanca.UseVisualStyleBackColor = true;
            this.bCobrarBanca.Click += new System.EventHandler(this.bCobrarBanca_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 640);
            this.Controls.Add(this.bCobrarBanca);
            this.Controls.Add(this.bSalir);
            this.Controls.Add(this.turnoJ4);
            this.Controls.Add(this.turnoJ3);
            this.Controls.Add(this.turnoJ2);
            this.Controls.Add(this.turnoJ1);
            this.Controls.Add(this.bComprarSuelo);
            this.Controls.Add(this.posVerde);
            this.Controls.Add(this.posAmarillo);
            this.Controls.Add(this.posAzul);
            this.Controls.Add(this.posRojo);
            this.Controls.Add(this.colorJugIni);
            this.Controls.Add(this.bVerHoteles);
            this.Controls.Add(this.bConstruir);
            this.Controls.Add(this.bComprar);
            this.Controls.Add(this.controlJ4);
            this.Controls.Add(this.controlJ3);
            this.Controls.Add(this.controlJ2);
            this.Controls.Add(this.controlJ1);
            this.Controls.Add(this.bDadoCons);
            this.Controls.Add(this.bReiniciar);
            this.Controls.Add(this.bTurno);
            this.Controls.Add(this.jug_ini);
            this.Controls.Add(this.resDado);
            this.Controls.Add(this.bDado);
            this.Controls.Add(this.bColores);
            this.Controls.Add(this.grupoNJugadores);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.imgTablero);
            this.Controls.Add(this.labelTablero);
            this.MaximizeBox = false;
            this.Name = "Principal";
            this.Text = "Hotel - Construye tu propio imperio hotelero";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Principal_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.imgTablero)).EndInit();
            this.grupoNJugadores.ResumeLayout(false);
            this.grupoNJugadores.PerformLayout();
            this.controlJ1.ResumeLayout(false);
            this.controlJ1.PerformLayout();
            this.controlJ2.ResumeLayout(false);
            this.controlJ2.PerformLayout();
            this.controlJ3.ResumeLayout(false);
            this.controlJ3.PerformLayout();
            this.controlJ4.ResumeLayout(false);
            this.controlJ4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posRojo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posAzul)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posAmarillo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posVerde)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTablero;
        private System.Windows.Forms.PictureBox imgTablero;
        private System.Windows.Forms.Button bIniciar;
        private System.Windows.Forms.RadioButton dos_jugadores;
        private System.Windows.Forms.RadioButton tres_jugadores;
        private System.Windows.Forms.RadioButton cuatro_jugadores;
        private System.Windows.Forms.GroupBox grupoNJugadores;
        private System.Windows.Forms.Button bColores;
        private System.Windows.Forms.Button bDado;
        private System.Windows.Forms.Label resDado;
        private System.Windows.Forms.Label jug_ini;
        private System.Windows.Forms.Button bTurno;
        private System.Windows.Forms.Button bReiniciar;
        private System.Windows.Forms.Label colorJugIni;
        private System.Windows.Forms.Button bDadoCons;
        private System.Windows.Forms.GroupBox controlJ1;
        private System.Windows.Forms.Button bVerHotelesJ1;
        private System.Windows.Forms.Label posJ1;
        private System.Windows.Forms.Label dineroJ1;
        private System.Windows.Forms.Button bVerHotelesJ2;
        private System.Windows.Forms.GroupBox controlJ2;
        private System.Windows.Forms.Label dineroJ2;
        private System.Windows.Forms.Label posJ2;
        private System.Windows.Forms.Button bVerHotelesJ3;
        private System.Windows.Forms.GroupBox controlJ3;
        private System.Windows.Forms.Label dineroJ3;
        private System.Windows.Forms.Label posJ3;
        private System.Windows.Forms.Button bVerHotelesJ4;
        private System.Windows.Forms.GroupBox controlJ4;
        private System.Windows.Forms.Label dineroJ4;
        private System.Windows.Forms.Label posJ4;
        private System.Windows.Forms.Button bComprar;
        private System.Windows.Forms.Button bConstruir;
        private System.Windows.Forms.Label turnoJ1;
        private System.Windows.Forms.Label turnoJ2;
        private System.Windows.Forms.Label turnoJ3;
        private System.Windows.Forms.Label turnoJ4;
        private System.Windows.Forms.Button bEntradasJ1;
        private System.Windows.Forms.Button bEntradasJ2;
        private System.Windows.Forms.Button bEntradasJ3;
        private System.Windows.Forms.Button bEntradasJ4;
        private System.Windows.Forms.Button bVerHoteles;
        private System.Windows.Forms.PictureBox posRojo;
        private System.Windows.Forms.PictureBox posAzul;
        private System.Windows.Forms.PictureBox posAmarillo;
        private System.Windows.Forms.PictureBox posVerde;
        private System.Windows.Forms.Button bComprarSuelo;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.Button bPedirNochesJ1;
        private System.Windows.Forms.Button bPedirNochesJ2;
        private System.Windows.Forms.Button bPedirNochesJ3;
        private System.Windows.Forms.Button bPedirNochesJ4;
        private System.Windows.Forms.Button bCobrarBanca;
    }
}