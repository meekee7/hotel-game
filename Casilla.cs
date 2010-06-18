using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    public class Casilla
    {
        public short numero;
        public bool ocupada;
        public Tipos.Tcasilla tipo;
        public Tipos.Tnombre_hotel hotel_der;
        public Tipos.Tnombre_hotel hotel_izq;
        public float grados; // Para dibujar el coche correctamente sobre el tablero en cada casilla
        public int posX, posY;

        public Casilla (short numero)
        {
            this.numero = numero;
            this.ocupada = false;
            this.grados = 0; // Por defecto, en vertical

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
                        this.grados = 315;
                        this.posX = 70;
                        this.posY = 281;
                        break;
                case 2: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.posX = 57;
                        this.posY = 221;
                        break;
                case 3:
                case 5: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 3)
                        {
                            this.grados = 20;
                            this.posX = 65;
                            this.posY = 176;
                        }
                        else
                        {
                            this.grados = 75;
                            this.posX = 143;
                            this.posY = 118;
                        }
                        break;
                case 4:
                case 6: this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Boomerang;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 4)
                        {
                            this.grados = 45;
                            this.posX = 95;
                            this.posY = 140;
                        }
                        else
                        {
                            this.grados = 95;
                            this.posX = 191;
                            this.posY = 119;
                        }
                        break;
                case 7:
                        this.hotel_der = Tipos.Tnombre_hotel.Fujiyama;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.grados = 110;
                        this.posX = 234;
                        this.posY = 135;
                        break;
                case 8:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.grados = 80;
                        this.posX = 284;
                        this.posY = 141;
                        break;
                case 9:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Ninguno;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.grados = 70;
                        this.posX = 330;
                        this.posY = 123;
                        break;
                case 10:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.grados = 120;
                        this.posX = 379;
                        this.posY = 132;
                        break;
                case 11:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.grados = 120;
                        this.posX = 415;
                        this.posY = 164;
                        break;
                case 12:
                case 14:
                case 16:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 12)
                        {
                            this.grados = 120;
                            this.posX = 458;
                            this.posY = 188;
                        }
                        else if (this.numero == 14)
                        {
                            this.grados = 100;
                            this.posX = 561;
                            this.posY = 208;
                        }
                        else
                        {
                            this.grados = 170;
                            this.posX = 623;
                            this.posY = 271;
                        }
                        break;
                case 13:
                case 15:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.President;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 13)
                        {
                            this.grados = 115;
                            this.posX = 507;
                            this.posY = 202;
                        }
                        else
                        {
                            this.grados = 150;
                            this.posX = 603;
                            this.posY = 231;
                        }
                        break;
                case 17:
                case 20:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 17)
                        {
                            this.grados = 190;
                            this.posX = 616;
                            this.posY = 319;
                        }
                        else
                        {
                            this.grados = 300;
                            this.posX = 483;
                            this.posY = 356;
                        }
                        break;
                case 18:
                case 21:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 18)
                        {
                            this.grados = 225;
                            this.posX = 581;
                            this.posY = 355;
                        }
                        else
                        {
                            this.grados = 300;
                            this.posX = 437;
                            this.posY = 341;
                        }
                        break;
                case 19:
                        this.hotel_der = Tipos.Tnombre_hotel.Royal;
                        this.hotel_izq = Tipos.Tnombre_hotel.Waikiki;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.grados = 270;
                        this.posX = 534;
                        this.posY = 366;
                        break;
                case 22:
                case 24:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.comprar;
                        if (this.numero == 22)
                        {
                            this.grados = 300;
                            this.posX = 387;
                            this.posY = 327;
                        }
                        else
                        {
                            this.grados = 200;
                            this.posX = 294;
                            this.posY = 347;
                        }
                        break;
                case 23:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.grados = 250;
                        this.posX = 336;
                        this.posY = 318;
                        break;
                case 25:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.fase_gratis;
                        this.grados = 180;
                        this.posX = 287;
                        this.posY = 391;
                        break;
                case 26:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Taj_Mahal;
                        this.tipo = Tipos.Tcasilla.construir;
                        this.grados = 225;
                        this.posX = 270;
                        this.posY = 439;
                        break;
                case 27:
                case 28:
                case 31:
                        this.hotel_der = Tipos.Tnombre_hotel.Ninguno;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.construir;
                        if (this.numero == 27)
                        {
                            this.grados = 285;
                            this.posX = 211;
                            this.posY = 453;
                        }
                        else if (this.numero == 28)
                        {
                            this.grados = 350;
                            this.posX = 172;
                            this.posY = 418;
                        }
                        else
                        {
                            this.grados = 300;
                            this.posX = 140;
                            this.posY = 288;
                        }
                        break;
                case 29:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.comprar;
                        this.grados = 0;
                        this.posX = 169;
                        this.posY = 367;
                        break;                
                case 30:
                        this.hotel_der = Tipos.Tnombre_hotel.Letoile;
                        this.hotel_izq = Tipos.Tnombre_hotel.Safari;
                        this.tipo = Tipos.Tcasilla.entrada_gratis;
                        this.grados = 345;
                        this.posX = 168;
                        this.posY = 321;
                        break;
            }
        }
    }
}
