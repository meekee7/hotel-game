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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dado_construccion));
            this.cuadro = new System.Windows.Forms.PictureBox();
            this.bCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cuadro)).BeginInit();
            this.SuspendLayout();
            // 
            // cuadro
            // 
            resources.ApplyResources(this.cuadro, "cuadro");
            this.cuadro.Name = "cuadro";
            this.cuadro.TabStop = false;
            this.cuadro.Click += new System.EventHandler(this.cuadro_Click);
            // 
            // bCancelar
            // 
            resources.ApplyResources(this.bCancelar, "bCancelar");
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            // 
            // Dado_construccion
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ControlBox = false;
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.cuadro);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Dado_construccion";
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