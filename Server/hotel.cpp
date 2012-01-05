#include "hotel.h"

Hotel::Hotel(wstring name)
{
   this->prices_matrix = vector<vector<int> > (2, vector<int>(3));
   this->prices_matrix[0][0] = 1;
   this->prices_matrix[0][1] = 2;
   this->prices_matrix[1][2] = 3;
   this->name = name;
   this->owner = NULL;
}

void Hotel::Extend()
{
   this->n_built_expansions++;
}

Hotel::~Hotel(void)
{
}
