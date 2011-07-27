#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "dlib/threads.h"
#define MAXCONN 100
#define MAXDATALEN 100

using namespace std;
using namespace dlib;

volatile int closing = 0;
portable_socket* socket_server;
portable_socket* socket_client;
list<player*> plist; // Player list
list<game*> glist; // Game list
list<player*> chat_list; // Players in global chat

void unhook_signals()
{
   signal(SIGINT, 0);
   signal(SIGTERM, 0);
   #ifdef _WIN32
      signal(SIGBREAK, 0);
   #endif
}

void empty_chat_list()
{
   list<player*>::iterator i;
	for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
	{
		*i = NULL;
	}
}

void empty_glist()
{
   list<game*>::iterator i;
	for (i = glist.begin() ; i != glist.end() ; ++i)
	{
      game* g = *i;;
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

void cerrar (int signum)
{
	closing = 1;
   cout << endl << "Closing server" << endl;
   unhook_signals();
   delete socket_server;
}

void hook_signals()
{
   signal(SIGINT, cerrar);
   signal(SIGTERM, cerrar);
   #ifdef _WIN32
      signal(SIGBREAK, cerrar);
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
	{
		cout << "Player not found" << endl;
	}
	else
	{
		cout << "Player deleted from list" << endl;
		plist.erase(i);
	}
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

void handle_command(string command, player* p)
{
	if (command == "get_users")
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
      send_int(p, res.length());
		if (res.length() != 0)
         send_string(p, res);
	}
	else if (command == "create_game")
	{
      int bytes_received;
      int long_name = receive_int(p, &bytes_received);
      string name = receive_string(p, long_name, &bytes_received);
      int n_players = receive_int(p, &bytes_received);
      cout << "New game! Name: " << name << " | Number of players: " << n_players << endl;
      game* new_game = new game();
      new_game->name = name;
      new_game->n_players = n_players;
      new_game->creator = p;
      new_game->plist.push_back(p);
      glist.push_back(new_game);
   }
   else if (command == "join_chat")
   {
      chat_list.push_back(p);
   }
   else if (command == "leave_chat")
   {
      chat_list.remove(p);
   }
   else if (command == "get_chat_users")
   {
      // Get all users and join into a string with the separator ~
		string res = "";
		list<player*>::iterator i;
		for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
		{
			res += (*i)->name;
			if (i != --chat_list.end())
				res += '~';
		}
      send_int(p, res.length());
      send_string(p, res);
   }
   else if (command == "send_global_msg")
   {
      int bytes_received;
      int long_msg = receive_int(p, &bytes_received);
      string msg = receive_string(p, long_msg, &bytes_received);
      msg = p->name + ": " + msg;
      list<player*>::iterator i;
      for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
      {
         send_int(p, msg.length());
      }
   }
}

void handle_client(void* arg)
{
   player* p = (player*) arg;
   cout << "Handling new player. Player name: ";
   int bytes_received;
   int long_name = receive_int(p, &bytes_received);
   p->name = receive_string(p, long_name, &bytes_received);
   cout << p->name << endl;
   if (add_player_to_list(p) == -1)
   {
      p->socket->psend("login ko", 8, 0);
	   delete p;
      cout << "Disconnecting client" << endl;
   }
   else
   {
      p->socket->psend("login ok", 8, 0);
      bool online = true;
      while (online)
      {
         int long_command = receive_int(p, &bytes_received);
         string command = receive_string(p, long_command, &bytes_received);
         if (bytes_received > 0)
         {
            cout << "Received command: " << command << endl;
            handle_command(command, p);
         }
         else
         {
            online = false;
            cout << "Client disconnected" << endl;
            delete_player_from_list(p);
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
            empty_chat_list();
            cout << "Chat list cleaned" << endl;
            empty_glist();
            cout << "Game list cleaned" << endl;
            empty_plist();
            cout << "Player list cleaned" << endl;
            return;
         }
      }
      p = new player();
      p->ip = inet_ntoa(client_info.sin_addr);
      p->socket = socket_client;
      create_new_thread(handle_client, (void*) p);
   }
}

int main(int argc, char* argv[])
{
   run_server();
}
