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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerHoteles));
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
            resources.ApplyResources(this.selHotel, "selHotel");
            this.selHotel.Name = "selHotel";
            // 
            // listaHoteles
            // 
            resources.ApplyResources(this.listaHoteles, "listaHoteles");
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // imgTarjeta
            // 
            resources.ApplyResources(this.imgTarjeta, "imgTarjeta");
            this.imgTarjeta.Name = "imgTarjeta";
            this.imgTarjeta.TabStop = false;
            // 
            // bOK
            // 
            resources.ApplyResources(this.bOK, "bOK");
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bOK.Name = "bOK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // n_amplis
            // 
            resources.ApplyResources(this.n_amplis, "n_amplis");
            this.n_amplis.Name = "n_amplis";
            // 
            // sueloComprado
            // 
            resources.ApplyResources(this.sueloComprado, "sueloComprado");
            this.sueloComprado.Name = "sueloComprado";
            // 
            // dueño
            // 
            resources.ApplyResources(this.dueño, "dueño");
            this.dueño.Name = "dueño";
            // 
            // n_entradas
            // 
            resources.ApplyResources(this.n_entradas, "n_entradas");
            this.n_entradas.Name = "n_entradas";
            // 
            // VerHoteles
            // 
            this.AcceptButton = this.bOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bOK;
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