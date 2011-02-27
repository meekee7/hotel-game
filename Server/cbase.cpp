#include "cbase.h"

cbase::cbase()
{
   #ifdef _WIN32
   /* start winsock 2.2 */
   WSAData winsock_info;
   WSAStartup(MAKEWORD(2,2),&winsock_info);
   /* start winsock 2.2 */
   #endif

   /* create socket */
   s=socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
   /* create socket */

   /* init delay */
   delay.tv_sec=delay.tv_usec=0;
   /* init delay */
}

cbase::~cbase()
{
   /* close socket */
   closesocket(s);
   /* close socket */

   #ifdef _WIN32
   /* exit winsock */
   WSACleanup();
   /* exit winsock */
   #endif
}

/*bool cbase::connected()
{
   //virtual 
}*/