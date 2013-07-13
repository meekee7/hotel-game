#pragma once
#include "Player.h"
#include "Game.h"
#include "Chat.h"

class ServerState
{
public:
    list<Player*> plist; // Player list
    list<Game*> glist; // Game list
    list<Player*> global_chat_list; // Players in global chat
    list<Chat*> chat_list;

    dlib::mutex mutex_ids;
    dlib::mutex mutex_lists;
    dlib::mutex mutex_disconnects;
    dlib::mutex debt_mutex;

    CRandomMT* random_gen;
    int id_count;

    ServerState(void);
    ~ServerState(void);
};

