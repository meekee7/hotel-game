#include "player.h"


Player::Player(string ip, Portable_socket* socket_client)
{
   this->ip = ip;
   this->socket = socket_client;
   this->connected = true;
}

void Player::calculate_total_money()
{
   this->total_money = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
}

Player::~Player(void)
{
   if (this->socket != NULL)
      delete this->socket;
}
