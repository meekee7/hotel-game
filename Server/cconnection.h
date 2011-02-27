#ifndef CCONNECTION_H
#define CCONNECTION_H

#define MAX_BUFFER 32768

#ifdef _WIN32
   #include <winsock2.h>
   #pragma comment(lib, "Ws2_32.lib")
#else
   #include <unistd.h>
   #define SOCKET int
   #define closesocket close
#endif

class cconnection
{
 private:
   SOCKET s;
   int id;
   char rb[MAX_BUFFER];
   char sb[MAX_BUFFER];

 public:
   cconnection();
   SOCKET get_s();
   int get_id();
   char* get_rb();
   char* get_sb();
   void create(SOCKET as,int n);
   void reset();
   void destroy();
};

#endif
