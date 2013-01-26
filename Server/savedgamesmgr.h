#pragma once
#include "mysqlmgr.h"

class SavedgamesMgr
{
public:
    MySQLMgr* db;
    SavedgamesMgr(void);
    ~SavedgamesMgr(void);
};