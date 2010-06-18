namespace Juego_Hotel
{
    partial class ComprarHotel
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
            this.bIzq = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.cajaIzquierda = new System.Windows.Forms.GroupBox();
            this.nombreIzq = new System.Windows.Forms.Label();
            this.bDer = new System.Windows.Forms.Button();
            this.cajaDerecha = new System.Windows.Forms.GroupBox();
            this.nombreDer = new System.Windows.Forms.Label();
            this.cajaIzquierda.SuspendLayout();
            this.cajaDerecha.SuspendLayout();
            this.SuspendLayout();
            // 
            // bIzq
            // 
            this.bIzq.Enabled = false;
            this.bIzq.Location = new System.Drawing.Point(26, 60);
            this.bIzq.Name = "bIzq";
            this.bIzq.Size = new System.Drawing.Size(90, 23);
            this.bIzq.TabIndex = 1;
            this.bIzq.Text = "Comprar";
            this.bIzq.UseVisualStyleBackColor = true;
            this.bIzq.Click += new System.EventHandler(this.bIzq_Click);
            // 
            // bCancelar
            // 
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Location = new System.Drawing.Point(110, 117);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(90, 23);
            this.bCancelar.TabIndex = 7;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // cajaIzquierda
            // 
            this.cajaIzquierda.Controls.Add(this.bIzq);
            this.cajaIzquierda.Controls.Add(this.nombreIzq);
            this.cajaIzquierda.Location = new System.Drawing.Point(7, 22);
            this.cajaIzquierda.Name = "cajaIzquierda";
            this.cajaIzquierda.Size = new System.Drawing.Size(140, 89);
            this.cajaIzquierda.TabIndex = 4;
            this.cajaIzquierda.TabStop = false;
            this.cajaIzquierda.Text = "Zona Izquierda";
            // 
            // nombreIzq
            // 
            this.nombreIzq.AutoSize = true;
            this.nombreIzq.Location = new System.Drawing.Point(27, 22);
            this.nombreIzq.Name = "nombreIzq";
            this.nombreIzq.Size = new System.Drawing.Size(0, 13);
            this.nombreIzq.TabIndex = 0;
            // 
            // bDer
            // 
            this.bDer.Enabled = false;
            this.bDer.Location = new System.Drawing.Point(29, 58);
            this.bDer.Name = "bDer";
            this.bDer.Size = new System.Drawing.Size(90, 23);
            this.bDer.TabIndex = 1;
            this.bDer.Text = "Comprar";
            this.bDer.UseVisualStyleBackColor = true;
            this.bDer.Click += new System.EventHandler(this.bDer_Click);
            // 
            // cajaDerecha
            // 
            this.cajaDerecha.Controls.Add(this.bDer);
            this.cajaDerecha.Controls.Add(this.nombreDer);
            this.cajaDerecha.Location = new System.Drawing.Point(158, 24);
            this.cajaDerecha.Name = "cajaDerecha";
            this.cajaDerecha.Size = new System.Drawing.Size(140, 87);
            this.cajaDerecha.TabIndex = 5;
            this.cajaDerecha.TabStop = false;
            this.cajaDerecha.Text = "Zona Derecha";
            // 
            // nombreDer
            // 
            this.nombreDer.AutoSize = true;
            this.nombreDer.Location = new System.Drawing.Point(27, 20);
            this.nombreDer.Name = "nombreDer";
            this.nombreDer.Size = new System.Drawing.Size(0, 13);
            this.nombreDer.TabIndex = 0;
            // 
            // ComprarHotel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(305, 151);
            this.ControlBox = false;
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.cajaIzquierda);
            this.Controls.Add(this.cajaDerecha);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ComprarHotel";
            this.Text = "Comprar Hotel";
            this.cajaIzquierda.ResumeLayout(false);
            this.cajaIzquierda.PerformLayout();
            this.cajaDerecha.ResumeLayout(false);
            this.cajaDerecha.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bIzq;
        private System.Windows.Forms.Button bCancelar;
        private System.Windows.Forms.GroupBox cajaIzquierda;
        private System.Windows.Forms.Label nombreIzq;
        private System.Windows.Forms.Button bDer;
        private System.Windows.Forms.GroupBox cajaDerecha;
        private System.Windows.Forms.Label nombreDer;
    }
}