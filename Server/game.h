#pragma once
#include <list>
#include <string>
#include "player.h"
#include "chat.h"
#include "dlib/threads.h"
#include "dlib/rand.h"

class Game
{
public:
   int id;
   wstring name;
   Player* creator;
   int n_players;
   list<Player*> plist;
   Chat* chat;
   bool started;
   dlib::rand random;
   Player* current_player;
   int starting_player;

   bool join(Player* p);
   bool leave(Player* p);
   void start();
   int roll_dice();

   Game(wstring name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count);
   ~Game(void);
};
