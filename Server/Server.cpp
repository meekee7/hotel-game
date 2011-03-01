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

void handle_client(player* p)
{
   cout << "Handling new player. Player name: ";
   char* data = (char*) malloc(sizeof(char)*100);
   int* bytes_received = (int*) malloc(sizeof(int));
   *bytes_received = recv(p->socket->get_fd(), data, 100, 0);
   p->name = data;
   delete data;
   delete bytes_received;
   cout << p->name << endl;
   if (add_player_to_list(p) == -1)
	   delete p;
   dlib::sleep(1000);
}

void run_server()
{
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
   if (socket_server->plisten(SOMAXCONN) < 0)
   {
	   cout << "listen error: " << socket_server->get_last_error() << endl;
	   delete socket_server;
	   return;
   }

   // Ejemplo iterador (recordatorio)
   /*list<player*>::iterator i;
   for (i=player_list.begin(); i != player_list.end(); ++i)
   {
      player* player = *i;
      cout << player->name << " "; cout << endl;
   }*/

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
      thread_function thread(handle_client, p);
	  cout << "Volviendo de la llamada al thread" << endl;
   }
}

void run_client()
{
   int error;
   /* connect to server */
   portable_socket* socket = new portable_socket();
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   server_info.sin_addr.s_addr=inet_addr("140.0.24.92");
   error = socket->pconnect((sockaddr*) &server_info,sizeof(server_info))==0;
   if (error > 0)
      cout << "Connection successful" << endl;
   else
      cout << "Connection error: " << socket->get_last_error() << endl;
   /* connect to server */
   send(socket->get_fd(), "Hola!", 6, 0);
   delete socket;
}

int main(int argc, char* argv[])
{
   cout << "Starting Hotel server" << endl;
   run_server();
}
