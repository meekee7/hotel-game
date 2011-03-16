#include "game.h"


game::game(void)
{
}


game::~game(void)
{
   list<player>::iterator i;
	for (i = this->plist.begin() ; i != this->plist.end() ; ++i)
	{
      this->plist.erase(i);
	}
}
