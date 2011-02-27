#include <iostream>
#include <stdio.h>
#include "cclient.h"
#include "cserver.h"

using namespace std;

void hacer_de_cliente ()
{
	// don't forget to add cbase.cpp and cclient.cpp to your project

   cbase* client=new cclient("78.47.226.210",12345);
   // create client and connect to 127.0.0.1:12345

   if (!client->connected()) printf("Could not connect!\n");
   // if connection is established connected() returns 1 else 0

   else
   {
      char buffer[256]="Packet from client";
      // single packet can be up to 255 bytes long

      client->write_packet(0,buffer);
      // first parameter is ignored on client side
      // client can only send packets to the server

      client->update();
      // sending and receiving is actually done here

      while (client->read_packet(buffer)>0)
      // as long as packets are available
      printf("Server sent '%s'.\n",buffer);
      // read_packet() returns source id or 0 if no packet is available
   }

   delete client;
   // disconnect and delete client
}

void hacer_de_server ()
{
	   cbase* server=new cserver(12345);
   // create server listening on port 12345

   while (1)
   {
      char buffer[256]="Packet from server";
      // single packet can be up to 255 bytes long

      server->write_packet(1,buffer);
      // first parameter is the target id (2-64) or 1 to send the packet to all clients

      server->update();
      // sending and receiving is actually done here

      int i;
      // to store source id
      do
      {
         i=server->read_packet(buffer);
         // read_packet() returns source id or 0 if no packet is available
         if (i>0)
         // if a packet has been received
            if (strlen(buffer)==0) printf("ID %i connected/disconnected.\n",i);
            // if buffersize is 0 the client with id i has just connected/disconnected
            else printf("ID %i sent '%s'.\n",i,buffer);
            // if buffersize is >0 a packet has been received
      }
      while (i>0);
      // as long as packets are available
   }

   delete server;
   // shutdown and delete server
}

int main(int argc, char* argv[])
{
	cout << "Hola" << endl;
}