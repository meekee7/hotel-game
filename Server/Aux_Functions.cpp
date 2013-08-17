#include "Aux_Functions.h"

int get_utf8_length(wstring data)
{
    int res;
#ifdef _WIN32
    res = WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
#else
    res = wcstombs(NULL, data.c_str(), 0);
#endif
    return res;
}

wstring utf8_to_utf16 (string data)
{
    if (data.empty())
        return L"";
    wstring res;
#ifdef _WIN32
    res.resize(MultiByteToWideChar(CP_UTF8, 0, data.data(), data.length(), NULL, 0));
    MultiByteToWideChar(CP_UTF8, 0, data.data(), data.length(), &res[0], res.length());
#else
    res.resize(mbstowcs(NULL, data.c_str(), 0)+1);
    mbstowcs((wchar_t*)&res.data()[0], data.c_str(), res.size());
#endif
    return res;
}

string utf16_to_utf8 (wstring data)
{
    if (data.empty())
        return "";
#ifdef _WIN32
    int size_utf8 = WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
#else
    //setlocale(LC_ALL, "es_ES.utf8");
    int size_utf8 = wcstombs(NULL, data.data(), 0);
#endif
    string data_utf8;
    data_utf8.resize(size_utf8);
#ifdef _WIN32
    WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), &data_utf8[0], data_utf8.length(), NULL, NULL);
#else
    //setlocale(LC_ALL, "es_ES.utf8");
    wcstombs(&data_utf8[0], data.data(), size_utf8);
#endif
    return data_utf8;
}

const wstring currentDateTime()
{
    time_t     now = time(0);
    struct tm  tstruct;
    char       buf[80];
#ifdef _WIN32
    localtime_s(&tstruct, &now);
#else
    tstruct = *localtime(&now);
#endif
    // Visit http://www.cplusplus.com/reference/clibrary/ctime/strftime/
    // for more information about date/time format
    strftime(buf, sizeof(buf), "%Y-%m-%d %X -> ", &tstruct);
    return utf8_to_utf16(buf);
}

Player* get_player_from_name(wstring name, list<Player*>* plist)
{
    bool found = false;
    list<Player*>::iterator i = plist->begin();
    while (!found && i != plist->end())
    {
        if (((*i)->name) == wstring(name))
            found = true;
        else
            ++i;
    }
    if (!found)
        return NULL;
    else
        return (*i);
}

Player* get_player_from_game(wstring name, Game* game)
{
    bool found = false;
    list<Player*>::iterator i = game->plist.begin();
    while (!found && i != game->plist.end())
    {
        if (((*i)->name) == wstring(name))
            found = true;
        else
            ++i;
    }
    if (!found)
        return NULL;
    else
    {
        if (game->is_active(*i)) // To avoid hack: sending commands when retired
            return (*i);
        else
            return NULL;
    }
}

Hotel* get_hotel_from_name(wstring name_txt, Game* game)
{
    bool found = false;
    vector<Hotel*>::iterator i = game->hlist.begin();
    while (!found && i != game->hlist.end())
    {
        if (((*i)->name_txt).compare(wstring(name_txt.data())) == 0)
            found = true;
        else
            ++i;
    }
    if (!found)
    {
        wcout << currentDateTime() << L"Warning: Hotel name " << name_txt << " not found in hotel list of game " << game->name << endl;
        return NULL;
    }
    else
        return (*i);
}

Game* get_game_from_name(wstring name, list<Game*>* glist)
{
    bool found = false;
    list<Game*>::iterator i = glist->begin();
    while (!found && i != glist->end())
    {
        if (((*i)->name) == wstring(name))
            found = true;
        else
            ++i;
    }
    if (!found)
        return NULL;
    else
        return (*i);
}

Game* get_game_from_id(int id, list<Game*>* glist)
{
    bool found = false;
    list<Game*>::iterator i = glist->begin();
    while (!found && i != glist->end())
    {
        if ((*i)->id == id)
            found = true;
        else
            ++i;
    }
    if (!found)
        return NULL;
    else
        return (*i);
}

Game* get_game_from_bd_id(int bd_id, list<Game*>* glist)
{
    bool found = false;
    list<Game*>::iterator i = glist->begin();
    while (!found && i != glist->end())
    {
        if (((*i)->bd_id != -1) && ((*i)->bd_id == bd_id))
            found = true;
        else
            ++i;
    }
    if (!found)
        return NULL;
    else
        return (*i);
}

Chat* get_chat_from_id(int id, list<Chat*>* chat_list, list<Game*>* glist)
{
    bool found = false;
    list<Chat*>::iterator i = chat_list->begin();
    while (!found && i != chat_list->end())
    {
        if ((*i)->id == id)
            found = true;
        else
            ++i;
    }
    if (!found)
    {
        // If not found it should be a game chat
        Game* game = get_game_from_id(id, glist);
        if (game != NULL)
            return game->chat;
        else
            return NULL;
    }
    else
        return (*i);
}

string receive_string (Player* p, int length, int* bytes_received)
{
    char* data = new char[length+1];
    *bytes_received = p->socket->precv(data, length, 0);
    string s_data;
    if (*bytes_received > 0)
    {
        data[length] = '\0';
        s_data = string(data);
    }
    else
        s_data = string("");
    delete data;
    return s_data;
}

wstring receive_wstring (Player* p, int length, int* bytes_received)
{
    char* data = new char[length+1];
    *bytes_received = p->socket->precv(data, length, 0);
    wstring s_data;
    if (*bytes_received > 0)
    {
        data[length] = '\0';
        s_data = utf8_to_utf16(data);
    }
    else
        s_data = wstring(L"");
    delete data;
    return s_data;
}

int receive_int (Player* p, int* bytes_received)
{
    int data;
    *bytes_received = p->socket->precv(&data, sizeof(data), 0);
    if (*bytes_received > 0)
        data = ntohl(data);
    else
        data = 0;
    return data;
}

int send_string (Player* p, string data)
{
    if (p->socket != NULL)
        return p->socket->psend(data.c_str(), data.length(), 0);
    else
        return -1;
}

int send_wstring (Player* p, wstring data)
{
    string data_utf8 = utf16_to_utf8(data);
    const char* c_data = data_utf8.c_str();
    int c_length = strlen(c_data);
    if (p->socket != NULL)
        return p->socket->psend(c_data, c_length, 0);
    else
        return -1;
}

int send_int (Player* p, int data)
{
    data = htonl(data);
    if (p->socket != NULL)
        return p->socket->psend(&data, sizeof(data), 0);
    else
        return -1;
}

void send_command(string command, Player* p)
{
    wstring w_command;
    w_command.assign(command.begin(), command.end());
    wcout << currentDateTime() << L"Sending command to player " << p->name << L": " << w_command << endl;
    send_int(p, command.length());
    send_string(p, command);
}

void SendGameList(Player* p, list<Game*>* glist)
{
    list<Game*>::iterator i;
    send_command("game_list", p);
    send_int(p, glist->size());
    if (glist->size() != 0)
    {
        for (i = glist->begin() ; i != glist->end() ; ++i)
        {
            send_int(p, get_utf8_length((*i)->name));
            send_wstring(p, (*i)->name);
            send_int(p, (*i)->n_players);
            send_int(p, (*i)->plist.size());
            send_int(p, ((*i)->started ? 1 : 0));
            send_int(p, ((*i)->ended ? 1 : 0));
            send_int(p, ((*i)->bd_id > -1 ? 1 : 0));
        }
    }
}

bool add_player_to_player_list (Player* p, dlib::mutex* mutex_lists, list<Player*>* plist)
{
    mutex_lists->lock();
    bool found = false;
    list<Player*>::iterator i = plist->begin();
    while (!found && i != plist->end())
    {
        if ((*i)->name == p->name)
            found = true;
        else
            ++i;
    }
    if (found)
    {
        wcout << currentDateTime() << L"Player already connected" << endl;
        mutex_lists->unlock();
        return false;
    }
    else
    {
        wcout << currentDateTime() << L"Player accepted" << endl;
        plist->push_back(p);
        mutex_lists->unlock();
        return true;
    }
}

bool delete_player_from_player_list(Player* p, dlib::mutex* mutex_lists, list<Player*>* plist)
{
    mutex_lists->lock();
    bool found = false;
    list<Player*>::iterator i = plist->begin();
    while (!found && i != plist->end())
    {
        if ((*i)->name == p->name)
            found = true;
        else
            ++i;
    }
    if (!found)
    {
        wcout << currentDateTime() << L"Player not found in player list" << endl;
        mutex_lists->unlock();
        return false;
    }
    else
    {
        wcout << currentDateTime() << L"Player deleted from player list" << endl;
        plist->erase(i);
        mutex_lists->unlock();
        return true;
    }
}

bool delete_chat_if_empty(Chat* chat, dlib::mutex* mutex_lists, list<Chat*>* chat_list)
{
    if (chat == NULL)
        return true;
    mutex_lists->lock();
    if (chat->players.empty())
    {
        wcout << currentDateTime() << L"Chat " << chat->id << L" deleted because it's empty" << endl;
        chat_list->remove(chat);
        delete chat;
        mutex_lists->unlock();
        return true;
    }
    mutex_lists->unlock();
    return false;
}

bool delete_game_if_empty(Game* game, dlib::mutex* mutex_lists, list<Game*>* glist)
{
    mutex_lists->lock();
    if (game->plist.empty())
    {
        wcout << currentDateTime() << L"Game " << game->name << L" deleted because it's empty" << endl;
        glist->remove(game);
        delete game;
        mutex_lists->unlock();
        return true;
    }
    else
    {
        mutex_lists->unlock();
        return false;
    }
}

void disconnect_client(Player* p, bool kicking, ServerState* serverState)
{
    if (!p->connected)
        return;
    serverState->mutex_disconnects.lock();
    // Leave all normal chats and global chat
    list<Chat*>::iterator i;
    for (i = serverState->chat_list.begin() ; i != serverState->chat_list.end() ; ++i)
    {
        if ((*i)->check_already_joined(p))
        {
            (*i)->leave(p);
            if (delete_chat_if_empty(get_chat_from_id((*i)->id, &serverState->chat_list, &serverState->glist), &serverState->mutex_lists, &serverState->chat_list))
            {
                if (serverState->chat_list.size() > 0)
                    i = serverState->chat_list.begin(); // When deleting a chat, I prefer starting again to avoid segmentation faults->
                else
                    break; // If it was the last chat, chat_list.begin() returns an invalid pointer, so the loop must end
            }
            else
            {
                // Notify all chat users of the player disconnexion
                list<Player*>::iterator i4, j;
                Player* dest;
                for (i4 = (*i)->players.begin() ; i4 != (*i)->players.end() ; ++i4)
                {
                    dest = *i4;
                    send_command("chat_userlist", dest);
                    send_int(dest, (*i)->id);
                    send_int(dest, (*i)->players.size()); // Number of players
                    for (j = (*i)->players.begin() ; j != (*i)->players.end() ; ++j)
                    {
                        send_int(dest, get_utf8_length((*j)->name));
                        send_wstring(dest, (*j)->name);
                    }
                }
            }
        }
    }
    serverState->global_chat_list.remove(p);
    // Leave games
    list<Game*>::iterator i2;
    for (i2 = serverState->glist.begin() ; i2 != serverState->glist.end() ; ++i2)
    {
        if ((*i2)->check_already_joined(p))
        {
            (*i2)->leave(p);
            if (delete_game_if_empty(*i2, &serverState->mutex_lists, &serverState->glist))
            {
                if (serverState->glist.size() > 0)
                    i2 = serverState->glist.begin(); // When deleting a game, I prefer starting again to avoid segmentation faults
                else
                    break; // If it was the last game, glist.begin() returns an invalid pointer, so the loop must end
            }
            else
            {
                // Notify the rest of players that the player left the game
                list<Player*>::iterator i3, j;
                Player* dest, * winner;
                for (i3 = (*i2)->plist.begin() ; i3 != (*i2)->plist.end() ; ++i3)
                {
                    dest = (*i3);
                    if (kicking)
                    {
                        send_command("player_kicked", dest);
                        send_int(dest, (*i2)->id);
                        send_int(dest, get_utf8_length(p->name));
                        send_wstring(dest, p->name);
                    }
                    else
                    {
                        send_command("player_retired", dest);
                        send_int(dest, (*i2)->id);
                        send_int(dest, get_utf8_length(p->name));
                        send_wstring(dest, p->name);
                    }
                    send_command("chat_userlist", dest);
                    send_int(dest, (*i2)->id);
                    send_int(dest, (*i2)->plist.size()); // Number of players
                    for (j = (*i2)->plist.begin() ; j != (*i2)->plist.end() ; ++j)
                    {
                        send_int(dest, get_utf8_length((*j)->name));
                        send_wstring(dest, (*j)->name);
                    }
                    if ((*i2)->get_active_players_count() == 1)
                    {
                        (*i2)->ended = true;
                        send_command("game_ended", dest);
                        send_int(dest, (*i2)->id);
                        winner = (*i2)->get_winner();
                        send_int(dest, get_utf8_length(winner->name));
                        send_wstring(dest, winner->name);
                    }
                }
            }
        }
    }
    delete_player_from_player_list(p, &serverState->mutex_lists, &serverState->plist);
    p->connected = false;
    serverState->mutex_disconnects.unlock();
}

void insert_and_sort(list<Player*>* plist, Player* player, int game_id)
{
    for (list<Player*>::iterator i = plist->begin() ; i != plist->end() ; i++)
    {
        if (player->GetState(game_id)->num < (*i)->GetState(game_id)->num)
        {
            plist->insert(i, player);
            break;
        }
    }
}