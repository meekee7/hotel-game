#include "player.h"


player::player(void)
{
}


player::~player(void)
{
   if (this->socket != NULL)
      delete this->socket;
}
