#pragma once
#include <string>
#include "portable_socket.h"
using namespace std;

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

   Player(string ip, Portable_socket* socket_client);
   void calculate_total_money();
   void Eliminate();
   ~Player(void);
};

