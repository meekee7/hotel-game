#include "hotel.h"

Hotel::Hotel(THotel_name name)
{
   this->name = name;
   this->owner = NULL;
   this->entrances = list<int>();
   this->n_entrances = 0;
   this->entrance_bought_last_turn = false;
   this->ground_bought = false;
   this->n_built_phases = 0;
   switch (this->name)
   {
         case Boomerang :
            this->name_txt = L"Boomerang";
            this->price = 500;
            this->expropriation_price = 250;
            this->entrance_price = 100;
            this->n_max_phases = 2;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 1800;
            this->phases_prices[1] = 250;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
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
            this->n_max_phases = 4;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 2200;
            this->phases_prices[1] = 1400;
            this->phases_prices[2] = 1400;
            this->phases_prices[3] = 500;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
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
            this->n_max_phases = 5;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 5000;
            this->phases_prices[1] = 3000;
            this->phases_prices[2] = 2250;
            this->phases_prices[3] = 1750;
            this->phases_prices[4] = 5000;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
            this->prices_matrix[0][0] = 200;
            this->prices_matrix[0][1] = 400;
            this->prices_matrix[0][2] = 600;
            this->prices_matrix[0][3] = 800;
            this->prices_matrix[0][4] = 1000;
            this->prices_matrix[0][5] = 1200;
            this->prices_matrix[1][0] = 400;
            this->prices_matrix[1][1] = 800;
            this->prices_matrix[1][2] = 1200;
            this->prices_matrix[1][3] = 1600;
            this->prices_matrix[1][4] = 2000;
            this->prices_matrix[1][5] = 2400;
            this->prices_matrix[2][0] = 600;
            this->prices_matrix[2][1] = 1200;
            this->prices_matrix[2][2] = 1800;
            this->prices_matrix[2][3] = 2400;
            this->prices_matrix[2][4] = 3000;
            this->prices_matrix[2][5] = 3600;
            this->prices_matrix[3][0] = 800;
            this->prices_matrix[3][1] = 1600;
            this->prices_matrix[3][2] = 2400;
            this->prices_matrix[3][3] = 3200;
            this->prices_matrix[3][4] = 4000;
            this->prices_matrix[3][5] = 4800;
            this->prices_matrix[4][0] = 1100;
            this->prices_matrix[4][1] = 2200;
            this->prices_matrix[4][2] = 3300;
            this->prices_matrix[4][3] = 4400;
            this->prices_matrix[4][4] = 5500;
            this->prices_matrix[4][5] = 6600;
            break;
         case Taj_Mahal :
            this->name_txt = L"Taj Mahal";
            this->price = 1500;
            this->expropriation_price = 750;
            this->entrance_price = 100;
            this->n_max_phases = 4;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 2400;
            this->phases_prices[1] = 1000;
            this->phases_prices[2] = 500;
            this->phases_prices[3] = 1000;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
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
            this->prices_matrix[3][0] = 300;
            this->prices_matrix[3][1] = 600;
            this->prices_matrix[3][2] = 900;
            this->prices_matrix[3][3] = 1200;
            this->prices_matrix[3][4] = 1500;
            this->prices_matrix[3][5] = 1800;
            break;
         case Waikiki :
            this->name_txt = L"Waikiki";
            this->price = 2500;
            this->expropriation_price = 1250;
            this->entrance_price = 200;
            this->n_max_phases = 6;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 3500;
            this->phases_prices[1] = 2500;
            this->phases_prices[2] = 2500;
            this->phases_prices[3] = 1750;
            this->phases_prices[4] = 1750;
            this->phases_prices[5] = 2500;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
            this->prices_matrix[0][0] = 200;
            this->prices_matrix[0][1] = 400;
            this->prices_matrix[0][2] = 600;
            this->prices_matrix[0][3] = 800;
            this->prices_matrix[0][4] = 1000;
            this->prices_matrix[0][5] = 1200;
            this->prices_matrix[1][0] = 350;
            this->prices_matrix[1][1] = 700;
            this->prices_matrix[1][2] = 1050;
            this->prices_matrix[1][3] = 1400;
            this->prices_matrix[1][4] = 1750;
            this->prices_matrix[1][5] = 2100;
            this->prices_matrix[2][0] = 500;
            this->prices_matrix[2][1] = 1000;
            this->prices_matrix[2][2] = 1500;
            this->prices_matrix[2][3] = 2000;
            this->prices_matrix[2][4] = 2500;
            this->prices_matrix[2][5] = 3000;
            this->prices_matrix[3][0] = 500;
            this->prices_matrix[3][1] = 1000;
            this->prices_matrix[3][2] = 1500;
            this->prices_matrix[3][3] = 2000;
            this->prices_matrix[3][4] = 2500;
            this->prices_matrix[3][5] = 3000;
            this->prices_matrix[4][0] = 650;
            this->prices_matrix[4][1] = 1300;
            this->prices_matrix[4][2] = 1950;
            this->prices_matrix[4][3] = 2600;
            this->prices_matrix[4][4] = 3250;
            this->prices_matrix[4][5] = 3900;
            this->prices_matrix[5][0] = 1000;
            this->prices_matrix[5][1] = 2000;
            this->prices_matrix[5][2] = 3000;
            this->prices_matrix[5][3] = 4000;
            this->prices_matrix[5][4] = 5000;
            this->prices_matrix[5][5] = 6000;
            break;
         case Royal:
            this->name_txt = L"Royal";
            this->price = 2500;
            this->expropriation_price = 1250;
            this->entrance_price = 200;
            this->n_max_phases = 5;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 3600;
            this->phases_prices[1] = 2600;
            this->phases_prices[2] = 1800;
            this->phases_prices[3] = 1800;
            this->phases_prices[4] = 3000;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
            this->prices_matrix[0][0] = 150;
            this->prices_matrix[0][1] = 300;
            this->prices_matrix[0][2] = 450;
            this->prices_matrix[0][3] = 600;
            this->prices_matrix[0][4] = 750;
            this->prices_matrix[0][5] = 900;
            this->prices_matrix[1][0] = 300;
            this->prices_matrix[1][1] = 600;
            this->prices_matrix[1][2] = 900;
            this->prices_matrix[1][3] = 1200;
            this->prices_matrix[1][4] = 1500;
            this->prices_matrix[1][5] = 1800;
            this->prices_matrix[2][0] = 300;
            this->prices_matrix[2][1] = 600;
            this->prices_matrix[2][2] = 900;
            this->prices_matrix[2][3] = 1200;
            this->prices_matrix[2][4] = 1500;
            this->prices_matrix[2][5] = 1800;
            this->prices_matrix[3][0] = 450;
            this->prices_matrix[3][1] = 900;
            this->prices_matrix[3][2] = 1350;
            this->prices_matrix[3][3] = 1800;
            this->prices_matrix[3][4] = 2250;
            this->prices_matrix[3][5] = 2700;
            this->prices_matrix[4][0] = 600;
            this->prices_matrix[4][1] = 1200;
            this->prices_matrix[4][2] = 1800;
            this->prices_matrix[4][3] = 2400;
            this->prices_matrix[4][4] = 3000;
            this->prices_matrix[4][5] = 3600;
            break;
         case Safari:
            this->name_txt = L"Safari";
            this->price = 2000;
            this->expropriation_price = 1000;
            this->entrance_price = 150;
            this->n_max_phases = 4;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 2600;
            this->phases_prices[1] = 1200;
            this->phases_prices[2] = 1200;
            this->phases_prices[3] = 2000;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
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
            this->prices_matrix[2][0] = 250;
            this->prices_matrix[2][1] = 500;
            this->prices_matrix[2][2] = 750;
            this->prices_matrix[2][3] = 1000;
            this->prices_matrix[2][4] = 1250;
            this->prices_matrix[2][5] = 1500;
            this->prices_matrix[3][0] = 500;
            this->prices_matrix[3][1] = 1000;
            this->prices_matrix[3][2] = 1500;
            this->prices_matrix[3][3] = 2000;
            this->prices_matrix[3][4] = 2500;
            this->prices_matrix[3][5] = 3000;
            break;
         case Letoile:
            this->name_txt = L"L'etoile";
            this->price = 3000;
            this->expropriation_price = 1500;
            this->entrance_price = 250;
            this->n_max_phases = 6;
            this->phases_prices = vector<int>(this->n_max_phases);
            this->phases_prices[0] = 3300;
            this->phases_prices[1] = 2200;
            this->phases_prices[2] = 1800;
            this->phases_prices[3] = 1800;
            this->phases_prices[4] = 1800;
            this->phases_prices[5] = 4000;
            this->prices_matrix = vector<vector<int> > (this->n_max_phases, vector<int>(6));
            this->prices_matrix[0][0] = 150;
            this->prices_matrix[0][1] = 300;
            this->prices_matrix[0][2] = 450;
            this->prices_matrix[0][3] = 600;
            this->prices_matrix[0][4] = 750;
            this->prices_matrix[0][5] = 900;
            this->prices_matrix[1][0] = 300;
            this->prices_matrix[1][1] = 600;
            this->prices_matrix[1][2] = 900;
            this->prices_matrix[1][3] = 1200;
            this->prices_matrix[1][4] = 1500;
            this->prices_matrix[1][5] = 1800;
            this->prices_matrix[2][0] = 300;
            this->prices_matrix[2][1] = 600;
            this->prices_matrix[2][2] = 900;
            this->prices_matrix[2][3] = 1200;
            this->prices_matrix[2][4] = 1500;
            this->prices_matrix[2][5] = 1800;
            this->prices_matrix[3][0] = 300;
            this->prices_matrix[3][1] = 600;
            this->prices_matrix[3][2] = 900;
            this->prices_matrix[3][3] = 1200;
            this->prices_matrix[3][4] = 1500;
            this->prices_matrix[3][5] = 1800;
            this->prices_matrix[4][0] = 450;
            this->prices_matrix[4][1] = 900;
            this->prices_matrix[4][2] = 1350;
            this->prices_matrix[4][3] = 1800;
            this->prices_matrix[4][4] = 2250;
            this->prices_matrix[4][5] = 2700;
            this->prices_matrix[5][0] = 750;
            this->prices_matrix[5][1] = 1500;
            this->prices_matrix[5][2] = 2250;
            this->prices_matrix[5][3] = 3000;
            this->prices_matrix[5][4] = 3750;
            this->prices_matrix[5][5] = 4500;
            break;
         case None:
            break;
   }
}

void Hotel::Extend()
{
   this->n_built_phases++;
   if (this->n_built_phases == this->n_max_phases)
      this->ground_bought = true;
}

void Hotel::Add_entrance(int position)
{
   this->entrances.push_back(position);
   this->n_entrances++;
}

int Hotel::Price_next_expansion()
{
   return this->phases_prices[this->n_built_phases];
}

bool Hotel::Can_extend()
{
   return !this->ground_bought;
}

int Hotel::Calculate_nights(int number)
{
   return this->prices_matrix[this->n_built_phases-1][number-1]; // Substract to access correctly to prices matrix
}

void Hotel::Return_to_bank()
{
   // The hotel remains with everything
   this->owner = NULL;
}

Hotel::~Hotel(void)
{
}
