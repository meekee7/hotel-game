#pragma once
#include <string>
#include <list>
#include "portable_socket.h"
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
   int n_5000, n_1000, n_500, n_100, n_50;
   int total_money;
   int position;
   list<Hotel*> hotels;

   Player(string ip, Portable_socket* socket_client);
   void Calculate_total_money();
   void Eliminate();
   void Charge_bank();
   void Buy_hotel(Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50);
   ~Player(void);
};

