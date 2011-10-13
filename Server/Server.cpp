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
#include "dlib/threads.h"
#include "dlib/string.h"

#define MAXCONN 100
//#define MAXDATALEN 100

using namespace std;

volatile int closing = 0;
Portable_socket* socket_server;
Portable_socket* socket_client;
list<Player*> plist; // Player list
list<Game*> glist; // Game list
list<Player*> global_chat_list; // Players in global chat
list<Chat*> chat_list;
dlib::mutex mutex_ids;
int id_count = 0;
string config_content;

struct config_bills
{
   int n_5000;
   int n_1000;
   int n_500;
   int n_100;
   int n_50;
};

struct config
{
   struct config_bills two_players;
   struct config_bills three_or_four_players;
} configuration;

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
		return false;
	}
	else
	{
		wcout << L"Player accepted" << endl;
		plist.push_back(p);
		return true;
	}
}

bool delete_player_from_player_list(Player* p)
{
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
      return false;
   }
	else
	{
		wcout << L"Player deleted from player list" << endl;
		plist.erase(i);
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

bool delete_chat_if_empty(Chat* chat)
{
   if (chat->players.empty())
   {
      chat_list.remove(chat);
      wcout << L"Chat " << chat->id << L" deleted because it's empty" << endl;
      delete chat;
      return true;
   }
   return false;
}

bool delete_game_if_empty(Game* game)
{
   if (game->plist.empty())
   {
      glist.remove(game);
      wcout << L"Game " << game->name << L" deleted because it's empty" << endl;
      delete game;
      return true;
   }
   else
      return false;
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

void disconnect_client(Player* p)
{
   if (!p->connected)
      return;
   // Leave all normal chats and global chat
   list<Chat*>::iterator i;
   for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
   {
      (*i)->leave(p);
      if (delete_chat_if_empty(get_chat_from_id((*i)->id)))
      {
         if (chat_list.size() > 0)
            i = chat_list.begin(); // When deleting a chat, I prefer starting again to avoid segmentation faults
         else
            break; // If it was the last chat, chat_list.begin() returns an invalid pointer, so the loop must end
      }
   }
   global_chat_list.remove(p);
   // Leave games
   list<Game*>::iterator i2;
   for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
   {
      (*i2)->leave(p);
      if (delete_game_if_empty(*i2))
      {
         if (glist.size() > 0)
            i2 = glist.begin(); // When deleting a game, I prefer starting again to avoid segmentation faults
         else
            break; // If it was the last game, glist.begin() returns an invalid pointer, so the loop must end
      }
   }
   delete_player_from_player_list(p);
   p->connected = false;
}

int get_utf8_length(wstring data)
{
   #ifdef _WIN32
      return WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
   #else
      return wcstombs(NULL, data.data(), 0);
   #endif
}

wstring utf8_to_utf16 (string data)
{
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

void handle_command(string command, Player* p)
{
   if (command == "#disconnect#")
   {
      disconnect_client(p);
      send_command("#disconnect#", p);
      // Send player list to all players, so they are notified about the diconnected user
      // Get all users and join into a string with the separator ~
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --plist.end())
            res += '~';
      }
      // Get all games and join into a string with the separator ~
		wstring res2 = L"";
		list<Game*>::iterator i2;
		for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
		{
			res2 += (*i2)->name;
			if (i2 != --glist.end())
				res2 += '~';
		}
      Player* dest;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         dest = *i;
         send_command("player_list", dest);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
         send_command("game_list", dest);
		   send_int(dest, get_utf8_length(res2));
		   if (res2.length() != 0)
			   send_wstring(dest, res2);
      }
	}
   else if (command == "get_players")
   {
      // Get all users and join into a string with the separator ~
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --plist.end())
            res += '~';
      }
      send_command("player_list", p);
      send_int(p, get_utf8_length(res));
      send_wstring(p, res);
	}
	else if (command == "get_games")
	{
		// Get all games and join into a string with the separator ~
		wstring res = L"";
		list<Game*>::iterator i;
		for (i = glist.begin() ; i != glist.end() ; ++i)
		{
			res += (*i)->name;
			if (i != --glist.end())
				res += '~';
		}
      send_command("game_list", p);
		send_int(p, get_utf8_length(res));
		if (res.length() != 0)
			send_wstring(p, res);
	}
	else if (command == "create_game")
	{
      int bytes_received;
      int long_name = receive_int(p, &bytes_received);
      wstring name = receive_wstring(p, long_name, &bytes_received);
      int long_n_players = receive_int(p, &bytes_received);
      int n_players = atoi(receive_string(p, long_n_players, &bytes_received).c_str());
      if (name.find('~') != string::npos)
      {
         wcout << L"New game rejected because the name contained invalid character ~ (WARNING: possible hacked client)" << endl;
         return;
      }
      Game* new_game = new Game(name, n_players, p, &mutex_ids, &id_count);
      wcout << L"New game! Name: " << name << " (ID " << new_game->id << ") | Number of players: " << n_players << endl;
      glist.push_back(new_game);
      send_command("joined_game", p);
      send_int(p, get_utf8_length(name));
      send_wstring(p, name);
      send_int(p, new_game->chat->id);
      send_int(p, get_utf8_length(new_game->creator->name));
      send_wstring(p, new_game->creator->name);
      // Get all games and join into a string with the separator ~
		wstring res = L"";
		list<Game*>::iterator i;
		for (i = glist.begin() ; i != glist.end() ; ++i)
		{
			res += (*i)->name;
			if (i != --glist.end())
				res += '~';
		}
      list<Player*>::iterator i2;
      Player* dest;
      for (i2 = plist.begin() ; i2 != plist.end() ; ++i2)
      {
         dest = *i2;
         send_command("game_list", dest);
		   send_int(dest, get_utf8_length(res));
		   if (res.length() != 0)
			   send_wstring(dest, res);
      }
   }
   else if (command == "join_game")
   {
      int bytes_received;
      int long_name = receive_int(p, &bytes_received);
      wstring name = receive_wstring(p, long_name, &bytes_received);
      Game* game = get_game_from_name(name);
      if (game->join(p))
      {
         send_command("joined_game", p);
         send_int(p, get_utf8_length(name));
         send_wstring(p, name);
         send_int(p, game->chat->id);
         send_int(p, get_utf8_length(game->creator->name));
         send_wstring(p, game->creator->name);
         wstring res = L"";
         list<Player*>::iterator i;
         for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
         {
            res += (*i)->name;
            if (i != --game->plist.end())
               res += '~';
         }
         Player* dest;
         for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
         {
            // We exclude recently joined player because the command is sent too quickly.
            // He will ask for player list just after joining
            dest = *i;
            if (dest != p)
            {
               send_command("chat_userlist", dest);
               send_int(dest, game->id);
               send_int(dest, get_utf8_length(res));
               send_wstring(dest, res);
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
      int long_id = receive_int(p, &bytes_received);
      string id = receive_string(p, long_id, &bytes_received);
      Game* game = get_game_from_id(atoi(id.c_str()));
      game->leave(p);
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --game->plist.end())
            res += '~';
      }
      Player* dest;
      for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
      {
         dest = *i;
         send_command("chat_userlist", dest);
         send_int(dest, game->id);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
      }
      delete_game_if_empty(game);
      // Get all games and join into a string with the separator ~
		res = L"";
		list<Game*>::iterator i2;
		for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
		{
			res += (*i2)->name;
			if (i2 != --glist.end())
				res += '~';
		}
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         dest = *i;
         send_command("game_list", dest);
		   send_int(dest, get_utf8_length(res));
		   if (res.length() != 0)
			   send_wstring(dest, res);
      }
   }
   else if (command == "create_chat")
   {
      int bytes_received;
      int long_list = receive_int(p, &bytes_received);
      wstring plist = receive_wstring(p, long_list, &bytes_received);
      vector<wstring> player_list = dlib::split(plist, L"~");
      Chat* new_chat = new Chat(p, true, &mutex_ids, &id_count);
      chat_list.push_back(new_chat);
      wcout << L"New chat with ID " << new_chat->id << ". Number of players: " << player_list.size() << endl;
      // Send commands to selected players to ask them to join the chat
      Player* dest;
      vector<wstring>::iterator i;
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
      // Get all global chat users and join into a string with the separator ~
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --global_chat_list.end())
            res += '~';
      }
      Player* dest;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         dest = *i;
         send_command("global_chat_userlist", dest);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
      }
   }
   else if (command == "join_chat")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      string id = receive_string(p, long_id, &bytes_received);
      Chat* chat = get_chat_from_id(atoi(id.c_str()));
      chat->join(p);
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --chat->players.end())
            res += '~';
      }
      Player* dest;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         dest = *i;
         send_command("chat_userlist", dest);
         send_int(dest, chat->id);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
      }
   }
   else if (command == "leave_global_chat")
   {
      global_chat_list.remove(p);
      // Get all global chat users and join into a string with the separator ~
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --global_chat_list.end())
            res += '~';
      }
      Player* dest;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         dest = *i;
         send_command("global_chat_userlist", dest);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
      }
   }
   else if (command == "leave_chat")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      string id = receive_string(p, long_id, &bytes_received);
      Chat* chat = get_chat_from_id(atoi(id.c_str()));
      chat->leave(p);
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --chat->players.end())
            res += '~';
      }
      Player* dest;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         dest = *i;
         send_command("chat_userlist", dest);
         send_int(dest, chat->id);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
      }
      delete_chat_if_empty(chat);
   }
   else if (command == "get_global_chat_users")
   {
      // Get all global chat users and join into a string with the separator ~
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --global_chat_list.end())
            res += '~';
      }
      send_command("global_chat_userlist", p);
      send_int(p, get_utf8_length(res));
      send_wstring(p, res);
   }
   else if (command == "get_chat_users")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, long_id, &bytes_received).c_str());
      // Get all users and join into a string with the separator ~
      Chat* chat = get_chat_from_id(id);
      wstring res = L"";
      list<Player*>::iterator i; 
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --chat->players.end())
            res += '~';
      }
      send_command("chat_userlist", p);
      send_int(p, id);
      send_int(p, get_utf8_length(res));
      send_wstring(p, res);
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
      int long_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, long_id, &bytes_received).c_str());
      int long_msg = receive_int(p, &bytes_received);
      wstring msg = receive_wstring(p, long_msg, &bytes_received);
      Chat* chat = get_chat_from_id(id);
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
   else if (command == "roll_dice")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, long_id, &bytes_received).c_str());
      int long_name = receive_int(p, &bytes_received);
      wstring player_name = receive_wstring(p, long_name, &bytes_received);
      list<Player*>::iterator i;
      Player* dest;
      Game* game = get_game_from_id(id);
      for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
      {
         dest = (*i);
         send_command("rolled_dice", dest);
         send_int(dest, game->roll_dice());
         send_int(dest, get_utf8_length(player_name));
         send_wstring(dest, player_name);
      }
   }
   else if (command == "start_game")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, long_id, &bytes_received).c_str());
      Game* game = get_game_from_id(id);
      game->start();
      list<Player*>::iterator i;
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
      }
   }
}

void handle_client(void* arg)
{
   Player* p = (Player*) arg;
   int bytes_received;
   int long_name = receive_int(p, &bytes_received);
   p->name = receive_wstring(p, long_name, &bytes_received);
   wcout << L"Handling new player. Player name: " << p->name << endl;
   if (p->name.find('~') != string::npos)
   {
      wcout << L"Player " << p->name << L" rejected because the name contains invalid character '~' (WARNING: possible hacked client)" << endl;
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
      // Get all users and join into a string with the separator ~
      wstring res = L"";
      list<Player*>::iterator i;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --plist.end())
            res += '~';
      }
      Player* dest;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         dest = *i;
         send_command("player_list", dest);
         send_int(dest, get_utf8_length(res));
         send_wstring(dest, res);
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
            disconnect_client(p);
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

void run_server()
{
   #ifdef _WIN32
      SetConsoleOutputCP(CP_UTF8);
      //wcout.imbue(locale("Spanish_Spain.1256"));
   #else
      setlocale(LC_ALL, "es_ES.utf8");
   #endif
   wcout << L"Starting Hotel server..." << endl;
   socket_server = new Portable_socket();
   sockaddr_in server_info;
   sockaddr_in client_info;
   socklen_t addrlen;

   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
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
   srand(time(0));
   // Read Config.xml as string to send it to players
   ifstream config_file ("Config.xml");
   string line;
   config_content = "";
   while (getline(config_file, line))
   {
      config_content += line + '\n';
   }
   TiXmlDocument config_xml("Config.xml");
   if (!(config_xml.LoadFile()))
   {
      wcout << L"Error loading Config.xml" << endl;
      return;
   }
   // Load Config.xml to have configuration loaded for future checks
   read_config(&config_xml);
   config_xml.~TiXmlDocument();
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
   run_server();
}
