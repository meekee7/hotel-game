#pragma once
#include "MySQLMgr.h"
#include "Game.h"
#include "Player.h"
#include "Random.h"

class SavedgamesMgr
{
public:
    dlib::mutex saving_mutex;
    MySQLMgr* db;
    bool db_loaded_ok;
    int keep_alive_interval;
    bool keeping_alive;
    void KeepAlive();
    bool SaveGame(Game* game, Player* creator);
    Game* LoadGame(int id, wstring password, Player* creator, dlib::mutex* mutex_ids, int* id_count, CRandomMT* random_gen, int* error_code);
    void CleanInconsistentData();
    void RollBack(Game* game);
    SavedgamesMgr(void);
    ~SavedgamesMgr(void);
};