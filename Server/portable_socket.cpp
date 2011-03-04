/* Developed by Alberto Salinas, please keep this line */
#include "portable_socket.h"

portable_socket::portable_socket(void)
{
   #ifdef _WIN32
      /* start winsock 2.2 */
      WSAData winsock_info;
      this->error = WSAStartup(MAKEWORD(2,2),&winsock_info);
      /* start winsock 2.2 */
   #endif

   /* create socket */
   s=socket(AF_INET,SOCK_STREAM,IPPROTO_TCP);
   /* create socket */

   #ifndef _WIN32
      this->error = errno;
   #endif

   /* init delay */
   this->delay.tv_sec=this->delay.tv_usec=0;
   /* init delay */
}

int portable_socket::pbind (const struct sockaddr *addr, socklen_t addrlen)
{
   return bind(this->s, addr, addrlen);
}

int portable_socket::plisten(int backlog)
{
   return listen(this->s, backlog);
}

portable_socket* portable_socket::paccept(struct sockaddr *addr, socklen_t *addrlen)
{
   int s = accept(this->s, addr, addrlen);
   if (s >= 0)
   {
      portable_socket* socket_cliente = new portable_socket();
      socket_cliente->set_fd(s);
      return socket_cliente;
   }
   else
      return NULL;
}

int portable_socket::pconnect(const struct sockaddr *addr, socklen_t addrlen)
{
   return connect(this->s, addr, addrlen);
}

int portable_socket::psend(const void *buf, size_t len, int flags)
{
   return send(this->get_fd(), (char*) buf, len, flags);
}

int portable_socket::precv(void *buf, size_t len, int flags)
{
   return recv(this->get_fd(), (char*) buf, len, flags);
}

void portable_socket::set_fd(SOCKET s)
{
   this->s = s;
}

SOCKET portable_socket::get_fd()
{
   return this->s;
}

int portable_socket::get_last_error()
{
   #ifdef _WIN32
      this->error = WSAGetLastError();
      return this->error;
   #else
      this->error = errno;
      return this->error;
   #endif
}

portable_socket::~portable_socket(void)
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