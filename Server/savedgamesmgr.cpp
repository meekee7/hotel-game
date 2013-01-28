#include "savedgamesmgr.h"
#include "hotel.h"
#include "aux_functions.h"
#include <sstream>
#include <vector>
#include "dlib/string.h"
#include "dlib/threads.h"
#include <boost/format.hpp>

void KeepAliveWrapper(void* sgm)
{
    ((SavedgamesMgr*) sgm)->KeepAlive();
}

SavedgamesMgr::SavedgamesMgr(void)
{
    ifstream config_file ("DBdata.txt");
    string line, host, username, password, dbname;
    int port = 0;
    this->keep_alive_interval = -1;
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
        if (elements[0] == "keepalive")
        {
            this->keep_alive_interval = atoi(elements[1].c_str());
            continue;
        }
    }
    config_file.close();
    if (host == "" || port == 0 || username == "" || password == "" || dbname == "" || this->keep_alive_interval == -1)
        wcout << "Error reading connection data, check DBdata.txt" << endl;
    else
    {
        this->db = new MySQLMgr();
        if (this->db->Connect(host, port, username, password, dbname))
        {
            dlib::create_new_thread(KeepAliveWrapper, (void*) this);
            this->db_loaded_ok = true;
        }
        else
            this->db_loaded_ok = false;
    }
}

void SavedgamesMgr::KeepAlive()
{
    int milliseconds = this->keep_alive_interval * 1000;
    this->keeping_alive = true;
    int interval = 1000;
    int i = interval;
    while (this->keeping_alive)
    {
        while (i <= milliseconds)
        {
            if (!this->keeping_alive)
                return;
            #ifdef _WIN32
                Sleep(interval);
            #else
                usleep(interval * 1000);
            #endif
            i += interval;
        }
        i = interval;
        this->db->KeepAlive();
    }
}

bool SavedgamesMgr::SaveGame(Game* game)
{
    if (!this->db_loaded_ok)
    {
        wcout << "Cannot save game because DB was not loaded ok" << endl;
        return false;
    }
    // Player status
    list<Player*>::iterator i;
    vector<int> bd_player_id_list;
    vector<int> bd_hotel_id_list;
    Player* p;
    for (i = game->active_plist.begin() ; i != game->active_plist.end() ; i++)
    {
        p = (*i);
        wstring hotel_list = L"";
        if (p->hotels.size() > 0)
        {
            list<Hotel*>::iterator j;
            int idx = 1;
            hotel_list += L"";
            for (j = p->hotels.begin() ; j != p->hotels.end() ; j++)
            {
                hotel_list += (*j)->name_txt;
                if (idx < (int)p->hotels.size())
                    hotel_list += L"@";
                idx++;
            }
        }
        string query = str(boost::format("INSERT INTO estado_jugador VALUES (NULL,\"%s\",%d,%d,%d,%d,%d,%d,%d,\"%s\");")
            % utf16_to_utf8(p->name) % p->position->number % p->paid_last_turn % p->n_50 % p->n_100 % p->n_500
            % p->n_1000 % p->n_5000 % utf16_to_utf8(hotel_list));
        if (this->db->ExecuteQueryWithoutData(query) <= 0)
        {
            wcout << "Error inserting player data" << endl;
            return false;
        }
        else
        {
            int bd_id = this->db->GetLastInsertId();
            wcout << "Player data inserted successfully with id " << bd_id << endl;
            bd_player_id_list.push_back(bd_id);
        }
    }
    // Hotel status
    list<Hotel*>::iterator k;
    Hotel* h;
    for (k = game->hlist.begin() ; k != game->hlist.end() ; k++)
    {
        h = (*k);
        string entrance_list = "";
        if (h->entrances.size() > 0)
        {
            list<int>::iterator l;
            int idx = 1;
            for (l = h->entrances.begin() ; l != h->entrances.end() ; l++)
            {
                entrance_list += (*l);
                if (idx < (int)h->entrances.size())
                    entrance_list += "@";
                idx++;
            }
        }
        string query = str(boost::format("INSERT INTO estado_hotel VALUES (NULL,\"%s\",%d,%d,%d,\"%s\");")
            % utf16_to_utf8(h->name_txt) % h->n_built_phases % h->entrance_bought_last_turn % h->ground_bought % entrance_list);
        if (this->db->ExecuteQueryWithoutData(query) <= 0)
        {
            wcout << "Error inserting hotel data, rolling back..." << endl;
            // Rollback
            vector<int>::iterator a;
            for (a = bd_player_id_list.begin() ; a != bd_player_id_list.end() ; a++)
            {
                query = str(boost::format("DELETE FROM estado_jugador WHERE id = %d;") % (*a));
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                    wcout << "Error deleting player data with id " << (*a) << endl;
                else
                    wcout << "Deleted player data with id " << (*a) << endl;
            }
            for (a = bd_hotel_id_list.begin() ; a != bd_hotel_id_list.end() ; a++)
            {
                query = str(boost::format("DELETE FROM estado_hotel WHERE id = %d;") % (*a));
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                    wcout << "Error deleting hotel data with id " << (*a) << endl;
                else
                    wcout << "Deleted hotel data with id " << (*a) << endl;
            }
            return false;
        }
        else
        {
            int bd_id = this->db->GetLastInsertId();
            wcout << "Hotel data inserted successfully with id " << bd_id << endl;
            bd_hotel_id_list.push_back(bd_id);
        }
    }
    // Game data
    string query = str(boost::format("INSERT INTO partida VALUES (NULL,\"%s\",\"%s\",NOW(),%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%s,%s);")
        % utf16_to_utf8(game->password) % utf16_to_utf8(game->name) % game->turn_count % game->n_players % game->active_plist.size() % game->starting_player
        % game->current_player->num % game->last_dice_res % game->last_auto_advance
        % bd_hotel_id_list[0] % bd_hotel_id_list[1] % bd_hotel_id_list[2] % bd_hotel_id_list[3] % bd_hotel_id_list[4] % bd_hotel_id_list[5] % bd_hotel_id_list[6] % bd_hotel_id_list[7]
        % bd_player_id_list[0] % bd_player_id_list[1] % (bd_player_id_list.size() > 2 ? str(boost::format("%d") % bd_player_id_list[2]) : "NULL")
        % (bd_player_id_list.size() > 3 ? str(boost::format("%d") % bd_player_id_list[2]) : "NULL"));
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
    {
        wcout << "Error inserting game data, rolling back..." << endl;
        // Rollback
        vector<int>::iterator a;
        for (a = bd_player_id_list.begin() ; a != bd_player_id_list.end() ; a++)
        {
            query = str(boost::format("DELETE FROM estado_jugador WHERE id = %d;") % (*a));
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
                wcout << "Error deleting player data with id " << (*a) << endl;
            else
                wcout << "Deleted player data with id " << (*a) << endl;
        }
        for (a = bd_hotel_id_list.begin() ; a != bd_hotel_id_list.end() ; a++)
        {
            query = str(boost::format("DELETE FROM estado_hotel WHERE id = %d;") % (*a));
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
                wcout << "Error deleting hotel data with id " << (*a) << endl;
            else
                wcout << "Deleted hotel data with id " << (*a) << endl;
        }
        return false;
    }
    else
    {
        int bd_id = this->db->GetLastInsertId();
        wcout << "Game data inserted successfully with id " << bd_id << endl;
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
    this->keeping_alive = false;
    wcout << "hey1" << endl;
    this->db->Disconnect();
    delete this->db;
}
