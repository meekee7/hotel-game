#include "savedgamesmgr.h"
#include "hotel.h"
#include "aux_functions.h"
#include <fstream>
#include <vector>
#include "dlib/string.h"

SavedgamesMgr::SavedgamesMgr(void)
{
    ifstream config_file ("DBdata.txt");
    string line, host, username, password, dbname;
    int port = 0;
    while (getline(config_file, line))
    {
        vector<string> elements = dlib::split(line, "=");
        if (elements.size() != 2)
            continue;
        if (elements[0] == "host")
        {
            host = elements[1];
            continue;
        }
        if (elements[0] == "port")
        {
            port = atoi(elements[1].c_str());
            continue;
        }
        if (elements[0] == "username")
        {
            username = elements[1];
            continue;
        }
        if (elements[0] == "password")
        {
            password = elements[1];
            continue;
        }
        if (elements[0] == "dbname")
        {
            dbname = elements[1];
            continue;
        }
    }
    config_file.close();
    if (host == "" || port == 0 || username == "" || password == "" || dbname == "")
        wcout << "Error reading connection data, check DBnames.txt" << endl;
    else
    {
        this->db = new MySQLMgr();
        if (this->db->Connect(host, port, username, password, dbname))
            this->db_loaded_ok = true;
        else
            this->db_loaded_ok = false;
    }
}

bool SavedgamesMgr::SaveGame(Game* game)
{
    list<Player*>::iterator i;
    Player* p;
    wstringstream query;
    for (i = game->active_plist.begin() ; i != game->active_plist.end() ; i++)
    {
        p = (*i);
        query.clear();
        query.str(L"");
        wstring color;
        switch (p->color)
        {
            case red: color = L"rojo";
                break;
            case yellow: color = L"amarillo";
                break;
            case blue: color = L"azul";
                break;
            case green: color = L"verde";
                break;
        }
        query << "INSERT INTO estado_jugador VALUES (NULL,\"" << p->name << "\"," << p->position->number << ",\"" << color << "\"," << p->paid_last_turn << ",";
        query << p->n_50 << "," << p->n_100 << "," << p->n_500 << "," << p->n_1000 << "," << p->n_5000 << ",";
        wstring hotel_list;
        if (p->hotels.size() > 0)
        {
            list<Hotel*>::iterator j;
            int idx = 1;
            hotel_list += L"\"";
            for (j = p->hotels.begin() ; j != p->hotels.end() ; j++)
            {
                hotel_list += (*j)->name_txt;
                if (idx < (int)p->hotels.size())
                    hotel_list += L"@";
                idx++;
            }
            hotel_list += L"\"";
        }
        else
            hotel_list = L"\"\"";
        query << hotel_list << ");";
        if (this->db->ExecuteQueryWithOutData(utf16_to_utf8(query.str())) == 0)
        {
            wcout << "Error inserting player data" << endl;
            return false;
        }
        else
            wcout << "Player data inserted successfully with id " << this->db->GetLastInsertId() << endl;
    }
    return true;
}

    /*MySQLResult* res = mgr->ExecuteQueryWithData("SELECT * FROM partida");
    if (res->fetch_row())
    {
        int id = res->get_int_field("id");
        string nombre = res->get_string_field("nombre");
        wcout << id << " " << nombre.c_str() << endl;
    }
    else
        wcout << "No data" << endl;*/

SavedgamesMgr::~SavedgamesMgr(void)
{
}
