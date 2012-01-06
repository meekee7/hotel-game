#include "hotel.h"

Hotel::Hotel(wstring name)
{
   this->prices_matrix = vector<vector<int> > (2, vector<int>(3));
   this->prices_matrix[0][0] = 1;
   this->prices_matrix[0][1] = 2;
   this->prices_matrix[1][2] = 3;
   this->name = name;
   this->owner = NULL;
   this->entrances = list<int>();
   this->n_entrances = 0;
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
