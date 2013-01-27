#pragma once
#include "mysqlmgr.h"
#include "game.h"
#include "player.h"

class SavedgamesMgr
{
public:
    MySQLMgr* db;
    bool db_loaded_ok;
    bool SaveGame(Game* game);
    SavedgamesMgr(void);
    ~SavedgamesMgr(void);
};