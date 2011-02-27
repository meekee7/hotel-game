#include <iostream>
#include <stdio.h>
#include "portable_socket.h"

using namespace std;

int hacer_de_server()
{
   portable_socket* socket = new portable_socket();
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(12345);
   server_info.sin_addr.s_addr=INADDR_ANY;
   socket->pbind((sockaddr*) &server_info,sizeof(server_info));
   socket->plisten(SOMAXCONN);
   cout << "Hecho, error: " << socket->get_error() << endl;
   return 0;
}

int main(int argc, char* argv[])
{
   cout << "Hola" << endl;
   hacer_de_server();
}
