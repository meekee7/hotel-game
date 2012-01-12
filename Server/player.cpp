#include "player.h"

Player::Player(string ip, Portable_socket* socket_client)
{
   this->ip = ip;
   this->socket = socket_client;
   this->connected = true;
   this->active = true;
   this->paid_last_turn = false;
   this->position = new Position(0);
}

void Player::Calculate_total_money()
{
   this->total_money = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
}

void Player::Eliminate()
{
   this->active = false;
}

void Player::Charge_bank()
{
   this->n_1000 += 2;
   this->Calculate_total_money();
}

void Player::Buy_hotel(Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
   // Money its already calculated, client does the math
   this->hotels.push_back(hotel);
   this->Set_money(n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Expropriate_hotel(Hotel* hotel, Player* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50, int po_n_5000, int po_n_1000, int po_n_500, int po_n_100, int po_n_50)
{
   // Money its already calculated, client does the math
   this->hotels.push_back(hotel);
   this->Set_money(n_5000, n_1000, n_500, n_100, n_50);
   previous_owner->Set_money(po_n_5000, po_n_1000, po_n_500, po_n_100, po_n_50);
   previous_owner->hotels.remove(hotel);
}

void Player::Set_money(int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
   this->n_5000 = n_5000;
   this->n_1000 = n_1000;
   this->n_500 = n_500;
   this->n_100 = n_100;
   this->n_50 = n_50;
   this->Calculate_total_money();
}

void Player::Pay_nights (Player* to_player, int n5000, int n1000, int n500, int n100, int n50)
{
   // Transfiere los fondos del jugador que paga al que cobra, el precio viene calculado de la IU
   this->n_5000 -= n5000;
   this->n_1000 -= n1000;
   this->n_500 -= n500;
   this->n_100 -= n100;
   this->n_50 -= n50;
   this->Calculate_total_money();
   to_player->n_5000 += n5000;
   to_player->n_1000 += n1000;
   to_player->n_500 += n500;
   to_player->n_100 += n100;
   to_player->n_50 += n50;
   to_player->Calculate_total_money();
}

void Player::Return_change(int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
   this->n_5000 += n_5000;
   this->n_1000 += n_1000;
   this->n_500 += n_500;
   this->n_100 += n_100;
   this->n_50 += n_50;
   this->Calculate_total_money();
}

void Player::Pay_phase_or_entrance(int n5000, int n1000, int n500, int n100, int n50)
{
   this->n_5000 -= n5000;
   this->n_1000 -= n1000;
   this->n_500 -= n500;
   this->n_100 -= n100;
   this->n_50 -= n50;
   this->Calculate_total_money();
}

void Player::Take_5000_without_having_b5000(int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50)
{
   *n_5000 = 0;
   *n_1000 = 0;
   *n_500 = 0;
   *n_100 = 0;
   *n_50 = 0;

   int acumulado = 0;

   while (acumulado < 5000)
   {
      if (this->n_5000 > 0)
      {
         acumulado += 5000;
         (*n_5000)++;
         this->n_5000--;
      }
      else if (this->n_1000 > 0)
      {
         acumulado += 1000;
         (*n_1000)++;
         this->n_1000--;
      }
      else if (this->n_500 > 0)
      {
         acumulado += 500;
         (*n_500)++;
         this->n_500--;
      }
      else if (this->n_100 > 0)
      {
         acumulado += 100;
         (*n_100)++;
         this->n_100--;
      }
      else if (this->n_50 > 0)
      {
         acumulado += 50;
         (*n_50)++;
         this->n_50--;
      }
   }
   this->Calculate_total_money();
}

void Player::Take_1000_without_having_b1000(int* n_1000, int* n_500, int* n_100, int* n_50)
{
   *n_1000 = 0;
   *n_500 = 0;
   *n_100 = 0;
   *n_50 = 0;

   int acumulado = 0;

   while (acumulado < 1000)
   {
      if (this->n_1000 > 0)
      {
         acumulado += 1000;
         (*n_1000)++;
         this->n_1000--;
      }
      else if (this->n_500 > 0)
      {
         acumulado += 500;
         (*n_500)++;
         this->n_500--;
      }
      else if (this->n_100 > 0)
      {
         acumulado += 100;
         (*n_100)++;
         this->n_100--;
      }
      else if (this->n_50 > 0)
      {
         acumulado += 50;
         (*n_50)++;
         this->n_50--;
      }
      else // When there are not available bills, change them with the bank
      {
         if (this->n_5000 > 0)
         {
            this->n_5000--;
            this->n_1000 += 5;
         }
      }
   }
   this->Calculate_total_money();
}

void Player::Take_500_without_having_b500(int* n_500, int* n_100, int* n_50)
{
   *n_500 = 0;
   *n_100 = 0;
   *n_50 = 0;

   int acumulado = 0;

   while (acumulado < 500)
   {
      if (this->n_500 > 0)
      {
         acumulado += 500;
         (*n_500)++;
         this->n_500--;
      }
      else if (this->n_100 > 0)
      {
         acumulado += 100;
         (*n_100)++;
         this->n_100--;
      }
      else if (this->n_50 > 0)
      {
         acumulado += 50;
         (*n_50)++;
         this->n_50--;
      }
      else // When there are not available bills, change them with the bank
      {
         if (this->n_1000 > 0)
         {
            this->n_1000--;
            this->n_500 += 2;
         }
         else if (this->n_5000 > 0)
         {
            this->n_5000--;
            this->n_1000 += 5;
         }
      }
   }
   this->Calculate_total_money();
}

void Player::Take_100_without_having_b100(int* n_100, int* n_50)
{
   *n_100 = 0;
   *n_50 = 0;

   int acumulado = 0;

   while (acumulado < 100)
   {
      if (this->n_100 > 0)
      {
         acumulado += 100;
         (*n_100)++;
         this->n_100--;
      }
      else if (this->n_50 > 0)
      {
         acumulado += 50;
         (*n_50)++;
         this->n_50--;
      }
      else // When there are not available bills, change them with the bank
      {
         if (this->n_500 > 0)
         {
            this->n_500--;
            this->n_100 += 5;
         }
         else if (this->n_1000 > 0)
         {
            this->n_1000--;
            this->n_500 += 2;
         }
         else if (this->n_5000 > 0)
         {
            this->n_5000--;
            this->n_1000 += 5;
         }
      }
   }
   this->Calculate_total_money();
}

void Player::Take_50_without_having_b50(int* n_50)
{
   *n_50 = 0;

   int acumulado = 0;

   while (acumulado < 50)
   {
      if (this->n_50 > 0)
      {
         acumulado += 50;
         (*n_50)++;
         this->n_50--;
      }
      else // When there are not available bills, change them with the bank
      {
         if (this->n_100 > 0)
         {
            this->n_100--;
            this->n_50 += 2;
         }
         else if (this->n_500 > 0)
         {
            this->n_500--;
            this->n_100 += 5;
         }
         else if (this->n_1000 > 0)
         {
            this->n_1000--;
            this->n_500 += 2;
         }
         else if (this->n_5000 > 0)
         {
            this->n_5000--;
            this->n_1000 += 5;
         }
      }
   }
   this->Calculate_total_money();
}

Player::~Player(void)
{
   if (this->socket != NULL)
      delete this->socket;
   delete this->position;
}
