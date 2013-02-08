#include "SavedgamesMgr.h"
#include "Hotel.h"
#include "Aux_Functions.h"
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
            this->CleanInconsistentData();
            dlib::create_new_thread(KeepAliveWrapper, (void*) this);
            this->db_loaded_ok = true;
        }
        else
            this->db_loaded_ok = false;
    }
}

void SavedgamesMgr::CleanInconsistentData()
{
    wcout << "Inconsistent data cleanup in progress" << endl;
    wcout << "Deleted " << this->db->ExecuteQueryWithoutData("DELETE FROM partida WHERE estado_Fujiyama IS NULL OR estado_Boomerang IS NULL OR estado_Letoile IS NULL OR estado_President IS NULL OR estado_Royal IS NULL OR estado_Waikiki IS NULL OR estado_TajMahal IS NULL OR estado_Safari IS NULL OR estado_j1 IS NULL OR estado_j2 IS NULL;") << " saved games with incomplete data" << endl;
    wcout << "Deleted " << this->db->ExecuteQueryWithoutData("DELETE FROM estado_hotel WHERE id_partida IS NULL OR id_partida NOT IN (SELECT id FROM partida);") << " hotel statuses" << endl;
    wcout << "Deleted " << this->db->ExecuteQueryWithoutData("DELETE FROM estado_jugador WHERE id_partida IS NULL OR id_partida NOT IN (SELECT id FROM partida);") << " player statuses" << endl;
    wcout << "Cleanup completed" << endl;
}

void SavedgamesMgr::KeepAlive()
{
    int milliseconds = this->keep_alive_interval * 1000;
    this->keeping_alive = true;
    int interval = 200;
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
    bool overwriting;
    if (game->bd_id > -1)
        overwriting = true;
    else
        overwriting = false;
    // Player statuses
    vector<int> bd_player_id_list;
    vector<int> bd_hotel_id_list;
    Player* p;
    for (list<Player*>::iterator i = game->active_plist.begin() ; i != game->active_plist.end() ; i++)
    {
        p = (*i);
        wstring hotel_list = L"";
        PlayerGameState* player_state = p->GetState(game->id);
        if (player_state->hotels.size() > 0)
        {
            int idx = 1;
            hotel_list += L"";
            for (list<Hotel*>::iterator j = player_state->hotels.begin() ; j != player_state->hotels.end() ; j++)
            {
                hotel_list += (*j)->name_txt;
                if (idx < (int)player_state->hotels.size())
                    hotel_list += L"@";
                idx++;
            }
        }
        string query = str(boost::format("REPLACE INTO estado_jugador VALUES (%s,NULL,\"%s\",%d,%d,%d,%d,%d,%d,%d,\"%s\");")
            % (overwriting == true ? str(boost::format("%d") % p->GetState(game->id)->bd_id) : "NULL")
            % utf16_to_utf8(p->name) % player_state->position->number % player_state->paid_last_turn % player_state->n_50 % player_state->n_100 % player_state->n_500
            % player_state->n_1000 % player_state->n_5000 % utf16_to_utf8(hotel_list));
        if (this->db->ExecuteQueryWithoutData(query) <= 0)
        {
            wcout << "Error inserting player data, reconnecting to try again..." << endl;
            if (this->db->ReConnectWithLastUsedValues())
            {
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                {
                    wcout << "Error inserting player data" << endl;
                    return false;
                }
            }
            else
            {
                wcout << "Error inserting player data" << endl;
                return false;
            }
        }
        int bd_id;
        if (overwriting == false)
        {
            bd_id = this->db->GetLastInsertId();
            if (bd_id < 0)
            {
                if (this->db->ReConnectWithLastUsedValues())
                {
                    bd_id = this->db->GetLastInsertId();
                }
                else
                {
                    wcout << "Error getting last insert id" << endl;
                    return false;
                }
            }
            wcout << "Player data inserted successfully with id " << bd_id << endl;
            p->GetState(game->id)->bd_id = bd_id;
        }
        else
        {
            bd_id = p->GetState(game->id)->bd_id;
            wcout << "Player data with id " << bd_id << " updated successfully" << endl;
        }
        bd_player_id_list.push_back(bd_id);
    }
    // Hotel status
    Hotel* h;
    for (list<Hotel*>::iterator k = game->hlist.begin() ; k != game->hlist.end() ; k++)
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
        string query = str(boost::format("REPLACE INTO estado_hotel VALUES (%d,NULL,\"%s\",%d,%d,%d,\"%s\");")
            % (overwriting == true ? str(boost::format("%d") % h->bd_id) : "NULL")
            % utf16_to_utf8(h->name_txt) % h->n_built_phases % h->entrance_bought_last_turn % h->ground_bought % entrance_list);
        if (this->db->ExecuteQueryWithoutData(query) <= 0)
        {
            wcout << "Error inserting hotel data, reconnecting to try again..." << endl;
            if (this->db->ReConnectWithLastUsedValues())
            {
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
            }
            else
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
        }
        int bd_id;
        if (overwriting == false)
        {
            bd_id = this->db->GetLastInsertId();
            if (bd_id < 0)
            {
                if (this->db->ReConnectWithLastUsedValues())
                {
                    bd_id = this->db->GetLastInsertId();
                }
                else
                {
                    wcout << "Error getting last insert id" << endl;
                    return false;
                }
            }
            wcout << "Hotel data inserted successfully with id " << bd_id << endl;
            h->bd_id = bd_id;
        }
        else
        {
            bd_id = h->bd_id;
            wcout << "Hotel data with id " << bd_id << " updated successfully" << endl;
        }
        bd_hotel_id_list.push_back(bd_id);
    }
    // Game data
    string query = str(boost::format("REPLACE INTO partida VALUES (%d,\"%s\",\"%s\",FROM_UNIXTIME(%l),NOW(),%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%s,%s);")
        % (overwriting == true ? str(boost::format("%d") % game->bd_id) : "NULL")
        % utf16_to_utf8(game->password) % utf16_to_utf8(game->name) % game->creation_date % game->turn_count % game->n_players % game->active_plist.size() % game->starting_player
        % game->current_player->GetState(game->id)->num % game->last_dice_res % game->last_auto_advance
        % bd_hotel_id_list[0] % bd_hotel_id_list[1] % bd_hotel_id_list[2] % bd_hotel_id_list[3] % bd_hotel_id_list[4] % bd_hotel_id_list[5] % bd_hotel_id_list[6] % bd_hotel_id_list[7]
        % bd_player_id_list[0] % bd_player_id_list[1] % (bd_player_id_list.size() > 2 ? str(boost::format("%d") % bd_player_id_list[2]) : "NULL")
        % (bd_player_id_list.size() > 3 ? str(boost::format("%d") % bd_player_id_list[2]) : "NULL"));
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
    {
        wcout << "Error inserting game data, reconnecting to try again..." << endl;
        if (this->db->ReConnectWithLastUsedValues())
        {
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
        }
        else
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
    }
    int bd_id;
    if (overwriting == false)
    {
        bd_id = this->db->GetLastInsertId();
        if (bd_id < 0)
        {
            if (this->db->ReConnectWithLastUsedValues())
                bd_id = this->db->GetLastInsertId();
            else
            {
                wcout << "Error getting last insert id" << endl;
                return false;
            }
        }
    }
    else
        bd_id = game->bd_id;
    // Associate all statuses with game ID
    query = str(boost::format("UPDATE estado_hotel SET id_partida = %d WHERE id IN (%d, %d, %d, %d, %d, %d, %d, %d);") % bd_id % bd_hotel_id_list[0] % bd_hotel_id_list[1]
        % bd_hotel_id_list[2] % bd_hotel_id_list[3] % bd_hotel_id_list[4] % bd_hotel_id_list[5] % bd_hotel_id_list[6] % bd_hotel_id_list[7]);
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
    {
        wcout << "Error associating hotels with game data, reconnecting to try again..." << endl;
        if (this->db->ReConnectWithLastUsedValues())
        {
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
            {
                wcout << "Error associating hotels with game data, rolling back..." << endl;
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
                query = str(boost::format("DELETE FROM partida WHERE id = %d;") % bd_id);
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                    wcout << "Error deleting game data with id " << bd_id << endl;
                else
                    wcout << "Deleted game data with id " << bd_id << endl;
                return false;
            }
        }
        else
        {
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
            query = str(boost::format("DELETE FROM partida WHERE id = %d;") % bd_id);
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
                wcout << "Error deleting game data with id " << bd_id << endl;
            else
                wcout << "Deleted game data with id " << bd_id << endl;
            return false;
        }
    }
    if (bd_player_id_list.size() == 2)
        query = str(boost::format("UPDATE estado_jugador SET id_partida = %d WHERE id IN (%d, %d);") % bd_id % bd_player_id_list[0] % bd_player_id_list[1]);
    else if (bd_player_id_list.size() == 3)
    {
        query = str(boost::format("UPDATE estado_jugador SET id_partida = %d WHERE id IN (%d, %d, %d);") % bd_id % bd_player_id_list[0] % bd_player_id_list[1]
        % bd_player_id_list[2]);
    }
    else
    {
        query = str(boost::format("UPDATE estado_jugador SET id_partida = %d WHERE id IN (%d, %d, %d, %d);") % bd_id % bd_player_id_list[0] % bd_player_id_list[1]
        % bd_player_id_list[2] % bd_player_id_list[3]);
    }
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
    {
        wcout << "Error associating players with game data, reconnecting to try again..." << endl;
        if (this->db->ReConnectWithLastUsedValues())
        {
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
            {
                wcout << "Error associating players with game data, rolling back..." << endl;
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
                query = str(boost::format("DELETE FROM partida WHERE id = %d;") % bd_id);
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                    wcout << "Error deleting game data with id " << bd_id << endl;
                else
                    wcout << "Deleted game data with id " << bd_id << endl;
                return false;
            }
        }
        else
        {
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
            query = str(boost::format("DELETE FROM partida WHERE id = %d;") % bd_id);
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
                wcout << "Error deleting game data with id " << bd_id << endl;
            else
                wcout << "Deleted game data with id " << bd_id << endl;
            return false;
        }
    }
    if (overwriting)
        wcout << "Game data with id " << bd_id << " updated successfully " << endl;
    else
    {
        wcout << "Game data inserted successfully with id " << bd_id << endl;
        game->bd_id = bd_id;
        game->n_players_last_save = game->active_plist.size();
    }
    return true;
}

Game* SavedgamesMgr::LoadGame(int id, wstring password, Player* creator, dlib::mutex* mutex_ids, int* id_count, CRandomMT* random_gen, int* error_code)
{
    if (!this->db_loaded_ok)
    {
        wcout << "Cannot load game because DB was not loaded ok" << endl;
        (*error_code) = 1;
        return NULL;
    }
    /*list<Player*>::iterator i;
    vector<int> bd_player_id_list;
    vector<int> bd_hotel_id_list;*/
    MySQLResult* res = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM partida WHERE id = %d;") % id));
    if (res->fetch_row())
    {
        if (res->get_string_field("password") != utf16_to_utf8(password))
        {
            (*error_code) = 2;
            return NULL;
        }
        wstring name = utf8_to_utf16(res->get_string_field("nombre"));
        int n_players = res->get_int_field("num_jugadores");
        int n_active_players = res->get_int_field("num_jugadores_activos");
        int n_turns = res->get_int_field("num_turnos");
        int starting_player = res->get_int_field("jugador_inicial");
        int current_player = res->get_int_field("jugador_actual");
        int last_dice_res = res->get_int_field("ultimo_res_dado");
        int last_auto_advance = res->get_int_field("ultimo_avance_auto");
        int Fujiyama_status_id = res->get_int_field("estado_Fujiyama");
        int Boomerang_status_id = res->get_int_field("estado_Boomerang");
        int Letoile_status_id = res->get_int_field("estado_Letoile");
        int President_status_id = res->get_int_field("estado_President");
        int Royal_status_id = res->get_int_field("estado_Royal");
        int Waikiki_status_id = res->get_int_field("estado_Waikiki");
        int TajMahal_status_id = res->get_int_field("estado_TajMahal");
        int Safari_status_id = res->get_int_field("estado_Safari");
        MySQLResult* res_hotel_statuses = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM estado_hotel WHERE id IN (%d, %d, %d, %d, %d, %d, %d, %d) ORDER BY id ASC;")
            % Fujiyama_status_id % Boomerang_status_id % Letoile_status_id % President_status_id % Royal_status_id % Waikiki_status_id % TajMahal_status_id % Safari_status_id));
        int p1_status_id = res->get_int_field("estado_j1");
        int p2_status_id = res->get_int_field("estado_j2");
        MySQLResult* res_player_statuses;
        if (n_players == 2)
            res_player_statuses = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM estado_jugador WHERE id IN (%d, %d) ORDER BY id ASC;") % p1_status_id % p2_status_id));
        else
        {
            if (n_players > 2)
            {
                int p3_status_id = res->get_int_field("estado_j3");
                res_player_statuses = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM estado_jugador WHERE id IN (%d, %d, %d) ORDER BY id ASC;")
                    % p1_status_id % p2_status_id % p3_status_id));
                if (n_players > 3)
                {
                    int p4_status_id = res->get_int_field("estado_j4");
                    res_player_statuses = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM estado_jugador WHERE id IN (%d, %d, %d, %d) ORDER BY id ASC;")
                        % p1_status_id % p2_status_id % p3_status_id));
                }
            }
        }
        Game* game = new Game(name, n_players, creator, mutex_ids, id_count, random_gen);
        return game;
    }
    else
    {
        wcout << "Error retrieving game from DB" << endl;
        return NULL;
    }
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
    #ifdef _WIN32
        Sleep(1000);
    #else
        usleep(1000 * 1000);
    #endif
    this->db->Disconnect();
    delete this->db;
}
