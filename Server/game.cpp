#include "game.h"
#include <iostream>


game::game(string name, int n_players, player* creator)
{
   this->name = name;
   this->n_players = n_players;
   this->creator = creator;
   this->plist.push_back(creator);
}

bool game::join(player* p)
{
   this->plist.push_back(p);
   cout << "Player "<< p->name <<" joined game " << this->name << endl;
   return true;
}

bool game::leave(player* p)
{
   bool found = false;
   list<player*>::iterator i = this->plist.begin();
	while (!found && i != this->plist.end())
	{
      if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (!found)
		cout << "Player " << p->name << " not found in game " << this->name << endl;
   else
   {
      this->plist.erase(i);
		cout << "Player "<< p->name <<" left game " << this->name << endl;
   }
   return found;
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
