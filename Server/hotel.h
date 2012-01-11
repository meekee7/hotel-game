#pragma once
#include <vector>
#include "player.h"
#include "types.h"

class Hotel
{
public:
   THotel_name name;
   wstring name_txt;
   int price, expropriation_price;
   bool ground_bought;
   bool entrance_bought_last_turn;
   int n_max_expansions;
   int n_built_expansions;
   int entrance_price;
   int n_entrances;
   list<int> entrances;
   Player* owner;
   vector<vector<int> > prices_matrix;
   vector<int> expansions_prices;

   Hotel(THotel_name real_name);
   void Extend();
   void Add_entrance(int position);
   int Price_next_expansion();
   bool Can_extend();
   int Calculate_nights(int number);
   void Return_to_bank();
   ~Hotel(void);
};

