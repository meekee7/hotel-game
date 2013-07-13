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
#include "SavedgamesMgr.h"
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
    
    SavedgamesMgr* savedgamesmgr;
    CommandHandler* ch;
    ServerState* serverState;

    Server();
    ~Server();
    void Run(int port);
    void unhook_signals();
    void handle_client(void* arg);
private:
    void handle_command(string command, Player* player);
    void read_config(TiXmlDocument* config_xml);
    void hook_signals();
    void empty_plist();
    void empty_glist();
    void empty_chat_list();
    void empty_global_chat_list();
};