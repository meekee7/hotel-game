#include <string>
#include <csignal>
#include "Server.h"
Server* server;

void unhook_signals()
{
    signal(SIGINT, 0);
    signal(SIGTERM, 0);
#ifdef _WIN32
    signal(SIGBREAK, 0);
#endif
}

void close_server (int signum)
{
    unhook_signals();
    server->Stop(signum);
}

void hook_signals()
{
    signal(SIGINT, close_server);
    signal(SIGTERM, close_server);
#ifdef _WIN32
    signal(SIGBREAK, close_server);
#endif
}

int main(int argc, char* argv[])
{
    int port;
    if (argc == 2) // Port specified
        port = atoi(argv[1]);
    else
        port = 12345;
    hook_signals();
    server = new Server();
    server->Run(port);
    delete server;
}