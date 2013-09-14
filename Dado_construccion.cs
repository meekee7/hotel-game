using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Juego_Hotel.Resources;


namespace Juego_Hotel
{
    public partial class Dado_construccion : Form
    {
        Random rand;
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, System.Int32 dwRop);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

        Math3D.Cube dado;
        Point origen;
        public Tipos.Resultado_dado_cons resultado;
        Online frm_online;
        int game_id;
        Hotel hotel_seleccionado;

        public Dado_construccion(Online frm_online, int game_id, Hotel hotel_seleccionado)
        {
            InitializeComponent();
            this.rand = new Random();
            this.frm_online = frm_online;
            this.game_id = game_id;
            this.hotel_seleccionado = hotel_seleccionado;
        }

        private void Dado_construccion_Load(object sender, EventArgs e)
        {
            dado = new Math3D.Cube(50, 50, 50);
            dado.FillBack = true;
            dado.FillBottom = true;
            dado.FillFront = true;
            dado.FillLeft = true;
            dado.FillRight = true;
            dado.FillTop = true;
            origen = new Point(this.cuadro.Width / 2, this.cuadro.Height / 2);
            dado.RotateX = 10;
            dado.RotateY = 15;
            dado.RotateZ = 0;
        }

        private void Render()
        {
            cuadro.Image = dado.DrawCube(origen);
        }

        private void Dado_construccion_Paint(object sender, PaintEventArgs e)
        {
            this.Render();
        }

        private void cuadro_Click(object sender, EventArgs e)
        {
            // Enviar comando al server si estamos en modo online
            Tipos.Resultado_dado_cons res_online;
            if (this.frm_online != null)
            {
                this.frm_online.enviar_comando("roll_construction_dice", this.game_id.ToString(), this.hotel_seleccionado.nombre_txt);
                this.frm_online.Buscar_partida(game_id).interfaz.juego.sem_dado_cons.WaitOne(10000); // Esperamos a que llegue el comando con el resultado del dado
                res_online = this.frm_online.ultimo_res_dado_cons;

                do
                {
                    for (int i = 0; i < 5; i++)
                    {
                        dado.RotateX += this.rand.Next(50);
                        dado.RotateY += this.rand.Next(50);
                        dado.RotateZ += this.rand.Next(50);
                        this.Refresh();
                        Thread.Sleep(50);
                    }
                    // Detectar qué cara está más cerca de la pantalla
                    Math3D.Cube.Face cara_mas_cercana = dado.faces[0];
                    for (int i = 1; i < dado.faces.Length; i++)
                    {
                        if (cara_mas_cercana.CompareTo(dado.faces[i]) > 0)
                            cara_mas_cercana = dado.faces[i];
                    }

                    if (cara_mas_cercana.color == Brushes.Green)
                        this.resultado = Tipos.Resultado_dado_cons.Permitido;
                    else if (cara_mas_cercana.color == Brushes.Red)
                        this.resultado = Tipos.Resultado_dado_cons.Denegado;
                    else if (cara_mas_cercana.color == Brushes.Yellow)
                        this.resultado = Tipos.Resultado_dado_cons.Gratis;
                    else if (cara_mas_cercana.color == Brushes.Orange)
                        this.resultado = Tipos.Resultado_dado_cons.Doble;
                } while (resultado != res_online);
                if (resultado == Tipos.Resultado_dado_cons.Permitido)
                    MessageBox.Show(Mensajes.mensajeConstruccionPermitida);
                else if (resultado == Tipos.Resultado_dado_cons.Gratis)
                    MessageBox.Show(Mensajes.mensajeConstruccionGratuita);
                else if (resultado == Tipos.Resultado_dado_cons.Doble)
                    MessageBox.Show(Mensajes.mensajeConstruccionDobleCoste);
                else if (resultado == Tipos.Resultado_dado_cons.Denegado)
                    MessageBox.Show(Mensajes.mensajeConstruccionDenegada);
            }
            else
            {
                for (int i = 0; i < 30; i++)
                {
                    dado.RotateX += this.rand.Next(50);
                    dado.RotateY += this.rand.Next(50);
                    dado.RotateZ += this.rand.Next(50);
                    this.Refresh();
                    Thread.Sleep(50);
                }
                // Detectar qué cara está más cerca de la pantalla
                Math3D.Cube.Face cara_mas_cercana = dado.faces[0];
                for (int i = 1; i < dado.faces.Length; i++)
                {
                    if (cara_mas_cercana.CompareTo(dado.faces[i]) > 0)
                        cara_mas_cercana = dado.faces[i];
                }

                if (cara_mas_cercana.color == Brushes.Green)
                {
                    MessageBox.Show(Mensajes.mensajeConstruccionPermitida);
                    this.resultado = Tipos.Resultado_dado_cons.Permitido;
                }
                else if (cara_mas_cercana.color == Brushes.Red)
                {
                    MessageBox.Show(Mensajes.mensajeConstruccionDenegada);
                    this.resultado = Tipos.Resultado_dado_cons.Denegado;
                }
                else if (cara_mas_cercana.color == Brushes.Yellow)
                {
                    MessageBox.Show(Mensajes.mensajeConstruccionGratuita);
                    this.resultado = Tipos.Resultado_dado_cons.Gratis;
                }
                else if (cara_mas_cercana.color == Brushes.Orange)
                {
                    MessageBox.Show(Mensajes.mensajeConstruccionDobleCoste);
                    this.resultado = Tipos.Resultado_dado_cons.Doble;
                }
            }
            this.Hide();
        }
    }
}
