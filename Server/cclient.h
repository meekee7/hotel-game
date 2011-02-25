#ifndef CCLIENT_H
#define CCLIENT_H

#define MAX_BUFFER 32768

#include <string.h>
#include "cbase.h"

class cclient : public cbase
{
 private:
   char rb[MAX_BUFFER];
   char sb[MAX_BUFFER];
   bool connection;

 public:
   cclient(char* ip,unsigned short port);
   bool connected();
   void write_packet(int n,char* packet);
   void update();
   int read_packet(char* packet);
};

#endif
