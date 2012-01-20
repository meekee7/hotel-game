#include "chat.h"

Chat::Chat(Player* creator, bool normal_chat, dlib::mutex* mutex_ids, int* id_count)
{
   mutex_ids->lock();
   this->id = *id_count;
   ++(*id_count);
   mutex_ids->unlock();
   this->creator = creator;
   this->normal_chat = normal_chat;
}

bool Chat::join(Player* p)
{
   this->players.push_back(p);
   wcout << L"Player "<< p->name << L" joined chat " << this->id << endl;
	return true;
}

bool Chat::check_already_joined(Player* p)
{
   list<Player*>::iterator i = find(this->players.begin(), this->players.end(), p);
   if (i == this->players.end())
      return false;
   else
      return true;
}

bool Chat::leave(Player* p)
{
   bool found = false;
   list<Player*>::iterator i = this->players.begin();
	while (!found && i != this->players.end())
	{
      if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (!found)
		wcout << L"Player " << p->name << L" not found in chat " << this->id << " (WARNING: Possible hack)" << endl;
   else
   {
      this->players.erase(i);
		wcout << L"Player "<< p->name << L" left chat " << this->id << endl;
   }
   return found;
}

Chat::~Chat(void)
{
   this->creator = NULL;
   this->players.clear();
}
