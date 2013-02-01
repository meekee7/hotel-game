#pragma once
#include "mysqlmgr.h"
#include "game.h"
#include "player.h"
#include "random.h"

class SavedgamesMgr
{
public:
    MySQLMgr* db;
    bool db_loaded_ok;
    int keep_alive_interval;
    bool keeping_alive;
    void KeepAlive();
    bool SaveGame(Game* game);
    Game* LoadGame(int id, wstring password, Player* creator, dlib::mutex* mutex_ids, int* id_count, CRandomMT* random_gen, int* error_code);
    void CleanInconsistentData();
    SavedgamesMgr(void);
    ~SavedgamesMgr(void);
};