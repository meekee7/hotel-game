#pragma once

#ifdef _WIN32
   #include <winsock2.h>
   #pragma comment(lib, "Ws2_32.lib")
   #define socklen_t int
   #define ssize_t int
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
   int paccept(struct sockaddr *addr, socklen_t *addrlen);
   int pconnect(const struct sockaddr *addr, socklen_t addrlen);
   ssize_t precv(void *buf, size_t len, int flags);
   ssize_t psend(const void *buf, size_t len, int flags);
   int get_last_error();
	~portable_socket(void);
};

