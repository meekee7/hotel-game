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
            this.mensajes = new System.Windows.Forms.TextBox();
            this.mensaje = new System.Windows.Forms.TextBox();
            this.bEnviar = new System.Windows.Forms.Button();
            this.listaJugadores = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // mensajes
            // 
            this.mensajes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mensajes.Location = new System.Drawing.Point(12, 12);
            this.mensajes.Multiline = true;
            this.mensajes.Name = "mensajes";
            this.mensajes.ReadOnly = true;
            this.mensajes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mensajes.Size = new System.Drawing.Size(242, 211);
            this.mensajes.TabIndex = 0;
            this.mensajes.WordWrap = false;
            // 
            // mensaje
            // 
            this.mensaje.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mensaje.Location = new System.Drawing.Point(12, 230);
            this.mensaje.Name = "mensaje";
            this.mensaje.Size = new System.Drawing.Size(242, 20);
            this.mensaje.TabIndex = 1;
            this.mensaje.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mensaje_PreviewKeyDown);
            // 
            // bEnviar
            // 
            this.bEnviar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bEnviar.Location = new System.Drawing.Point(265, 227);
            this.bEnviar.Name = "bEnviar";
            this.bEnviar.Size = new System.Drawing.Size(75, 23);
            this.bEnviar.TabIndex = 2;
            this.bEnviar.Text = "Enviar";
            this.bEnviar.UseVisualStyleBackColor = true;
            this.bEnviar.Click += new System.EventHandler(this.bEnviar_Click);
            // 
            // listaJugadores
            // 
            this.listaJugadores.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listaJugadores.FormattingEnabled = true;
            this.listaJugadores.Location = new System.Drawing.Point(265, 11);
            this.listaJugadores.Name = "listaJugadores";
            this.listaJugadores.Size = new System.Drawing.Size(75, 212);
            this.listaJugadores.TabIndex = 3;
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 262);
            this.Controls.Add(this.listaJugadores);
            this.Controls.Add(this.bEnviar);
            this.Controls.Add(this.mensaje);
            this.Controls.Add(this.mensajes);
            this.MinimumSize = new System.Drawing.Size(368, 300);
            this.Name = "Chat";
            this.Text = "Chat";
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