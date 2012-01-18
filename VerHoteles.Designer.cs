namespace Juego_Hotel
{
    partial class VerHoteles
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
            this.selHotel = new System.Windows.Forms.Label();
            this.listaHoteles = new System.Windows.Forms.ComboBox();
            this.imgTarjeta = new System.Windows.Forms.PictureBox();
            this.bOK = new System.Windows.Forms.Button();
            this.n_amplis = new System.Windows.Forms.Label();
            this.sueloComprado = new System.Windows.Forms.Label();
            this.dueño = new System.Windows.Forms.Label();
            this.n_entradas = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgTarjeta)).BeginInit();
            this.SuspendLayout();
            // 
            // selHotel
            // 
            this.selHotel.AutoSize = true;
            this.selHotel.Location = new System.Drawing.Point(10, 17);
            this.selHotel.Name = "selHotel";
            this.selHotel.Size = new System.Drawing.Size(100, 13);
            this.selHotel.TabIndex = 0;
            this.selHotel.Text = "Selecciona el hotel:";
            // 
            // listaHoteles
            // 
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Location = new System.Drawing.Point(132, 12);
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.Size = new System.Drawing.Size(121, 21);
            this.listaHoteles.TabIndex = 2;
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // imgTarjeta
            // 
            this.imgTarjeta.Location = new System.Drawing.Point(132, 39);
            this.imgTarjeta.Name = "imgTarjeta";
            this.imgTarjeta.Size = new System.Drawing.Size(513, 301);
            this.imgTarjeta.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgTarjeta.TabIndex = 3;
            this.imgTarjeta.TabStop = false;
            // 
            // bOK
            // 
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bOK.Location = new System.Drawing.Point(18, 317);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 4;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // n_amplis
            // 
            this.n_amplis.AutoSize = true;
            this.n_amplis.Location = new System.Drawing.Point(10, 69);
            this.n_amplis.Name = "n_amplis";
            this.n_amplis.Size = new System.Drawing.Size(0, 13);
            this.n_amplis.TabIndex = 5;
            // 
            // sueloComprado
            // 
            this.sueloComprado.AutoSize = true;
            this.sueloComprado.Location = new System.Drawing.Point(10, 113);
            this.sueloComprado.Name = "sueloComprado";
            this.sueloComprado.Size = new System.Drawing.Size(0, 13);
            this.sueloComprado.TabIndex = 6;
            // 
            // dueño
            // 
            this.dueño.AutoSize = true;
            this.dueño.Location = new System.Drawing.Point(10, 143);
            this.dueño.Name = "dueño";
            this.dueño.Size = new System.Drawing.Size(0, 13);
            this.dueño.TabIndex = 7;
            // 
            // n_entradas
            // 
            this.n_entradas.AutoSize = true;
            this.n_entradas.Location = new System.Drawing.Point(10, 173);
            this.n_entradas.Name = "n_entradas";
            this.n_entradas.Size = new System.Drawing.Size(0, 13);
            this.n_entradas.TabIndex = 8;
            // 
            // VerHoteles
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bOK;
            this.ClientSize = new System.Drawing.Size(657, 352);
            this.ControlBox = false;
            this.Controls.Add(this.n_entradas);
            this.Controls.Add(this.dueño);
            this.Controls.Add(this.sueloComprado);
            this.Controls.Add(this.n_amplis);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.imgTarjeta);
            this.Controls.Add(this.listaHoteles);
            this.Controls.Add(this.selHotel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "VerHoteles";
            this.Text = "Tus hoteles";
            this.Shown += new System.EventHandler(this.VerHoteles_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.imgTarjeta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label selHotel;
        private System.Windows.Forms.ComboBox listaHoteles;
        private System.Windows.Forms.PictureBox imgTarjeta;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Label n_amplis;
        private System.Windows.Forms.Label sueloComprado;
        private System.Windows.Forms.Label dueño;
        private System.Windows.Forms.Label n_entradas;
    }
}