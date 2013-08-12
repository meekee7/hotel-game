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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComprarHotel));
            this.bIzq = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.cajaIzquierda = new System.Windows.Forms.GroupBox();
            this.precioIzq = new System.Windows.Forms.Label();
            this.nombreIzq = new System.Windows.Forms.Label();
            this.bDer = new System.Windows.Forms.Button();
            this.cajaDerecha = new System.Windows.Forms.GroupBox();
            this.precioDer = new System.Windows.Forms.Label();
            this.nombreDer = new System.Windows.Forms.Label();
            this.cajaIzquierda.SuspendLayout();
            this.cajaDerecha.SuspendLayout();
            this.SuspendLayout();
            // 
            // bIzq
            // 
            resources.ApplyResources(this.bIzq, "bIzq");
            this.bIzq.Name = "bIzq";
            this.bIzq.UseVisualStyleBackColor = true;
            this.bIzq.Click += new System.EventHandler(this.bIzq_Click);
            // 
            // bCancelar
            // 
            resources.ApplyResources(this.bCancelar, "bCancelar");
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // cajaIzquierda
            // 
            resources.ApplyResources(this.cajaIzquierda, "cajaIzquierda");
            this.cajaIzquierda.Controls.Add(this.precioIzq);
            this.cajaIzquierda.Controls.Add(this.bIzq);
            this.cajaIzquierda.Controls.Add(this.nombreIzq);
            this.cajaIzquierda.Name = "cajaIzquierda";
            this.cajaIzquierda.TabStop = false;
            // 
            // precioIzq
            // 
            resources.ApplyResources(this.precioIzq, "precioIzq");
            this.precioIzq.Name = "precioIzq";
            // 
            // nombreIzq
            // 
            resources.ApplyResources(this.nombreIzq, "nombreIzq");
            this.nombreIzq.Name = "nombreIzq";
            // 
            // bDer
            // 
            resources.ApplyResources(this.bDer, "bDer");
            this.bDer.Name = "bDer";
            this.bDer.UseVisualStyleBackColor = true;
            this.bDer.Click += new System.EventHandler(this.bDer_Click);
            // 
            // cajaDerecha
            // 
            resources.ApplyResources(this.cajaDerecha, "cajaDerecha");
            this.cajaDerecha.Controls.Add(this.precioDer);
            this.cajaDerecha.Controls.Add(this.bDer);
            this.cajaDerecha.Controls.Add(this.nombreDer);
            this.cajaDerecha.Name = "cajaDerecha";
            this.cajaDerecha.TabStop = false;
            // 
            // precioDer
            // 
            resources.ApplyResources(this.precioDer, "precioDer");
            this.precioDer.Name = "precioDer";
            // 
            // nombreDer
            // 
            resources.ApplyResources(this.nombreDer, "nombreDer");
            this.nombreDer.Name = "nombreDer";
            // 
            // ComprarHotel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ControlBox = false;
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.cajaIzquierda);
            this.Controls.Add(this.cajaDerecha);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ComprarHotel";
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
        private System.Windows.Forms.Label precioIzq;
        private System.Windows.Forms.Label precioDer;
    }
}