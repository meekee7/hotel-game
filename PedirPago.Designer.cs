namespace Juego_Hotel
{
    partial class PedirPago
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
            this.img100 = new System.Windows.Forms.PictureBox();
            this.img50 = new System.Windows.Forms.PictureBox();
            this.img500 = new System.Windows.Forms.PictureBox();
            this.img1000 = new System.Windows.Forms.PictureBox();
            this.img5000 = new System.Windows.Forms.PictureBox();
            this.bOk = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.n50 = new System.Windows.Forms.Label();
            this.n100 = new System.Windows.Forms.Label();
            this.n500 = new System.Windows.Forms.Label();
            this.n1000 = new System.Windows.Forms.Label();
            this.n5000 = new System.Windows.Forms.Label();
            this.total = new System.Windows.Forms.Label();
            this.n50j = new System.Windows.Forms.Label();
            this.n100j = new System.Windows.Forms.Label();
            this.n500j = new System.Windows.Forms.Label();
            this.n5000j = new System.Windows.Forms.Label();
            this.n1000j = new System.Windows.Forms.Label();
            this.bReset = new System.Windows.Forms.Button();
            this.necesario = new System.Windows.Forms.Label();
            this.bSubastar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.img100)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img500)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img1000)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.img5000)).BeginInit();
            this.SuspendLayout();
            // 
            // img100
            // 
            this.img100.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.img100.Cursor = System.Windows.Forms.Cursors.Hand;
            this.img100.Image = global::Juego_Hotel.Properties.Resources.Billete_100;
            this.img100.Location = new System.Drawing.Point(318, 12);
            this.img100.Name = "img100";
            this.img100.Size = new System.Drawing.Size(300, 125);
            this.img100.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img100.TabIndex = 1;
            this.img100.TabStop = false;
            this.img100.Click += new System.EventHandler(this.img100_Click);
            // 
            // img50
            // 
            this.img50.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.img50.Cursor = System.Windows.Forms.Cursors.Hand;
            this.img50.Image = global::Juego_Hotel.Properties.Resources.Billete_50;
            this.img50.Location = new System.Drawing.Point(12, 12);
            this.img50.Name = "img50";
            this.img50.Size = new System.Drawing.Size(300, 125);
            this.img50.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img50.TabIndex = 0;
            this.img50.TabStop = false;
            this.img50.Click += new System.EventHandler(this.img50_Click);
            // 
            // img500
            // 
            this.img500.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.img500.Cursor = System.Windows.Forms.Cursors.Hand;
            this.img500.Image = global::Juego_Hotel.Properties.Resources.Billete_500;
            this.img500.Location = new System.Drawing.Point(624, 12);
            this.img500.Name = "img500";
            this.img500.Size = new System.Drawing.Size(300, 125);
            this.img500.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img500.TabIndex = 2;
            this.img500.TabStop = false;
            this.img500.Click += new System.EventHandler(this.img500_Click);
            // 
            // img1000
            // 
            this.img1000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.img1000.Cursor = System.Windows.Forms.Cursors.Hand;
            this.img1000.Image = global::Juego_Hotel.Properties.Resources.Billete_1000;
            this.img1000.Location = new System.Drawing.Point(164, 188);
            this.img1000.Name = "img1000";
            this.img1000.Size = new System.Drawing.Size(300, 125);
            this.img1000.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img1000.TabIndex = 3;
            this.img1000.TabStop = false;
            this.img1000.Click += new System.EventHandler(this.img1000_Click);
            // 
            // img5000
            // 
            this.img5000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.img5000.Cursor = System.Windows.Forms.Cursors.Hand;
            this.img5000.Image = global::Juego_Hotel.Properties.Resources.Billete_5000;
            this.img5000.Location = new System.Drawing.Point(470, 188);
            this.img5000.Name = "img5000";
            this.img5000.Size = new System.Drawing.Size(300, 125);
            this.img5000.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.img5000.TabIndex = 4;
            this.img5000.TabStop = false;
            this.img5000.Click += new System.EventHandler(this.img5000_Click);
            // 
            // bOk
            // 
            this.bOk.Location = new System.Drawing.Point(12, 385);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 23);
            this.bOk.TabIndex = 5;
            this.bOk.Text = "OK";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bCancelar
            // 
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Enabled = false;
            this.bCancelar.Location = new System.Drawing.Point(261, 385);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(75, 23);
            this.bCancelar.TabIndex = 6;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // n50
            // 
            this.n50.AutoSize = true;
            this.n50.Location = new System.Drawing.Point(163, 141);
            this.n50.Name = "n50";
            this.n50.Size = new System.Drawing.Size(43, 13);
            this.n50.TabIndex = 7;
            this.n50.Text = "Usas: 0";
            // 
            // n100
            // 
            this.n100.AutoSize = true;
            this.n100.Location = new System.Drawing.Point(472, 141);
            this.n100.Name = "n100";
            this.n100.Size = new System.Drawing.Size(43, 13);
            this.n100.TabIndex = 8;
            this.n100.Text = "Usas: 0";
            // 
            // n500
            // 
            this.n500.AutoSize = true;
            this.n500.Location = new System.Drawing.Point(778, 141);
            this.n500.Name = "n500";
            this.n500.Size = new System.Drawing.Size(43, 13);
            this.n500.TabIndex = 9;
            this.n500.Text = "Usas: 0";
            // 
            // n1000
            // 
            this.n1000.AutoSize = true;
            this.n1000.Location = new System.Drawing.Point(318, 316);
            this.n1000.Name = "n1000";
            this.n1000.Size = new System.Drawing.Size(43, 13);
            this.n1000.TabIndex = 10;
            this.n1000.Text = "Usas: 0";
            // 
            // n5000
            // 
            this.n5000.AutoSize = true;
            this.n5000.Location = new System.Drawing.Point(624, 316);
            this.n5000.Name = "n5000";
            this.n5000.Size = new System.Drawing.Size(43, 13);
            this.n5000.TabIndex = 11;
            this.n5000.Text = "Usas: 0";
            // 
            // total
            // 
            this.total.AutoSize = true;
            this.total.Location = new System.Drawing.Point(564, 354);
            this.total.Name = "total";
            this.total.Size = new System.Drawing.Size(43, 13);
            this.total.TabIndex = 12;
            this.total.Text = "Total: 0";
            // 
            // n50j
            // 
            this.n50j.AutoSize = true;
            this.n50j.Location = new System.Drawing.Point(106, 141);
            this.n50j.Name = "n50j";
            this.n50j.Size = new System.Drawing.Size(51, 13);
            this.n50j.TabIndex = 13;
            this.n50j.Text = "Tienes: 0";
            // 
            // n100j
            // 
            this.n100j.AutoSize = true;
            this.n100j.Location = new System.Drawing.Point(412, 141);
            this.n100j.Name = "n100j";
            this.n100j.Size = new System.Drawing.Size(51, 13);
            this.n100j.TabIndex = 14;
            this.n100j.Text = "Tienes: 0";
            // 
            // n500j
            // 
            this.n500j.AutoSize = true;
            this.n500j.Location = new System.Drawing.Point(721, 141);
            this.n500j.Name = "n500j";
            this.n500j.Size = new System.Drawing.Size(51, 13);
            this.n500j.TabIndex = 15;
            this.n500j.Text = "Tienes: 0";
            // 
            // n5000j
            // 
            this.n5000j.AutoSize = true;
            this.n5000j.Location = new System.Drawing.Point(564, 316);
            this.n5000j.Name = "n5000j";
            this.n5000j.Size = new System.Drawing.Size(51, 13);
            this.n5000j.TabIndex = 16;
            this.n5000j.Text = "Tienes: 0";
            // 
            // n1000j
            // 
            this.n1000j.AutoSize = true;
            this.n1000j.Location = new System.Drawing.Point(258, 316);
            this.n1000j.Name = "n1000j";
            this.n1000j.Size = new System.Drawing.Size(51, 13);
            this.n1000j.TabIndex = 17;
            this.n1000j.Text = "Tienes: 0";
            // 
            // bReset
            // 
            this.bReset.Location = new System.Drawing.Point(567, 385);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(80, 23);
            this.bReset.TabIndex = 18;
            this.bReset.Text = "Reestablecer";
            this.bReset.UseVisualStyleBackColor = true;
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // necesario
            // 
            this.necesario.AutoSize = true;
            this.necesario.Location = new System.Drawing.Point(258, 354);
            this.necesario.Name = "necesario";
            this.necesario.Size = new System.Drawing.Size(58, 13);
            this.necesario.TabIndex = 19;
            this.necesario.Text = "Necesario:";
            // 
            // bSubastar
            // 
            this.bSubastar.Location = new System.Drawing.Point(849, 385);
            this.bSubastar.Name = "bSubastar";
            this.bSubastar.Size = new System.Drawing.Size(75, 23);
            this.bSubastar.TabIndex = 20;
            this.bSubastar.Text = "Subastar";
            this.bSubastar.UseVisualStyleBackColor = true;
            this.bSubastar.Click += new System.EventHandler(this.bSubastar_Click);
            // 
            // PedirPago
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(936, 420);
            this.ControlBox = false;
            this.Controls.Add(this.bSubastar);
            this.Controls.Add(this.necesario);
            this.Controls.Add(this.bReset);
            this.Controls.Add(this.n1000j);
            this.Controls.Add(this.n5000j);
            this.Controls.Add(this.n500j);
            this.Controls.Add(this.n100j);
            this.Controls.Add(this.n50j);
            this.Controls.Add(this.total);
            this.Controls.Add(this.n5000);
            this.Controls.Add(this.n1000);
            this.Controls.Add(this.n500);
            this.Controls.Add(this.n100);
            this.Controls.Add(this.n50);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.img5000);
            this.Controls.Add(this.img1000);
            this.Controls.Add(this.img500);
            this.Controls.Add(this.img100);
            this.Controls.Add(this.img50);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PedirPago";
            this.Text = "Seleccione billetes";
            ((System.ComponentModel.ISupportInitialize)(this.img100)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img500)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img1000)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.img5000)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox img50;
        private System.Windows.Forms.PictureBox img100;
        private System.Windows.Forms.PictureBox img500;
        private System.Windows.Forms.PictureBox img1000;
        private System.Windows.Forms.PictureBox img5000;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bCancelar;
        private System.Windows.Forms.Label n50;
        private System.Windows.Forms.Label n100;
        private System.Windows.Forms.Label n500;
        private System.Windows.Forms.Label n1000;
        private System.Windows.Forms.Label n5000;
        private System.Windows.Forms.Label total;
        private System.Windows.Forms.Label n50j;
        private System.Windows.Forms.Label n100j;
        private System.Windows.Forms.Label n500j;
        private System.Windows.Forms.Label n5000j;
        private System.Windows.Forms.Label n1000j;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.Label necesario;
        private System.Windows.Forms.Button bSubastar;

    }
}