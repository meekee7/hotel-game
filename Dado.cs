using System;

namespace Juego_Hotel
{
    public class Dado
    {
        readonly int n_caras;
        readonly Random rand;
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
