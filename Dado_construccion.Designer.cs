namespace Juego_Hotel
{
    partial class Dado_construccion
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
            this.cuadro = new System.Windows.Forms.PictureBox();
            this.bCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cuadro)).BeginInit();
            this.SuspendLayout();
            // 
            // cuadro
            // 
            this.cuadro.Location = new System.Drawing.Point(12, 12);
            this.cuadro.Name = "cuadro";
            this.cuadro.Size = new System.Drawing.Size(170, 148);
            this.cuadro.TabIndex = 0;
            this.cuadro.TabStop = false;
            this.cuadro.Click += new System.EventHandler(this.cuadro_Click);
            // 
            // bCancelar
            // 
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Enabled = false;
            this.bCancelar.Location = new System.Drawing.Point(60, 171);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(75, 23);
            this.bCancelar.TabIndex = 1;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            // 
            // Dado_construccion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(194, 206);
            this.ControlBox = false;
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.cuadro);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Dado_construccion";
            this.Text = "Dado de construcción";
            this.Load += new System.EventHandler(this.Dado_construccion_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Dado_construccion_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.cuadro)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox cuadro;
        private System.Windows.Forms.Button bCancelar;
    }
}