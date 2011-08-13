#pragma once
#include <string>
#include <list>
#include "player.h"
using namespace std;

class chat
{

public:
   int id;
   player* creator;
   list<player*> players;

   chat(void);
   ~chat(void);
};

