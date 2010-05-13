using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Casilla
    {
        public int numero;
        public Tipos.Tcasilla tipo;
        public bool puede_entrada_der = false;
        public bool puede_entrada_izq = false;
        public Entrada entrada_der;
        public Entrada entrada_izq;

        public Casilla (int numero)
        {
            this.numero = numero;
            this.entrada_der = null;
            this.entrada_izq = null;
                
            switch (this.numero) // Cada casilla va metida a capón
            {
                case 0: this.puede_entrada_der = false;
                        this.puede_entrada_izq = false;
                        this.tipo = Tipos.Tcasilla.salida;
                        break;
                case 1: this.puede_entrada_der = false;
                        this.puede_entrada_izq = false;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        break;
                case 2: this.puede_entrada_der = true;
                        this.puede_entrada_izq = false;
                        this.tipo = Tipos.Tcasilla.construir;
                        break;
                case 3:
                case 5:
                case 10:
                case 12:
                case 14:
                case 16:
                case 18:
                case 21:
                case 22:
                case 24:
                case 29:
                        this.puede_entrada_der = true;
                        this.puede_entrada_izq = true;
                        this.tipo = Tipos.Tcasilla.comprar;
                        break;
                case 4:
                case 6:
                case 13:
                case 15:
                case 17:
                case 20:
                case 23:
                        this.puede_entrada_der = true;
                        this.puede_entrada_izq = true;
                        this.tipo = Tipos.Tcasilla.construir;
                        break;
                case 7:
                        this.puede_entrada_der = true;
                        this.puede_entrada_izq = false;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        break;
                case 8:
                        this.puede_entrada_der = false;
                        this.puede_entrada_izq = false;
                        this.tipo = Tipos.Tcasilla.construir;
                        break;
                case 9:
                        this.puede_entrada_der = true;
                        this.puede_entrada_izq = false;
                        this.tipo = Tipos.Tcasilla.comprar;
                        break;
                case 11:
                case 25:
                        this.puede_entrada_der = true;
                        this.puede_entrada_izq = true;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        break;
                case 19:
                case 30:
                        this.puede_entrada_der = true;
                        this.puede_entrada_izq = true;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        break;
                case 26:
                case 27:
                case 28:
                case 31:
                        this.puede_entrada_der = false;
                        this.puede_entrada_izq = true;
                        this.tipo = Tipos.Tcasilla.construir;
                        break;
            }
        }
    }
}
