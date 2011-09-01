#include "player.h"


Player::Player(string ip, Portable_socket* socket_client)
{
   this->ip = ip;
   this->socket = socket_client;
   this->connected = true;
}


Player::~Player(void)
{
   if (this->socket != NULL)
      delete this->socket;
}
