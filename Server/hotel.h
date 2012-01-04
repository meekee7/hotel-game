#pragma once
#include <vector>
#include "player.h"

class Hotel
{
public:
   wstring name;
   int price, expropriation_price;
   bool ground_bought;
   bool entrance_bought_last_turn;
   int n_max_expansions;
   int n_built_expansions;
   int entrance_price;
   int n_entrances;
   Player* owner;
   vector<vector<int>> prices_matrix;
   vector<int> expansions_prices;

   Hotel(wstring name);
   ~Hotel(void);
};

