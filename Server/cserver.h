#ifndef CSERVER_H
#define CSERVER_H

#define MAX_CLIENTS 63

#include <string.h>
#include "cbase.h"
#include "cconnection.h"

class cserver : public cbase
{
 private:
   char ids[128];
   cconnection connections[MAX_CLIENTS];

 public:
   cserver(unsigned short port);
   ~cserver();
   void write_packet(int n,char* packet);
   void update();
   int read_packet(char* packet);
   bool connected();
};

#endif
