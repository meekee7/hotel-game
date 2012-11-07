namespace Juego_Hotel
{
    partial class ReporteBug
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReporteBug));
            this.TextoBug = new System.Windows.Forms.TextBox();
            this.bEnviar = new System.Windows.Forms.Button();
            this.labelTitulo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextoBug
            // 
            this.TextoBug.AcceptsReturn = true;
            this.TextoBug.AcceptsTab = true;
            resources.ApplyResources(this.TextoBug, "TextoBug");
            this.TextoBug.Name = "TextoBug";
            // 
            // bEnviar
            // 
            resources.ApplyResources(this.bEnviar, "bEnviar");
            this.bEnviar.Name = "bEnviar";
            this.bEnviar.UseVisualStyleBackColor = true;
            this.bEnviar.Click += new System.EventHandler(this.bEnviar_Click);
            // 
            // labelTitulo
            // 
            resources.ApplyResources(this.labelTitulo, "labelTitulo");
            this.labelTitulo.Name = "labelTitulo";
            // 
            // ReporteBug
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelTitulo);
            this.Controls.Add(this.bEnviar);
            this.Controls.Add(this.TextoBug);
            this.Name = "ReporteBug";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextoBug;
        private System.Windows.Forms.Button bEnviar;
        private System.Windows.Forms.Label labelTitulo;
    }
}