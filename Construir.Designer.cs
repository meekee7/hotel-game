namespace Juego_Hotel
{
    partial class Construir
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Construir));
            this.bCancelar = new System.Windows.Forms.Button();
            this.bSelHotel = new System.Windows.Forms.Button();
            this.bAmpli1 = new System.Windows.Forms.Button();
            this.bPrincipal = new System.Windows.Forms.Button();
            this.bAmpli2 = new System.Windows.Forms.Button();
            this.bAmpli3 = new System.Windows.Forms.Button();
            this.bAmpli4 = new System.Windows.Forms.Button();
            this.bSuelo = new System.Windows.Forms.Button();
            this.nombreHotel = new System.Windows.Forms.Label();
            this.listaHoteles = new System.Windows.Forms.ComboBox();
            this.HotelSeleccionado = new System.Windows.Forms.Label();
            this.bVerHoteles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bCancelar
            // 
            resources.ApplyResources(this.bCancelar, "bCancelar");
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // bSelHotel
            // 
            resources.ApplyResources(this.bSelHotel, "bSelHotel");
            this.bSelHotel.Name = "bSelHotel";
            this.bSelHotel.UseVisualStyleBackColor = true;
            this.bSelHotel.Click += new System.EventHandler(this.bSelHotel_Click);
            // 
            // bAmpli1
            // 
            resources.ApplyResources(this.bAmpli1, "bAmpli1");
            this.bAmpli1.Name = "bAmpli1";
            this.bAmpli1.UseVisualStyleBackColor = true;
            this.bAmpli1.Click += new System.EventHandler(this.bAmpli1_Click);
            // 
            // bPrincipal
            // 
            resources.ApplyResources(this.bPrincipal, "bPrincipal");
            this.bPrincipal.Name = "bPrincipal";
            this.bPrincipal.UseVisualStyleBackColor = true;
            this.bPrincipal.Click += new System.EventHandler(this.bPrincipal_Click);
            // 
            // bAmpli2
            // 
            resources.ApplyResources(this.bAmpli2, "bAmpli2");
            this.bAmpli2.Name = "bAmpli2";
            this.bAmpli2.UseVisualStyleBackColor = true;
            this.bAmpli2.Click += new System.EventHandler(this.bAmpli2_Click);
            // 
            // bAmpli3
            // 
            resources.ApplyResources(this.bAmpli3, "bAmpli3");
            this.bAmpli3.Name = "bAmpli3";
            this.bAmpli3.UseVisualStyleBackColor = true;
            this.bAmpli3.Click += new System.EventHandler(this.bAmpli3_Click);
            // 
            // bAmpli4
            // 
            resources.ApplyResources(this.bAmpli4, "bAmpli4");
            this.bAmpli4.Name = "bAmpli4";
            this.bAmpli4.UseVisualStyleBackColor = true;
            this.bAmpli4.Click += new System.EventHandler(this.bAmpli4_Click);
            // 
            // bSuelo
            // 
            resources.ApplyResources(this.bSuelo, "bSuelo");
            this.bSuelo.Name = "bSuelo";
            this.bSuelo.UseVisualStyleBackColor = true;
            this.bSuelo.Click += new System.EventHandler(this.bSuelo_Click);
            // 
            // nombreHotel
            // 
            resources.ApplyResources(this.nombreHotel, "nombreHotel");
            this.nombreHotel.Name = "nombreHotel";
            // 
            // listaHoteles
            // 
            resources.ApplyResources(this.listaHoteles, "listaHoteles");
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // HotelSeleccionado
            // 
            resources.ApplyResources(this.HotelSeleccionado, "HotelSeleccionado");
            this.HotelSeleccionado.Name = "HotelSeleccionado";
            // 
            // bVerHoteles
            // 
            resources.ApplyResources(this.bVerHoteles, "bVerHoteles");
            this.bVerHoteles.Name = "bVerHoteles";
            this.bVerHoteles.UseVisualStyleBackColor = true;
            this.bVerHoteles.Click += new System.EventHandler(this.bVerHoteles_Click);
            // 
            // Construir
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ControlBox = false;
            this.Controls.Add(this.bVerHoteles);
            this.Controls.Add(this.HotelSeleccionado);
            this.Controls.Add(this.listaHoteles);
            this.Controls.Add(this.nombreHotel);
            this.Controls.Add(this.bSuelo);
            this.Controls.Add(this.bAmpli4);
            this.Controls.Add(this.bAmpli3);
            this.Controls.Add(this.bAmpli2);
            this.Controls.Add(this.bPrincipal);
            this.Controls.Add(this.bAmpli1);
            this.Controls.Add(this.bSelHotel);
            this.Controls.Add(this.bCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Construir";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Construir_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bCancelar;
        private System.Windows.Forms.Button bSelHotel;
        private System.Windows.Forms.Button bAmpli1;
        private System.Windows.Forms.Button bPrincipal;
        private System.Windows.Forms.Button bAmpli2;
        private System.Windows.Forms.Button bAmpli3;
        private System.Windows.Forms.Button bAmpli4;
        private System.Windows.Forms.Button bSuelo;
        private System.Windows.Forms.Label nombreHotel;
        private System.Windows.Forms.ComboBox listaHoteles;
        private System.Windows.Forms.Label HotelSeleccionado;
        private System.Windows.Forms.Button bVerHoteles;
    }
}