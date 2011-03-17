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
   game(void);
   ~game(void);
};
