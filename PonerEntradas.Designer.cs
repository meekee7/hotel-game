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
            this.bCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCerrar.Location = new System.Drawing.Point(169, 101);
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.Size = new System.Drawing.Size(75, 23);
            this.bCerrar.TabIndex = 0;
            this.bCerrar.Text = "Cerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
            // 
            // listaHoteles
            // 
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Location = new System.Drawing.Point(123, 6);
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.Size = new System.Drawing.Size(121, 21);
            this.listaHoteles.TabIndex = 4;
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // selHotel
            // 
            this.selHotel.AutoSize = true;
            this.selHotel.Location = new System.Drawing.Point(12, 9);
            this.selHotel.Name = "selHotel";
            this.selHotel.Size = new System.Drawing.Size(100, 13);
            this.selHotel.TabIndex = 3;
            this.selHotel.Text = "Selecciona el hotel:";
            // 
            // listaCasillas
            // 
            this.listaCasillas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaCasillas.Enabled = false;
            this.listaCasillas.FormattingEnabled = true;
            this.listaCasillas.Location = new System.Drawing.Point(123, 54);
            this.listaCasillas.Name = "listaCasillas";
            this.listaCasillas.Size = new System.Drawing.Size(121, 21);
            this.listaCasillas.TabIndex = 5;
            this.listaCasillas.SelectedIndexChanged += new System.EventHandler(this.listaCasillas_SelectedIndexChanged);
            // 
            // selCasilla
            // 
            this.selCasilla.AutoSize = true;
            this.selCasilla.Location = new System.Drawing.Point(12, 57);
            this.selCasilla.Name = "selCasilla";
            this.selCasilla.Size = new System.Drawing.Size(106, 13);
            this.selCasilla.TabIndex = 6;
            this.selCasilla.Text = "Selecciona la casilla:";
            // 
            // bUnaMas
            // 
            this.bUnaMas.Enabled = false;
            this.bUnaMas.Location = new System.Drawing.Point(88, 101);
            this.bUnaMas.Name = "bUnaMas";
            this.bUnaMas.Size = new System.Drawing.Size(75, 23);
            this.bUnaMas.TabIndex = 7;
            this.bUnaMas.Text = "Una más";
            this.bUnaMas.UseVisualStyleBackColor = true;
            this.bUnaMas.Click += new System.EventHandler(this.bUnaMas_Click);
            // 
            // bComprar
            // 
            this.bComprar.Enabled = false;
            this.bComprar.Location = new System.Drawing.Point(7, 101);
            this.bComprar.Name = "bComprar";
            this.bComprar.Size = new System.Drawing.Size(75, 23);
            this.bComprar.TabIndex = 8;
            this.bComprar.Text = "Comprar";
            this.bComprar.UseVisualStyleBackColor = true;
            this.bComprar.Click += new System.EventHandler(this.bComprar_Click);
            // 
            // PonerEntradas
            // 
            this.AcceptButton = this.bCerrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCerrar;
            this.ClientSize = new System.Drawing.Size(256, 136);
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
            this.Text = "Poner Entradas";
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