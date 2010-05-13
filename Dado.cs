using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Dado
    {
        short n_caras;
        Random rand;
        public Dado(short caras)
        {
            this.n_caras = caras;
            this.rand = new Random();
        }

        public short tirar()
        {
            return (short) this.rand.Next(1, this.n_caras+1);
        }
    }
}
