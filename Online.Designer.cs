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
            this.SuspendLayout();
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(93, 44);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(100, 20);
            this.txtLogin.TabIndex = 0;
            // 
            // bLogin
            // 
            this.bLogin.Enabled = false;
            this.bLogin.Location = new System.Drawing.Point(12, 42);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(75, 23);
            this.bLogin.TabIndex = 1;
            this.bLogin.Text = "Login";
            this.bLogin.UseVisualStyleBackColor = true;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // bConectar
            // 
            this.bConectar.Location = new System.Drawing.Point(12, 13);
            this.bConectar.Name = "bConectar";
            this.bConectar.Size = new System.Drawing.Size(75, 23);
            this.bConectar.TabIndex = 2;
            this.bConectar.Text = "Conectar";
            this.bConectar.UseVisualStyleBackColor = true;
            this.bConectar.Click += new System.EventHandler(this.bConectar_Click);
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(93, 15);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(100, 20);
            this.txtServidor.TabIndex = 3;
            // 
            // listaUsuarios
            // 
            this.listaUsuarios.FormattingEnabled = true;
            this.listaUsuarios.HorizontalScrollbar = true;
            this.listaUsuarios.Location = new System.Drawing.Point(12, 72);
            this.listaUsuarios.Name = "listaUsuarios";
            this.listaUsuarios.Size = new System.Drawing.Size(181, 277);
            this.listaUsuarios.TabIndex = 4;
            // 
            // Online
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 358);
            this.Controls.Add(this.listaUsuarios);
            this.Controls.Add(this.txtServidor);
            this.Controls.Add(this.bConectar);
            this.Controls.Add(this.bLogin);
            this.Controls.Add(this.txtLogin);
            this.Name = "Online";
            this.Text = "Online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Online_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Button bLogin;
        private System.Windows.Forms.Button bConectar;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.ListBox listaUsuarios;
    }
}