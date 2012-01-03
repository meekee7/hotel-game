namespace Juego_Hotel
{
    partial class PartidaOnline
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
            this.listaJugadores = new System.Windows.Forms.ListBox();
            this.bEnviar = new System.Windows.Forms.Button();
            this.mensaje = new System.Windows.Forms.TextBox();
            this.mensajes = new System.Windows.Forms.TextBox();
            this.grupoChat = new System.Windows.Forms.GroupBox();
            this.txtNombre = new System.Windows.Forms.Label();
            this.bAbandonar = new System.Windows.Forms.Button();
            this.bIniciar = new System.Windows.Forms.Button();
            this.txtCreador = new System.Windows.Forms.Label();
            this.txtNJugadores = new System.Windows.Forms.Label();
            this.grupoChat.SuspendLayout();
            this.SuspendLayout();
            // 
            // listaJugadores
            // 
            this.listaJugadores.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listaJugadores.FormattingEnabled = true;
            this.listaJugadores.Location = new System.Drawing.Point(293, 19);
            this.listaJugadores.Name = "listaJugadores";
            this.listaJugadores.Size = new System.Drawing.Size(75, 264);
            this.listaJugadores.TabIndex = 4;
            // 
            // bEnviar
            // 
            this.bEnviar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bEnviar.Location = new System.Drawing.Point(293, 288);
            this.bEnviar.Name = "bEnviar";
            this.bEnviar.Size = new System.Drawing.Size(75, 23);
            this.bEnviar.TabIndex = 2;
            this.bEnviar.Text = "Enviar";
            this.bEnviar.UseVisualStyleBackColor = true;
            this.bEnviar.Click += new System.EventHandler(this.bEnviar_Click);
            // 
            // mensaje
            // 
            this.mensaje.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mensaje.Location = new System.Drawing.Point(6, 289);
            this.mensaje.Name = "mensaje";
            this.mensaje.Size = new System.Drawing.Size(281, 20);
            this.mensaje.TabIndex = 0;
            this.mensaje.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mensaje_PreviewKeyDown);
            // 
            // mensajes
            // 
            this.mensajes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mensajes.Location = new System.Drawing.Point(6, 19);
            this.mensajes.Multiline = true;
            this.mensajes.Name = "mensajes";
            this.mensajes.ReadOnly = true;
            this.mensajes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mensajes.Size = new System.Drawing.Size(281, 264);
            this.mensajes.TabIndex = 3;
            this.mensajes.WordWrap = false;
            // 
            // grupoChat
            // 
            this.grupoChat.Controls.Add(this.mensajes);
            this.grupoChat.Controls.Add(this.bEnviar);
            this.grupoChat.Controls.Add(this.listaJugadores);
            this.grupoChat.Controls.Add(this.mensaje);
            this.grupoChat.Location = new System.Drawing.Point(12, 114);
            this.grupoChat.Name = "grupoChat";
            this.grupoChat.Size = new System.Drawing.Size(374, 314);
            this.grupoChat.TabIndex = 8;
            this.grupoChat.TabStop = false;
            this.grupoChat.Text = "Chat";
            // 
            // txtNombre
            // 
            this.txtNombre.AutoSize = true;
            this.txtNombre.Location = new System.Drawing.Point(15, 12);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(47, 13);
            this.txtNombre.TabIndex = 6;
            this.txtNombre.Text = "Nombre:";
            // 
            // bAbandonar
            // 
            this.bAbandonar.Location = new System.Drawing.Point(99, 80);
            this.bAbandonar.Name = "bAbandonar";
            this.bAbandonar.Size = new System.Drawing.Size(75, 23);
            this.bAbandonar.TabIndex = 5;
            this.bAbandonar.Text = "Abandonar";
            this.bAbandonar.UseVisualStyleBackColor = true;
            this.bAbandonar.Click += new System.EventHandler(this.bAbandonar_Click);
            // 
            // bIniciar
            // 
            this.bIniciar.Enabled = false;
            this.bIniciar.Location = new System.Drawing.Point(18, 80);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(75, 23);
            this.bIniciar.TabIndex = 1;
            this.bIniciar.Text = "Iniciar";
            this.bIniciar.UseVisualStyleBackColor = true;
            this.bIniciar.Click += new System.EventHandler(this.bIniciar_Click);
            // 
            // txtCreador
            // 
            this.txtCreador.AutoSize = true;
            this.txtCreador.Location = new System.Drawing.Point(15, 34);
            this.txtCreador.Name = "txtCreador";
            this.txtCreador.Size = new System.Drawing.Size(47, 13);
            this.txtCreador.TabIndex = 9;
            this.txtCreador.Text = "Creador:";
            // 
            // txtNJugadores
            // 
            this.txtNJugadores.AutoSize = true;
            this.txtNJugadores.Location = new System.Drawing.Point(15, 57);
            this.txtNJugadores.Name = "txtNJugadores";
            this.txtNJugadores.Size = new System.Drawing.Size(111, 13);
            this.txtNJugadores.TabIndex = 10;
            this.txtNJugadores.Text = "Número de jugadores:";
            // 
            // PartidaOnline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 440);
            this.Controls.Add(this.txtNJugadores);
            this.Controls.Add(this.txtCreador);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.bAbandonar);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.grupoChat);
            this.Name = "PartidaOnline";
            this.Text = "Partida Online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PartidaOnline_FormClosing);
            this.Shown += new System.EventHandler(this.PartidaOnline_Shown);
            this.grupoChat.ResumeLayout(false);
            this.grupoChat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listaJugadores;
        private System.Windows.Forms.Button bEnviar;
        private System.Windows.Forms.TextBox mensaje;
        private System.Windows.Forms.TextBox mensajes;
        private System.Windows.Forms.GroupBox grupoChat;
        private System.Windows.Forms.Label txtNombre;
        private System.Windows.Forms.Button bAbandonar;
        private System.Windows.Forms.Button bIniciar;
        private System.Windows.Forms.Label txtCreador;
        private System.Windows.Forms.Label txtNJugadores;
    }
}