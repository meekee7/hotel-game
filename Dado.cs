using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Dado
    {
        int n_caras;
        Random rand;
        public Dado(int caras)
        {
            this.n_caras = caras;
            this.rand = new Random();
        }

        public int tirar()
        {
            return this.rand.Next(1, this.n_caras+1);
        }
    }
}
