#include "game.h"
#include <iostream>

Game::Game(wstring name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count)
{
   this->name = name;
   this->n_players = n_players;
   this->creator = creator;
   this->plist.push_back(creator);
   this->chat = new Chat(this->creator, false, mutex_ids, id_count);
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
      wcout << "Player "<< p->name <<" joined game " << this->name << endl;
      return true;
   }
   else
   {
      wcout << "Player "<< p->name <<" cant join game " << this->name << " because it's full" << endl;
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
		wcout << "Player " << p->name << " not found in game " << this->name << endl;
   else
   {
      this->chat->leave(p);
      this->plist.erase(i);
		wcout << "Player "<< p->name <<" left game " << this->name << endl;
   }
   return found;
}

void Game::start()
{
}

int Game::dice()
{
   /*for (int i = 0 ; i < 10 ; i++)
   {
      rand();
   }*/
   //return (rand() % 6) + 1;
   return (this->random.get_random_32bit_number() % 6 + 1);
}

Game::~Game(void)
{
   this->creator = NULL;
   delete this->chat;
   this->plist.clear();
}
