#include "hotel.h"

Hotel::Hotel(void)
{
   this->prices_matrix = vector<vector<int> > (2, vector<int>(3));
   this->prices_matrix[0][0] = 1;
   this->prices_matrix[0][1] = 2;
   this->prices_matrix[1][2] = 3;
}

Hotel::~Hotel(void)
{
}
