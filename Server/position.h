#pragma once
#include "types.h"

class position
{
public:
   int number;
   bool occupied;
   TPosition type;
   THotel_name hotel_right;
   THotel_name hotel_left;
   bool entrance_left;
   bool entrance_right;

   position(void);
   ~position(void);
};