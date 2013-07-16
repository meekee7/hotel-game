#pragma once
#include "Portable_Socket.h"
#include "Player.h"
#include "PlayerGameState.h"
#include "Game.h"
#include "Chat.h"
#include "Aux_Functions.h"
#include "TinyXML.h"
#include "Hotel.h"
#include "Types.h"
#include "Random.h"
#include "CommandHandler.h"
#include "ServerState.h"

class Server
{
public:
    string compatible_version;
    volatile int closing;
    Portable_socket* socket_server;
    Portable_socket* socket_client;
    struct config configuration;
    CommandHandler* ch;

    Server();
    ~Server();
    void Run(int port);
    void Stop(int signum);
    void handle_client(void* arg);
private:
    void handle_command(string command, Player* player);
    void read_config(TiXmlDocument* config_xml);
    void empty_lists();
};