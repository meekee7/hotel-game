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
   this->active_plist.push_back(creator);
   this->chat = new Chat(this->creator, false, mutex_ids, id_count);
   this->chat->join(this->creator);
   this->id = this->chat->id;
   this->started = false;
   this->ended = false;
   this->turn_count = 1;
   this->hotel_at_auction = NULL;
   this->best_bid = 0;
   this->best_bidder = NULL;
   // Create all positions
   this->positions = vector<Position*>(32);
   for (int i = 0 ; i < 32 ; i++)
      this->positions[i] = new Position(i);
   this->hlist.push_back(new Hotel(Fujiyama));
   this->hlist.push_back(new Hotel(Boomerang));
   this->hlist.push_back(new Hotel(Letoile));
   this->hlist.push_back(new Hotel(President));
   this->hlist.push_back(new Hotel(Royal));
   this->hlist.push_back(new Hotel(Waikiki));
   this->hlist.push_back(new Hotel(Taj_Mahal));
   this->hlist.push_back(new Hotel(Safari));
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
   if (((int) this->plist.size() < this->n_players))
   {
      if (this->started)
      {
         wcout << L"Player " << p->name << L" cant join game " << this->name << L" because is already started" << endl;
         return false;
      }
      if (this->ended)
      {
         wcout << L"Player " << p->name << L" cant join game " << this->name << L" because is already finished" << endl;
         return false;
      }
      this->plist.push_back(p);
      this->active_plist.push_back(p);
      this->chat->join(p);
      wcout << L"Player " << p->name << L" joined game " << this->name << endl;
      return true;
   }
   else
   {
      wcout << L"Player " << p->name << L" cant join game " << this->name << L" because is full" << endl;
      return false;
   }
}

bool Game::check_already_joined(Player* p)
{
   list<Player*>::iterator i = find(this->plist.begin(), this->plist.end(), p);
   if (i == this->plist.end())
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
		wcout << L"Player " << p->name << L" not found in game " << this->name << " (WARNING: Possible hack)" << endl;
   else
   {
      if (this->is_active(p))
         this->eliminate_player(p, NULL);
      this->chat->leave(p);
      this->plist.erase(i);
      if ((this->plist.size()) > 0 && (this->creator == p))
         this->creator = this->plist.front(); // New creator
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
   this->started = true;
   wcout << "Game " << this->name << " started. Player " << (*i)->name << " is the first (" << res << ")" << endl;
}

int Game::roll_dice()
{
   this->last_dice_res = this->random->RollDice(6, 1);
   wcout << "Dice result: " << this->last_dice_res << endl;
   return this->last_dice_res;
}

TBuild_dice_res Game::roll_construction_dice()
{
   int res = this->random->RollDice(6, 1);
   switch (res)
   {
      case 1:
      case 2:
      case 3: this->last_construction_dice_res = Allow;
              break;
      case 4: this->last_construction_dice_res = Free;
              break;
      case 5: this->last_construction_dice_res = Double;
              break;
      case 6: this->last_construction_dice_res = Deny;
              break;
   }
   wcout << L"Construction dice result: ";
   switch (this->last_construction_dice_res)
   {
      case 0: wcout << L"Allow" << endl;
              break;
      case 1: wcout << L"Free" << endl;
              break;
      case 2: wcout << L"Double" << endl;
              break;
      case 3: wcout << L"Deny" << endl;
              break;
   }
   this->rolled_construction_dice = true;
   return this->last_construction_dice_res;
}

void Game::move_player(Player* p, dlib::mutex* debt_mutex)
{
   // Uses last dice result
   p->position->occupied = false; // Free the position
   if ((p->position->number + this->last_dice_res) <= 31) // No new lap yet
      p->position = this->positions[p->position->number + this->last_dice_res];
   else
      p->position = this->positions[p->position->number + this->last_dice_res - 31];
   this->last_auto_advance = 0;
   while (p->position->occupied) // We need to advance because it's occupied
   {
      if (p->position->number < 31) // No new lap yet
         p->position = this->positions[p->position->number + 1];
      else
         p->position = this->positions[1];
      this->last_auto_advance++;
   }
   p->position->occupied = true; // Occupy the position
   this->current_player->bought_last_turn = false;
   this->current_player->built_last_turn = false;
   this->current_player->charged_bank_last_turn = false;
   this->current_player->paid_last_turn = false;
   this->current_player->free_entrance_used = false;
   this->rolled_construction_dice = false;
   if (this->current_player->debt_last_turn > 0)
   {
      debt_mutex->lock(); // To avoid skipping a debt just when player is passing turn, because ask_nights command is asynchronous
      this->current_player->debt_last_turn = 0;
      this->current_player->debt_nights_to_last_turn = NULL;
      debt_mutex->unlock();
   }
   // Allow again all players to ask for nights
   list<Player*>::iterator i;
   for (i = this->active_plist.begin() ; i != this->active_plist.end() ; ++i)
      (*i)->asked_nights_last_turn = false;
   list<Hotel*>::iterator i2;
   // Allow new entrances in all player hotels
   for (i2 = this->current_player->hotels.begin() ; i2 != this->current_player->hotels.end() ; ++i2)
      (*i2)->entrance_bought_last_turn = false;
}

Player* Game::turn_pass(dlib::mutex* debt_mutex)
{
   this->current_player->rolled_last_turn = false;
   // There is no need to reset more variables because the player always must roll before doing anything, and the function move_player does the reset
   if (this->current_player->debt_last_turn > 0)
   {
      debt_mutex->lock(); // To avoid skipping a debt just when player is passing turn, because ask_nights command is somehow asynchronous
      this->current_player->debt_last_turn = 0;
      this->current_player->debt_nights_to_last_turn = NULL;
      debt_mutex->unlock();
   }

   list<Player*>::iterator i;
   i = find(this->active_plist.begin(), this->active_plist.end(), this->current_player);
   if (i == --this->active_plist.end())
   {
      i = active_plist.begin();
      this->current_player = (*i);
   }
   else
   {
      advance(i, 1);
      this->current_player = *i;
   }
   this->turn_count++;
   wcout << L"Turn passed, next player: " << this->current_player->name << endl;
   return this->current_player;
}

int Game::get_active_players_count()
{
   return this->active_plist.size();
   /*int num = 0;
   list<Player*>::iterator i;
   for (i = this->plist.begin() ; i != this->plist.end() ; ++i)
   {
      if ((*i)->active)
         num++;
   }
   return num;*/
}

Player* Game::get_winner() // Only called when active players count is 1, so it gets first active player in list
{
   return this->active_plist.front();
   /*list<Player*>::iterator i = this->active_plist.begin();
   bool found = false;
   while (!found && (i != this->active_plist.end())) // Second part shouldn't happen
   {
      if ((*i)->active)
         found = true;
      else
         ++i;
   }
   return (*i);*/
}

bool Game::is_active(Player* player)
{
   list<Player*>::iterator i;
   i = find(this->active_plist.begin(), this->active_plist.end(), player);
   if (i == this->active_plist.end())
      return false;
   else
      return true;
}

void Game::eliminate_player(Player* player, Player* reiciving_player)
{
   // Find the player
   list<Player*>::iterator i;
   if (this->active_plist.size() > 0)
   {
      i = find(this->active_plist.begin(), this->active_plist.end(), player);
      if (i != this->active_plist.end())
         this->active_plist.erase(i);
   }
   // Return all hotels to bank
   list<Hotel*>::iterator i2;
   for (i2 = player->hotels.begin() ; i2 != player->hotels.end() ; ++i2)
   {
      (*i2)->Return_to_bank();
   }
   player->hotels.clear();
   if (reiciving_player != NULL)
   {
      player->Pay_nights(reiciving_player, player->n_5000, player->n_1000, player->n_500, player->n_100, player->n_50);
   }
}

int Game::get_money_for_nights(Player* owner, Player* player, int* nights) // Checks 'player' position in 'owner' hotels, rolls a dice and returns total amount (0 if not in any entrance or already paid)
{
   list<Hotel*>::iterator i;
   int amount = 0, dice_res = 0;
   list<int>::iterator pos;
   for (i = owner->hotels.begin() ; i != owner->hotels.end() ; ++i)
   {
      pos = find((*i)->entrances.begin(), (*i)->entrances.end(), player->position->number);
      if ((pos != (*i)->entrances.end()) && (!player->paid_last_turn)) // Player is in an entrance of this hotel, (it can't be in any other entrance). Enters only if player hasn't already paid this turn
      {
         dice_res = this->random->RollDice(6, 1);
         wcout << L"Player: " << player->name << " must pay " << dice_res << " nights to player " << owner->name << endl;
         amount = (*i)->prices_matrix[(*i)->n_built_phases-1][dice_res-1];
      }
   }
   (*nights) = dice_res;
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

bool Game::can_buy_entrances(Player* p)
{
   int pos = p->position->number;
   if (pos - this->last_dice_res - this->last_auto_advance < 1) // In case of doing a lap in same roll
      pos += 31;
   if ((pos >= 27) && ((pos - this->last_dice_res - this->last_auto_advance) < 27))
      return true;
   else
      return false;
}

void Game::calculate_return (int quantity, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50)
{
   (*n_5000) = 0;
   (*n_1000) = 0;
   (*n_500) = 0;
   (*n_100) = 0;
   (*n_50) = 0;

   while (quantity > 0)
   {
         if (quantity >= 5000)
         {
            quantity -= 5000;
            (*n_5000)++;
         }
         else if (quantity >= 1000)
         {
            quantity -= 1000;
            (*n_1000)++;
         }
         else if (quantity >= 500)
         {
            quantity -= 500;
            (*n_500)++;
         }
         else if (quantity >= 100)
         {
            quantity -= 100;
            (*n_100)++;
         }
         else if (quantity >= 50)
         {
            quantity -= 50;
            (*n_50)++;
         }
   }
}

void Game::calculate_return(Player* player, int quantity, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50)
{
   // Al restar la devolución a un jugador, hay que haber ingresado los fondos previamente por si acaso el jugador
   // no tiene fondos suficientes para devolver antes de haber recibido el cobro
   // En el caso de que no tenga cambio justo, el sistema automáticamente obtendrá los billetes necesarios para que así sea, cambiando billetes con la banca
   (*n_5000) = 0;
   (*n_1000) = 0;
   (*n_500) = 0;
   (*n_100) = 0;
   (*n_50) = 0;
   int n_5000_, n_1000_, n_500_, n_100_, n_50_;

   while (quantity > 0)
   {
         if (quantity >= 5000)
         {
            if (player->n_5000 > 0)
            {
               player->n_5000--;
               (*n_5000)++;
            }
            else
            {
               player->Take_5000_without_having_b5000(&n_5000_, &n_1000_, &n_500_, &n_100_, &n_50_);
               (*n_5000) += n_5000_;
               (*n_1000) += n_1000_;
               (*n_500) += n_500_;
               (*n_100) += n_100_;
               (*n_50) += n_50_;
            }
            quantity -= 5000;
         }
         else if (quantity >= 1000)
         {
            if (player->n_1000 > 0)
            {
               player->n_1000--;
               (*n_1000)++;
            }
            else
            {
               player->Take_1000_without_having_b1000(&n_1000_, &n_500_, &n_100_, &n_50_);
               (*n_1000) += n_1000_;
               (*n_500) += n_500_;
               (*n_100) += n_100_;
               (*n_50) += n_50_;
            }
            quantity -= 1000;
         }
         else if (quantity >= 500)
         {
            if (player->n_500 > 0)
            {
               player->n_500--;
               (*n_500)++;
            }
            else
            {
               player->Take_500_without_having_b500(&n_500_, &n_100_, &n_50_);
               (*n_500) += n_500_;
               (*n_100) += n_100_;
               (*n_50) += n_50_;
            }
            quantity -= 500;
         }
         else if (quantity >= 100)
         {
            if (player->n_100 > 0)
            {
               player->n_100--;
               (*n_100)++;
            }
            else
            {
               player->Take_100_without_having_b100(&n_100_, &n_50_);
               (*n_100) += n_100_;
               (*n_50) += n_50_;
            }
            quantity -= 100;
         }
         else if (quantity >= 50)
         {
            if (player->n_50 > 0)
            {
               player->n_50--;
               (*n_50)++;
            }
            else
            {
               player->Take_50_without_having_b50(&n_50_);
               (*n_50) += n_50_;
            }
            quantity -= 50;
         }
   }
   player->Calculate_total_money();
}

Game::~Game(void)
{
   this->creator = NULL;
   delete this->chat;
   this->plist.clear();
   this->active_plist.clear();
}
