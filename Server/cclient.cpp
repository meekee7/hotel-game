#include "cclient.h"

cclient::cclient(char* ip,unsigned short port)
{
   /* connect to server */
   sockaddr_in server_info;
   server_info.sin_family=AF_INET;
   server_info.sin_port=htons(port);
   server_info.sin_addr.s_addr=inet_addr(ip);
   connection=connect(s,(sockaddr*) &server_info,sizeof(server_info))==0;
   /* connect to server */

   /* init buffers */
   rb[0]=0;
   sb[0]=0;
   /* init buffers */
}

bool cclient::connected()
{
   return connection;
}

void cclient::write_packet(int n,char* packet)
{
   if (strlen(sb)+strlen(packet)+1<MAX_BUFFER)
   {
      /* header */
      char header[2]={strlen(packet),0};
      /* header */

      /* append header + packet to sendbuffer */
      strcat(sb,header);
      strcat(sb,packet);
      /* append header + packet to sendbuffer */
   }
   else
   {
      closesocket(s);
      connection=0;
   }
}

void cclient::update()
{
   /* clear FD_SET */
   FD_ZERO(&read_sockets);
   FD_ZERO(&write_sockets);
   /* clear FD_SET */

   /* add socket to FD_SET */
   FD_SET(s,&read_sockets);
   FD_SET(s,&write_sockets);
   /* add socket to FD_SET */

   /* read and write data */
   if (select(s+1,&read_sockets,&write_sockets,0,&delay)>0)
   {
      /* read data and disconnect */
      if (FD_ISSET(s,&read_sockets))
      {
         char buffer[1025];
         int bytes=recv(s,buffer,1024,0);
         if ((bytes>0) && (strlen(rb)+bytes<MAX_BUFFER))
         {
            buffer[bytes]=0;
            strcat(rb,buffer);
         }
         else
         {
            closesocket(s);
            connection=0;
         }
      }
      /* read data and disconnect */

      /* send data and remove it from the sendbuffer */
      if (FD_ISSET(s,&write_sockets))
      {
         int bytes=send(s,sb,strlen(sb),0);
         strcpy(sb,sb+bytes);
      }
      /* send data and remove it from the sendbuffer */
   }
   /* read and write data */
}

int cclient::read_packet(char* packet)
{
   /* packet flag */
   int n=0;
   /* packet flag */

   /* copy and remove packet from receivebuffer */
   if (strlen(rb)>1)
   {
      int length=rb[0];
      if (strlen(rb)>length)
      {
         strncpy(packet,rb+1,length);
         strcpy(rb,rb+length+1);
         packet[length]=0;
         n=1;
      }
   }
   /* copy and remove packet from receivebuffer */

   /* return packet flag */
   return n;
   /* return packet flag */
}
