namespace Juego_Hotel
{
    partial class PonerEntradas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PonerEntradas));
            this.bCerrar = new System.Windows.Forms.Button();
            this.listaHoteles = new System.Windows.Forms.ComboBox();
            this.selHotel = new System.Windows.Forms.Label();
            this.listaCasillas = new System.Windows.Forms.ComboBox();
            this.selCasilla = new System.Windows.Forms.Label();
            this.bUnaMas = new System.Windows.Forms.Button();
            this.bComprar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bCerrar
            // 
            resources.ApplyResources(this.bCerrar, "bCerrar");
            this.bCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
            // 
            // listaHoteles
            // 
            resources.ApplyResources(this.listaHoteles, "listaHoteles");
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // selHotel
            // 
            resources.ApplyResources(this.selHotel, "selHotel");
            this.selHotel.Name = "selHotel";
            // 
            // listaCasillas
            // 
            resources.ApplyResources(this.listaCasillas, "listaCasillas");
            this.listaCasillas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaCasillas.FormattingEnabled = true;
            this.listaCasillas.Name = "listaCasillas";
            this.listaCasillas.SelectedIndexChanged += new System.EventHandler(this.listaCasillas_SelectedIndexChanged);
            // 
            // selCasilla
            // 
            resources.ApplyResources(this.selCasilla, "selCasilla");
            this.selCasilla.Name = "selCasilla";
            // 
            // bUnaMas
            // 
            resources.ApplyResources(this.bUnaMas, "bUnaMas");
            this.bUnaMas.Name = "bUnaMas";
            this.bUnaMas.UseVisualStyleBackColor = true;
            this.bUnaMas.Click += new System.EventHandler(this.bUnaMas_Click);
            // 
            // bComprar
            // 
            resources.ApplyResources(this.bComprar, "bComprar");
            this.bComprar.Name = "bComprar";
            this.bComprar.UseVisualStyleBackColor = true;
            this.bComprar.Click += new System.EventHandler(this.bComprar_Click);
            // 
            // PonerEntradas
            // 
            this.AcceptButton = this.bCerrar;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCerrar;
            this.ControlBox = false;
            this.Controls.Add(this.bComprar);
            this.Controls.Add(this.bUnaMas);
            this.Controls.Add(this.selCasilla);
            this.Controls.Add(this.listaCasillas);
            this.Controls.Add(this.listaHoteles);
            this.Controls.Add(this.selHotel);
            this.Controls.Add(this.bCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PonerEntradas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bCerrar;
        private System.Windows.Forms.ComboBox listaHoteles;
        private System.Windows.Forms.Label selHotel;
        private System.Windows.Forms.ComboBox listaCasillas;
        private System.Windows.Forms.Label selCasilla;
        private System.Windows.Forms.Button bUnaMas;
        private System.Windows.Forms.Button bComprar;
    }
}