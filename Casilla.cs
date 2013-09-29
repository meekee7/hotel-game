using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Juego_Hotel.Resources;

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
            this.pos_coche.Establecer(0, 0, 0);
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
                        this.pos_entrada_der.Establecer(0, 101, 330);
                        break;
                case 3:
                case 5: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 3)
                        {
                            this.pos_coche.Establecer(20, 65, 176);
                            this.pos_entrada_der.Establecer(20, 107, 261);
                            this.pos_entrada_izq.Establecer(200, 55, 235);
                        }
                        else
                        {
                            this.pos_coche.Establecer(75, 143, 118);
                            this.pos_entrada_der.Establecer(75, 200, 185);
                            this.pos_entrada_izq.Establecer(255, 185, 128);
                        }
                        break;
                case 4:
                case 6: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 4)
                        {
                            this.pos_coche.Establecer(47, 95, 140);
                            this.pos_entrada_der.Establecer(47, 140, 209);
                            this.pos_entrada_izq.Establecer(230, 105, 164);
                        }
                        else
                        {
                            this.pos_coche.Establecer(100, 191, 119);
                            this.pos_entrada_der.Establecer(100, 260, 185);
                            this.pos_entrada_izq.Establecer(280, 270, 127);
                        }
                        break;
                case 7:
                        this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(114, 234, 135);
                        this.pos_entrada_der.Establecer(114, 325, 212);
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
                        this.pos_coche.Establecer(75, 330, 123);
                        this.pos_entrada_der.Establecer(75, 490, 193);
                        break;
                case 10:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(123, 379, 132);
                        this.pos_entrada_der.Establecer(123, 545, 201);
                        this.pos_entrada_izq.Establecer(303, 569, 146);
                        break;
                case 11:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(130, 415, 164);
                        this.pos_entrada_der.Establecer(130, 596, 250);
                        this.pos_entrada_izq.Establecer(310, 635, 205);
                        break;
                case 12:
                case 14:
                case 16:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 12)
                        {
                            this.pos_coche.Establecer(115, 458, 188);
                            this.pos_entrada_der.Establecer(115, 672, 302);
                            this.pos_entrada_izq.Establecer(295, 693, 245);
                        }
                        else if (this.numero == 14)
                        {
                            this.pos_coche.Establecer(105, 561, 208);
                            this.pos_entrada_der.Establecer(105, 830, 337);
                            this.pos_entrada_izq.Establecer(285, 845, 281);
                        }
                        else
                        {
                            this.pos_coche.Establecer(170, 623, 271);
                            this.pos_entrada_der.Establecer(170, 917, 418);
                            this.pos_entrada_izq.Establecer(350, 975, 402);
                        }
                        break;
                case 13:
                case 15:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 13)
                        {
                            this.pos_coche.Establecer(100, 507, 202);
                            this.pos_entrada_der.Establecer(100, 751, 329);
                            this.pos_entrada_izq.Establecer(280, 765, 273);
                        }
                        else
                        {
                            this.pos_coche.Establecer(139, 603, 231);
                            this.pos_entrada_der.Establecer(139, 887, 365);
                            this.pos_entrada_izq.Establecer(319, 925, 318);
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
                            this.pos_entrada_der.Establecer(190, 921, 480);
                            this.pos_entrada_izq.Establecer(10, 977, 496);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 483, 356);
                            this.pos_entrada_der.Establecer(300, 735, 536);
                            this.pos_entrada_izq.Establecer(120, 715, 592);
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
                            this.pos_entrada_der.Establecer(225, 860, 536);
                            this.pos_entrada_izq.Establecer(45, 883, 595);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 437, 341);
                            this.pos_entrada_der.Establecer(300, 661, 511);
                            this.pos_entrada_izq.Establecer(120, 641, 566);
                        }
                        break;
                case 19:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(270, 534, 366);
                        this.pos_entrada_der.Establecer(270, 802, 559);
                        this.pos_entrada_izq.Establecer(90, 791, 615);
                        break;
                case 22:
                case 24:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 22)
                        {
                            this.pos_coche.Establecer(300, 387, 327);
                            this.pos_entrada_der.Establecer(300, 586, 486);
                            this.pos_entrada_izq.Establecer(120, 570, 542);
                        }
                        else
                        {
                            this.pos_coche.Establecer(200, 294, 347);
                            this.pos_entrada_der.Establecer(200, 414, 520);
                            this.pos_entrada_izq.Establecer(32, 475, 558);
                        }
                        break;
                case 23:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(250, 336, 318);
                        this.pos_entrada_der.Establecer(250, 476, 483);
                        this.pos_entrada_izq.Establecer(70, 496, 535);
                        break;
                case 25:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(180, 287, 391);
                        this.pos_entrada_der.Establecer(180, 403, 619);
                        this.pos_entrada_izq.Establecer(180, 460, 619);
                        break;
                case 26:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(225, 270, 439);
                        this.pos_entrada_izq.Establecer(45, 413, 724);
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
                            this.pos_entrada_izq.Establecer(105, 300, 761);
                        }
                        else if (this.numero == 28)
                        {
                            this.pos_coche.Establecer(350, 172, 418);
                            this.pos_entrada_izq.Establecer(160, 222, 683);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 140, 288);
                            this.pos_entrada_izq.Establecer(120, 177, 475);
                        }
                        break;
                case 29:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(0, 169, 367);
                        this.pos_entrada_der.Establecer(0, 278, 583);
                        this.pos_entrada_izq.Establecer(180, 217, 583);
                        break;                
                case 30:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(345, 168, 321);
                        this.pos_entrada_der.Establecer(345, 269, 490);
                        this.pos_entrada_izq.Establecer(170, 214, 510);
                        break;
            }
        }

        public String ObtenerTipoTxt()
        {
            switch (this.tipo)
            {
                case Tipos.Tcasilla.salida: return Mensajes.TipoCasillaSalida;
                case Tipos.Tcasilla.comprar: return Mensajes.TipoCasillaComprar;
                case Tipos.Tcasilla.construir: return Mensajes.TipoCasillaConstruir;
                case Tipos.Tcasilla.entrada_gratis: return Mensajes.TipoCasillaEntradaGratis;
                case Tipos.Tcasilla.fase_gratis: return Mensajes.TipoCasillaFaseGratis;
                default: return String.Empty;
            }
        }
    }
}
