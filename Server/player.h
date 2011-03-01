#pragma once
#include <string>
#include "portable_socket.h"
using namespace std;

class player
{

public:
   string name;
   string ip;
   portable_socket* socket;

   player(void);
   ~player(void);
};

