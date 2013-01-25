/* Developed by Alberto Salinas, please keep this line */
#pragma once

#ifdef _WIN32
    #include <winsock2.h>
    #pragma comment(lib, "Ws2_32.lib")
    typedef int socklen_t;
#else
    #include <sys/types.h>
    #include <sys/socket.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
    #include <errno.h>
    #define SOCKET int
    #define closesocket close
#endif

class Portable_socket
{
protected:
    int error; // Check after creating new instance
    SOCKET s;
	timeval delay;
public:
	Portable_socket(void);
    int pbind (const struct sockaddr *addr, socklen_t addrlen);
    int plisten(int backlog);
    Portable_socket* paccept(struct sockaddr *addr, socklen_t *addrlen);
    int pconnect(const struct sockaddr *addr, socklen_t addrlen);
    int psend(const void *buf, size_t len, int flags);
    int precv(void *buf, size_t len, int flags);
    void set_fd(SOCKET s);
    SOCKET get_fd();
    int get_last_error();
	~Portable_socket(void);
};

