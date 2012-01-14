#include "game.h"
#include "position.h"
#include <iostream>

Game::Game(wstring name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count, CRandomMT* random)
{
   this->random = random;
   this->name = name;
   this->n_players = n_players;
   this->creator = creator;
   this->plist.push_back(creator);
   this->chat = new Chat(this->creator, false, mutex_ids, id_count);
   this->chat->join(this->creator);
   this->id = this->chat->id;
   this->started = false;
   // Create all positions
   this->positions = vector<Position*>(32);
   for (int i = 0 ; i < 32 ; i++)
   {
      this->positions[i] = new Position(i);
   }
}

void Game::set_players_money(config configuration)
{
   list<Player*>::iterator i;
   if (this->n_players > 2)
   {
      for (i = this->plist.begin(); i != this->plist.end() ; ++i)
      {
         (*i)->n_50 = configuration.three_or_four_players.n_50;
         (*i)->n_100 = configuration.three_or_four_players.n_100;
         (*i)->n_500 = configuration.three_or_four_players.n_500;
         (*i)->n_1000 = configuration.three_or_four_players.n_1000;
         (*i)->n_5000 = configuration.three_or_four_players.n_5000;
         (*i)->Calculate_total_money();
      }
   }
   else
   {
      for (i = this->plist.begin(); i != this->plist.end() ; ++i)
      {
         (*i)->n_50 = configuration.two_players.n_50;
         (*i)->n_100 = configuration.two_players.n_100;
         (*i)->n_500 = configuration.two_players.n_500;
         (*i)->n_1000 = configuration.two_players.n_1000;
         (*i)->n_5000 = configuration.two_players.n_5000;
         (*i)->Calculate_total_money();
      }
   }
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

bool Game::check_already_joined(Player* p)
{
   list<Player*>::iterator i = find(this->plist.begin(), this->plist.end(), p);
   if (i == plist.end())
      return false;
   else
      return true;
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
   int res = (this->random->RollDice(6, 1) - 1) % this->n_players;
   list<Player*>::iterator i = this->plist.begin();
   advance(i, res);
   this->current_player = (*i);
   this->starting_player = res;
   wcout << "Game " << this->name << " started. Player " << (*i)->name << " is the first (" << res << ")" << endl;
}

int Game::roll_dice()
{
   this->last_dice_res = this->random->RollDice(6, 1);
   wcout << "Dice result: " << this->last_dice_res << endl;
   return this->last_dice_res;
}

void Game::move_player(Player* p)
{
   // Uses last dice result
   p->position->occupied = false; // Free the position
   if ((p->position->number + this->last_dice_res) <= 31) // One lap
      p->position = this->positions[p->position->number + this->last_dice_res];
   else
   {
         p->position = this->positions[p->position->number + this->last_dice_res - 31];
   }
   this->last_auto_advance = 0;
   while (p->position->occupied) // We need to advance because it's occupied
   {
      if (p->position->number < 31) // Proteger la vuelta al tablero
         p->position = this->positions[p->position->number + 1];
      else
         p->position = this->positions[1];
      this->last_auto_advance++;
   }
   p->position->occupied = true; // Ocupamos la casilla
}

Player* Game::turn_pass()
{
   list<Player*>::iterator i;
   i = find(this->plist.begin(), this->plist.end(), this->current_player);
   bool valid = false;
   while (!valid)
   {
      if (i == --this->plist.end())
      {
         this->current_player = *(this->plist.begin());
         i = plist.begin();
      }
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

int Game::get_active_players_count()
{
   int num = 0;
   list<Player*>::iterator i;
   for (i = this->plist.begin() ; i != this->plist.end() ; ++i)
   {
      if ((*i)->active)
         num++;
   }
   return num;
}

Player* Game::get_winner() // Only called when active players count is 1, so it gets first active player in list
{
   list<Player*>::iterator i = this->plist.begin();
   bool found = false;
   while (!found && (i != this->plist.end())) // Second part shouldn't happen
   {
      if ((*i)->active)
         found = true;
      else
         ++i;
   }
   return (*i);
}

void Game::eliminate_player(Player* player)
{
   player->Eliminate();
   // Return all hotels to bank
   list<Hotel*>::iterator i;
   for (i = player->hotels.begin() ; i != player->hotels.end() ; ++i)
   {
      (*i)->Return_to_bank();
   }
   player->hotels.clear();
}

int Game::get_money_for_nights(Player* owner, Player* player) // Checks player position in 'owner' hotels, rolls a dice and returns total amount (0 if not in any entrance)
{
   list<Hotel*>::iterator i;
   int amount = 0, dice_res = 0;
   list<int>::iterator pos;
   for (i = owner->hotels.begin() ; i != owner->hotels.end() ; ++i)
   {
      pos = find((*i)->entrances.begin(), (*i)->entrances.end(), player->position->number);
      if (pos != (*i)->entrances.end()) // Player is in a entrance of this hotel, and it cant be in any other entrance
      {
         dice_res = this->random->RollDice(6, 1);
         wcout << L"Player: " << player->name << " must pay " << dice_res << " nights to player " << owner->name << endl;
         amount = (*i)->prices_matrix[(*i)->n_built_expansions-1][dice_res-1];
      }
   }
   return amount;
}

bool Game::can_charge_bank(Player* p)
{
   if ((this->n_players == 2) || ((this->get_active_players_count() > 2) && (this->n_players > 2)))
   {
      if ((p->position->number >= 8) && ((p->position->number - this->last_dice_res - this->last_auto_advance) < 8))
         return true;
      else
         return false;
   }
   else
      return false;
}

Game::~Game(void)
{
   this->creator = NULL;
   delete this->chat;
   this->plist.clear();
}
