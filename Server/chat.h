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
   bool normal_chat; // True: standard chat; False: game chat

   bool join(player* p);
   bool leave(player* p);

   chat(player* creator, bool normal_chat);
   ~chat(void);
};

