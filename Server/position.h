#pragma once
#include "types.h"

class Position
{
public:
   int number;
   bool occupied;
   TPosition type;
   THotel_name hotel_right;
   THotel_name hotel_left;
   bool entrance_left;
   bool entrance_right;

   Position(int number);
   ~Position(void);
};