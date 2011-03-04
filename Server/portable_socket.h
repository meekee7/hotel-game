#pragma once

#ifdef _WIN32
   #include <winsock2.h>
   #pragma comment(lib, "Ws2_32.lib")
   #define socklen_t int
#else
   #include <sys/types.h>
   #include <sys/socket.h>
   #include <netinet/in.h>
   #include <arpa/inet.h>
   #include <sys/select.h>
   #include <unistd.h>
   #include <errno.h>
   #define SOCKET int
   #define closesocket close
#endif

class portable_socket
{
protected:
   int error; // Check after creating new instance
   SOCKET s;
	timeval delay;
public:
	portable_socket(void);
   int pbind (const struct sockaddr *addr, socklen_t addrlen);
   int plisten(int backlog);
   portable_socket* paccept(struct sockaddr *addr, socklen_t *addrlen);
   int pconnect(const struct sockaddr *addr, socklen_t addrlen);
   int psend(const void *buf, size_t len, int flags);
   int precv(void *buf, size_t len, int flags);
   void set_fd(SOCKET s);
   SOCKET get_fd();
   int get_last_error();
	~portable_socket(void);
};

