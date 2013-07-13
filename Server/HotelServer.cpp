#include <string>
#include "Server.h"

int main(int argc, char* argv[])
{
    int port;
    if (argc == 2) // Port specified
        port = atoi(argv[1]);
    else
        port = 12345;
    Server server;
    server.Run(port);
}