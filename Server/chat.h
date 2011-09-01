#pragma once
#include <string>
#include <list>
#include <iostream>
#include "player.h"
using namespace std;

class Chat
{

public:
   int id;
   Player* creator;
   list<Player*> players;
   bool normal_chat; // True: standard chat; False: game chat

   bool join(Player* p);
   bool leave(Player* p);

   Chat(Player* creator, bool normal_chat);
   ~Chat(void);
};

