#include "position.h"


position::position(void)
{
   this->number = number;
   this->occupied = false;
   this->entrance_left = false;
   this->entrance_right = false;

   switch (this->number) // Cada casilla va metida a capón
   {
         case 0: this->hotel_right = None;
               this->hotel_left = None;
               this->type = start;
               this->occupied = true;
               break;
         case 1: this->hotel_right = None;
               this->hotel_left = None;
               this->type = free_phase;
               break;
         case 2: this->hotel_right = Fujiyama;
               this->hotel_left = None;
               this->type = build;
               break;
         case 3:
         case 5: this->hotel_right = Fujiyama;
               this->hotel_left = Boomerang;
               this->type = buy;
               break;
         case 4:
         case 6: this->hotel_right = Fujiyama;
               this->hotel_left = Boomerang;
               this->type = build;
               break;
         case 7:
               this->hotel_right = Fujiyama;
               this->hotel_left = None;
               this->type = free_entrance;
               break;
         case 8:
               this->hotel_right = None;
               this->hotel_left = None;
               this->type = build;
               break;
         case 9:
               this->hotel_right = Letoile;
               this->hotel_left = None;
               this->type = buy;
               break;
         case 10:
               this->hotel_right = Letoile;
               this->hotel_left = President;
               this->type = buy;
               break;
         case 11:
               this->hotel_right = Letoile;
               this->hotel_left = President;
               this->type = free_phase;
               break;
         case 12:
         case 14:
         case 16:
               this->hotel_right = Royal;
               this->hotel_left = President;
               this->type = buy;
               break;
         case 13:
         case 15:
               this->hotel_right = Royal;
               this->hotel_left = President;
               this->type = build;
               break;
         case 17:
         case 20:
               this->hotel_right = Royal;
               this->hotel_left = Waikiki;
               this->type = build;
               break;
         case 18:
         case 21:
               this->hotel_right = Royal;
               this->hotel_left = Waikiki;
               this->type = buy;
               break;
         case 19:
               this->hotel_right = Royal;
               this->hotel_left = Waikiki;
               this->type = free_entrance;
               break;
         case 22:
         case 24:
               this->hotel_right = Letoile;
               this->hotel_left = Taj_Mahal;
               this->type = buy;
               break;
         case 23:
               this->hotel_right = Letoile;
               this->hotel_left = Taj_Mahal;
               this->type = build;
               break;
         case 25:
               this->hotel_right = Letoile;
               this->hotel_left = Taj_Mahal;
               this->type = free_phase;
               break;
         case 26:
               this->hotel_right = None;
               this->hotel_left = Taj_Mahal;
               this->type = build;
               break;
         case 27:
         case 28:
         case 31:
               this->hotel_right = None;
               this->hotel_left = Safari;
               this->type = build;
               break;
         case 29:
               this->hotel_right = Letoile;
               this->hotel_left = Safari;
               this->type = buy;
               break;                
         case 30:
               this->hotel_right = Letoile;
               this->hotel_left = Safari;
               this->type = free_entrance;
               break;
   }
}


position::~position(void)
{
}
