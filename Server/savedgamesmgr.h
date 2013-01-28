#pragma once
#include "mysqlmgr.h"
#include "game.h"
#include "player.h"

class SavedgamesMgr
{
public:
    MySQLMgr* db;
    bool db_loaded_ok;
    int keep_alive_interval;
    bool keeping_alive;
    void KeepAlive();
    bool SaveGame(Game* game);
    SavedgamesMgr(void);
    ~SavedgamesMgr(void);
};