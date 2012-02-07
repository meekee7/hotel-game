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
            this.img_reglas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.img_reglas)).BeginInit();
            this.SuspendLayout();
            // 
            // img_reglas
            // 
            this.img_reglas.Image = global::Juego_Hotel.Properties.Resources.Reglas;
            this.img_reglas.Location = new System.Drawing.Point(0, 0);
            this.img_reglas.Name = "img_reglas";
            this.img_reglas.Size = new System.Drawing.Size(2816, 1800);
            this.img_reglas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img_reglas.TabIndex = 0;
            this.img_reglas.TabStop = false;
            // 
            // Reglas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(879, 599);
            this.Controls.Add(this.img_reglas);
            this.Name = "Reglas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reglas del juego";
            ((System.ComponentModel.ISupportInitialize)(this.img_reglas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox img_reglas;
    }
}