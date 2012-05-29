#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include <ctime>
#include <cstdlib>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "chat.h"
#include "tinyxml.h"
#include "hotel.h"
#include "types.h"
#include "random.h"
#include "dlib/threads.h"
#include "dlib/string.h"

#define MAXCONN 100

using namespace std;

volatile int closing = 0;
Portable_socket* socket_server;
Portable_socket* socket_client;
list<Player*> plist; // Player list
list<Game*> glist; // Game list
list<Player*> global_chat_list; // Players in global chat
list<Chat*> chat_list;
dlib::mutex mutex_ids;
dlib::mutex mutex_lists;
dlib::mutex mutex_disconnects;
dlib::mutex debt_mutex;
int id_count = 0;
string config_content;
struct config configuration;
CRandomMT* random_gen;

void unhook_signals()
{
   signal(SIGINT, 0);
   signal(SIGTERM, 0);
   #ifdef _WIN32
      signal(SIGBREAK, 0);
   #endif
}

void empty_global_chat_list()
{
   list<Player*>::iterator i;
	for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
	{
		*i = NULL;
	}
}

void empty_chat_list()
{
   list<Chat*>::iterator i;
	for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
	{
      Chat* c = *i;
      delete c;
	}
}

void empty_glist()
{
   list<Game*>::iterator i;
	for (i = glist.begin() ; i != glist.end() ; ++i)
	{
      Game* g = *i;
      delete g;
	}
}

void empty_plist()
{
	list<Player*>::iterator i;
	for (i = plist.begin() ; i != plist.end() ; ++i)
	{
      Player* p = *i;
      delete p;
	}
}

void close_server (int signum)
{
   closing = 1;
   wcout << endl << L"Closing server" << endl;
   unhook_signals();
   delete socket_server;
}

void hook_signals()
{
   signal(SIGINT, close_server);
   signal(SIGTERM, close_server);
   #ifdef _WIN32
      signal(SIGBREAK, close_server);
   #endif
}

bool add_player_to_player_list (Player* p)
{
   mutex_lists.lock();
	bool found = false;
	list<Player*>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (found)
	{
		wcout << L"Player already connected" << endl;
      mutex_lists.unlock();
		return false;
	}
	else
	{
		wcout << L"Player accepted" << endl;
		plist.push_back(p);
      mutex_lists.unlock();
		return true;
	}
}

bool delete_player_from_player_list(Player* p)
{
   mutex_lists.lock();
   bool found = false;
	list<Player*>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (!found)
   {
		wcout << L"Player not found in player list" << endl;
      mutex_lists.unlock();
      return false;
   }
	else
	{
		wcout << L"Player deleted from player list" << endl;
		plist.erase(i);
      mutex_lists.unlock();
      return true;
	}
}

Player* get_player_from_name(wstring name)
{
   bool found = false;
	list<Player*>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if ((*i)->name == name)
			found = true;
      else
         ++i;
	}
	if (!found)
		return NULL;
	else
		return (*i);
}

Player* get_player_from_game(wstring name, Game* game)
{
   bool found = false;
	list<Player*>::iterator i = game->plist.begin();
	while (!found && i != game->plist.end())
	{
		if ((*i)->name == name)
			found = true;
      else
         ++i;
	}
	if (!found)
		return NULL;
	else
   {
      if (game->is_active(*i)) // To avoid hack: sending commands when retired
		   return (*i);
      else
         return NULL;
   }
}

Hotel* get_hotel_from_name(wstring name_txt, Game* game)
{
   bool found = false;
	list<Hotel*>::iterator i = game->hlist.begin();
	while (!found && i != game->hlist.end())
	{
		if ((*i)->name_txt == name_txt)
			found = true;
      else
         ++i;
	}
	if (!found)
		return NULL;
	else
		return (*i);
}

bool delete_chat_if_empty(Chat* chat)
{
   mutex_lists.lock();
   if (chat->players.empty())
   {
      wcout << L"Chat " << chat->id << L" deleted because it's empty" << endl;
      chat_list.remove(chat);
      delete chat;
      mutex_lists.unlock();
      return true;
   }
   mutex_lists.unlock();
   return false;
}

bool delete_game_if_empty(Game* game)
{
   mutex_lists.lock();
   if (game->plist.empty())
   {
      wcout << L"Game " << game->name << L" deleted because it's empty" << endl;
      glist.remove(game);
      delete game;
      mutex_lists.unlock();
      return true;
   }
   else
   {
      mutex_lists.unlock();
      return false;
   }
}

Game* get_game_from_name(wstring name)
{
   bool found = false;
   list<Game*>::iterator i = glist.begin();
   while (!found && i != glist.end())
   {
      if ((*i)->name == name)
         found = true;
      else
         ++i;
   }
   if (!found)
      return NULL;
   else
      return (*i);
}

Game* get_game_from_id(int id)
{
   bool found = false;
   list<Game*>::iterator i = glist.begin();
   while (!found && i != glist.end())
   {
      if ((*i)->id == id)
         found = true;
      else
         ++i;
   }
   if (!found)
      return NULL;
   else
      return (*i);
}

Chat* get_chat_from_id(int id)
{
   bool found = false;
	list<Chat*>::iterator i = chat_list.begin();
	while (!found && i != chat_list.end())
	{
		if ((*i)->id == id)
			found = true;
      else
         ++i;
	}
   if (!found)
   {
      // If not found it should be a game chat
      return get_game_from_id(id)->chat;
   }
   else
      return (*i);
}

int get_utf8_length(wstring data)
{
   int res;
   #ifdef _WIN32
      res = WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
   #else
      res = wcstombs(NULL, data.c_str(), 0);
   #endif
   return res;
}

wstring utf8_to_utf16 (string data)
{
   if (data.empty())
      return L"";
   wstring res;
   #ifdef _WIN32
      res.resize(MultiByteToWideChar(CP_UTF8, 0, data.data(), data.length(), NULL, 0));
      MultiByteToWideChar(CP_UTF8, 0, data.data(), data.length(), &res[0], res.length());
   #else
      res.resize(mbstowcs(NULL, data.c_str(), 0)+1);
      mbstowcs((wchar_t*)&res.data()[0], data.c_str(), res.size());
   #endif
   return res;
}

string utf16_to_utf8 (wstring data)
{
   if (data.empty())
      return "";
   #ifdef _WIN32
      int size_utf8 = WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
   #else
      //setlocale(LC_ALL, "es_ES.utf8");
      int size_utf8 = wcstombs(NULL, data.data(), 0);
   #endif
   string data_utf8;
   data_utf8.resize(size_utf8);
   #ifdef _WIN32
      WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), &data_utf8[0], data_utf8.length(), NULL, NULL);
   #else
      //setlocale(LC_ALL, "es_ES.utf8");
      wcstombs(&data_utf8[0], data.data(), size_utf8);
   #endif
   return data_utf8;
}

string receive_string (Player* p, int length, int* bytes_received)
{
   char* data = new char[length+1];
   *bytes_received = p->socket->precv(data, length, 0);
   string s_data;
   if (*bytes_received > 0)
   {
      data[length] = '\0';
      s_data = string(data);
   }
   else
      s_data = string("");
   delete data;
   return s_data;
}

wstring receive_wstring (Player* p, int length, int* bytes_received)
{
   char* data = new char[length+1];
   *bytes_received = p->socket->precv(data, length, 0);
   wstring s_data;
   if (*bytes_received > 0)
   {
      data[length] = '\0';
      s_data = utf8_to_utf16(data);
   }
   else
      s_data = wstring(L"");
   delete data;
   return s_data;
}

int receive_int (Player* p, int* bytes_received)
{
   int data;
   *bytes_received = p->socket->precv(&data, sizeof(data), 0);
   if (*bytes_received > 0)
      data = ntohl(data);
   else
      data = 0;
   return data;
}

int send_string (Player* p, string data)
{
   return p->socket->psend(data.c_str(), data.length(), 0);
}

int send_wstring (Player* p, wstring data)
{
   string data_utf8 = utf16_to_utf8(data);
   const char* c_data = data_utf8.c_str();
   int c_length = strlen(c_data);
   return p->socket->psend(c_data, c_length, 0);
}

int send_int (Player* p, int data)
{
   data = htonl(data);
   return p->socket->psend(&data, sizeof(data), 0);
}

void send_command(string command, Player* p)
{
   wstring w_command;
   w_command.assign(command.begin(), command.end());
	wcout << L"Sending command to player " << p->name << L": " << w_command << endl;
   send_int(p, command.length());
   send_string(p, command);
}

void disconnect_client(Player* p, bool kicking)
{
   if (!p->connected)
      return;
   mutex_disconnects.lock();
   // Leave all normal chats and global chat
   list<Chat*>::iterator i;
   for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
   {
      if ((*i)->check_already_joined(p))
      {
         (*i)->leave(p);
         if (delete_chat_if_empty(get_chat_from_id((*i)->id)))
         {
            if (chat_list.size() > 0)
               i = chat_list.begin(); // When deleting a chat, I prefer starting again to avoid segmentation faults
            else
               break; // If it was the last chat, chat_list.begin() returns an invalid pointer, so the loop must end
         }
         else
         {
            // Notify all chat users of the player disconnexion
            list<Player*>::iterator i4, j;
            Player* dest;
            for (i4 = (*i)->players.begin() ; i4 != (*i)->players.end() ; ++i4)
            {
               dest = *i4;
               send_command("chat_userlist", dest);
               send_int(dest, (*i)->id);
               send_int(dest, (*i)->players.size()); // Number of players
               for (j = (*i)->players.begin() ; j != (*i)->players.end() ; ++j)
               {
                  send_int(dest, get_utf8_length((*j)->name));
                  send_wstring(dest, (*j)->name);
               }
            }
         }
      }
   }
   global_chat_list.remove(p);
   // Leave games
   list<Game*>::iterator i2;
   for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
   {
      if ((*i2)->check_already_joined(p))
      {
         (*i2)->leave(p);
         if (delete_game_if_empty(*i2))
         {
            if (glist.size() > 0)
               i2 = glist.begin(); // When deleting a game, I prefer starting again to avoid segmentation faults
            else
               break; // If it was the last game, glist.begin() returns an invalid pointer, so the loop must end
         }
         else
         {
            // Notify the rest of players that the player left the game
            list<Player*>::iterator i3, j;
            Player* dest, * winner;
            for (i3 = (*i2)->plist.begin() ; i3 != (*i2)->plist.end() ; ++i3)
            {
               dest = (*i3);
               if (kicking)
               {
                  send_command("player_kicked", dest);
                  send_int(dest, (*i2)->id);
                  send_int(dest, get_utf8_length(p->name));
                  send_wstring(dest, p->name);
               }
               else
               {
                  send_command("player_retired", dest);
                  send_int(dest, (*i2)->id);
                  send_int(dest, get_utf8_length(p->name));
                  send_wstring(dest, p->name);
               }
               send_command("chat_userlist", dest);
               send_int(dest, (*i2)->id);
               send_int(dest, (*i2)->plist.size()); // Number of players
               for (j = (*i2)->plist.begin() ; j != (*i2)->plist.end() ; ++j)
               {
                  send_int(dest, get_utf8_length((*j)->name));
                  send_wstring(dest, (*j)->name);
               }
               if ((*i2)->get_active_players_count() == 1)
               {
                  (*i2)->ended = true;
                  send_command("game_ended", dest);
                  send_int(dest, (*i2)->id);
                  winner = (*i2)->get_winner();
                  send_int(dest, get_utf8_length(winner->name));
                  send_wstring(dest, winner->name);
               }
            }
         }
      }
   }
   delete_player_from_player_list(p);
   p->connected = false;
   mutex_disconnects.unlock();
}

void kick_hacker(int reason, Player* p) // Retire from all games and disconnect him using existing function
{
   wcout << "Kicking player " << p->name << " for cheating. Reason code: " << reason << " (See source code for code correspondence)" << endl;
   send_command("#disconnect#", p);
   disconnect_client(p, true);
}

void handle_command(string command, Player* p)
{
   if (command == "#disconnect#")
   {
      disconnect_client(p, false);
      send_command("#disconnect#", p);
      // Send player list to all players, so they are notified about the diconnected user
      // Send as much strings as connected players, with a count first
      list<Player*>::iterator i, j;
		list<Game*>::iterator i2;
      Player* dest;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         dest = *i;
         send_command("player_list", dest);
         send_int(dest, plist.size()); // Number of players
         for (j = plist.begin() ; j != plist.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
         send_command("game_list", dest);
         send_int(dest, glist.size());
         if (glist.size() != 0)
         {
            for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
            {
               send_int(dest, get_utf8_length((*i2)->name));
               send_wstring(dest, (*i2)->name);
            }
         }			   
      }
	}
   else if (command == "get_players")
   {
      list<Player*>::iterator i;
      send_command("player_list", p);
      send_int(p, plist.size()); // Number of players
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         send_int(p, get_utf8_length((*i)->name));
         send_wstring(p, (*i)->name);
      }
	}
	else if (command == "get_games")
	{
		list<Game*>::iterator i;
      send_command("game_list", p);
      send_int(p, glist.size());
      if (glist.size() != 0)
      {
         for (i = glist.begin() ; i != glist.end() ; ++i)
         {
            send_int(p, get_utf8_length((*i)->name));
            send_wstring(p, (*i)->name);
         }
      }
	}
	else if (command == "create_game")
	{
      int bytes_received;
      int len_name = receive_int(p, &bytes_received);
      wstring name = receive_wstring(p, len_name, &bytes_received);
      int long_n_players = receive_int(p, &bytes_received);
      int n_players = atoi(receive_string(p, long_n_players, &bytes_received).c_str());
      if (name.empty()) // Hack, kick player
         return kick_hacker(1, p);
      Game* new_game = new Game(name, n_players, p, &mutex_ids, &id_count, random_gen);
      wcout << L"New game! Name: " << name << " (ID " << new_game->id << ") | Number of players: " << n_players << endl;
      glist.push_back(new_game);
      send_command("joined_game", p);
      send_int(p, get_utf8_length(name));
      send_wstring(p, name);
      send_int(p, new_game->chat->id);
      send_int(p, get_utf8_length(new_game->creator->name));
      send_wstring(p, new_game->creator->name);
      send_int(p, new_game->n_players);
		list<Game*>::iterator i;
      list<Player*>::iterator i2;
      Player* dest;
      for (i2 = plist.begin() ; i2 != plist.end() ; ++i2)
      {
         dest = *i2;
         send_command("game_list", dest);
		   send_int(dest, glist.size());
         if (glist.size() != 0)
         {
            for (i = glist.begin() ; i != glist.end() ; ++i)
            {
               send_int(dest, get_utf8_length((*i)->name));
               send_wstring(dest, (*i)->name);
            }
         }	
      }
   }
   else if (command == "join_game")
   {
      int bytes_received;
      int len_name = receive_int(p, &bytes_received);
      wstring name = receive_wstring(p, len_name, &bytes_received);
      Game* game = get_game_from_name(name);
      if (game == NULL) // To avoid commands sent when chat does not exist anymore
         return;
      if (game->check_already_joined(p))
      {
         send_command("cant_join_already_joined", p);
         send_int(p, get_utf8_length(name));
         send_wstring(p, name);
      }
      else if (game->join(p))
      {
         send_command("joined_game", p);
         send_int(p, get_utf8_length(name));
         send_wstring(p, name);
         send_int(p, game->chat->id);
         send_int(p, get_utf8_length(game->creator->name));
         send_wstring(p, game->creator->name);
         send_int(p, game->n_players);
         list<Player*>::iterator i, j;
         Player* dest;
         for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
         {
            // We exclude joined player because the command is sent too quickly.
            // He will ask for player list just after joining
            dest = *i;
            if (dest != p)
            {
               send_command("chat_userlist", dest);
               send_int(dest, game->id);
               send_int(dest, game->plist.size()); // Number of players
               for (j = game->plist.begin() ; j != game->plist.end() ; ++j)
               {
                  send_int(dest, get_utf8_length((*j)->name));
                  send_wstring(dest, (*j)->name);
               }
            }
         }
      }
      else
      {
         send_command("cant_join_game_full", p);
         send_int(p, get_utf8_length(name));
         send_wstring(p, name);
      }
   }
   else if (command == "leave_game")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      game->eliminate_player(p, NULL);
      if (!game->leave(p)) // Possible hack
         return kick_hacker(2, p);
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
      {
         dest = *i;
         send_command("chat_userlist", dest);
         send_int(dest, game->id);
         send_int(dest, game->plist.size()); // Number of players
         for (j = game->plist.begin() ; j != game->plist.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
      if (delete_game_if_empty(game))
      {
		   list<Game*>::iterator i2;
         for (i = plist.begin() ; i != plist.end() ; ++i)
         {
            dest = *i;
            send_command("game_list", dest);
		      send_int(dest, glist.size());
            if (glist.size() != 0)
            {
               for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
               {
                  send_int(dest, get_utf8_length((*i2)->name));
                  send_wstring(dest, (*i2)->name);
               }
            }
         }
      }
   }
   else if (command == "create_chat")
   {
      int bytes_received;
      int len_quantity = receive_int(p, &bytes_received);
      int quantity = atoi(receive_string(p, len_quantity, &bytes_received).c_str());
      vector<wstring> player_list = vector<wstring>(quantity+1);
      vector<wstring>::iterator i;
      // Receive every player and add the player who sends the command
      int len_name;
      for (i = player_list.begin() ; i != --player_list.end() ; ++i)
      {
         len_name = receive_int(p, &bytes_received);
         (*i) = receive_wstring(p, len_name, &bytes_received);
      }
      (*i) = p->name;
      Chat* new_chat = new Chat(p, true, &mutex_ids, &id_count);
      chat_list.push_back(new_chat);
      wcout << L"New chat with ID " << new_chat->id << ". Number of players: " << player_list.size() << endl;
      // Send commands to selected players to ask them to join the chat
      Player* dest;
      for (i = player_list.begin() ; i != player_list.end() ; ++i)
      {
         dest = get_player_from_name(*i);
         send_command("ask_join_chat", dest);
         send_int(dest, new_chat->id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
      }
   }
   else if (command == "join_global_chat")
   {
      global_chat_list.push_back(p);
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         dest = *i;
         send_command("global_chat_userlist", dest);
         send_int(dest, global_chat_list.size()); // Number of players
         for (j = global_chat_list.begin() ; j != global_chat_list.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
   }
   else if (command == "join_chat")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      Chat* chat = get_chat_from_id(id);
      if (chat == NULL) // To avoid commands sent when chat does not exist anymore
         return;
      if (chat->check_already_joined(p)) // Don't allow join a chat twice (hack)
         return kick_hacker(3, p);
      chat->join(p);
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         dest = *i;
         send_command("chat_userlist", dest);
         send_int(dest, chat->id);
         send_int(dest, chat->players.size()); // Number of players
         for (j = chat->players.begin() ; j != chat->players.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
   }
   else if (command == "leave_global_chat")
   {
      global_chat_list.remove(p);
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         dest = *i;
         send_command("global_chat_userlist", dest);
         send_int(dest, global_chat_list.size()); // Number of players
         for (j = global_chat_list.begin() ; j != global_chat_list.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
   }
   else if (command == "leave_chat")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      Chat* chat = get_chat_from_id(id);
      if (chat == NULL) // To avoid commands sent when chat does not exist anymore
         return;
      if (!chat->leave(p)) // Possible hack
         return kick_hacker(4, p);
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         dest = *i;
         send_command("chat_userlist", dest);
         send_int(dest, chat->id);
         send_int(dest, chat->players.size()); // Number of players
         for (j = chat->players.begin() ; j != chat->players.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
      delete_chat_if_empty(chat);
   }
   else if (command == "get_global_chat_users")
   {
      list<Player*>::iterator i, j;
      send_command("global_chat_userlist", p);
      send_int(p, global_chat_list.size()); // Number of players
      for (j = global_chat_list.begin() ; j != global_chat_list.end() ; ++j)
      {
         send_int(p, get_utf8_length((*j)->name));
         send_wstring(p, (*j)->name);
      }
   }
   else if (command == "get_chat_users")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      Chat* chat = get_chat_from_id(id);
      if (chat == NULL) // To avoid commands sent when chat does not exist anymore
         return;
      list<Player*>::iterator i; 
      send_command("chat_userlist", p);
      send_int(p, id);
      send_int(p, chat->players.size()); // Number of players
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         send_int(p, get_utf8_length((*i)->name));
         send_wstring(p, (*i)->name);
      }
   }
   else if (command == "send_global_chat_msg")
   {
      int bytes_received;
      int long_msg = receive_int(p, &bytes_received);
      wstring msg = receive_wstring(p, long_msg, &bytes_received);
      list<Player*>::iterator i;
      Player* dest;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         dest = *i;
         send_command("new_global_chat_msg", dest);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, get_utf8_length(msg));
         send_wstring(dest, msg);
      }
   }
   else if (command == "send_chat_msg")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      int long_msg = receive_int(p, &bytes_received);
      wstring msg = receive_wstring(p, long_msg, &bytes_received);
      Chat* chat = get_chat_from_id(id);
      if (chat == NULL) // To avoid commands sent when chat does not exist anymore
         return;
      if (!chat->check_already_joined(p)) // hack, send messages to chats the player hasn't joined: kick player
         return kick_hacker(5, p);
      if ((msg.length() > 1024) || (dlib::trim(msg).length() == 0)) // hack, send messages longer or shorter than limits
         return kick_hacker(6, p);
      list<Player*>::iterator i;
      Player* dest;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         dest = *i;
         send_command("new_chat_msg", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, get_utf8_length(msg));
         send_wstring(dest, msg);
      }
   }
   else if (command == "start_game")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if (game->creator != p) // Hack, trying to start a game not created by the player
         return kick_hacker(7, p);
      if (game->started || game->ended) // Hack, start a already started game or an ended game
         return kick_hacker(8, p);
      game->set_players_money(configuration);
      game->start();
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
      {
         dest = *i;
         send_command("game_started", dest);
         send_int(dest, id);
         send_int(dest, game->n_players);
         // Send configuration
         send_int(dest, config_content.size());
         send_string(dest, config_content);
         send_int(dest, game->starting_player);
         // Send player list
         send_int(dest, game->plist.size());
         for (j = game->plist.begin() ; j != game->plist.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
   }
   else if (command == "roll_dice")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      list<Player*>::iterator i;
      Player* dest;
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(9, p);
      if (game->current_player != p) // Hack, retire player
         return kick_hacker(10, p); 
      if ((p->rolled_last_turn) && (game->last_dice_res < 6)) // Hack, retire player
         return kick_hacker(11, p);
      if (p->debt_last_turn > 0) // Hack, retire player
         return kick_hacker(12, p);
      game->roll_dice();
      game->move_player(p, &debt_mutex);
      p->rolled_last_turn = true;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("rolled_dice", dest);
         send_int(dest, id);
         send_int(dest, game->last_dice_res);
         send_int(dest, game->last_auto_advance);
         send_int(dest, p->position->number);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
      }
   }
   else if (command == "roll_construction_dice")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(13, p);
      if (game->current_player != p) // Hack, retire player
         return kick_hacker(14, p);
      if (p->position->type != build) // Hack, retire player
         return kick_hacker(15, p);
      TBuild_dice_res construction_dice_res = game->roll_construction_dice();
      send_command("rolled_construction_dice", p);
      send_int(p, id);
      send_int(p, (int)construction_dice_res);
      if (construction_dice_res == Deny)
         p->built_last_turn = true; // Avoid hacking, because if construction is denied, the player can't try again in the same turn
   }
   else if (command == "turn_pass")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      list<Player*>::iterator i;
      Player* dest;
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(16, p);
      if (game->current_player != p) // Hack, retire player
         return kick_hacker(17, p);
      if ((!p->rolled_last_turn) && (game->last_dice_res < 6)) // Hack, retire player
         return kick_hacker(18, p);
      if (p->debt_last_turn > 0) // Hack, retire player
         return kick_hacker(19, p);
      Player* next_player = game->turn_pass(&debt_mutex);
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("turn_passed", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(next_player->name));
         send_wstring(dest, next_player->name);
      }
   }
   else if (command == "charge_bank")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      list<Player*>::iterator i;
      Player* dest;
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(20, p);
      if ((game->current_player != p) || (game->can_charge_bank(p) == false) || (p->charged_bank_last_turn)) // Hack, retire player
         return kick_hacker(21, p);
      else
         p->Charge_bank();
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, p->n_50);
         send_int(dest, p->n_100);
         send_int(dest, p->n_500);
         send_int(dest, p->n_1000);
         send_int(dest, p->n_5000);
      }
   }
   else if (command == "buy_hotel")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int len_name = receive_int(p, &bytes_received);
      wstring hotel_name = receive_wstring(p, len_name, &bytes_received);
      len_int = receive_int(p, &bytes_received);
      int n_5000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_1000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_500 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_100 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_50 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      // We have selected money by player, change needs to be calculated
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(22, p);
      Player* player = get_player_from_game(p->name, game);
      if (game->current_player != player) // Hack, retire player
         return kick_hacker(23, p);
      if (player->position->type != buy)  // Hack, retire player
         return kick_hacker(24, p);
      if (player->bought_last_turn) // Hack, retire player
         return kick_hacker(25, p);
      Hotel* hotel = get_hotel_from_name(hotel_name, game);
      if (hotel->owner != NULL) // Hack, retire player
         return kick_hacker(26, p);
      if ((player->position->hotel_left != hotel->name) && (player->position->hotel_right != hotel->name)) // Hack, retire player
         return kick_hacker(27, p);
      int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
      if (total_selected < hotel->price) // Hack, retire player
         return kick_hacker(28, p);
      hotel->owner = player;
      player->Buy_hotel(hotel, n_5000, n_1000, n_500, n_100, n_50);
      // Calculate change
      if (total_selected > hotel->price)
      {
         game->calculate_return(total_selected - hotel->price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
         player->Return_change(n_5000, n_1000, n_500, n_100, n_50);
      }
      player->bought_last_turn = true;
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("hotel_purchased", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, get_utf8_length(hotel_name));
         send_wstring(dest, hotel_name);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, player->n_50);
         send_int(dest, player->n_100);
         send_int(dest, player->n_500);
         send_int(dest, player->n_1000);
         send_int(dest, player->n_5000);
      }
   }
   else if (command == "expropriate_hotel")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int len_name = receive_int(p, &bytes_received);
      wstring hotel_name = receive_wstring(p, len_name, &bytes_received);
      len_int = receive_int(p, &bytes_received);
      int n_5000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_1000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_500 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_100 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int n_50 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      // We have selected money by player, change needs to be calculated
      // Needs checking: hotel has owner and is different than player, hotel can be expropriated (player position is next to the hotel, no phases built),
      // player total money is previous total - hotel expropriation price, previous owner total money is previous total + hotel_expropriation price
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(29, p);
      Player* player = get_player_from_game(p->name, game);
      if (game->current_player != player) // Hack, retire player
         return kick_hacker(30, p);
      if (player->position->type != buy)  // Hack, retire player
         return kick_hacker(31, p);
      if (player->bought_last_turn) // Hack, retire player
         return kick_hacker(32, p);
      Hotel* hotel = get_hotel_from_name(hotel_name, game);
      if ((hotel->owner == NULL) || (hotel->owner == player) || (hotel->n_built_phases > 0)) // Hack, retire player
         return kick_hacker(33, p);
      if ((player->position->hotel_left != hotel->name) && (player->position->hotel_right != hotel->name)) // Hack, retire player
         return kick_hacker(34, p);
      int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
      if (total_selected < hotel->expropriation_price) // Hack, retire player
         return kick_hacker(35, p);
      Player* previous_owner = hotel->owner;
      hotel->owner = player;
      previous_owner->Expropriate_hotel(hotel);
      player->Buy_hotel(hotel, previous_owner, n_5000, n_1000, n_500, n_100, n_50);
      // Calculate change
      if (total_selected > hotel->expropriation_price)
      {
         game->calculate_return(previous_owner, total_selected - hotel->expropriation_price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
         player->Return_change(n_5000, n_1000, n_500, n_100, n_50);
      }
      player->bought_last_turn = true;
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("hotel_expropriated", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, get_utf8_length(hotel_name));
         send_wstring(dest, hotel_name);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, player->n_50);
         send_int(dest, player->n_100);
         send_int(dest, player->n_500);
         send_int(dest, player->n_1000);
         send_int(dest, player->n_5000);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(previous_owner->name));
         send_wstring(dest, previous_owner->name);
         send_int(dest, previous_owner->n_50);
         send_int(dest, previous_owner->n_100);
         send_int(dest, previous_owner->n_500);
         send_int(dest, previous_owner->n_1000);
         send_int(dest, previous_owner->n_5000);
      }
   }
   else if (command == "build_phase")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int len_name = receive_int(p, &bytes_received);
      wstring hotel_name = receive_wstring(p, len_name, &bytes_received);
      int n_5000, n_1000, n_500, n_100, n_50;
      len_int = receive_int(p, &bytes_received);
      int type = atoi(receive_string(p, len_int, &bytes_received).c_str());
      if (type == 2)
      {
         len_int = receive_int(p, &bytes_received);
         n_5000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_1000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_500 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_100 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_50 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      }
      // We have selected money by player, change needs to be calculated
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(36, p);
      Player* player = get_player_from_game(p->name, game);
      if (game->current_player != player) // Hack, retire player
         return kick_hacker(37, p);
      if ((player->position->type != build) && (player->position->type != free_phase)) // Hack, retire player
         return kick_hacker(38, p);
      if (player->built_last_turn) // Hack, retire player
         return kick_hacker(39, p);
      Hotel* hotel = get_hotel_from_name(hotel_name, game);
      if (hotel->owner != player) // Hack, retire player
         return kick_hacker(40, p);
      if ((type == 0) && (player->position->type != free_phase)) // Hack, retire player
         return kick_hacker(41, p);
      if (hotel->next_expansion_is_ground)
         game->rolled_construction_dice = true; // As it is not really rolled, simplify checks
      if ((type == 1) && (hotel->next_expansion_is_ground)) // Hack, retire player, ground can't be free (only in free phases positions) because construction dice is not rolled
         return kick_hacker(42, p);
      if ((type == 1) && (!game->rolled_construction_dice)) // Hack, retire player, he didn't ask for permission when building anything different than ground
         return kick_hacker(43, p);
      if ((type == 1) && (game->last_construction_dice_res != Free)) // Hack, retire player
         return kick_hacker(44, p);
      if ((type == 2) && (!game->rolled_construction_dice)) // Hack, retire player, didn't ask for permission when building anything different than ground
         return kick_hacker(45, p);
      int total_selected = 0;
      if (!hotel->Can_extend()) // Hack, retire player
         return kick_hacker(46, p);
      int total_price;
      switch (type)
      {
         case 0:
         case 1:  total_price = 0;
                  break;
         case 2:  total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
                  total_price = hotel->Price_next_expansion();
                  if (game->last_construction_dice_res == Double)
                     total_price = hotel->Price_next_expansion() * 2;
                  if (total_selected < hotel->Price_next_expansion()) // Hack, retire player
                     return kick_hacker(47, p);
                  break;
         default: return kick_hacker(48, p); // Hack, retire player
      }
      hotel->Extend();
      if (type == 2)
      {
         player->Pay_phase_or_entrance(n_5000, n_1000, n_500, n_100, n_50);
         // Calculate change
         if (total_selected > total_price)
         {
            game->calculate_return(total_selected - total_price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
            player->Return_change(n_5000, n_1000, n_500, n_100, n_50);
         }
      }
      player->built_last_turn = true;
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("phase_built", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(hotel_name));
         send_wstring(dest, hotel_name);
         if (type == 2)
         {
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(p->name));
            send_wstring(dest, p->name);
            send_int(dest, player->n_50);
            send_int(dest, player->n_100);
            send_int(dest, player->n_500);
            send_int(dest, player->n_1000);
            send_int(dest, player->n_5000);
         }
      }
   }
   else if (command == "buy_entrance")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int len_name = receive_int(p, &bytes_received);
      wstring hotel_name = receive_wstring(p, len_name, &bytes_received);
      len_int = receive_int(p, &bytes_received);
      int position = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int n_5000, n_1000, n_500, n_100, n_50;
      len_int = receive_int(p, &bytes_received);
      int type = atoi(receive_string(p, len_int, &bytes_received).c_str());
      if (type == 1)
      {
         len_int = receive_int(p, &bytes_received);
         n_5000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_1000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_500 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_100 = atoi(receive_string(p, len_int, &bytes_received).c_str());
         len_int = receive_int(p, &bytes_received);
         n_50 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      }
      // We have selected money by player, change needs to be calculated
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(49, p);
      Player* player = get_player_from_game(p->name, game);
      if (game->current_player != player) // Hack, retire player
         return kick_hacker(50, p);
      if ((player->position->type != free_entrance) && (!game->can_buy_entrances(player))) // Hack, retire player
         return kick_hacker(51, p);
      Hotel* hotel = get_hotel_from_name(hotel_name, game);
      if (hotel->owner != player) // Hack, retire player
         return kick_hacker(52, p);
      if (hotel->n_built_phases == 0) // Hack, retire player
         return kick_hacker(53, p);
      if (hotel->entrance_bought_last_turn) // Hack, retire player
         return kick_hacker(54, p);
      if ((type == 0) && (player->position->type != free_entrance)) // Hack, retire player
         return kick_hacker(55, p);
      if ((type == 0) && player->free_entrance_used) // Hack, retire player
         return kick_hacker(56, p);
      if (hotel->Has_entrance_in_position(position)) // Hack, retire player
         return kick_hacker(57, p);
      if (!hotel->Is_a_valid_entrance_position(game->positions[position])) // Hack, retire player
         return kick_hacker(58, p);
      hotel->Add_entrance(position);
      if (type == 1)
      {
         int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
         player->Pay_phase_or_entrance(n_5000, n_1000, n_500, n_100, n_50);
         // Calculate change
         if (total_selected > hotel->entrance_price)
         {
            game->calculate_return(total_selected - hotel->entrance_price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
            player->Return_change(n_5000, n_1000, n_500, n_100, n_50);
         }
         hotel->entrance_bought_last_turn = true; // Only if it is not free
      }
      else if (type == 0)
         player->free_entrance_used = true;
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("entrance_added", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(hotel_name));
         send_wstring(dest, hotel_name);
         send_int(dest, position);
         if (type == 1)
         {
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(p->name));
            send_wstring(dest, p->name);
            send_int(dest, player->n_50);
            send_int(dest, player->n_100);
            send_int(dest, player->n_500);
            send_int(dest, player->n_1000);
            send_int(dest, player->n_5000);
         }
      }
   }
   else if (command == "retire")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int type = atoi(receive_string(p, len_int, &bytes_received).c_str());
      Player* receiving_player = NULL;
      if (type == 1) // Player is automatically retired because he tried to create an auction and he doesn't have any hotel or enough money to pay
      {
         int len_name = receive_int(p, &bytes_received);
         wstring receiver_name = receive_wstring(p, len_name, &bytes_received);
         receiving_player = get_player_from_name(receiver_name);
      }
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(59, p);
      Player* player = get_player_from_game(p->name, game);
      if (player == NULL) // Hack, retire player
         return kick_hacker(60, p);
      if (!game->is_active(player)) // Hack, already retired
         return kick_hacker(61, p);
      Player* next_player;
      if (game->get_active_players_count() > 2)
         next_player = game->turn_pass(&debt_mutex);
      game->eliminate_player(player, receiving_player);
      list<Player*>::iterator i;
      Player* dest, * winner;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i) // Avoid sending commands to retired players, because their forms can be already closed and could lead to a client crash
      {
         dest = (*i);
         send_command("player_retired", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         if (type == 1) // Update affected players money
         {
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(receiving_player->name));
            send_wstring(dest, receiving_player->name);
            send_int(dest, receiving_player->n_50);
            send_int(dest, receiving_player->n_100);
            send_int(dest, receiving_player->n_500);
            send_int(dest, receiving_player->n_1000);
            send_int(dest, receiving_player->n_5000);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(p->name));
            send_wstring(dest, p->name);
            send_int(dest, p->n_50);
            send_int(dest, p->n_100);
            send_int(dest, p->n_500);
            send_int(dest, p->n_1000);
            send_int(dest, p->n_5000);
         }
         if (game->get_active_players_count() == 1)
         {
            game->ended = true;
            send_command("game_ended", dest);
            send_int(dest, id);
            winner = game->get_winner();
            send_int(dest, get_utf8_length(winner->name));
            send_wstring(dest, winner->name);
         }
         else
         {
            send_command("turn_passed", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(next_player->name));
            send_wstring(dest, next_player->name);
         }
      }
   }
   else if (command == "ask_nights")
   {
      int bytes_received;
      int len_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_id, &bytes_received).c_str());
      list<Player*>::iterator i;
      Player* dest;
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(62, p);
      if (p->asked_nights_last_turn) // Hack, retire player
         return kick_hacker(63, p);
      p->asked_nights_last_turn = true;
      int amount = 0, nights = 0;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i) // Check if players are in entrances of hotels of the asking player
      {
         dest = (*i);
         if (dest != p) // Player doesn't have to pay himself :D
         {
            amount = game->get_money_for_nights(p, dest, &nights);
            if (amount > 0) // The player is in a entrance and hasn't paid this turn
            {
               debt_mutex.lock(); // To avoid creating a debt just when player is passing turn, because this command is asynchronous
               dest->debt_last_turn = amount;
               dest->debt_nights_to_last_turn = p;
               dest->paid_last_turn = false;
               debt_mutex.unlock();
               send_command("ask_pay_nights", dest); // Will force the player to pay nights, if he hacks the game, in next turn pass he will be retired
               send_int(dest, id);
               send_int(dest, get_utf8_length(p->name));
               send_wstring(dest, p->name);
               send_int(dest, amount);
               send_int(dest, nights);
            }
         }
      }
   }
   else if (command == "pay_nights")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int n_5000, n_1000, n_500, n_100, n_50;
      len_int = receive_int(p, &bytes_received);
      n_5000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_1000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_500 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_100 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_50 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      // We have selected money by player, change needs to be calculated
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if ((!game->started) || game->ended) // Hack, game not started yet or already ended
         return kick_hacker(64, p);
      Player* player = get_player_from_game(p->name, game);
      if (player == NULL) // Hack, retire player
         return kick_hacker(65, p);
      int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
      if (total_selected <= 0) // Hack, retire player, the command is only sent if player has something to pay
         return kick_hacker(66, p);
      if ((player->debt_last_turn == 0) || (player->debt_nights_to_last_turn == NULL)) // Hack, retire player
         return kick_hacker(67, p);
      player->Pay_nights(player->debt_nights_to_last_turn, n_5000, n_1000, n_500, n_100, n_50);
      player->paid_last_turn = true;
      // Calculate change
      if (total_selected > player->debt_last_turn)
      {
         game->calculate_return(player->debt_nights_to_last_turn, total_selected - player->debt_last_turn, &n_5000, &n_1000, &n_500, &n_100, &n_50);
         player->Return_change(n_5000, n_1000, n_500, n_100, n_50);
      }
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(player->name));
         send_wstring(dest, player->name);
         send_int(dest, player->n_50);
         send_int(dest, player->n_100);
         send_int(dest, player->n_500);
         send_int(dest, player->n_1000);
         send_int(dest, player->n_5000);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(player->debt_nights_to_last_turn->name));
         send_wstring(dest, player->debt_nights_to_last_turn->name);
         send_int(dest, player->debt_nights_to_last_turn->n_50);
         send_int(dest, player->debt_nights_to_last_turn->n_100);
         send_int(dest, player->debt_nights_to_last_turn->n_500);
         send_int(dest, player->debt_nights_to_last_turn->n_1000);
         send_int(dest, player->debt_nights_to_last_turn->n_5000);
      }
      debt_mutex.lock(); // To avoid creating a debt just when player is passing turn, because this command is asynchronous
      player->debt_last_turn = 0;
      player->debt_nights_to_last_turn = NULL;
      debt_mutex.unlock();
   }
   else if (command == "auction_start")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int len_name = receive_int(p, &bytes_received);
      wstring hotel_name = receive_wstring(p, len_name, &bytes_received);
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if (!game->check_already_joined(p)) // Hack, retire player, he is not part of the game
         return kick_hacker(68, p);
      Hotel* hotel = get_hotel_from_name(hotel_name, game);
      if (hotel->owner != p) // Hack, retire player
         return kick_hacker(69, p);
      if (game->hotel_at_auction != NULL) // Hack, auction already in progress
         return kick_hacker(70, p);
      game->hotel_at_auction = hotel;
      game->best_bid = 0;
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("auction_started", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(hotel->name_txt));
         send_wstring(dest, hotel->name_txt);
      }
   }
   else if (command == "auction_bid")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      int amount = atoi(receive_string(p, len_int, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if (!game->check_already_joined(p)) // Hack, retire player, he is not part of the game
         return kick_hacker(71, p);
      if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
         return kick_hacker(72, p);
      if (p == game->hotel_at_auction->owner) // Hack, retire player, he is trying to bid in his own auction
         return kick_hacker(73, p);
      if ((amount <= 0) || (amount < game->best_bid) || amount > p->total_money || ((amount % 50) != 0)) // Hack, retire player, invalid values
         return kick_hacker(74, p);
      game->best_bid = amount;
      game->best_bidder = p;
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("auction_bid_placed", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, amount);
      }
   }
   else if (command == "auction_sell")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if (!game->check_already_joined(p)) // Hack, retire player, he is not part of the game
         return kick_hacker(75, p);
      if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
         return kick_hacker(76, p);
      if (p != game->hotel_at_auction->owner) // Hack, retire player, he is trying to end the auction without being the owner
         return kick_hacker(77, p);
      if (game->best_bid == 0) // Hack, retire player, no one has placed a bid yet
         return kick_hacker(78, p);
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("auction_sold", dest);
         send_int(dest, id);
         send_int(dest, game->best_bid);
      }
   }
   else if (command == "auction_pay")
   {
      int bytes_received;
      int len_int = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, len_int, &bytes_received).c_str());
      int n_5000, n_1000, n_500, n_100, n_50;
      len_int = receive_int(p, &bytes_received);
      n_5000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_1000 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_500 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_100 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      len_int = receive_int(p, &bytes_received);
      n_50 = atoi(receive_string(p, len_int, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      if (game == NULL) // To avoid commands sent when game does not exist anymore
         return;
      if (!game->check_already_joined(p)) // Hack, retire player, he is not part of the game
         return kick_hacker(79, p);
      if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
         return kick_hacker(80, p);
      if (game->best_bid == 0) // Hack, retire player, no one has placed a bid yet
         return kick_hacker(81, p);
      if (p != game->best_bidder) // Hack, retire player, the player who pays must be the best bidder
         return kick_hacker(82, p);
      int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
      if ((total_selected <= 0) || (total_selected < game->best_bid)) // Hack, retire player, the command is only sent if player has something to pay and >= than best bid
         return kick_hacker(83, p);
      Player* previous_owner = game->hotel_at_auction->owner;
      previous_owner->Expropriate_hotel(game->hotel_at_auction);
      p->Buy_hotel(game->hotel_at_auction, previous_owner, n_5000, n_1000, n_500, n_100, n_50);
      game->hotel_at_auction->owner = p;
      // Calculate change
      if (total_selected > game->best_bid)
      {
         game->calculate_return(previous_owner, total_selected - game->best_bid, &n_5000, &n_1000, &n_500, &n_100, &n_50);
         p->Return_change(n_5000, n_1000, n_500, n_100, n_50);
      }
      list<Player*>::iterator i;
      Player* dest;
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("hotel_expropriated", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, get_utf8_length(game->hotel_at_auction->name_txt));
         send_wstring(dest, game->hotel_at_auction->name_txt);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(p->name));
         send_wstring(dest, p->name);
         send_int(dest, p->n_50);
         send_int(dest, p->n_100);
         send_int(dest, p->n_500);
         send_int(dest, p->n_1000);
         send_int(dest, p->n_5000);
         send_command("update_player_money", dest);
         send_int(dest, id);
         send_int(dest, get_utf8_length(previous_owner->name));
         send_wstring(dest, previous_owner->name);
         send_int(dest, previous_owner->n_50);
         send_int(dest, previous_owner->n_100);
         send_int(dest, previous_owner->n_500);
         send_int(dest, previous_owner->n_1000);
         send_int(dest, previous_owner->n_5000);
      }
      // If this command is sent before updating hotels and money to everyone, the client doesn't know who owns the hotels
      for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
      {
         dest = (*i);
         send_command("auction_ended", dest);
         send_int(dest, id);
      }
      game->best_bidder = NULL;
      game->best_bid = 0;
      game->hotel_at_auction = NULL;
   }
}

void handle_client(void* arg)
{
   Player* p = (Player*) arg;
   int bytes_received;
   int len_name = receive_int(p, &bytes_received);
   p->name = receive_wstring(p, len_name, &bytes_received);
   wcout << L"Handling new player. Player name: " << p->name << endl;
   if (p->name.length() > 20)
   {
      wcout << L"Player " << p->name << L" rejected because the name is too long (WARNING: possible hacked client)" << endl;
      p->socket->psend("login no", 8, 0);
	   delete p;
      wcout << L"Disconnecting client" << endl;
   }
   else if (add_player_to_player_list(p) == false)
   {
      p->socket->psend("login ko", 8, 0);
	   delete p;
      wcout << L"Disconnecting client" << endl;
   }
   else
   {
      p->socket->psend("login ok", 8, 0);
      bool online = true;
      // Send player list to all players, so they are notified about the new user
      // Send as much strings as connected players, with a count first
      list<Player*>::iterator i, j;
      Player* dest;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         dest = *i;
         send_command("player_list", dest);
         send_int(dest, plist.size()); // Number of players
         for (j = plist.begin() ; j != plist.end() ; ++j)
         {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
         }
      }
      int long_command;
      while (online)
      {
         long_command = receive_int(p, &bytes_received);
         string command = receive_string(p, long_command, &bytes_received);
         if (bytes_received > 0)
         {
            wstring w_command;
            w_command.assign(command.begin(), command.end());
            wcout << L"Received command from player " << p->name << ": " << w_command << endl;
            handle_command(command, p);
         }
         else
         {
            online = false;
            disconnect_client(p, false);
            delete p;
            wcout << L"Client disconnected" << endl;
         }
      }
   }
}

void read_config(TiXmlDocument* config_xml)
{
   // For two players
   TiXmlNode* node = config_xml->LastChild()->FirstChild()->FirstChild()->FirstChild();
   configuration.two_players.n_5000 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.two_players.n_1000 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.two_players.n_500 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.two_players.n_100 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.two_players.n_50 = atoi(node->FirstChild()->Value());
   // Now for three or four players
   node = node->Parent()->NextSibling()->FirstChild();
   configuration.three_or_four_players.n_5000 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.three_or_four_players.n_1000 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.three_or_four_players.n_500 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.three_or_four_players.n_100 = atoi(node->FirstChild()->Value());
   node = node->NextSiblingElement();
   configuration.three_or_four_players.n_50 = atoi(node->FirstChild()->Value());
}

void run_server(int port)
{
   #ifdef _WIN32
      SetConsoleOutputCP(CP_UTF8);
      //wcout.imbue(locale("Spanish_Spain.1256"));
   #else
      setlocale(LC_ALL, "es_ES.utf8");
   #endif
   wcout << L"Starting Hotel server on port " << port << "..." << endl;
   socket_server = new Portable_socket();
   sockaddr_in server_info;
   sockaddr_in client_info;
   socklen_t addrlen;

   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(port);
   server_info.sin_addr.s_addr=INADDR_ANY;
   if (socket_server->pbind((sockaddr*) &server_info,sizeof(server_info)) < 0)
   {
	   wcout << L"bind error: " << socket_server->get_last_error() << endl;
	   delete socket_server;
	   return;
   }
   if (socket_server->plisten(MAXCONN) < 0)
   {
	   wcout << L"listen error: " << socket_server->get_last_error() << endl;
	   delete socket_server;
	   return;
   }
   wcout << L"Listening for connections" << endl;

   hook_signals();
   Player* p;
   random_gen = new CRandomMT();
   // Read Config.xml as string to send it to players
   ifstream config_file ("Config.xml");
   string line;
   config_content = "";
   while (getline(config_file, line))
   {
      config_content += line + '\n';
   }
   config_file.close();
   TiXmlDocument config_xml("Config.xml");
   if (!(config_xml.LoadFile()))
   {
      wcout << L"Error loading Config.xml" << endl;
      return;
   }
   // Load Config.xml to have configuration loaded for future checks
   read_config(&config_xml);
   while (closing == 0)
   {
      addrlen = sizeof(client_info);
      socket_client = socket_server->paccept((sockaddr*) &client_info, &addrlen);
      if (socket_client != NULL)
         wcout << L"Client connection from " << inet_ntoa(client_info.sin_addr) << ":" << ntohs(client_info.sin_port) << endl;
      else
      {
         if (closing == 0)
            wcout << L"accept error: " << socket_server->get_last_error() << endl;
         else
         {
            // The server is closing
            empty_global_chat_list();
            wcout << L"Global Chat cleaned" << endl;
            empty_chat_list();
            wcout << L"Chat list cleaned" << endl;
            empty_glist();
            wcout << L"Game list cleaned" << endl;
            empty_plist();
            wcout << L"Player list cleaned" << endl;
            return;
         }
      }
      p = new Player(inet_ntoa(client_info.sin_addr), socket_client);
      dlib::create_new_thread(handle_client, (void*) p);
   }
}

int main(int argc, char* argv[])
{
   int port;
   if (argc == 2) // Port specified
      port = atoi(argv[1]);
   else
      port = 12345;
   run_server(port);
}
