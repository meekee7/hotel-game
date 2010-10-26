using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Juego_Hotel
{
    public partial class Subastas : Form
    {
        public int n_5000;
        public int n_1000;
        public int n_500;
        public int n_100;
        public int n_50;

        public Subastas(ref Juego juego)
        {
            InitializeComponent();
            this.n_5000 = 0;
            this.n_1000 = 0;
            this.n_500 = 0;
            this.n_100 = 0;
            this.n_50 = 0;
        }
    }
}
