#ifndef CBASE_H
#define CBASE_H

#ifdef _WIN32
   #include <winsock2.h>
   #pragma comment(lib, "Ws2_32.lib")
#else
   #include <sys/types.h>
   #include <sys/socket.h>
   #include <netinet/in.h>
   #include <arpa/inet.h>
   #include <sys/select.h>
   #include <unistd.h>
   #define SOCKET int
   #define closesocket close
#endif

class cbase
{
 protected:
   SOCKET s;
   fd_set read_sockets,write_sockets;
   timeval delay;

 public:
   cbase();
   virtual ~cbase();
   virtual bool connected() = 0;
   virtual void write_packet(int n,char* packet)=0;
   virtual void update()=0;
   virtual int read_packet(char* packet)=0;
};

#endif
