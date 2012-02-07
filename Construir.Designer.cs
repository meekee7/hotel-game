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
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Location = new System.Drawing.Point(56, 170);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(75, 23);
            this.bCancelar.TabIndex = 1;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // bSelHotel
            // 
            this.bSelHotel.Location = new System.Drawing.Point(12, 12);
            this.bSelHotel.Name = "bSelHotel";
            this.bSelHotel.Size = new System.Drawing.Size(101, 23);
            this.bSelHotel.TabIndex = 2;
            this.bSelHotel.Text = "Seleccionar Hotel";
            this.bSelHotel.UseVisualStyleBackColor = true;
            this.bSelHotel.Click += new System.EventHandler(this.bSelHotel_Click);
            // 
            // bAmpli1
            // 
            this.bAmpli1.Enabled = false;
            this.bAmpli1.Location = new System.Drawing.Point(165, 66);
            this.bAmpli1.Name = "bAmpli1";
            this.bAmpli1.Size = new System.Drawing.Size(75, 23);
            this.bAmpli1.TabIndex = 3;
            this.bAmpli1.Text = "Ampliación 1";
            this.bAmpli1.UseVisualStyleBackColor = true;
            this.bAmpli1.Click += new System.EventHandler(this.bAmpli1_Click);
            // 
            // bPrincipal
            // 
            this.bPrincipal.Enabled = false;
            this.bPrincipal.Location = new System.Drawing.Point(12, 66);
            this.bPrincipal.Name = "bPrincipal";
            this.bPrincipal.Size = new System.Drawing.Size(92, 23);
            this.bPrincipal.TabIndex = 4;
            this.bPrincipal.Text = "Edificio Principal";
            this.bPrincipal.UseVisualStyleBackColor = true;
            this.bPrincipal.Click += new System.EventHandler(this.bPrincipal_Click);
            // 
            // bAmpli2
            // 
            this.bAmpli2.Enabled = false;
            this.bAmpli2.Location = new System.Drawing.Point(268, 66);
            this.bAmpli2.Name = "bAmpli2";
            this.bAmpli2.Size = new System.Drawing.Size(75, 23);
            this.bAmpli2.TabIndex = 5;
            this.bAmpli2.Text = "Ampliación 2";
            this.bAmpli2.UseVisualStyleBackColor = true;
            this.bAmpli2.Click += new System.EventHandler(this.bAmpli2_Click);
            // 
            // bAmpli3
            // 
            this.bAmpli3.Enabled = false;
            this.bAmpli3.Location = new System.Drawing.Point(165, 118);
            this.bAmpli3.Name = "bAmpli3";
            this.bAmpli3.Size = new System.Drawing.Size(75, 23);
            this.bAmpli3.TabIndex = 6;
            this.bAmpli3.Text = "Ampliación 3";
            this.bAmpli3.UseVisualStyleBackColor = true;
            this.bAmpli3.Click += new System.EventHandler(this.bAmpli3_Click);
            // 
            // bAmpli4
            // 
            this.bAmpli4.Enabled = false;
            this.bAmpli4.Location = new System.Drawing.Point(268, 118);
            this.bAmpli4.Name = "bAmpli4";
            this.bAmpli4.Size = new System.Drawing.Size(75, 23);
            this.bAmpli4.TabIndex = 7;
            this.bAmpli4.Text = "Ampliación 4";
            this.bAmpli4.UseVisualStyleBackColor = true;
            this.bAmpli4.Click += new System.EventHandler(this.bAmpli4_Click);
            // 
            // bSuelo
            // 
            this.bSuelo.Enabled = false;
            this.bSuelo.Location = new System.Drawing.Point(12, 118);
            this.bSuelo.Name = "bSuelo";
            this.bSuelo.Size = new System.Drawing.Size(119, 23);
            this.bSuelo.TabIndex = 8;
            this.bSuelo.Text = "Complejos recreativos";
            this.bSuelo.UseVisualStyleBackColor = true;
            this.bSuelo.Click += new System.EventHandler(this.bSuelo_Click);
            // 
            // nombreHotel
            // 
            this.nombreHotel.AutoSize = true;
            this.nombreHotel.Location = new System.Drawing.Point(162, 17);
            this.nombreHotel.Name = "nombreHotel";
            this.nombreHotel.Size = new System.Drawing.Size(35, 13);
            this.nombreHotel.TabIndex = 9;
            this.nombreHotel.Text = "Hotel:";
            // 
            // listaHoteles
            // 
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Location = new System.Drawing.Point(203, 15);
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.Size = new System.Drawing.Size(101, 21);
            this.listaHoteles.TabIndex = 10;
            this.listaHoteles.Visible = false;
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // HotelSeleccionado
            // 
            this.HotelSeleccionado.AutoSize = true;
            this.HotelSeleccionado.Location = new System.Drawing.Point(204, 17);
            this.HotelSeleccionado.Name = "HotelSeleccionado";
            this.HotelSeleccionado.Size = new System.Drawing.Size(0, 13);
            this.HotelSeleccionado.TabIndex = 11;
            // 
            // bVerHoteles
            // 
            this.bVerHoteles.Location = new System.Drawing.Point(207, 170);
            this.bVerHoteles.Name = "bVerHoteles";
            this.bVerHoteles.Size = new System.Drawing.Size(83, 23);
            this.bVerHoteles.TabIndex = 26;
            this.bVerHoteles.Text = "Ver hoteles";
            this.bVerHoteles.UseVisualStyleBackColor = true;
            this.bVerHoteles.Click += new System.EventHandler(this.bVerHoteles_Click);
            // 
            // Construir
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(360, 205);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Construir partes de un hotel";
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