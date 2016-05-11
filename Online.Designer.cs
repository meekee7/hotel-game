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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Online));
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
            this.checkSrvOficial = new System.Windows.Forms.CheckBox();
            this.txtPuerto = new System.Windows.Forms.TextBox();
            this.labelDosPuntos = new System.Windows.Forms.Label();
            this.bCargarPartida = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLogin
            // 
            resources.ApplyResources(this.txtLogin, "txtLogin");
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLogin_KeyDown);
            // 
            // bLogin
            // 
            resources.ApplyResources(this.bLogin, "bLogin");
            this.bLogin.Name = "bLogin";
            this.bLogin.UseVisualStyleBackColor = true;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // bConectar
            // 
            resources.ApplyResources(this.bConectar, "bConectar");
            this.bConectar.Name = "bConectar";
            this.bConectar.UseVisualStyleBackColor = true;
            this.bConectar.Click += new System.EventHandler(this.bConectar_Click);
            // 
            // txtServidor
            // 
            resources.ApplyResources(this.txtServidor, "txtServidor");
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServidor_PreviewKeyDown);
            // 
            // listaUsuarios
            // 
            resources.ApplyResources(this.listaUsuarios, "listaUsuarios");
            this.listaUsuarios.FormattingEnabled = true;
            this.listaUsuarios.Name = "listaUsuarios";
            this.listaUsuarios.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // bDesconectar
            // 
            resources.ApplyResources(this.bDesconectar, "bDesconectar");
            this.bDesconectar.Name = "bDesconectar";
            this.bDesconectar.UseVisualStyleBackColor = true;
            this.bDesconectar.Click += new System.EventHandler(this.bDesconectar_Click);
            // 
            // bCrearPartida
            // 
            resources.ApplyResources(this.bCrearPartida, "bCrearPartida");
            this.bCrearPartida.Name = "bCrearPartida";
            this.bCrearPartida.UseVisualStyleBackColor = true;
            this.bCrearPartida.Click += new System.EventHandler(this.bCrearPartida_Click);
            // 
            // bCrearConv
            // 
            resources.ApplyResources(this.bCrearConv, "bCrearConv");
            this.bCrearConv.Name = "bCrearConv";
            this.bCrearConv.UseVisualStyleBackColor = true;
            this.bCrearConv.Click += new System.EventHandler(this.bCrearConv_Click);
            // 
            // listaPartidas
            // 
            resources.ApplyResources(this.listaPartidas, "listaPartidas");
            this.listaPartidas.FormattingEnabled = true;
            this.listaPartidas.Name = "listaPartidas";
            this.listaPartidas.SelectedIndexChanged += new System.EventHandler(this.listaPartidas_SelectedIndexChanged);
            this.listaPartidas.DoubleClick += new System.EventHandler(this.listaPartidas_DoubleClick);
            // 
            // LUsuarios
            // 
            resources.ApplyResources(this.LUsuarios, "LUsuarios");
            this.LUsuarios.Name = "LUsuarios";
            // 
            // LPartidas
            // 
            resources.ApplyResources(this.LPartidas, "LPartidas");
            this.LPartidas.Name = "LPartidas";
            // 
            // bChatGlobal
            // 
            resources.ApplyResources(this.bChatGlobal, "bChatGlobal");
            this.bChatGlobal.Name = "bChatGlobal";
            this.bChatGlobal.UseVisualStyleBackColor = true;
            this.bChatGlobal.Click += new System.EventHandler(this.bChatGlobal_Click);
            // 
            // bUnirse
            // 
            resources.ApplyResources(this.bUnirse, "bUnirse");
            this.bUnirse.Name = "bUnirse";
            this.bUnirse.UseVisualStyleBackColor = true;
            this.bUnirse.Click += new System.EventHandler(this.bUnirse_Click);
            // 
            // checkSrvOficial
            // 
            resources.ApplyResources(this.checkSrvOficial, "checkSrvOficial");
            this.checkSrvOficial.Checked = true;
            this.checkSrvOficial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSrvOficial.Name = "checkSrvOficial";
            this.checkSrvOficial.UseVisualStyleBackColor = true;
            this.checkSrvOficial.CheckedChanged += new System.EventHandler(this.checkSrvOficial_CheckedChanged);
            // 
            // txtPuerto
            // 
            resources.ApplyResources(this.txtPuerto, "txtPuerto");
            this.txtPuerto.Name = "txtPuerto";
            this.txtPuerto.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPuerto_PreviewKeyDown);
            // 
            // labelDosPuntos
            // 
            resources.ApplyResources(this.labelDosPuntos, "labelDosPuntos");
            this.labelDosPuntos.Name = "labelDosPuntos";
            // 
            // bCargarPartida
            // 
            resources.ApplyResources(this.bCargarPartida, "bCargarPartida");
            this.bCargarPartida.Name = "bCargarPartida";
            this.bCargarPartida.UseVisualStyleBackColor = true;
            this.bCargarPartida.Click += new System.EventHandler(this.bCargarPartida_Click);
            // 
            // Online
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtPuerto);
            this.Controls.Add(this.bCargarPartida);
            this.Controls.Add(this.labelDosPuntos);
            this.Controls.Add(this.checkSrvOficial);
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
        private System.Windows.Forms.CheckBox checkSrvOficial;
        private System.Windows.Forms.TextBox txtPuerto;
        private System.Windows.Forms.Label labelDosPuntos;
        private System.Windows.Forms.Button bCargarPartida;
    }
}