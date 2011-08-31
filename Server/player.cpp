#include "player.h"


player::player(string ip, portable_socket* socket_client)
{
   this->ip = ip;
   this->socket = socket_client;
   this->connected = true;
}


player::~player(void)
{
   if (this->socket != NULL)
      delete this->socket;
}
