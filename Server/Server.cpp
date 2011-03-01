#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <cstdlib>
#include <signal.h>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "dlib/threads.h"

using namespace std;
using namespace dlib;

volatile int closing = 0;
portable_socket* socket_server;
portable_socket* socket_cliente;
list<player*> player_list;
list<game*> game_list;

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
   if (socket_cliente != NULL)
      delete socket_cliente;
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

void tratar_cliente(player* p)
{
   cout << "Tratando player con ip " << p->ip << endl;
}

void hacer_de_server()
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
      socket_cliente = socket_server->paccept((sockaddr*) &client_info, &addrlen);
      if (socket_cliente != NULL)
         cout << "Client connection from " << inet_ntoa(client_info.sin_addr) << ":" << ntohs(client_info.sin_port) << endl;
      else
      {
         cout << "accept error: " << socket_server->get_last_error() << endl;
         delete socket_server;
         return;
      }
      player* p = new player();
      p->ip = inet_ntoa(client_info.sin_addr);
      player_list.push_back(p);
      thread_function(tratar_cliente, p);
   }
}

void hacer_de_cliente()
{
   int error;
   /* connect to server */
   portable_socket* socket = new portable_socket();
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   server_info.sin_addr.s_addr=inet_addr("88.14.160.181");
   error = socket->pconnect((sockaddr*) &server_info,sizeof(server_info))==0;
   if (error > 0)
      cout << "Connection successful" << endl;
   else
      cout << "Connection error: " << socket->get_last_error() << endl;
   /* connect to server */
   delete socket;
}

int main(int argc, char* argv[])
{
   cout << "Starting Hotel server" << endl;
   hacer_de_cliente();
}
