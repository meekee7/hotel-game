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
            this.sel_colores = new Juego_Hotel.Sel_colores();
            ((System.ComponentModel.ISupportInitialize)(this.imgTablero)).BeginInit();
            this.grupoNJugadores.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTablero
            // 
            this.labelTablero.AutoSize = true;
            this.labelTablero.Location = new System.Drawing.Point(13, 13);
            this.labelTablero.Name = "labelTablero";
            this.labelTablero.Size = new System.Drawing.Size(43, 13);
            this.labelTablero.TabIndex = 0;
            this.labelTablero.Text = "Tablero";
            // 
            // imgTablero
            // 
            this.imgTablero.Location = new System.Drawing.Point(13, 30);
            this.imgTablero.Name = "imgTablero";
            this.imgTablero.Size = new System.Drawing.Size(649, 477);
            this.imgTablero.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgTablero.TabIndex = 1;
            this.imgTablero.TabStop = false;
            // 
            // bIniciar
            // 
            this.bIniciar.Location = new System.Drawing.Point(708, 53);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(75, 23);
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
            this.bColores.Size = new System.Drawing.Size(75, 23);
            this.bColores.TabIndex = 8;
            this.bColores.Text = "Colores";
            this.bColores.UseVisualStyleBackColor = true;
            this.bColores.Click += new System.EventHandler(this.bColores_Click);
            // 
            // bDado
            // 
            this.bDado.Location = new System.Drawing.Point(708, 231);
            this.bDado.Name = "bDado";
            this.bDado.Size = new System.Drawing.Size(75, 23);
            this.bDado.TabIndex = 9;
            this.bDado.Text = "Tirar dado";
            this.bDado.UseVisualStyleBackColor = true;
            this.bDado.Click += new System.EventHandler(this.bDado_Click);
            // 
            // resDado
            // 
            this.resDado.AutoSize = true;
            this.resDado.Location = new System.Drawing.Point(711, 270);
            this.resDado.Name = "resDado";
            this.resDado.Size = new System.Drawing.Size(39, 13);
            this.resDado.TabIndex = 10;
            this.resDado.Text = "Dado: ";
            // 
            // jug_ini
            // 
            this.jug_ini.AutoSize = true;
            this.jug_ini.Location = new System.Drawing.Point(711, 283);
            this.jug_ini.Name = "jug_ini";
            this.jug_ini.Size = new System.Drawing.Size(80, 13);
            this.jug_ini.TabIndex = 11;
            this.jug_ini.Text = "Jugador inicial: ";
            // 
            // sel_colores
            // 
            this.sel_colores.Location = new System.Drawing.Point(2, 231);
            this.sel_colores.Name = "sel_colores";
            this.sel_colores.Size = new System.Drawing.Size(712, 145);
            this.sel_colores.TabIndex = 7;
            this.sel_colores.Visible = false;
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 681);
            this.Controls.Add(this.jug_ini);
            this.Controls.Add(this.resDado);
            this.Controls.Add(this.bDado);
            this.Controls.Add(this.bColores);
            this.Controls.Add(this.sel_colores);
            this.Controls.Add(this.grupoNJugadores);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.imgTablero);
            this.Controls.Add(this.labelTablero);
            this.Name = "Principal";
            this.Text = "Hotel - Construye tu propio imperio hotelero";
            ((System.ComponentModel.ISupportInitialize)(this.imgTablero)).EndInit();
            this.grupoNJugadores.ResumeLayout(false);
            this.grupoNJugadores.PerformLayout();
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
        private Sel_colores sel_colores;
        private System.Windows.Forms.Button bColores;
        private System.Windows.Forms.Button bDado;
        private System.Windows.Forms.Label resDado;
        private System.Windows.Forms.Label jug_ini;
    }
}