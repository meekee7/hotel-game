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
    {
        this->db_loaded_ok = false;
        this->db = NULL;
        wcout << "Error reading connection data, check DBdata.txt" << endl;
    }
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
    wcout << "Deleted " << this->db->ExecuteQueryWithoutData("DELETE FROM partida WHERE id NOT IN (SELECT id_partida FROM estado_hotel) OR id NOT IN (SELECT id_partida FROM estado_jugador);") << " saved games with incomplete data" << endl;
    wcout << "Deleted " << this->db->ExecuteQueryWithoutData("DELETE FROM estado_hotel WHERE id_partida IS NULL OR id_partida NOT IN (SELECT id FROM partida);") << " hotel statuses" << endl;
    wcout << "Deleted " << this->db->ExecuteQueryWithoutData("DELETE FROM estado_jugador WHERE id_partida IS NULL OR id_partida NOT IN (SELECT id FROM partida);") << " player statuses" << endl;
    wcout << "Cleanup completed" << endl;
}

void SavedgamesMgr::RollBack(Game* game)
{
    string query = str(boost::format("DELETE FROM estado_jugador WHERE id_partida = %d;") % game->bd_id);
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
        wcout << "Error deleting player data of game with id " << game->bd_id << endl;
    else
        wcout << "Deleted player data of game with id " << game->bd_id << endl;
    query = str(boost::format("DELETE FROM estado_hotel WHERE id_partida = %d;") % game->bd_id);
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
        wcout << "Error deleting hotel data of game with id " << game->bd_id << endl;
    else
        wcout << "Deleted hotel data of game with id " << game->bd_id << endl;
    query = str(boost::format("DELETE FROM partida WHERE id = %d;") % game->bd_id);
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
        wcout << "Error deleting game with id " << game->bd_id << endl;
    else
        wcout << "Deleted game with id " << game->bd_id << endl;
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

bool SavedgamesMgr::SaveGame(Game* game, Player* creator)
{
    if (!this->db_loaded_ok)
    {
        wcout << "Cannot save game because DB was not loaded ok" << endl;
        return false;
    }
    this->saving_mutex.lock();
    bool overwriting;
    if (game->bd_id > -1)
        overwriting = true;
    else
        overwriting = false;
    // Game data
    string query = str(boost::format("REPLACE INTO partida VALUES (%d,\"%s\",\"%s\",\"%s\",FROM_UNIXTIME(%d),NOW(),%d,%d,%d,%d,%d,%d,%d,%d);")
        % (overwriting == true ? str(boost::format("%d") % game->bd_id) : "NULL")
        % utf16_to_utf8(game->password) % utf16_to_utf8(game->name) % utf16_to_utf8(creator->name) % game->creation_date % (game->ended ? 1 : 0) % game->turn_count % game->n_players % game->active_plist.size()
        % game->starting_player % game->current_player->GetState(game->id)->num % game->last_dice_res % game->last_auto_advance);
    if (this->db->ExecuteQueryWithoutData(query) <= 0)
    {
        wcout << "Error inserting game data, reconnecting to try again..." << endl;
        if (this->db->ReConnectWithLastUsedValues())
        {
            if (this->db->ExecuteQueryWithoutData(query) <= 0)
            {
                wcout << "Error inserting game data" << endl;
                return false;
            }
        }
        else
        {
            wcout << "Error inserting game data" << endl;
            return false;
        }
    }
    if (overwriting == false) // New game
    {
        game->bd_id = this->db->GetLastInsertId();
        if (game->bd_id < 0)
        {
            if (this->db->ReConnectWithLastUsedValues())
                game->bd_id = this->db->GetLastInsertId();
            else
            {
                wcout << "Error getting last insert id" << endl;
                this->RollBack(game);
                return false;
            }
        }
    }
    // Player statuses
    // Remove game id from player statuses in order to detect which players are no longer in the game. Later in the save process, the rows with NULL id_partida will be deleted
    query = str(boost::format("UPDATE estado_jugador SET id_partida = NULL WHERE id_partida = %d;") % game->bd_id);
    if (this->db->ExecuteQueryWithoutData(query) < 0)
    {
        wcout << "Error inserting player data, reconnecting to try again..." << endl;
        if (this->db->ReConnectWithLastUsedValues())
        {
            if (this->db->ExecuteQueryWithoutData(query) < 0)
            {
                wcout << "Error inserting player data, rolling back..." << endl;
                this->RollBack(game);
                return false;
            }
        }
    }
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
        query = str(boost::format("REPLACE INTO estado_jugador VALUES (%s,%s,\"%s\",%d,%d,%d,%d,%d,%d,%d,%d,\"%s\");")
            % (overwriting == true ? str(boost::format("%d") % p->GetState(game->id)->bd_id) : "NULL") % game->bd_id % utf16_to_utf8(p->name) % player_state->num
            % player_state->position->number % player_state->paid_last_turn % player_state->n_50 % player_state->n_100 % player_state->n_500
            % player_state->n_1000 % player_state->n_5000 % utf16_to_utf8(hotel_list));
        if (this->db->ExecuteQueryWithoutData(query) <= 0)
        {
            wcout << "Error inserting player data, reconnecting to try again..." << endl;
            if (this->db->ReConnectWithLastUsedValues())
            {
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                {
                    wcout << "Error inserting player data, rolling back..." << endl;
                    this->RollBack(game);
                    return false;
                }
            }
            else
            {
                wcout << "Error inserting player data, rolling back..." << endl;
                this->RollBack(game);
                return false;
            }
        }
        if (overwriting == false)
        {
            p->GetState(game->id)->bd_id = this->db->GetLastInsertId();
            if (p->GetState(game->id)->bd_id < 0)
            {
                if (this->db->ReConnectWithLastUsedValues())
                {
                    p->GetState(game->id)->bd_id = this->db->GetLastInsertId();
                }
                else
                {
                    wcout << "Error getting last insert id" << endl;
                    return false;
                }
            }
            wcout << "Player data inserted successfully with id " << p->GetState(game->id)->bd_id << endl;
        }
        else
        {
            wcout << "Player data with id " << p->GetState(game->id)->bd_id << " updated successfully" << endl;
        }
    }
    // Hotel status
    Hotel* h;
    for (vector<Hotel*>::iterator k = game->hlist.begin() ; k != game->hlist.end() ; k++)
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
        string query = str(boost::format("REPLACE INTO estado_hotel VALUES (%d,%s,\"%s\",%d,%d,%d,\"%s\");")
            % (overwriting == true ? str(boost::format("%d") % h->bd_id) : "NULL") % game->bd_id % utf16_to_utf8(h->name_txt)
            % h->n_built_phases % h->entrance_bought_last_turn % h->ground_bought % entrance_list);
        if (this->db->ExecuteQueryWithoutData(query) <= 0)
        {
            wcout << "Error inserting hotel data, reconnecting to try again..." << endl;
            if (this->db->ReConnectWithLastUsedValues())
            {
                if (this->db->ExecuteQueryWithoutData(query) <= 0)
                {
                    wcout << "Error inserting hotel data, rolling back..." << endl;
                    this->RollBack(game);
                    return false;
                }
            }
            else
            {
                wcout << "Error inserting hotel data, rolling back..." << endl;
                this->RollBack(game);
                return false;
            }
        }
        if (overwriting == false)
        {
            h->bd_id = this->db->GetLastInsertId();
            if (h->bd_id < 0)
            {
                if (this->db->ReConnectWithLastUsedValues())
                {
                    h->bd_id = this->db->GetLastInsertId();
                }
                else
                {
                    wcout << "Error getting last insert id" << endl;
                    return false;
                }
            }
            wcout << "Hotel data inserted successfully with id " << h->bd_id << endl;
        }
        else
        {
            wcout << "Hotel data with id " << h->bd_id << " updated successfully" << endl;
        }
    }
    if (overwriting == false)
    {
        wcout << "Game data inserted successfully with id " << game->bd_id << endl;
    }
    else
    {
        // Delete player statuses no longer associated with the game
        this->db->ExecuteQueryWithoutData("DELETE FROM estado_jugador WHERE id_partida IS NULL;");
        wcout << "Game data with id " << game->bd_id << " updated successfully" << endl;
    }
    game->n_players_last_save = game->active_plist.size();
    game->saved_current_player = game->current_player->GetState(game->id)->num;
    this->saving_mutex.unlock();
    return true;
}

Game* SavedgamesMgr::LoadGame(int id, wstring password, Player* creator, dlib::mutex* mutex_ids, int* id_count, CRandomMT* random_gen, int* error_code)
{
    if (!this->db_loaded_ok)
    {
        wcout << "Cannot load game because DB was not loaded ok" << endl;
        (*error_code) = 3; // DB not loaded ok
        return NULL;
    }
    MySQLResult* res = this->db->ExecuteQueryWithData(str(boost::format("SELECT *,UNIX_TIMESTAMP(fecha_creacion) as fc FROM partida WHERE id = %d;") % id));
    if (res->fetch_row())
    {
        if (res->get_string_field("password") != utf16_to_utf8(password))
        {
            (*error_code) = 4; // Invalid password
            delete res;
            return NULL;
        }
        if (res->get_bool_field("finalizada"))
        {
            (*error_code) = 5; // Game already ended
            delete res;
            return NULL;
        }
        if (creator->name != utf8_to_utf16(res->get_string_field("nombre_creador")))
        {
            (*error_code) = 6; // The player who tries to load the game is not the creator
            delete res;
            return NULL;
        }
        MySQLResult* res_hotel_statuses = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM estado_hotel WHERE id_partida = %d ORDER BY id ASC;") % id));
        MySQLResult* res_creator_status = this->db->ExecuteQueryWithData(str(
            boost::format("SELECT ej.* FROM estado_jugador as ej, partida as p WHERE ej.id_partida = %d AND p.id = ej.id_partida AND p.nombre_creador = \"%s\" AND ej.nombre = p.nombre_creador;")
            % id % utf16_to_utf8(creator->name)));
        if (res_creator_status == NULL || !res_creator_status->fetch_row())
        {
            (*error_code) = 6; // The player who tries to load the game is not the creator
            delete res_hotel_statuses;
            delete res_creator_status;
            delete res;
            return NULL;
        }
        Game* game = new Game(utf8_to_utf16(res->get_string_field("nombre")), res->get_int_field("num_jugadores_activos"), creator, mutex_ids, id_count, random_gen, true);
        game->bd_id = id;
        game->creation_date = res->get_int_field("fc");
        game->last_auto_advance = res->get_int_field("ultimo_avance_auto");
        game->last_dice_res = res->get_int_field("ultimo_res_dado");
        game->password = password;
        game->starting_player = res->get_int_field("jugador_inicial");
        game->saved_current_player = res->get_int_field("jugador_actual"); // When the loaded game is started, it will be used to determine the current player
        game->turn_count = res->get_int_field("num_turnos");
        // Fill hotel statuses
        int i = 0;
        while (res_hotel_statuses->fetch_row())
        {
            // Hotels are selected in the same order than they are created in a game
            game->hlist[i]->bd_id = res_hotel_statuses->get_int_field("id");
            game->hlist[i]->n_built_phases = res_hotel_statuses->get_int_field("num_fases_construidas");
            game->hlist[i]->entrance_bought_last_turn = res_hotel_statuses->get_bool_field("entrada_comprada_ultimo_turno");
            game->hlist[i]->ground_bought = res_hotel_statuses->get_bool_field("suelo_comprado");
            vector<string> entrances = dlib::split(res_hotel_statuses->get_string_field("posiciones_de_entradas"), "@");
            for (int j = 0 ; j < (int)entrances.size() ; j++)
                game->hlist[i]->entrances.push_back(atoi(entrances[j].c_str()));
            game->hlist[i]->n_entrances = (int)entrances.size();
            i++;
        }
        if (game->hlist.size() < 8)
        {
            (*error_code) = 7; // Errors in game data
            delete res_hotel_statuses;
            delete res_creator_status;
            delete res;
            delete game;
            return NULL;
        }
        PlayerGameState* creator_status = creator->GetState(game->id);
        creator_status->bd_id = res_creator_status->get_int_field("id");
        creator_status->position = game->positions[res_creator_status->get_int_field("posicion")];
        creator_status->paid_last_turn = res_creator_status->get_bool_field("pago_ultimo_turno");
        creator_status->n_5000 = res_creator_status->get_int_field("n_billetes_5000");
        creator_status->n_1000 = res_creator_status->get_int_field("n_billetes_1000");
        creator_status->n_500 = res_creator_status->get_int_field("n_billetes_500");
        creator_status->n_100 = res_creator_status->get_int_field("n_billetes_100");
        creator_status->n_50 = res_creator_status->get_int_field("n_billetes_50");
        vector<string> hotels = dlib::split(res_creator_status->get_string_field("hoteles_poseidos"), "@");
        for (int i = 0 ; i < (int)hotels.size() ; i++)
        {
            creator_status->hotels.push_back(get_hotel_from_name(utf8_to_utf16(hotels[i]), game));
        }
        delete res_hotel_statuses;
        delete res_creator_status;
        delete res;
        return game;
    }
    else
    {
        wcout << "Error retrieving game from DB" << endl;
        (*error_code) = 2; // Game does not exists
        return NULL;
    }
}

int SavedgamesMgr::LoadPlayerData(Game* game, Player* player)
{
    if (!this->db_loaded_ok)
    {
        wcout << "Cannot load player data because DB was not loaded ok" << endl;
        return 1; // DB not loaded ok
    }
    MySQLResult* res = this->db->ExecuteQueryWithData(str(boost::format("SELECT * FROM estado_jugador WHERE id_partida = %d AND nombre = \"%s\";") % game->bd_id % utf16_to_utf8(player->name)));
    if (!res->fetch_row())
    {
        wcout << utf8_to_utf16(str(boost::format("Player '%s' does not belong the game %d") % utf16_to_utf8(player->name) % game->id)) << endl;
        return 2; // Player does not exists
    }
    player->Join_Game(game->id);
    PlayerGameState* state = player->GetState(game->id);
    state->bd_id = res->get_int_field("id");
    state->num = res->get_int_field("numero");
    state->position = game->positions[res->get_int_field("posicion")];
    state->paid_last_turn = res->get_bool_field("pago_ultimo_turno");
    state->n_5000 = res->get_int_field("n_billetes_5000");
    state->n_1000 = res->get_int_field("n_billetes_1000");
    state->n_500 = res->get_int_field("n_billetes_500");
    state->n_100 = res->get_int_field("n_billetes_100");
    state->n_50 = res->get_int_field("n_billetes_50");
    vector<string> hotels = dlib::split(res->get_string_field("hoteles_poseidos"), "@");
    for (int i = 0 ; i < (int)hotels.size() ; i++)
    {
        state->hotels.push_back(get_hotel_from_name(utf8_to_utf16(hotels[i]), game));
    }
    insert_and_sort(&game->plist, player, game->id);
    insert_and_sort(&game->active_plist, player, game->id);
    return 0;
}

SavedgamesMgr::~SavedgamesMgr(void)
{
    this->keeping_alive = false;
    #ifdef _WIN32
        Sleep(1000);
    #else
        usleep(1000 * 1000);
    #endif
    if (this->db != NULL)
    {
        if (this->db_loaded_ok)
            this->db->Disconnect();
        delete this->db;
    }
}
