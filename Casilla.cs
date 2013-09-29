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
                            this.pos_entrada_der.Establecer(20, 108, 270);
                            this.pos_entrada_izq.Establecer(200, 65, 247);
                        }
                        else
                        {
                            this.pos_coche.Establecer(75, 143, 118);
                            this.pos_entrada_der.Establecer(75, 187, 202);
                            this.pos_entrada_izq.Establecer(255, 174, 154);
                        }
                        break;
                case 4:
                case 6: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 4)
                        {
                            this.pos_coche.Establecer(46, 95, 140);
                            this.pos_entrada_der.Establecer(47, 135, 225);
                            this.pos_entrada_izq.Establecer(230, 101, 184);
                        }
                        else
                        {
                            this.pos_coche.Establecer(97, 191, 119);
                            this.pos_entrada_der.Establecer(97, 265, 170);
                            this.pos_entrada_izq.Establecer(280, 270, 127);
                        }
                        break;
                case 7:
                        this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(110, 234, 135);
                        this.pos_entrada_der.Establecer(110, 305, 185);
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
                        this.pos_entrada_der.Establecer(70, 435, 213);
                        break;
                case 10:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(120, 379, 132);
                        this.pos_entrada_der.Establecer(120, 483, 225);
                        this.pos_entrada_izq.Establecer(120, 505, 178);
                        break;
                case 11:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(120, 415, 164);
                        this.pos_entrada_der.Establecer(120, 526, 268);
                        this.pos_entrada_izq.Establecer(120, 560, 231);
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
                            this.pos_entrada_der.Establecer(120, 587, 307);
                            this.pos_entrada_izq.Establecer(120, 610, 262);
                        }
                        else if (this.numero == 14)
                        {
                            this.pos_coche.Establecer(100, 561, 208);
                            this.pos_entrada_der.Establecer(100, 725, 339);
                            this.pos_entrada_izq.Establecer(100, 737, 288);
                        }
                        else
                        {
                            this.pos_coche.Establecer(170, 623, 271);
                            this.pos_entrada_der.Establecer(170, 790, 412);
                            this.pos_entrada_izq.Establecer(170, 837, 400);
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
                            this.pos_entrada_der.Establecer(115, 656, 331);
                            this.pos_entrada_izq.Establecer(115, 664, 280);
                        }
                        else
                        {
                            this.pos_coche.Establecer(150, 603, 231);
                            this.pos_entrada_der.Establecer(150, 767, 364);
                            this.pos_entrada_izq.Establecer(150, 805, 331);
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
                            this.pos_entrada_der.Establecer(190, 785, 466);
                            this.pos_entrada_izq.Establecer(190, 828, 490);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 483, 356);
                            this.pos_entrada_der.Establecer(300, 639, 511);
                            this.pos_entrada_izq.Establecer(300, 627, 562);
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
                            this.pos_entrada_der.Establecer(225, 748, 511);
                            this.pos_entrada_izq.Establecer(225, 770, 558);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 437, 341);
                            this.pos_entrada_der.Establecer(300, 577, 492);
                            this.pos_entrada_izq.Establecer(300, 562, 538);
                        }
                        break;
                case 19:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(270, 534, 366);
                        this.pos_entrada_der.Establecer(270, 694, 526);
                        this.pos_entrada_izq.Establecer(270, 694, 579);
                        break;
                case 22:
                case 24:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 22)
                        {
                            this.pos_coche.Establecer(300, 387, 327);
                            this.pos_entrada_der.Establecer(300, 514, 466);
                            this.pos_entrada_izq.Establecer(300, 364, 517);
                        }
                        else
                        {
                            this.pos_coche.Establecer(200, 294, 347);
                            this.pos_entrada_der.Establecer(200, 364, 510);
                            this.pos_entrada_izq.Establecer(200, 408, 532);
                        }
                        break;
                case 23:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(250, 336, 318);
                        this.pos_entrada_der.Establecer(250, 432, 454);
                        this.pos_entrada_izq.Establecer(250, 440, 505);
                        break;
                case 25:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.pos_coche.Establecer(180, 287, 391);
                        this.pos_entrada_der.Establecer(180, 353, 589);
                        this.pos_entrada_izq.Establecer(180, 403, 595);
                        break;
                case 26:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.pos_coche.Establecer(225, 270, 439);
                        this.pos_entrada_izq.Establecer(225, 369, 684);
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
                            this.pos_entrada_izq.Establecer(285, 270, 706);
                        }
                        else if (this.numero == 28)
                        {
                            this.pos_coche.Establecer(350, 172, 418);
                            this.pos_entrada_izq.Establecer(350, 206, 645);
                        }
                        else
                        {
                            this.pos_coche.Establecer(300, 140, 288);
                            this.pos_entrada_izq.Establecer(300, 248, 460);
                        }
                        break;
                case 29:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.pos_coche.Establecer(0, 169, 367);
                        this.pos_entrada_der.Establecer(0, 248, 553);
                        this.pos_entrada_izq.Establecer(0, 197, 553);
                        break;                
                case 30:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.pos_coche.Establecer(345, 168, 321);
                        this.pos_entrada_der.Establecer(345, 246, 475);
                        this.pos_entrada_izq.Establecer(345, 196, 490);
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
