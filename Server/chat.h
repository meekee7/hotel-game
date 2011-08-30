#pragma once
#include <string>
#include <list>
#include <iostream>
#include "player.h"
using namespace std;

class chat
{

public:
   int id;
   player* creator;
   list<player*> players;

   bool join(player* p);
   bool leave(player* p);

   chat(player* creator);
   ~chat(void);
};

