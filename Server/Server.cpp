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
list<player> plist; // Player list
list<game> glist; // Game list

void unhook_signals()
{
   signal(SIGINT, 0);
   signal(SIGTERM, 0);
   #ifdef _WIN32
      signal(SIGBREAK, 0);
   #endif
}

void empty_plist()
{
	list<player>::iterator i;
	for (i = plist.begin() ; i != plist.end() ; ++i)
	{
		delete i->socket;
	}
}

void cerrar (int signum)
{
	closing = 1;
   printf ("Closing server\n");
   unhook_signals();
   delete socket_server;
   if (socket_client != NULL)
      delete socket_client;
   empty_plist();
   exit(0);
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
	list<player>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if (i->name == p->name)
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
		plist.push_back(*p);
		return 0;
	}
}

void delete_player_from_list(player* p)
{
   bool found = false;
	list<player>::iterator i = plist.begin();
	while (!found && i != plist.end())
	{
		if (i->name == p->name)
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

void prepare_data (char* data, int length)
{
	data[length] = '\0';
	//return string(data);
}

void handle_command(string command, player* p)
{
	if (command == "get_users")
	{
		// Get all users and join into a string with the separator ~
		string res = "";
		list<player>::iterator i;
		for (i = plist.begin() ; i != plist.end() ; ++i)
		{
			res += i->name;
			if (!(i != plist.end() && (next(i) == plist.end())))
				res += '~';
		}
		int res_len = htonl(res.length());
		p->socket->psend(&res_len, sizeof(res_len), 0);
		p->socket->psend(res.c_str(), res.length(), 0);
	}
	else if (command == "get_games")
	{
		// Get all users and join into a string with the separator ~
		string res = "";
		list<game>::iterator i;
		for (i = glist.begin() ; i != glist.end() ; ++i)
		{
			res += i->name;
			if (!(i != glist.end() && (next(i) == glist.end())))
				res += '~';
		}
		int res_len = htonl(res.length());
		p->socket->psend(&res_len, sizeof(res_len), 0);
		if (res.length() != 0)
			p->socket->psend(res.c_str(), res.length(), 0);
	}
	else if (command == "create_game")
	{
      int n_players;
      char* data = (char*) malloc(sizeof(char) * MAXDATALEN);
      int bytes_received = p->socket->precv(data, MAXDATALEN, 0);
      prepare_data(data, bytes_received);
      bytes_received--;
      n_players = atoi(&data[bytes_received]);
      data[bytes_received] = '\0';
      string name(data);
      cout << "New game! Name: " << name << " | Number of players: " << n_players << endl;
      game new_game;
      new_game.name = name;
      new_game.n_players = n_players;
      new_game.creator = *p;
      new_game.plist.push_back(*p);
      glist.push_back(new_game);
      free (data);
   }
}

void handle_client(void* arg)
{
   player* p = (player*) arg;
   cout << "Handling new player. Player name: ";
   char* data = (char*) malloc(sizeof(char)*MAXDATALEN);
   int bytes_received;
   bytes_received = p->socket->precv(data, MAXDATALEN, 0);
   prepare_data(data, bytes_received);
   p->name = string(data);
   free (data);
   cout << p->name << endl;
   if (add_player_to_list(p) == -1)
   {
      p->socket->psend("username in use", 15, 0);
	   delete p;
   }
   else
   {
      p->socket->psend("login ok", 8, 0);
      bool online = true;
      while (online)
      {
         char* data = (char*) malloc(sizeof(char)*MAXDATALEN);
         cout << "Awaiting commands" << endl;
         int bytes_received = p->socket->precv(data, MAXDATALEN, 0);
         prepare_data(data, bytes_received);
         string command (data);
         if (bytes_received > 0)
         {
            cout << "Received: " << command << endl;
            handle_command(command, p);
         }
         else
         {
            online = false;
            cout << "Client disconnected" << endl;
            delete_player_from_list(p);
         }
		   free (data);
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
         cout << "accept error: " << socket_server->get_last_error() << endl;
         delete socket_server;
         return;
      }
      p = new player();
      p->ip = inet_ntoa(client_info.sin_addr);
      p->socket = socket_client;
      create_new_thread(handle_client, (void*) p);
   }
}

void run_client()
{
   cout << "Starting test client" << endl;
   int error;
   /* connect to server */
   portable_socket* socket = new portable_socket();
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   string ip;
   cout << "Type server ip address:" << endl;
   cin >> ip;
   server_info.sin_addr.s_addr=inet_addr(ip.c_str());
   error = socket->pconnect((sockaddr*) &server_info,sizeof(server_info))==0;
   if (error > 0)
      cout << "Connection successful" << endl;
   else
      cout << "Connection error: " << socket->get_last_error() << endl;
   /* connect to server */
   string name;
   cout << "Enter your name:" << endl;
   cin >> name;
   //send(socket->get_fd(), name.c_str(), name.length()+1, 0);
   socket->psend(name.c_str(), name.length()+1, 0);
   // Wait for successul login
   char* data = (char*) malloc (sizeof(char)*MAXDATALEN);
   if (!socket->precv(data, MAXDATALEN, 0))
   {
      cout << "Connection error" << endl;
      delete socket;
      return;
   }
   if (strcmp(data, "username in use") == 0)
   {
      cout << "Username already in use" << endl;
      delete socket;
      return;
   }
   cout << "Login ok" << endl;
   bool online = true;
   string input;
   while (online)
   {
      cout << "Enter data to send: ('exit' to disconnect)" << endl;
      cin >> input;
      if (input == "exit")
      {
         online = false;
         delete socket;
      }
      else
         socket->psend(input.c_str(), input.length()+1, 0);
   }
}

int main(int argc, char* argv[])
{
   cout << "Type in mode: " << endl << " 1 -> server" << endl << " 2 -> client" << endl;
   int mode;
   cin >> mode;
   if (mode == 1)
      run_server();
   else if (mode == 2)
      run_client();
   else
      cout << "Invalid mode" << endl;
}
