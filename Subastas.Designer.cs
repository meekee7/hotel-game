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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Subastas));
            this.bCerrar = new System.Windows.Forms.Button();
            this.etiHotel = new System.Windows.Forms.Label();
            this.listaHoteles = new System.Windows.Forms.ComboBox();
            this.bSubastar = new System.Windows.Forms.Button();
            this.grupoHotel = new System.Windows.Forms.GroupBox();
            this.precioMinimo = new System.Windows.Forms.Label();
            this.etiPrecioMinimo = new System.Windows.Forms.Label();
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
            resources.ApplyResources(this.bCerrar, "bCerrar");
            this.bCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
            // 
            // etiHotel
            // 
            resources.ApplyResources(this.etiHotel, "etiHotel");
            this.etiHotel.Name = "etiHotel";
            // 
            // listaHoteles
            // 
            resources.ApplyResources(this.listaHoteles, "listaHoteles");
            this.listaHoteles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listaHoteles.FormattingEnabled = true;
            this.listaHoteles.Name = "listaHoteles";
            this.listaHoteles.SelectedIndexChanged += new System.EventHandler(this.listaHoteles_SelectedIndexChanged);
            // 
            // bSubastar
            // 
            resources.ApplyResources(this.bSubastar, "bSubastar");
            this.bSubastar.Name = "bSubastar";
            this.bSubastar.UseVisualStyleBackColor = true;
            this.bSubastar.Click += new System.EventHandler(this.bSubastar_Click);
            // 
            // grupoHotel
            // 
            resources.ApplyResources(this.grupoHotel, "grupoHotel");
            this.grupoHotel.Controls.Add(this.precioMinimo);
            this.grupoHotel.Controls.Add(this.etiPrecioMinimo);
            this.grupoHotel.Controls.Add(this.bSubastar);
            this.grupoHotel.Controls.Add(this.etiHotel);
            this.grupoHotel.Controls.Add(this.listaHoteles);
            this.grupoHotel.Name = "grupoHotel";
            this.grupoHotel.TabStop = false;
            // 
            // precioMinimo
            // 
            resources.ApplyResources(this.precioMinimo, "precioMinimo");
            this.precioMinimo.Name = "precioMinimo";
            // 
            // etiPrecioMinimo
            // 
            resources.ApplyResources(this.etiPrecioMinimo, "etiPrecioMinimo");
            this.etiPrecioMinimo.Name = "etiPrecioMinimo";
            // 
            // bVerHoteles
            // 
            resources.ApplyResources(this.bVerHoteles, "bVerHoteles");
            this.bVerHoteles.Name = "bVerHoteles";
            this.bVerHoteles.UseVisualStyleBackColor = true;
            this.bVerHoteles.Click += new System.EventHandler(this.bVerHoteles_Click);
            // 
            // txtCantidad
            // 
            resources.ApplyResources(this.txtCantidad, "txtCantidad");
            this.txtCantidad.Name = "txtCantidad";
            // 
            // bJ1
            // 
            resources.ApplyResources(this.bJ1, "bJ1");
            this.bJ1.Name = "bJ1";
            this.bJ1.UseVisualStyleBackColor = true;
            this.bJ1.Click += new System.EventHandler(this.bJ1_Click);
            // 
            // bJ3
            // 
            resources.ApplyResources(this.bJ3, "bJ3");
            this.bJ3.Name = "bJ3";
            this.bJ3.UseVisualStyleBackColor = true;
            this.bJ3.Click += new System.EventHandler(this.bJ3_Click);
            // 
            // bJ2
            // 
            resources.ApplyResources(this.bJ2, "bJ2");
            this.bJ2.Name = "bJ2";
            this.bJ2.UseVisualStyleBackColor = true;
            this.bJ2.Click += new System.EventHandler(this.bJ2_Click);
            // 
            // bJ4
            // 
            resources.ApplyResources(this.bJ4, "bJ4");
            this.bJ4.Name = "bJ4";
            this.bJ4.UseVisualStyleBackColor = true;
            this.bJ4.Click += new System.EventHandler(this.bJ4_Click);
            // 
            // cantidad
            // 
            resources.ApplyResources(this.cantidad, "cantidad");
            this.cantidad.Name = "cantidad";
            // 
            // txt_precio_mayor
            // 
            resources.ApplyResources(this.txt_precio_mayor, "txt_precio_mayor");
            this.txt_precio_mayor.Name = "txt_precio_mayor";
            // 
            // precio_mayor
            // 
            resources.ApplyResources(this.precio_mayor, "precio_mayor");
            this.precio_mayor.Name = "precio_mayor";
            // 
            // grupoEstado
            // 
            resources.ApplyResources(this.grupoEstado, "grupoEstado");
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
            this.grupoEstado.Name = "grupoEstado";
            this.grupoEstado.TabStop = false;
            // 
            // bVender
            // 
            resources.ApplyResources(this.bVender, "bVender");
            this.bVender.Name = "bVender";
            this.bVender.UseVisualStyleBackColor = true;
            this.bVender.Click += new System.EventHandler(this.bVender_Click);
            // 
            // mayor_postor
            // 
            resources.ApplyResources(this.mayor_postor, "mayor_postor");
            this.mayor_postor.Name = "mayor_postor";
            // 
            // txt_mayor_postor
            // 
            resources.ApplyResources(this.txt_mayor_postor, "txt_mayor_postor");
            this.txt_mayor_postor.Name = "txt_mayor_postor";
            // 
            // Subastas
            // 
            this.AcceptButton = this.bCerrar;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCerrar;
            this.ControlBox = false;
            this.Controls.Add(this.grupoEstado);
            this.Controls.Add(this.bVerHoteles);
            this.Controls.Add(this.grupoHotel);
            this.Controls.Add(this.bCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Subastas";
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
        private System.Windows.Forms.Button bSubastar;
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
        private System.Windows.Forms.Label etiPrecioMinimo;
        private System.Windows.Forms.Label precioMinimo;
    }
}