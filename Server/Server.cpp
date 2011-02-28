#include <iostream>
#include <stdio.h>
#include "portable_socket.h"

using namespace std;

void hacer_de_server()
{
   portable_socket* socket = new portable_socket();
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   server_info.sin_addr.s_addr=INADDR_ANY;
   socket->pbind((sockaddr*) &server_info,sizeof(server_info));
   socket->plisten(SOMAXCONN);
   cout << "Hecho, error: " << socket->get_last_error() << endl;
   sockaddr_in client_info;
   socklen_t addrlen = sizeof(client_info);
   portable_socket* socket_cliente = socket->paccept((sockaddr*) &client_info, &addrlen);
   printf("Conexión de cliente desde %s:%d\n", inet_ntoa(client_info.sin_addr), ntohs(client_info.sin_port));
   char* data = (char*) malloc(sizeof(char)*200);
   int bytes = recv(socket_cliente->get_fd(),data, 200, 0);
   printf("Recibidos %i bytes: %s|\n", bytes, data);
   printf("Enviados %i bytes\n", send(socket_cliente->get_fd(), "Adios", 6, 0));
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
   hacer_de_cliente();
}