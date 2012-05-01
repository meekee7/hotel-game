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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartidaOnline));
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
            resources.ApplyResources(this.listaJugadores, "listaJugadores");
            this.listaJugadores.FormattingEnabled = true;
            this.listaJugadores.Name = "listaJugadores";
            // 
            // bEnviar
            // 
            resources.ApplyResources(this.bEnviar, "bEnviar");
            this.bEnviar.Name = "bEnviar";
            this.bEnviar.UseVisualStyleBackColor = true;
            this.bEnviar.Click += new System.EventHandler(this.bEnviar_Click);
            // 
            // mensaje
            // 
            resources.ApplyResources(this.mensaje, "mensaje");
            this.mensaje.Name = "mensaje";
            this.mensaje.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mensaje_PreviewKeyDown);
            // 
            // mensajes
            // 
            resources.ApplyResources(this.mensajes, "mensajes");
            this.mensajes.Name = "mensajes";
            this.mensajes.ReadOnly = true;
            // 
            // grupoChat
            // 
            resources.ApplyResources(this.grupoChat, "grupoChat");
            this.grupoChat.Controls.Add(this.mensajes);
            this.grupoChat.Controls.Add(this.bEnviar);
            this.grupoChat.Controls.Add(this.listaJugadores);
            this.grupoChat.Controls.Add(this.mensaje);
            this.grupoChat.Name = "grupoChat";
            this.grupoChat.TabStop = false;
            // 
            // txtNombre
            // 
            resources.ApplyResources(this.txtNombre, "txtNombre");
            this.txtNombre.Name = "txtNombre";
            // 
            // bAbandonar
            // 
            resources.ApplyResources(this.bAbandonar, "bAbandonar");
            this.bAbandonar.Name = "bAbandonar";
            this.bAbandonar.UseVisualStyleBackColor = true;
            this.bAbandonar.Click += new System.EventHandler(this.bAbandonar_Click);
            // 
            // bIniciar
            // 
            resources.ApplyResources(this.bIniciar, "bIniciar");
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.UseVisualStyleBackColor = true;
            this.bIniciar.Click += new System.EventHandler(this.bIniciar_Click);
            // 
            // txtCreador
            // 
            resources.ApplyResources(this.txtCreador, "txtCreador");
            this.txtCreador.Name = "txtCreador";
            // 
            // txtNJugadores
            // 
            resources.ApplyResources(this.txtNJugadores, "txtNJugadores");
            this.txtNJugadores.Name = "txtNJugadores";
            // 
            // PartidaOnline
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtNJugadores);
            this.Controls.Add(this.txtCreador);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.bAbandonar);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.grupoChat);
            this.Name = "PartidaOnline";
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