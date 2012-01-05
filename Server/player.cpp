#include "player.h"


Player::Player(string ip, Portable_socket* socket_client)
{
   this->ip = ip;
   this->socket = socket_client;
   this->connected = true;
   this->active = true;
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

void Player::Set_money(int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
   this->n_5000 = n_5000;
   this->n_1000 = n_1000;
   this->n_500 = n_500;
   this->n_100 = n_100;
   this->n_50 = n_50;
}

Player::~Player(void)
{
   if (this->socket != NULL)
      delete this->socket;
}
