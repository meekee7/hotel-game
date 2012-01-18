#pragma once
#include <string>
#include <list>
#include "portable_socket.h"
#include "types.h"
#include "position.h"

using namespace std;

class Hotel;

class Player
{
public:
   wstring name;
   string ip;
   Portable_socket* socket;
   bool connected;
   bool active;
   bool paid_last_turn, rolled_last_turn, bought_last_turn, built_last_turn, charged_bank_last_turn, put_entrance_last_turn;
   int n_5000, n_1000, n_500, n_100, n_50;
   int total_money;
   Position* position;
   TColor color;
   list<Hotel*> hotels;

   Player(string ip, Portable_socket* socket_client);
   void Calculate_total_money();
   void Eliminate();
   void Charge_bank();
   void Buy_hotel(Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50);
   void Buy_hotel(Hotel* hotel, Player* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50);
   void Expropriate_hotel(Hotel* hotel);
   void Set_money(int n_5000, int n_1000, int n_500, int n_100, int n_50);
   void Pay_nights(Player* to_player, int n5000, int n1000, int n500, int n100, int n50);
   void Return_change(int n_5000, int n_1000, int n_500, int n_100, int n_50);
   void Pay_phase_or_entrance(int n5000, int n1000, int n500, int n100, int n50);
   void Take_5000_without_having_b5000(int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50);
   void Take_1000_without_having_b1000(int* n_1000, int* n_500, int* n_100, int* n_50);
   void Take_500_without_having_b500(int* n_500, int* n_100, int* n_50);
   void Take_100_without_having_b100(int* n_100, int* n_50);
   void Take_50_without_having_b50(int* n_50);
   ~Player(void);
};

