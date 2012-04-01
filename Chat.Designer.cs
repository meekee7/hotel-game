namespace Juego_Hotel
{
    partial class Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.mensajes = new System.Windows.Forms.TextBox();
            this.mensaje = new System.Windows.Forms.TextBox();
            this.bEnviar = new System.Windows.Forms.Button();
            this.listaJugadores = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // mensajes
            // 
            resources.ApplyResources(this.mensajes, "mensajes");
            this.mensajes.Name = "mensajes";
            this.mensajes.ReadOnly = true;
            // 
            // mensaje
            // 
            resources.ApplyResources(this.mensaje, "mensaje");
            this.mensaje.Name = "mensaje";
            this.mensaje.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mensaje_PreviewKeyDown);
            // 
            // bEnviar
            // 
            resources.ApplyResources(this.bEnviar, "bEnviar");
            this.bEnviar.Name = "bEnviar";
            this.bEnviar.UseVisualStyleBackColor = true;
            this.bEnviar.Click += new System.EventHandler(this.bEnviar_Click);
            // 
            // listaJugadores
            // 
            resources.ApplyResources(this.listaJugadores, "listaJugadores");
            this.listaJugadores.FormattingEnabled = true;
            this.listaJugadores.Name = "listaJugadores";
            // 
            // Chat
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listaJugadores);
            this.Controls.Add(this.bEnviar);
            this.Controls.Add(this.mensaje);
            this.Controls.Add(this.mensajes);
            this.Name = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chat_FormClosing);
            this.Shown += new System.EventHandler(this.Chat_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mensajes;
        private System.Windows.Forms.TextBox mensaje;
        private System.Windows.Forms.Button bEnviar;
        private System.Windows.Forms.ListBox listaJugadores;
    }
}