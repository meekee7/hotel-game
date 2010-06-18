using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Dado_construccion
    {
        public enum Caras { verde1, verde2, verde3, gratis, doble, no };
        Random rand;

        public Dado_construccion()
        {
            this.rand = new Random();
        }

        public Caras tirar()
        {
            int res = this.rand.Next(0, 6);
            return (Caras) res;
        }
    }
}
