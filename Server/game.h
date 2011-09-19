#pragma once
#include <list>
#include <string>
#include "player.h"
#include "chat.h"
#include "dlib/threads.h"

class Game
{
public:
   int id;
   string name;
   Player* creator;
   int n_players;
   list<Player*> plist;
   Chat* chat;
   bool started;

   bool join(Player* p);
   bool leave(Player* p);
   void start();
   int dice();

   Game(string name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count);
   ~Game(void);
};
