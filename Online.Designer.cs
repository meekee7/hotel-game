namespace Juego_Hotel
{
    partial class Online
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
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.bLogin = new System.Windows.Forms.Button();
            this.bConectar = new System.Windows.Forms.Button();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.listaUsuarios = new System.Windows.Forms.ListBox();
            this.bDesconectar = new System.Windows.Forms.Button();
            this.bCrearPartida = new System.Windows.Forms.Button();
            this.bCrearConv = new System.Windows.Forms.Button();
            this.listaPartidas = new System.Windows.Forms.ListBox();
            this.LUsuarios = new System.Windows.Forms.Label();
            this.LPartidas = new System.Windows.Forms.Label();
            this.bChatGlobal = new System.Windows.Forms.Button();
            this.bUnirse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(93, 44);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(100, 20);
            this.txtLogin.TabIndex = 2;
            this.txtLogin.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLogin_PreviewKeyDown);
            // 
            // bLogin
            // 
            this.bLogin.Enabled = false;
            this.bLogin.Location = new System.Drawing.Point(12, 42);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(75, 23);
            this.bLogin.TabIndex = 3;
            this.bLogin.Text = "Login";
            this.bLogin.UseVisualStyleBackColor = true;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // bConectar
            // 
            this.bConectar.Location = new System.Drawing.Point(12, 13);
            this.bConectar.Name = "bConectar";
            this.bConectar.Size = new System.Drawing.Size(75, 23);
            this.bConectar.TabIndex = 1;
            this.bConectar.Text = "Conectar";
            this.bConectar.UseVisualStyleBackColor = true;
            this.bConectar.Click += new System.EventHandler(this.bConectar_Click);
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(93, 15);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(100, 20);
            this.txtServidor.TabIndex = 0;
            this.txtServidor.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServidor_PreviewKeyDown);
            // 
            // listaUsuarios
            // 
            this.listaUsuarios.FormattingEnabled = true;
            this.listaUsuarios.HorizontalScrollbar = true;
            this.listaUsuarios.Location = new System.Drawing.Point(12, 88);
            this.listaUsuarios.Name = "listaUsuarios";
            this.listaUsuarios.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listaUsuarios.Size = new System.Drawing.Size(181, 134);
            this.listaUsuarios.TabIndex = 4;
            // 
            // bDesconectar
            // 
            this.bDesconectar.Enabled = false;
            this.bDesconectar.Location = new System.Drawing.Point(210, 353);
            this.bDesconectar.Name = "bDesconectar";
            this.bDesconectar.Size = new System.Drawing.Size(80, 23);
            this.bDesconectar.TabIndex = 8;
            this.bDesconectar.Text = "Desconectar";
            this.bDesconectar.UseVisualStyleBackColor = true;
            this.bDesconectar.Click += new System.EventHandler(this.bDesconectar_Click);
            // 
            // bCrearPartida
            // 
            this.bCrearPartida.Enabled = false;
            this.bCrearPartida.Location = new System.Drawing.Point(210, 242);
            this.bCrearPartida.Name = "bCrearPartida";
            this.bCrearPartida.Size = new System.Drawing.Size(80, 23);
            this.bCrearPartida.TabIndex = 7;
            this.bCrearPartida.Text = "Crear partida";
            this.bCrearPartida.UseVisualStyleBackColor = true;
            this.bCrearPartida.Click += new System.EventHandler(this.bCrearPartida_Click);
            // 
            // bCrearConv
            // 
            this.bCrearConv.Enabled = false;
            this.bCrearConv.Location = new System.Drawing.Point(210, 88);
            this.bCrearConv.Name = "bCrearConv";
            this.bCrearConv.Size = new System.Drawing.Size(80, 35);
            this.bCrearConv.TabIndex = 5;
            this.bCrearConv.Text = "Crear conversación";
            this.bCrearConv.UseVisualStyleBackColor = true;
            this.bCrearConv.Click += new System.EventHandler(this.bCrearConv_Click);
            // 
            // listaPartidas
            // 
            this.listaPartidas.FormattingEnabled = true;
            this.listaPartidas.HorizontalScrollbar = true;
            this.listaPartidas.Location = new System.Drawing.Point(12, 242);
            this.listaPartidas.Name = "listaPartidas";
            this.listaPartidas.Size = new System.Drawing.Size(181, 134);
            this.listaPartidas.TabIndex = 6;
            this.listaPartidas.SelectedIndexChanged += new System.EventHandler(this.listaPartidas_SelectedIndexChanged);
            this.listaPartidas.DoubleClick += new System.EventHandler(this.listaPartidas_DoubleClick);
            // 
            // LUsuarios
            // 
            this.LUsuarios.AutoSize = true;
            this.LUsuarios.Location = new System.Drawing.Point(12, 72);
            this.LUsuarios.Name = "LUsuarios";
            this.LUsuarios.Size = new System.Drawing.Size(110, 13);
            this.LUsuarios.TabIndex = 9;
            this.LUsuarios.Text = "Usuarios conectados:";
            // 
            // LPartidas
            // 
            this.LPartidas.AutoSize = true;
            this.LPartidas.Location = new System.Drawing.Point(12, 226);
            this.LPartidas.Name = "LPartidas";
            this.LPartidas.Size = new System.Drawing.Size(89, 13);
            this.LPartidas.TabIndex = 10;
            this.LPartidas.Text = "Partidas creadas:";
            // 
            // bChatGlobal
            // 
            this.bChatGlobal.Location = new System.Drawing.Point(210, 129);
            this.bChatGlobal.Name = "bChatGlobal";
            this.bChatGlobal.Size = new System.Drawing.Size(80, 23);
            this.bChatGlobal.TabIndex = 11;
            this.bChatGlobal.Text = "Chat global";
            this.bChatGlobal.UseVisualStyleBackColor = true;
            this.bChatGlobal.Click += new System.EventHandler(this.bChatGlobal_Click);
            // 
            // bUnirse
            // 
            this.bUnirse.Enabled = false;
            this.bUnirse.Location = new System.Drawing.Point(210, 272);
            this.bUnirse.Name = "bUnirse";
            this.bUnirse.Size = new System.Drawing.Size(80, 34);
            this.bUnirse.TabIndex = 12;
            this.bUnirse.Text = "Unirse a partida";
            this.bUnirse.UseVisualStyleBackColor = true;
            this.bUnirse.Click += new System.EventHandler(this.bUnirse_Click);
            // 
            // Online
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 388);
            this.Controls.Add(this.bUnirse);
            this.Controls.Add(this.bChatGlobal);
            this.Controls.Add(this.LPartidas);
            this.Controls.Add(this.LUsuarios);
            this.Controls.Add(this.listaPartidas);
            this.Controls.Add(this.bCrearConv);
            this.Controls.Add(this.bCrearPartida);
            this.Controls.Add(this.bDesconectar);
            this.Controls.Add(this.listaUsuarios);
            this.Controls.Add(this.txtServidor);
            this.Controls.Add(this.bConectar);
            this.Controls.Add(this.bLogin);
            this.Controls.Add(this.txtLogin);
            this.Name = "Online";
            this.Text = "Online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Online_FormClosing);
            this.Load += new System.EventHandler(this.Online_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bLogin;
        private System.Windows.Forms.Button bConectar;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.ListBox listaUsuarios;
        private System.Windows.Forms.Button bDesconectar;
        private System.Windows.Forms.Button bCrearPartida;
        private System.Windows.Forms.Button bCrearConv;
        private System.Windows.Forms.ListBox listaPartidas;
        private System.Windows.Forms.Label LUsuarios;
        private System.Windows.Forms.Label LPartidas;
        private System.Windows.Forms.Button bChatGlobal;
        private System.Windows.Forms.Button bUnirse;
        public System.Windows.Forms.TextBox txtLogin;
    }
}