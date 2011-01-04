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
            this.listaHoteles = new System.Windows.Forms.ComboBox();
            this.bHotel = new System.Windows.Forms.Button();
            this.grupoHotel = new System.Windows.Forms.GroupBox();
            this.bVerHoteles = new System.Windows.Forms.Button();
            this.txtCantidad = new System.Windows.Forms.Label();
            this.bJ1 = new System.Windows.Forms.Button();
            this.bJ3 = new System.Windows.Forms.Button();
            this.bJ2 = new System.Windows.Forms.Button();
            this.bJ4 = new System.Windows.Forms.Button();
            this.cantidad = new System.Windows.Forms.TextBox();
            this.txt_precio_mayor = new System.Windows.Forms.Label();
            this.precio_mayor = new System.Windows.Forms.Label();
            this.grupoEstado = new System.Windows.Forms.GroupBox();
            this.bVender = new System.Windows.Forms.Button();
            this.mayor_postor = new System.Windows.Forms.Label();
            this.txt_mayor_postor = new System.Windows.Forms.Label();
            this.grupoHotel.SuspendLayout();
            this.grupoEstado.SuspendLayout();
            this.SuspendLayout();
            // 
            // bCerrar
            // 
            this.bCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCerrar.Location = new System.Drawing.Point(34, 188);
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.Size = new System.Drawing.Size(79, 23);
            this.bCerrar.TabIndex = 0;
            this.bCerrar.Text = "Cerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
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
            // listaHoteles
            // 
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Location = new System.Drawing.Point(107, 39);
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.Size = new System.Drawing.Size(121, 21);
            this.listaHoteles.TabIndex = 2;
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // bHotel
            // 
            this.bHotel.Enabled = false;
            this.bHotel.Location = new System.Drawing.Point(107, 71);
            this.bHotel.Name = "bHotel";
            this.bHotel.Size = new System.Drawing.Size(75, 23);
            this.bHotel.TabIndex = 3;
            this.bHotel.Text = "Subastar";
            this.bHotel.UseVisualStyleBackColor = true;
            this.bHotel.Click += new System.EventHandler(this.bHotel_Click);
            // 
            // grupoHotel
            // 
            this.grupoHotel.Controls.Add(this.bHotel);
            this.grupoHotel.Controls.Add(this.etiHotel);
            this.grupoHotel.Controls.Add(this.listaHoteles);
            this.grupoHotel.Location = new System.Drawing.Point(21, 22);
            this.grupoHotel.Name = "grupoHotel";
            this.grupoHotel.Size = new System.Drawing.Size(248, 100);
            this.grupoHotel.TabIndex = 4;
            this.grupoHotel.TabStop = false;
            this.grupoHotel.Text = "Subastar Hotel";
            // 
            // bVerHoteles
            // 
            this.bVerHoteles.Location = new System.Drawing.Point(170, 188);
            this.bVerHoteles.Name = "bVerHoteles";
            this.bVerHoteles.Size = new System.Drawing.Size(79, 23);
            this.bVerHoteles.TabIndex = 4;
            this.bVerHoteles.Text = "Ver Hoteles";
            this.bVerHoteles.UseVisualStyleBackColor = true;
            this.bVerHoteles.Click += new System.EventHandler(this.bVerHoteles_Click);
            // 
            // txtCantidad
            // 
            this.txtCantidad.AutoSize = true;
            this.txtCantidad.Location = new System.Drawing.Point(21, 30);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(97, 13);
            this.txtCantidad.TabIndex = 5;
            this.txtCantidad.Text = "Cantidad a ofrecer:";
            // 
            // bJ1
            // 
            this.bJ1.Enabled = false;
            this.bJ1.Location = new System.Drawing.Point(43, 54);
            this.bJ1.Name = "bJ1";
            this.bJ1.Size = new System.Drawing.Size(75, 23);
            this.bJ1.TabIndex = 6;
            this.bJ1.Text = "Pujar J1";
            this.bJ1.UseVisualStyleBackColor = true;
            this.bJ1.Click += new System.EventHandler(this.bJ1_Click);
            // 
            // bJ3
            // 
            this.bJ3.Enabled = false;
            this.bJ3.Location = new System.Drawing.Point(43, 83);
            this.bJ3.Name = "bJ3";
            this.bJ3.Size = new System.Drawing.Size(75, 23);
            this.bJ3.TabIndex = 7;
            this.bJ3.Text = "Pujar J3";
            this.bJ3.UseVisualStyleBackColor = true;
            this.bJ3.Click += new System.EventHandler(this.bJ3_Click);
            // 
            // bJ2
            // 
            this.bJ2.Enabled = false;
            this.bJ2.Location = new System.Drawing.Point(124, 54);
            this.bJ2.Name = "bJ2";
            this.bJ2.Size = new System.Drawing.Size(75, 23);
            this.bJ2.TabIndex = 8;
            this.bJ2.Text = "Pujar J2";
            this.bJ2.UseVisualStyleBackColor = true;
            this.bJ2.Click += new System.EventHandler(this.bJ2_Click);
            // 
            // bJ4
            // 
            this.bJ4.Enabled = false;
            this.bJ4.Location = new System.Drawing.Point(124, 83);
            this.bJ4.Name = "bJ4";
            this.bJ4.Size = new System.Drawing.Size(75, 23);
            this.bJ4.TabIndex = 9;
            this.bJ4.Text = "Pujar J4";
            this.bJ4.UseVisualStyleBackColor = true;
            this.bJ4.Click += new System.EventHandler(this.bJ4_Click);
            // 
            // cantidad
            // 
            this.cantidad.Enabled = false;
            this.cantidad.Location = new System.Drawing.Point(124, 28);
            this.cantidad.Name = "cantidad";
            this.cantidad.Size = new System.Drawing.Size(75, 20);
            this.cantidad.TabIndex = 10;
            // 
            // txt_precio_mayor
            // 
            this.txt_precio_mayor.AutoSize = true;
            this.txt_precio_mayor.Location = new System.Drawing.Point(31, 117);
            this.txt_precio_mayor.Name = "txt_precio_mayor";
            this.txt_precio_mayor.Size = new System.Drawing.Size(87, 13);
            this.txt_precio_mayor.TabIndex = 11;
            this.txt_precio_mayor.Text = "Máximo ofrecido:";
            // 
            // precio_mayor
            // 
            this.precio_mayor.AutoSize = true;
            this.precio_mayor.Location = new System.Drawing.Point(121, 117);
            this.precio_mayor.Name = "precio_mayor";
            this.precio_mayor.Size = new System.Drawing.Size(0, 13);
            this.precio_mayor.TabIndex = 12;
            // 
            // grupoEstado
            // 
            this.grupoEstado.Controls.Add(this.bVender);
            this.grupoEstado.Controls.Add(this.mayor_postor);
            this.grupoEstado.Controls.Add(this.txt_mayor_postor);
            this.grupoEstado.Controls.Add(this.txtCantidad);
            this.grupoEstado.Controls.Add(this.precio_mayor);
            this.grupoEstado.Controls.Add(this.bJ1);
            this.grupoEstado.Controls.Add(this.txt_precio_mayor);
            this.grupoEstado.Controls.Add(this.bJ3);
            this.grupoEstado.Controls.Add(this.cantidad);
            this.grupoEstado.Controls.Add(this.bJ2);
            this.grupoEstado.Controls.Add(this.bJ4);
            this.grupoEstado.Location = new System.Drawing.Point(285, 22);
            this.grupoEstado.Name = "grupoEstado";
            this.grupoEstado.Size = new System.Drawing.Size(226, 201);
            this.grupoEstado.TabIndex = 13;
            this.grupoEstado.TabStop = false;
            this.grupoEstado.Text = "Estado de la subasta";
            // 
            // bVender
            // 
            this.bVender.Location = new System.Drawing.Point(79, 166);
            this.bVender.Name = "bVender";
            this.bVender.Size = new System.Drawing.Size(75, 23);
            this.bVender.TabIndex = 14;
            this.bVender.Text = "Vender";
            this.bVender.UseVisualStyleBackColor = true;
            this.bVender.Click += new System.EventHandler(this.bVender_Click);
            // 
            // mayor_postor
            // 
            this.mayor_postor.AutoSize = true;
            this.mayor_postor.Location = new System.Drawing.Point(121, 141);
            this.mayor_postor.Name = "mayor_postor";
            this.mayor_postor.Size = new System.Drawing.Size(0, 13);
            this.mayor_postor.TabIndex = 15;
            // 
            // txt_mayor_postor
            // 
            this.txt_mayor_postor.AutoSize = true;
            this.txt_mayor_postor.Location = new System.Drawing.Point(47, 141);
            this.txt_mayor_postor.Name = "txt_mayor_postor";
            this.txt_mayor_postor.Size = new System.Drawing.Size(71, 13);
            this.txt_mayor_postor.TabIndex = 14;
            this.txt_mayor_postor.Text = "Mayor postor:";
            // 
            // Subastas
            // 
            this.AcceptButton = this.bCerrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCerrar;
            this.ClientSize = new System.Drawing.Size(530, 240);
            this.ControlBox = false;
            this.Controls.Add(this.grupoEstado);
            this.Controls.Add(this.bVerHoteles);
            this.Controls.Add(this.grupoHotel);
            this.Controls.Add(this.bCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Subastas";
            this.Text = "Subastas";
            this.grupoHotel.ResumeLayout(false);
            this.grupoHotel.PerformLayout();
            this.grupoEstado.ResumeLayout(false);
            this.grupoEstado.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bCerrar;
        private System.Windows.Forms.Label etiHotel;
        private System.Windows.Forms.ComboBox listaHoteles;
        private System.Windows.Forms.Button bHotel;
        private System.Windows.Forms.GroupBox grupoHotel;
        private System.Windows.Forms.Button bVerHoteles;
        private System.Windows.Forms.Label txtCantidad;
        private System.Windows.Forms.Button bJ1;
        private System.Windows.Forms.Button bJ3;
        private System.Windows.Forms.Button bJ2;
        private System.Windows.Forms.Button bJ4;
        private System.Windows.Forms.TextBox cantidad;
        private System.Windows.Forms.Label txt_precio_mayor;
        private System.Windows.Forms.Label precio_mayor;
        private System.Windows.Forms.GroupBox grupoEstado;
        private System.Windows.Forms.Label mayor_postor;
        private System.Windows.Forms.Label txt_mayor_postor;
        private System.Windows.Forms.Button bVender;
    }
}