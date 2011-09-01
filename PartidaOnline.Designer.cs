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
            this.components = new System.ComponentModel.Container();
            this.listaJugadores = new System.Windows.Forms.ListBox();
            this.bEnviar = new System.Windows.Forms.Button();
            this.mensaje = new System.Windows.Forms.TextBox();
            this.mensajes = new System.Windows.Forms.TextBox();
            this.grupoChat = new System.Windows.Forms.GroupBox();
            this.refrescoLista = new System.Windows.Forms.Timer(this.components);
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
            this.listaJugadores.TabIndex = 7;
            // 
            // bEnviar
            // 
            this.bEnviar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bEnviar.Location = new System.Drawing.Point(293, 288);
            this.bEnviar.Name = "bEnviar";
            this.bEnviar.Size = new System.Drawing.Size(75, 23);
            this.bEnviar.TabIndex = 6;
            this.bEnviar.Text = "Enviar";
            this.bEnviar.UseVisualStyleBackColor = true;
            this.bEnviar.Click += new System.EventHandler(this.bEnviar_Click);
            this.bEnviar.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.bEnviar_PreviewKeyDown);
            // 
            // mensaje
            // 
            this.mensaje.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mensaje.Location = new System.Drawing.Point(6, 289);
            this.mensaje.Name = "mensaje";
            this.mensaje.Size = new System.Drawing.Size(281, 20);
            this.mensaje.TabIndex = 5;
            // 
            // mensajes
            // 
            this.mensajes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mensajes.Enabled = false;
            this.mensajes.Location = new System.Drawing.Point(6, 19);
            this.mensajes.Multiline = true;
            this.mensajes.Name = "mensajes";
            this.mensajes.ReadOnly = true;
            this.mensajes.Size = new System.Drawing.Size(281, 264);
            this.mensajes.TabIndex = 4;
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
            // refrescoLista
            // 
            this.refrescoLista.Interval = 2500;
            this.refrescoLista.Tick += new System.EventHandler(this.refrescoLista_Tick_1);
            // 
            // PartidaOnline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 440);
            this.Controls.Add(this.grupoChat);
            this.Name = "PartidaOnline";
            this.Text = "PartidaOnline";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PartidaOnline_FormClosing);
            this.Shown += new System.EventHandler(this.PartidaOnline_Shown);
            this.grupoChat.ResumeLayout(false);
            this.grupoChat.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listaJugadores;
        private System.Windows.Forms.Button bEnviar;
        private System.Windows.Forms.TextBox mensaje;
        private System.Windows.Forms.TextBox mensajes;
        private System.Windows.Forms.GroupBox grupoChat;
        private System.Windows.Forms.Timer refrescoLista;
    }
}