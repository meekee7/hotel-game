#pragma once
#include <list>
#include <string>
#include "player.h"

class game
{
public:
   string name;
   player* creator;
   int n_players;
   list<player*> plist;

   bool join(player* p);
   bool leave(player* p);

   game(string name, int n_players, player* creator);
   ~game(void);
};
