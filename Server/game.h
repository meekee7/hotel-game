#pragma once
#include <list>
#include <string>
#include "player.h"

class game
{
	int id;
	string name;
	player* creator;
	list<player*> plist;
public:
   game(void);
   ~game(void);
};
