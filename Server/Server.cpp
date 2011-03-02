#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "dlib/threads.h"
#include "dlib/misc_api.h"
#include "dlib/ref.h"
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

void handle_client(void* arg)
{
   player* p = (player*) arg;
   cout << "Handling new player. Player name: ";
   char* data = (char*) malloc(sizeof(char)*MAXDATALEN);
   int bytes_received;
   bytes_received = recv(p->socket->get_fd(), data, MAXDATALEN, 0);
   p->name = data;
   delete data;
   cout << p->name << endl;
   if (add_player_to_list(p) == -1)
   {
      send(p->socket->get_fd(), "username in use", 16, 0);
	   delete p;
   }
   else
   {
      send(p->socket->get_fd(), "login ok", 9, 0);
      bool online = true;
      while (online)
      {
         char* data = (char*) malloc(sizeof(char)*MAXDATALEN);
         cout << "Awaiting data" << endl;
         if (recv(p->socket->get_fd(), data, MAXDATALEN, 0))
            cout << "Received: " << data << endl;
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
   send(socket->get_fd(), name.c_str(), name.length()+1, 0);
   // Wait for successul login
   char* data = (char*) malloc (sizeof(char)*MAXDATALEN);
   if (!recv(socket->get_fd(), data, MAXDATALEN, 0))
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
         send(socket->get_fd(), input.c_str(), input.length()+1, 0);
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
