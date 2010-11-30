namespace Juego_Hotel
{
    partial class Subastas
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
            this.etiHotel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.bHotel = new System.Windows.Forms.Button();
            this.grupoHotel = new System.Windows.Forms.GroupBox();
            this.grupoHotel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bCerrar
            // 
            this.bCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCerrar.Location = new System.Drawing.Point(486, 273);
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.Size = new System.Drawing.Size(75, 23);
            this.bCerrar.TabIndex = 0;
            this.bCerrar.Text = "Cerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            // 
            // etiHotel
            // 
            this.etiHotel.AutoSize = true;
            this.etiHotel.Location = new System.Drawing.Point(10, 42);
            this.etiHotel.Name = "etiHotel";
            this.etiHotel.Size = new System.Drawing.Size(91, 13);
            this.etiHotel.TabIndex = 1;
            this.etiHotel.Text = "Selecciona Hotel:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(107, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 2;
            // 
            // bHotel
            // 
            this.bHotel.Location = new System.Drawing.Point(62, 71);
            this.bHotel.Name = "bHotel";
            this.bHotel.Size = new System.Drawing.Size(75, 23);
            this.bHotel.TabIndex = 3;
            this.bHotel.Text = "Subastar";
            this.bHotel.UseVisualStyleBackColor = true;
            // 
            // grupoHotel
            // 
            this.grupoHotel.Controls.Add(this.bHotel);
            this.grupoHotel.Controls.Add(this.etiHotel);
            this.grupoHotel.Controls.Add(this.comboBox1);
            this.grupoHotel.Location = new System.Drawing.Point(50, 104);
            this.grupoHotel.Name = "grupoHotel";
            this.grupoHotel.Size = new System.Drawing.Size(234, 100);
            this.grupoHotel.TabIndex = 4;
            this.grupoHotel.TabStop = false;
            this.grupoHotel.Text = "Subastar Hotel";
            // 
            // Subastas
            // 
            this.AcceptButton = this.bCerrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCerrar;
            this.ClientSize = new System.Drawing.Size(573, 308);
            this.ControlBox = false;
            this.Controls.Add(this.grupoHotel);
            this.Controls.Add(this.bCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Subastas";
            this.Text = "Subastas";
            this.grupoHotel.ResumeLayout(false);
            this.grupoHotel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bCerrar;
        private System.Windows.Forms.Label etiHotel;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button bHotel;
        private System.Windows.Forms.GroupBox grupoHotel;
    }
}