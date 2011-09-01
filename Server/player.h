#pragma once
#include <string>
#include "portable_socket.h"
using namespace std;

class Player
{

public:
   string name;
   string ip;
   Portable_socket* socket;
   bool connected;

   Player(string ip, Portable_socket* socket_client);
   ~Player(void);
};

