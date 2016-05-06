namespace Juego_Hotel
{
    partial class Reglas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Reglas));
            this.img_reglas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.img_reglas)).BeginInit();
            this.SuspendLayout();
            // 
            // img_reglas
            // 
            this.img_reglas.Image = global::Juego_Hotel.Properties.Resources.Reglas_es;
            resources.ApplyResources(this.img_reglas, "img_reglas");
            this.img_reglas.Name = "img_reglas";
            this.img_reglas.TabStop = false;
            // 
            // Reglas
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.img_reglas);
            this.Name = "Reglas";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Reglas_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.img_reglas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox img_reglas;
    }
}