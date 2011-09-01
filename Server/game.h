#pragma once
#include <list>
#include <string>
#include "player.h"
#include "chat.h"

class Game
{
public:
   int id;
   string name;
   Player* creator;
   int n_players;
   list<Player*> plist;
   Chat* chat;

   bool join(Player* p);
   bool leave(Player* p);

   Game(string name, int n_players, Player* creator, list<Chat*>* chat_list);
   ~Game(void);
};
