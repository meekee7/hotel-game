#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "dlib/threads.h"

using namespace std;
using namespace dlib;

volatile int closing = 0;
portable_socket* socket_server;
portable_socket* socket_client;
list<player> player_list;
list<game> game_list;

void unhook_signals()
{
   signal(SIGINT, 0);
   signal(SIGTERM, 0);
   #ifdef _WIN32
      signal(SIGBREAK, 0);
   #endif
}

void cerrar (int signum)
{
	closing = 1;
   printf ("Closing server\n");
   unhook_signals();
   delete socket_server;
   if (socket_client != NULL)
      delete socket_client;
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

void handle_client(player* p)
{
   cout << "Handling new player. Player name: ";
   char* data = (char*) malloc(sizeof(char)*100);
   int bytes_received;
   bytes_received = recv(p->socket->get_fd(), data, 100, 0);
   p->name = data;
   cout << p->name << endl;
}

struct is_in_list: public std::binary_function< player, string, bool > {
  bool operator () ( const player &p, const string &name ) const {
    return p.name == name;
    }
  };

int add_player_to_list (player* p)
{
	list<player>::iterator found = find_if(player_list.begin(), player_list.end(), bind2nd(is_in_list(), p->name));
	if (found != player_list.end())
	{
		cout << "Player already connected" << endl;
		return 0;
	}
	else
	{
		cout << "Player accepted" << endl;
		return -1;
	}
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
      player* p = new player();
      p->ip = inet_ntoa(client_info.sin_addr);
      p->socket = socket_client;
      thread_function(handle_client, p);
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
   server_info.sin_addr.s_addr=inet_addr("78.47.226.210");
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
   run_client();
}
