#include "hotel.h"

Hotel::Hotel(THotel_name name)
{
   this->prices_matrix = vector<vector<int> > (2, vector<int>(3));
   this->prices_matrix[0][0] = 1;
   this->prices_matrix[0][1] = 2;
   this->prices_matrix[1][2] = 3;
   this->name = name;
   this->owner = NULL;
   this->entrances = list<int>();
   this->n_entrances = 0;
   this->entrance_bought_last_turn = false;
   this->ground_bought = false;
   this->n_built_expansions = 0;
   switch (this->name)
   {
         case Boomerang :
            this->name_txt = L"Boomerang";
            this->price = 500;
            this->expropriation_price = 250;
            this->entrance_price = 100;
            this->n_max_expansions = 2;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->expansions_prices[0] = 1800;
            this->expansions_prices[1] = 250;
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->prices_matrix[0][0] = 400;
            this->prices_matrix[0][1] = 800;
            this->prices_matrix[0][2] = 1200;
            this->prices_matrix[0][3] = 1600;
            this->prices_matrix[0][4] = 2000;
            this->prices_matrix[0][5] = 2400;
            this->prices_matrix[1][0] = 600;
            this->prices_matrix[1][1] = 1200;
            this->prices_matrix[1][2] = 1800;
            this->prices_matrix[1][3] = 2400;
            this->prices_matrix[1][4] = 3000;
            this->prices_matrix[1][5] = 3600;
            break;
         case Fujiyama :
            this->name_txt = L"Fujiyama";
            this->price = 1000;
            this->expropriation_price = 500;
            this->entrance_price = 100;
            this->n_max_expansions = 4;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->expansions_prices[0] = 2200;
            this->expansions_prices[1] = 1400;
            this->expansions_prices[2] = 1400;
            this->expansions_prices[3] = 500;
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->prices_matrix[0][0] = 100;
            this->prices_matrix[0][1] = 200;
            this->prices_matrix[0][2] = 300;
            this->prices_matrix[0][3] = 400;
            this->prices_matrix[0][4] = 500;
            this->prices_matrix[0][5] = 600;
            this->prices_matrix[1][0] = 100;
            this->prices_matrix[1][1] = 200;
            this->prices_matrix[1][2] = 300;
            this->prices_matrix[1][3] = 400;
            this->prices_matrix[1][4] = 500;
            this->prices_matrix[1][5] = 600;
            this->prices_matrix[2][0] = 200;
            this->prices_matrix[2][1] = 400;
            this->prices_matrix[2][2] = 600;
            this->prices_matrix[2][3] = 800;
            this->prices_matrix[2][4] = 1000;
            this->prices_matrix[2][5] = 1200;
            this->prices_matrix[3][0] = 400;
            this->prices_matrix[3][1] = 800;
            this->prices_matrix[3][2] = 1200;
            this->prices_matrix[3][3] = 1600;
            this->prices_matrix[3][4] = 2000;
            this->prices_matrix[3][5] = 2400;
            break;
         case President :
            this->name_txt = L"President";
            this->price = 3500;
            this->expropriation_price = 1750;
            this->entrance_price = 250;
            this->n_max_expansions = 5;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->expansions_prices = new int[5] { 5000, 3000, 2250, 1750, 5000 };
            this->prices_matrix = new int[5, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                   { 400, 800, 1200, 1600, 2000, 2400 },
                                                   { 600, 1200, 1800, 2400, 3000, 3600 },
                                                   { 800, 1600, 2400, 3200, 4000, 4800 },
                                                   { 1100, 2200, 3300, 4400, 5500, 6600 } };
            break;
         case Taj_Mahal :
            this->name_txt = L"Taj Mahal";
            this->price = 1500;
            this->expropriation_price = 750;
            this->entrance_price = 100;
            this->n_max_expansions = 4;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->expansions_prices = new int[4] { 2400, 1000, 500, 1000 };
            this->prices_matrix = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                   { 100, 200, 300, 400, 500, 600 },
                                                   { 200, 400, 600, 800, 1000, 1200 },
                                                   { 300, 600, 900, 1200, 1500, 1800 } };
            break;
         case Waikiki :
            this->name_txt = L"Waikiki";
            this->price = 2500;
            this->expropriation_price = 1250;
            this->entrance_price = 200;
            this->n_max_expansions = 6;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->expansions_prices = new int[6] { 3500, 2500, 2500, 1750, 1750, 2500 };
            this->prices_matrix = new int[6, 6] { { 200, 400, 600, 800, 1000, 1200 },
                                                   { 350, 700, 1050, 1400, 1750, 2100 },
                                                   { 500, 1000, 1500, 2000, 2500, 3000 },
                                                   { 500, 1000, 1500, 2000, 2500, 3000 },
                                                   { 650, 1300, 1950, 2600, 3250, 3900 },
                                                   { 1000, 2000, 3000, 4000, 5000, 6000 } };
            break;
         case Royal:
            this->name_txt = L"Royal";
            this->price = 2500;
            this->expropriation_price = 1250;
            this->entrance_price = 200;
            this->n_max_expansions = 5;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->expansions_prices = new int[5] { 3600, 2600, 1800, 1800, 3000 };
            this->prices_matrix = new int[5, 6] { { 150, 300, 450, 600, 750, 900 },
                                                   { 300, 600, 900, 1200, 1500, 1800 },
                                                   { 300, 600, 900, 1200, 1500, 1800 },
                                                   { 450, 900, 1350, 1800, 2250, 2700 },
                                                   { 600, 1200, 1800, 2400, 3000, 3600 } };
            break;
         case Safari:
            this->name_txt = L"Safari";
            this->price = 2000;
            this->expropriation_price = 1000;
            this->entrance_price = 150;
            this->n_max_expansions = 4;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->expansions_prices = new int[4] { 2600, 1200, 1200, 2000 };
            this->prices_matrix = new int[4, 6] { { 100, 200, 300, 400, 500, 600 },
                                                   { 100, 200, 300, 400, 500, 600 },
                                                   { 250, 500, 750, 1000, 1250, 1500 },
                                                   { 500, 1000, 1500, 2000, 2500, 3000 } };
            break;
         case Letoile:
            this->name_txt = L"L'etoile";
            this->price = 3000;
            this->expropriation_price = 1500;
            this->entrance_price = 250;
            this->n_max_expansions = 6;
            this->expansions_prices = vector<int>(this->n_max_expansions);
            this->prices_matrix = vector<vector<int> > (this->n_max_expansions, vector<int>(6));
            this->expansions_prices = new int[6] { 3300, 2200, 1800, 1800, 1800, 4000 };
            this->prices_matrix = new int[6, 6] { { 150, 300, 450, 600, 750, 900 },
                                                   { 300, 600, 900, 1200, 1500, 1800 },
                                                   { 300, 600, 900, 1200, 1500, 1800 },
                                                   { 300, 600, 900, 1200, 1500, 1800 },
                                                   { 450, 900, 1350, 1800, 2250, 2700 },
                                                   { 750, 1500, 2250, 3000, 3750, 4500 } };
            break;
   }
}

void Hotel::Extend()
{
   this->n_built_expansions++;
}

void Hotel::Add_entrance(int position)
{
   this->entrances.push_back(position);
   this->n_entrances++;
}

Hotel::~Hotel(void)
{
}
