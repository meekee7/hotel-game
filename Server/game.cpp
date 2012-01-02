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
      wcout << L"Player " << p->name << L" joined game " << this->name << endl;
      return true;
   }
   else
   {
      wcout << L"Player " << p->name << L" cant join game " << this->name << L" because it's full" << endl;
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
		wcout << L"Player " << p->name << L" not found in game " << this->name << endl;
   else
   {
      this->chat->leave(p);
      this->plist.erase(i);
		wcout << L"Player "<< p->name << L" left game " << this->name << endl;
   }
   return found;
}

void Game::start()
{
   // Roll dice to get first player
   int res = rand() % this->n_players;
   list<Player*>::iterator i = this->plist.begin();
   advance(i, res);
   this->current_player = (*i);
   this->starting_player = res;
   wcout << "Game " << this->name << " started. Player " << (*i)->name << " is the first" << endl;
}

int Game::roll_dice()
{
   int res = (rand() % 6) + 1;
   cout << "Dice result: " << res << endl;
   return res;
   //return (this->random.get_random_32bit_number() % 6 + 1);
}

Player* Game::turn_pass()
{
   list<Player*>::iterator i;
   i = find(this->plist.begin(), this->plist.end(), this->current_player);
   bool valid = false;
   while (!valid)
   {
      if (i == --this->plist.end())
         this->current_player = *(this->plist.begin());
      else
      {
         advance(i, 1);
         this->current_player = *i;
      }
      if (this->current_player->active)
         valid = true;
   }
   wcout << L"Turn passed, next player: " << this->current_player->name << endl;
   return this->current_player;
}

Game::~Game(void)
{
   this->creator = NULL;
   delete this->chat;
   this->plist.clear();
}
