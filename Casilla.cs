using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Casilla
    {
        public int numero;
        public bool ocupada;
        public Tipos.Tcasilla tipo;
        public Tipos.Tnombre_hotel hotel_der;
        public Tipos.Tnombre_hotel hotel_izq;
        public Boolean entrada_en_izq;
        public Boolean entrada_en_der;
        public Tipos.Posicion pos_coche;
        public Tipos.Posicion pos_entrada_der;
        public Tipos.Posicion pos_entrada_izq;

        public Casilla (int numero)
        {
            this.numero = numero;
            this.ocupada = false;
            this.entrada_en_izq = false;
            this.entrada_en_der = false;
            this.pos_coche = new Tipos.Posicion();
            this.pos_entrada_der = new Tipos.Posicion();
            this.pos_entrada_izq = new Tipos.Posicion();
            this.pos_entrada_der.Establecer(0, 0, 0);
            this.pos_entrada_izq.Establecer(0, 0, 0);

            switch (this.numero) // Cada casilla va metida a capón
            {
                case 0: this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.salida;
                        this.ocupada = true;
                        break;
                case 1: this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(315, 70, 281);
                        break;
                case 2: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(0, 57, 221);
                        this.pos_entrada_der.Establecer(0, 78, 223);
                        break;
                case 3:
                case 5: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 3)
                        {
                            this.pos_coche.Establecer(20, 65, 176);
                            this.pos_entrada_der.Establecer(20, 87, 184);
                            this.pos_entrada_izq.Establecer(20, 53, 167);
                        }
                        else
                        {
                            this.pos_coche.Establecer(75, 143, 118);
                            this.pos_entrada_der.Establecer(75, 148, 138);
                            this.pos_entrada_izq.Establecer(75, 137, 106);
                        }
                        break;
                case 4:
                case 6: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 4)
                        {
                            this.pos_coche.Establecer(45, 95, 140);
                            this.pos_entrada_der.Establecer(45, 109, 156);
                            this.pos_entrada_izq.Establecer(45, 86, 130);
                        }
                        else
                        {
                            this.pos_coche.Establecer(95, 191, 119);
                            this.pos_entrada_der.Establecer(95, 190, 138);
                            this.pos_entrada_izq.Establecer(95, 197, 104);
                        }
                        break;
                case 7:
                        this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(110, 234, 135);
                        this.pos_entrada_der.Establecer(110, 231, 155);
                        break;
                case 8:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(80, 284, 141);
                        break;
                case 9:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(70, 330, 123);
                        this.pos_entrada_der.Establecer(70, 335, 142);
                        break;
                case 10:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(120, 379, 132);
                        this.pos_entrada_der.Establecer(120, 372, 150);
                        this.pos_entrada_izq.Establecer(120, 389, 119);
                        break;
                case 11:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(120, 415, 164);
                        this.pos_entrada_der.Establecer(120, 405, 179);
                        this.pos_entrada_izq.Establecer(120, 431, 154);
                        break;
                case 12:
                case 14:
                case 16:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 12)
                        {
                            this.pos_coche.Establecer(120, 458, 188);
                            this.pos_entrada_der.Establecer(120, 452, 205);
                            this.pos_entrada_izq.Establecer(120, 469, 175);
                        }
                        else if (this.numero == 14)
                        {
                            this.pos_coche.Establecer(100, 561, 208);
                            this.pos_entrada_der.Establecer(100, 558, 226);
                            this.pos_entrada_izq.Establecer(100, 567, 192);
                        }
                        else
                        {
                            this.pos_coche.Establecer(170, 623, 271);
                            this.pos_entrada_der.Establecer(170, 608, 275);
                            this.pos_entrada_izq.Establecer(170, 644, 267);
                        }
                        break;
                case 13:
                case 15:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 13)
                        {
                            this.pos_coche.Establecer(115, 507, 202);
                            this.pos_entrada_der.Establecer(115, 505, 221);
                            this.pos_entrada_izq.Establecer(115, 511, 187);
                        }
                        else
                        {
                            this.pos_coche.Establecer(150, 603, 231);
                            this.pos_entrada_der.Establecer(150, 590, 243);
                            this.pos_entrada_izq.Establecer(150, 619, 221);
                        }
                        break;
                case 17:
                case 20:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 17)
                        {
                            this.pos_coche.Establecer(190, 616, 319);
                            this.pos_entrada_der.Establecer(190, 604, 311);
                            this.pos_entrada_izq.Establecer(190, 637, 327);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 483, 356);
                            this.pos_entrada_der.Establecer(300, 492, 341);
                            this.pos_entrada_izq.Establecer(300, 482, 375);
                        }
                        break;
                case 18:
                case 21:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 18)
                        {
                            this.pos_coche.Establecer(225, 581, 355);
                            this.pos_entrada_der.Establecer(225, 576, 341);
                            this.pos_entrada_izq.Establecer(225, 592, 372);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 437, 341);
                            this.pos_entrada_der.Establecer(300, 444, 328);
                            this.pos_entrada_izq.Establecer(300, 432, 359);
                        }
                        break;
                case 19:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(270, 534, 366);
                        this.pos_entrada_der.Establecer(270, 534, 351);
                        this.pos_entrada_izq.Establecer(270, 534, 386);
                        break;
                case 22:
                case 24:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 22)
                        {
                            this.pos_coche.Establecer(300, 387, 327);
                            this.pos_entrada_der.Establecer(300, 396, 311);
                            this.pos_entrada_izq.Establecer(300, 385, 345);
                        }
                        else
                        {
                            this.pos_coche.Establecer(200, 294, 347);
                            this.pos_entrada_der.Establecer(200, 280, 340);
                            this.pos_entrada_izq.Establecer(200, 314, 355);
                        }
                        break;
                case 23:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(250, 336, 318);
                        this.pos_entrada_der.Establecer(250, 333, 303);
                        this.pos_entrada_izq.Establecer(250, 339, 337);
                        break;
                case 25:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(180, 287, 391);
                        this.pos_entrada_der.Establecer(180, 272, 393);
                        this.pos_entrada_izq.Establecer(180, 310, 397);
                        break;
                case 26:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(225, 270, 439);
                        this.pos_entrada_izq.Establecer(225, 284, 456);
                        break;
                case 27:
                case 28:
                case 31:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 27)
                        {
                            this.pos_coche.Establecer(285, 211, 453);
                            this.pos_entrada_izq.Establecer(285, 208, 471);
                        }
                        else if (this.numero == 28)
                        {
                            this.pos_coche.Establecer(350, 172, 418);
                            this.pos_entrada_izq.Establecer(350, 159, 430);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 140, 288);
                            this.pos_entrada_izq.Establecer(300, 133, 307);
                        }
                        break;
                case 29:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(0, 169, 367);
                        this.pos_entrada_der.Establecer(0, 191, 369);
                        this.pos_entrada_izq.Establecer(0, 152, 369);
                        break;                
                case 30:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(345, 168, 321);
                        this.pos_entrada_der.Establecer(345, 189, 317);
                        this.pos_entrada_izq.Establecer(345, 151, 327);
                        break;
            }
        }
    }
}
