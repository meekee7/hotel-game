#include "chat.h"

chat::chat(player* creator, bool normal_chat)
{
   this->id = rand();
   this->creator = creator;
   this->normal_chat = normal_chat;
}

bool chat::join(player* p)
{
   this->players.push_back(p);
   cout << "Player "<< p->name <<" joined chat " << this->id << endl;
	return true;
}

bool chat::leave(player* p)
{
   bool found = false;
   list<player*>::iterator i = this->players.begin();
	while (!found && i != this->players.end())
	{
      if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (!found)
		cout << "Player " << p->name << " not found in chat " << this->id << endl;
   else
   {
      this->players.erase(i);
		cout << "Player "<< p->name <<" left chat " << this->id << endl;
   }
   return found;
}

chat::~chat(void)
{
   this->players.clear();
}
