#pragma once
#include <list>
#include <string>
#include "player.h"
#include "chat.h"
#include "types.h"
#include "hotel.h"
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
   int last_dice_res;
   int last_auto_advance;

   void set_players_money(config configuration);
   bool join(Player* p);
   bool check_already_joined(Player* p);
   bool leave(Player* p);
   void start();
   int roll_dice();
   Player* turn_pass();
   int get_active_players_count();
   Player* get_winner();
   void eliminate_player(Player* player);
   int get_money_for_nights(Player* owner, Player* player);

   Game(wstring name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count);
   ~Game(void);
};
