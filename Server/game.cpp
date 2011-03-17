#include "game.h"
#include <iostream>


game::game(void)
{
}


game::~game(void)
{
   this->creator = NULL;
   list<player*>::iterator i;
	for (i = this->plist.begin() ; i != this->plist.end() ; ++i)
	{
      *i = NULL;
	}
}
