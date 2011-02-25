#include "cserver.h"

cserver::cserver(unsigned short port)
{
   /* start server */
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(port);
   server_info.sin_addr.s_addr=INADDR_ANY;
   bind(s,(sockaddr*) &server_info,sizeof(server_info));
   listen(s,SOMAXCONN);
   /* start server */

   /* init buffer */
   ids[0]=0;
   /* init buffer */
}

cserver::~cserver()
{
   int i;
   for (i=0;i<MAX_CLIENTS;i++) connections[i].destroy();
}

void cserver::write_packet(int n,char* packet)
{
   /* header */
   char header[2]={strlen(packet),0};
   /* header */

   /* append header + packet to sendbuffer(s) */
   int i=0;
   while (i<MAX_CLIENTS)
   {
      if ((n==1) || (n==connections[i].get_id()))
      {
         if (strlen(connections[i].get_sb())+strlen(packet)+1<MAX_BUFFER)
         {
            strcat(connections[i].get_sb(),header);
            strcat(connections[i].get_sb(),packet);
         }
         else {connections[i].reset();}
      }

      if (n==connections[i].get_id()) i=MAX_CLIENTS;
      else i++;
   }
   /* append header + packet to sendbuffer(s) */
}

void cserver::update()
{
   int i;

   /* clear FD_SET */
   FD_ZERO(&read_sockets);
   FD_ZERO(&write_sockets);
   /* clear FD_SET */

   /* for connection requests */
   FD_SET(s,&read_sockets);
   /* for connection requests */

   /* highest file descriptor */
   int hfd=s;
   /* highest file descriptor */

   /* add sockets to FD_SET and check for highest file descriptor */
   for (i=0;i<MAX_CLIENTS;i++)
   if (connections[i].get_id()!=-1)
   {
      FD_SET(connections[i].get_s(),&read_sockets);
      FD_SET(connections[i].get_s(),&write_sockets);
      if ((int)connections[i].get_s()>hfd) hfd=connections[i].get_s();
   }
   /* add sockets to FD_SET and check for highest file descriptor */

   if (select(hfd+1,&read_sockets,&write_sockets,0,&delay)>0)
   {
      /* read data and remove disconnected connections */
      for (i=0;i<MAX_CLIENTS;i++)
      if ((connections[i].get_id()!=-1) && (FD_ISSET(connections[i].get_s(),&read_sockets)))
      {
         char buffer[1025];
         int bytes=recv(connections[i].get_s(),buffer,1024,0);
         if ((bytes>0) && (strlen(connections[i].get_rb())+bytes<MAX_BUFFER))
         {
            buffer[bytes]=0;
            strcat(connections[i].get_rb(),buffer);
         }
         else
         {
            char packet[2]={connections[i].get_id(),0};
            connections[i].reset();
            strcat(ids,packet);
         }
      }
      /* read data and remove disconnected connections */

      /* send data and remove it from the sendbuffer */
      for (i=0;i<MAX_CLIENTS;i++)
      if ((connections[i].get_id()!=-1) && (FD_ISSET(connections[i].get_s(),&write_sockets)))
      {
         int bytes=send(connections[i].get_s(),connections[i].get_sb(),strlen(connections[i].get_sb()),0);
         strcpy(connections[i].get_sb(),connections[i].get_sb()+bytes);
      }
      /* send data and remove it from the sendbuffer */

      /* accept/reject new connection */
      if (FD_ISSET(s,&read_sockets))
      {
         int clients=0;
         for (i=0;i<MAX_CLIENTS;i++) if (connections[i].get_id()!=-1) clients++;

         if (clients<MAX_CLIENTS)
         {
            /* search for unused id */
            i=0;
            int n=2;
            while (i<MAX_CLIENTS)
            if (connections[i].get_id()!=n) i++;
            else
            {
               n++;
               i=0;
            }
            /* search for unused id */

            /* create new connection and send new id to server */
            i=0;
            while (connections[i].get_id()!=-1) i++;
            connections[i].create(accept(s,0,0),n);
            char packet[2]={n,0};
            strcat(ids,packet);
            /* create new connection and send new id to server */
         }
         else
         {
            SOCKET reject=accept(s,0,0);
            closesocket(reject);
         }
      }
      /* accept/reject new connection */
   }
}

int cserver::read_packet(char* packet)
{
   /* id */
   int n=0;
   /* id */

   /* connected/disconnected clients */
   if (strlen(ids)>0)
   {
      packet[0]=0;
      n=ids[0];
      strcpy(ids,ids+1);
   }
   /* connected/disconnected clients */

   /* copy and remove packet from receivebuffer */
   else
   {
      int i=0;
      while (i<MAX_CLIENTS)
      if (connections[i].get_id()!=-1)
         if (strlen(connections[i].get_rb())>1)
         {
            int length=connections[i].get_rb()[0];
            if ((int)strlen(connections[i].get_rb())>(int)length)
            {
               strncpy(packet,connections[i].get_rb()+1,length);
               strcpy(connections[i].get_rb(),connections[i].get_rb()+length+1);
               packet[length]=0;
               n=connections[i].get_id();
               i=MAX_CLIENTS;
            }
            else i++;
         }
         else i++;
      else i++;
   }
   /* copy and remove packet from receivebuffer */

   /* return id */
   return n;
   /* return id */
}

bool cserver::connected()
{
	return false;
}
