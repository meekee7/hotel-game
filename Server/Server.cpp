#include <iostream>
#include <stdio.h>
#include "portable_socket.h"
#include <cstdlib>
#include <signal.h>
#include <windows.h>

using namespace std;

volatile int cerrando = 0;

void cerrar (int signum)
{
	cerrando = 1;
}

void capturar_señales()
{
	signal(SIGINT, cerrar);
    signal(SIGTERM, cerrar);
    #ifdef _WIN32
		signal(SIGBREAK, cerrar);
    #endif
}

void quitar_captura_señales()
{
    signal(SIGINT, 0);
    signal(SIGTERM, 0);
    #ifdef _WIN32
		signal(SIGBREAK, 0);
    #endif
}

void hacer_de_server()
{
   portable_socket* socket = new portable_socket();
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   server_info.sin_addr.s_addr=INADDR_ANY;
   if (socket->pbind((sockaddr*) &server_info,sizeof(server_info)) < 0)
   {
	   cout << "Error en bind: " << socket->get_last_error() << endl;
	   delete socket;
	   return;
   }
   if (socket->plisten(SOMAXCONN) < 0)
   {
	   cout << "Error en listen: " << socket->get_last_error() << endl;
	   delete socket;
	   return;
   }
   sockaddr_in client_info;
   socklen_t addrlen = sizeof(client_info);
   capturar_señales();
   while (cerrando == 0)
   {
   }
   printf ("Cerrando\n");
   quitar_captura_señales();
   portable_socket* socket_cliente = socket->paccept((sockaddr*) &client_info, &addrlen);
   printf("Conexión de cliente desde %s:%d\n", inet_ntoa(client_info.sin_addr), ntohs(client_info.sin_port));
   /*char* data = (char*) malloc(sizeof(char)*200);
   int bytes = recv(socket_cliente->get_fd(),data, 200, 0);
   printf("Recibidos %i bytes: %s|\n", bytes, data);
   printf("Enviados %i bytes\n", send(socket_cliente->get_fd(), "Adios", 6, 0));*/
   system("pause");
   delete socket;
}

void hacer_de_cliente()
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
      cout << "Conexión correcta" << endl;
   else
      cout << "Error en la conexión: " << socket->get_last_error() << endl;
   /* connect to server */
   cout << "Bytes enviados: " << send(socket->get_fd(),"Hola", 5, 0) << endl;
   char* data = (char*) malloc(sizeof(char)*200);
   //printf("Bytes recibidos: %i, len data: %i %s\n", socket->precv(data, 200, 0), strlen(data), data);
   printf("Bytes recibidos: %i, len data: %s\n", recv(socket->get_fd(),data, 200, 0), data);
   delete socket;
}

int main(int argc, char* argv[])
{
   cout << "Hola" << endl;
   hacer_de_server();
}