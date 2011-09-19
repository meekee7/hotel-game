#pragma once
#include <string>
#include <list>
#include <iostream>
#include "player.h"
#include "dlib/threads.h"
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

   Chat(Player* creator, bool normal_chat, dlib::mutex* mutex_ids, int* id_count);
   ~Chat(void);
};

