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
   cout << "Hecho, error: " << socket->get_error() << endl;
   sockaddr_in client_info;
   int addrlen = sizeof(client_info);
   socket->paccept((sockaddr*) &client_info, &addrlen);
   printf("Conexión de cliente desde %s:%d\n", inet_ntoa(client_info.sin_addr), ntohs(client_info.sin_port));
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
   if (error == 0)
      cout << "Conexión correcta" << endl;
   else
      cout << "Error en la conexión" << endl;
   /* connect to server */
}

int main(int argc, char* argv[])
{
   cout << "Hola" << endl;
   hacer_de_cliente();
}
