#pragma once
#include <list>
#include <string>
#include "player.h"
#include "chat.h"
#include "types.h"
#include "hotel.h"
#include "random.h"
#include "dlib/threads.h"

class Game
{
public:
   int id;
   wstring name;
   Player* creator;
   int n_players;
   list<Player*> plist, active_plist;
   list<Hotel*> hlist;
   Chat* chat;
   bool started, ended, rolled_construction_dice;
   Player* current_player;
   int starting_player;
   int last_dice_res;
   int last_auto_advance;
   int turn_count; // For statistics
   vector<Position*> positions;
   CRandomMT* random;
   TBuild_dice_res last_construction_dice_res;
   // Auctions
   Player* best_bidder;
   int best_bid;
   Hotel* hotel_at_auction;

   void set_players_money(config configuration);
   bool join(Player* p);
   bool check_already_joined(Player* p);
   bool leave(Player* p);
   void start();
   int roll_dice();
   TBuild_dice_res roll_construction_dice();
   void move_player(Player* p, dlib::mutex* debt_mutex);
   Player* turn_pass(dlib::mutex* debt_mutex);
   int get_active_players_count();
   Player* get_winner();
   bool is_active(Player* player);
   void eliminate_player(Player* player, Player* reiciving_player);
   int get_money_for_nights(Player* owner, Player* player, int* nights);
   bool can_charge_bank(Player* p);
   bool can_buy_entrances(Player* p);
   void calculate_return (int quantity, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50);
   void calculate_return(Player* player, int quantity, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50);

   Game(wstring name, int n_players, Player* creator, dlib::mutex* mutex_ids, int* id_count, CRandomMT* random);
   ~Game(void);
};
