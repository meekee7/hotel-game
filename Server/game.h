#pragma once
#include <list>
#include <string>
#include "player.h"
#include "chat.h"
#include "types.h"
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

   void set_players_money(config configuration);
   bool join(Player* p);
   bool check_already_joined(Player* p);
   bool leave(Player* p);
   void start();
   int roll_dice();
   Player* turn_pass();
   int get_active_players_count();
   Player* get_winner(); // Only called when active players count is 1, so it gets first active player in list

   Game(wstring name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count);
   ~Game(void);
};
