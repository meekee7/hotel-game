#include "savedgamesmgr.h"

SavedgamesMgr::SavedgamesMgr(void)
{
    MySQLMgr* mgr = new MySQLMgr();
    mgr->Connect("betovserver.no-ip.org", 3306, "hotel", "hotel", "hotel");
    MySQLResult* res = mgr->ExecuteQueryWithData("SELECT * FROM partida");
    res->fetch_row();
    int id = res->get_int_field("id");
    string nombre = res->get_string_field("nombre");
    wcout << id << " " << nombre.c_str() << endl;
}

SavedgamesMgr::~SavedgamesMgr(void)
{
}
