#include "game.h"
#include <iostream>


Game::Game(string name, int n_players, Player* creator)
{
   this->name = name;
   this->n_players = n_players;
   this->creator = creator;
   this->plist.push_back(creator);
   this->chat = new Chat(this->creator, false);
   this->chat->join(this->creator);
   this->id = this->chat->id;
   this->started = false;
}

bool Game::join(Player* p)
{
   if ((int) this->plist.size() < this->n_players)
   {
      this->plist.push_back(p);
      this->chat->join(p);
      cout << "Player "<< p->name <<" joined game " << this->name << endl;
      return true;
   }
   else
   {
      cout << "Player "<< p->name <<" cant join game " << this->name << " because it's full" << endl;
      return false;
   }
}

bool Game::leave(Player* p)
{
   bool found = false;
   list<Player*>::iterator i = this->plist.begin();
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
      this->chat->leave(p);
      this->plist.erase(i);
		cout << "Player "<< p->name <<" left game " << this->name << endl;
   }
   return found;
}

void Game::start()
{
}

int Game::dice()
{
   return (rand() % 6) + 1;
}

Game::~Game(void)
{
   this->creator = NULL;
   delete this->chat;
   this->plist.clear();
}
