#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "chat.h"
#include "dlib/threads.h"
#include "dlib/string.h"
#define MAXCONN 100
#define MAXDATALEN 100

using namespace std;
using namespace dlib;

volatile int closing = 0;
portable_socket* socket_server;
portable_socket* socket_client;
list<player*> plist; // Player list
list<game*> glist; // Game list
list<player*> global_chat_list; // Players in global chat
list<chat*> chat_list;

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
   list<player*>::iterator i;
	for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
	{
		*i = NULL;
	}
}

void empty_glist()
{
   list<game*>::iterator i;
	for (i = glist.begin() ; i != glist.end() ; ++i)
	{
      game* g = *i;
      delete g;
	}
}

void empty_plist()
{
	list<player*>::iterator i;
	for (i = plist.begin() ; i != plist.end() ; ++i)
	{
      player* p = *i;
      delete p;
	}
}

void close (int signum)
{
   closing = 1;
   cout << endl << "Closing server" << endl;
   unhook_signals();
   delete socket_server;
}

void hook_signals()
{
   signal(SIGINT, close);
   signal(SIGTERM, close);
   #ifdef _WIN32
      signal(SIGBREAK, close);
   #endif
}

int add_player_to_list (player* p)
{
	bool found = false;
	list<player*>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (found)
	{
		cout << "Player already connected" << endl;
		return -1;
	}
	else
	{
		cout << "Player accepted" << endl;
		plist.push_back(p);
		return 0;
	}
}

void delete_player_from_list(player* p)
{
   bool found = false;
	list<player*>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if ((*i)->name == p->name)
			found = true;
      else
         ++i;
	}
	if (!found)
		cout << "Player not found in user list" << endl;
	else
	{
		cout << "Player deleted from user list" << endl;
		plist.erase(i);
	}
}

chat* get_chat_from_id(int id)
{
   bool found = false;
	list<chat*>::iterator i = chat_list.begin();
	while (!found && i != chat_list.end())
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

player* get_player_from_name(string name)
{
   bool found = false;
	list<player*>::iterator i = plist.begin();
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

void delete_chat_if_empty(chat* chat)
{
   if (chat->players.empty())
   {
      chat_list.remove(chat);
      delete chat;
   }
}

game* get_game_from_name(string name)
{
   bool found = false;
   list<game*>::iterator i = glist.begin();
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

void disconnect_client(player* p)
{
   if (!p->connected)
      return;
   // Leave all normal chats and global chat
   list<chat*>::iterator i;
   for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
   {
      (*i)->leave(p);
      delete_chat_if_empty(get_chat_from_id((*i)->id));
   }
   global_chat_list.remove(p);
   // Leave games
   list<game*>::iterator i2;
   for (i2 = glist.begin() ; i2 != glist.end() ; ++i2)
   {
      (*i2)->leave(p);
   }
   delete_player_from_list(p);
   p->connected = false;
}

string receive_string (player* p, int length, int* bytes_received)
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

int receive_int (player* p, int* bytes_received)
{
   int data;
   *bytes_received = p->socket->precv(&data, sizeof(data), 0);
   if (*bytes_received > 0)
      data = ntohl(data);
   else
      data = 0;
   return data;
}

int send_string (player* p, string data)
{
   return p->socket->psend(data.c_str(), data.length(), 0);
}

int send_int (player* p, int data)
{
   data = htonl(data);
   return p->socket->psend(&data, sizeof(data), 0);
}

void send_command(string comando, player* p)
{
	cout << "Sending command to player " << p->name << ": " << comando << endl;
   send_int(p, comando.length());
   send_string(p, comando);
}

void handle_command(string command, player* p)
{
   if (command == "#disconnect#")
   {
      disconnect_client(p);
      send_command("#disconnect#", p);
	}
   else if (command == "get_users")
   {
      // Get all users and join into a string with the separator ~
      string res = "";
      list<player*>::iterator i;
      for (i = plist.begin() ; i != plist.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --plist.end())
            res += '~';
      }
      send_command("player_list", p);
      send_int(p, res.length());
      send_string(p, res);
	}
	else if (command == "get_games")
	{
		// Get all users and join into a string with the separator ~
		string res = "";
		list<game*>::iterator i;
		for (i = glist.begin() ; i != glist.end() ; ++i)
		{
			res += (*i)->name;
			if (i != --glist.end())
				res += '~';
		}
      send_command("game_list", p);
      send_int(p, res.length());
		if (res.length() != 0)      
         send_string(p, res);
	}
	else if (command == "create_game")
	{
      int bytes_received;
      int long_name = receive_int(p, &bytes_received);
      string name = receive_string(p, long_name, &bytes_received);
      int long_n_players = receive_int(p, &bytes_received);
      int n_players = atoi(receive_string(p, long_n_players, &bytes_received).c_str());
      if (name.find('~') != string::npos)
      {
         cout << "New game rejected because the name contained invalid character ~ (WARNING: possible hacked client)" << endl;
         return;
      }
      cout << "New game! Name: " << name << " | Number of players: " << n_players << endl;
      game* new_game = new game(name, n_players, p);
      glist.push_back(new_game);
      send_command("joined_game", p);
      send_int(p, name.length());
      send_string(p, name);
   }
   else if (command == "join_game")
   {
      int bytes_received;
      int long_name = receive_int(p, &bytes_received);
      string name = receive_string(p, long_name, &bytes_received);
      game* game = get_game_from_name(name);
      if (game->join(p))
      {
         send_command("joined_game", p);
         send_int(p, name.length());
         send_string(p, name);
      }
      else
      {
         send_command("cant_join_game_full", p);
         send_int(p, name.length());
         send_string(p, name);
      }
   }
   else if (command == "create_chat")
   {
      int bytes_received;
      int long_list = receive_int(p, &bytes_received);
      string plist = receive_string(p, long_list, &bytes_received);
      vector<string> player_list = split(plist, "~");
      cout << "New chat. Number of players: " << player_list.size() << endl;
      chat* new_chat = new chat(p, true);
      chat_list.push_back(new_chat);
      // Send commands to selected players to ask them to join the chat
      player* dest;
      vector<string>::iterator i;
      for (i = player_list.begin() ; i != player_list.end() ; ++i)
      {
         dest = get_player_from_name(*i);
         send_command("ask_join_chat", dest);
         send_int(dest, new_chat->id);
         send_int(dest, p->name.length());
         send_string(dest, p->name);
      }
   }
   else if (command == "join_global_chat")
   {
      global_chat_list.push_back(p);
   }
   else if (command == "join_chat")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      string id = receive_string(p, long_id, &bytes_received);
      chat* chat = get_chat_from_id(atoi(id.c_str()));
      chat->join(p);
   }
   else if (command == "leave_global_chat")
   {
      global_chat_list.remove(p);
   }
   else if (command == "leave_chat")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      string id = receive_string(p, long_id, &bytes_received);
      chat* chat = get_chat_from_id(atoi(id.c_str()));
      chat->leave(p);
      delete_chat_if_empty(get_chat_from_id(atoi(id.c_str())));
   }
   else if (command == "get_global_chat_users")
   {
      // Get all users and join into a string with the separator ~
      string res = "";
      list<player*>::iterator i;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --global_chat_list.end())
            res += '~';
      }
      send_command("global_chat_userlist", p);
      send_int(p, res.length());
      send_string(p, res);
   }
   else if (command == "get_chat_users")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, long_id, &bytes_received).c_str());
      // Get all users and join into a string with the separator ~
      chat* chat = get_chat_from_id(id);
      string res = "";
      list<player*>::iterator i;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         res += (*i)->name;
         if (i != --chat->players.end())
            res += '~';
      }
      send_command("chat_userlist", p);
      send_int(p, id);
      send_int(p, res.length());
      send_string(p, res);
   }
   else if (command == "send_global_chat_msg")
   {
      int bytes_received;
      int long_msg = receive_int(p, &bytes_received);
      string msg = receive_string(p, long_msg, &bytes_received);
      list<player*>::iterator i;
      player* dest;
      for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
      {
         dest = *i;
         send_command("new_global_chat_msg", dest);
         send_int(dest, p->name.length());
         send_string(dest, p->name);
         send_int(dest, msg.length());
         send_string(dest, msg);
      }
   }
   else if (command == "send_chat_msg")
   {
      int bytes_received;
      int long_id = receive_int(p, &bytes_received);
      int id = atoi(receive_string(p, long_id, &bytes_received).c_str());
      int long_msg = receive_int(p, &bytes_received);
      string msg = receive_string(p, long_msg, &bytes_received);
      chat* chat = get_chat_from_id(id);
      list<player*>::iterator i;
      player* dest;
      for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
      {
         dest = *i;
         send_command("new_chat_msg", dest);
         send_int(dest, id);
         send_int(dest, p->name.length());
         send_string(dest, p->name);
         send_int(dest, msg.length());
         send_string(dest, msg);
      }
   }
}

void handle_client(void* arg)
{
   player* p = (player*) arg;
   int bytes_received;
   int long_name = receive_int(p, &bytes_received);
   p->name = receive_string(p, long_name, &bytes_received);
   cout << "Handling new player. Player name: " << p->name << endl;
   if (p->name.find('~') != string::npos)
   {
      cout << "Player " << p->name << " rejected because the name contained invalid character '~' (WARNING: possible hacked client)" << endl;
      p->socket->psend("login no", 8, 0);
	   delete p;
      cout << "Disconnecting client" << endl;
   }
   else if (add_player_to_list(p) == -1)
   {
      p->socket->psend("login ko", 8, 0);
	   delete p;
      cout << "Disconnecting client" << endl;
   }
   else
   {
      p->socket->psend("login ok", 8, 0);
      bool online = true;
      int long_command;
      while (online)
      {
         long_command = receive_int(p, &bytes_received);
         string command = receive_string(p, long_command, &bytes_received);
         if (bytes_received > 0)
         {
            cout << "Received command from player " << p->name << ": " << command << endl;
            handle_command(command, p);
         }
         else
         {
            online = false;
            disconnect_client(p);
            delete p;
            cout << "Client disconnected" << endl;
         }
      }
   }
}

void run_server()
{
   cout << "Starting Hotel server..." << endl;
   socket_server = new portable_socket();
   sockaddr_in server_info;
   sockaddr_in client_info;
   socklen_t addrlen;

   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   server_info.sin_addr.s_addr=INADDR_ANY;
   if (socket_server->pbind((sockaddr*) &server_info,sizeof(server_info)) < 0)
   {
	   cout << "bind error: " << socket_server->get_last_error() << endl;
	   delete socket_server;
	   return;
   }
   if (socket_server->plisten(MAXCONN) < 0)
   {
	   cout << "listen error: " << socket_server->get_last_error() << endl;
	   delete socket_server;
	   return;
   }
   cout << "Listening for connections" << endl;

   hook_signals();
   player* p;
   while (closing == 0)
   {
      addrlen = sizeof(client_info);
      socket_client = socket_server->paccept((sockaddr*) &client_info, &addrlen);
      if (socket_client != NULL)
         cout << "Client connection from " << inet_ntoa(client_info.sin_addr) << ":" << ntohs(client_info.sin_port) << endl;
      else
      {
         if (closing == 0)
            cout << "accept error: " << socket_server->get_last_error() << endl;
         else
         {
            // The server is closing
            empty_global_chat_list();
            cout << "Chat list cleaned" << endl;
            empty_glist();
            cout << "Game list cleaned" << endl;
            empty_plist();
            cout << "Player list cleaned" << endl;
            return;
         }
      }
      p = new player(inet_ntoa(client_info.sin_addr), socket_client);
      create_new_thread(handle_client, (void*) p);
   }
}

int main(int argc, char* argv[])
{
   run_server();
}
