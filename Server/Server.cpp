#include "Server.h"
#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include <ctime>
#include <cstdlib>

#include "dlib/threads.h"
#include "dlib/string.h"

#define MAXCONN 100

using namespace std;

Server::Server()
{
    this->compatible_version = "2.1.6";
    this->closing = 0;
    this->id_count = 0;
}

Server::~Server()
{
}

void Server::unhook_signals()
{
    signal(SIGINT, 0);
    signal(SIGTERM, 0);
#ifdef _WIN32
    signal(SIGBREAK, 0);
#endif
}

void Server::empty_global_chat_list()
{
    list<Player*>::iterator i;
    for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
    {
        *i = NULL;
    }
}

void Server::empty_chat_list()
{
    list<Chat*>::iterator i;
    for (i = chat_list.begin() ; i != chat_list.end() ; ++i)
    {
        Chat* c = *i;
        delete c;
    }
}

void Server::empty_glist()
{
    list<Game*>::iterator i;
    for (i = glist.begin() ; i != glist.end() ; ++i)
    {
        Game* g = *i;
        delete g;
    }
}

void Server::empty_plist()
{
    list<Player*>::iterator i;
    for (i = plist.begin() ; i != plist.end() ; ++i)
    {
        Player* p = *i;
        delete p;
    }
}

void close_server (int signum, Server* server)
{
    server->closing = 1;
    wcout << currentDateTime() << endl << L"Closing server due to signal " << signum << endl;
    server->unhook_signals();
    delete server->socket_server;
}

void Server::hook_signals()
{
    signal(SIGINT, close_server);
    signal(SIGTERM, close_server);
#ifdef _WIN32
    signal(SIGBREAK, close_server);
#endif
}

void Server::kick_hacker(int reason, Player* p)
{
	kick_hacker_real(reason, p, &mutex_disconnects, &mutex_lists, &chat_list, &glist, &global_chat_list, &plist);
}

void Server::handle_command(string command, Player* player)
{
    if (command == "#disconnect#")
    {
		disconnect_client(player, false, &mutex_disconnects, &mutex_lists, &chat_list, &glist, &global_chat_list, &plist);
		ch->Disconnect(player, &plist, &glist);
    }
    else if (command == "get_players")
    {
        list<Player*>::iterator i;
        send_command("player_list", player);
        send_int(player, plist.size()); // Number of players
        for (i = plist.begin() ; i != plist.end() ; ++i)
        {
            send_int(player, get_utf8_length((*i)->name));
            send_wstring(player, (*i)->name);
        }
    }
    else if (command == "get_games")
    {
        SendGameList(player, &glist);
    }
    else if (command == "create_game")
    {
        int bytes_received;
        int len_name = receive_int(player, &bytes_received);
        wstring name = receive_wstring(player, len_name, &bytes_received);
        int long_n_players = receive_int(player, &bytes_received);
        int n_players = atoi(receive_string(player, long_n_players, &bytes_received).c_str());
        if (name.empty()) // Hack, kick player
            return kick_hacker(1, player);
        if (get_game_from_name(name, &glist) != NULL) // Repeated name
            return;
        Game* new_game = new Game(name, n_players, player, &mutex_ids, &id_count, random_gen, false);
        wcout << currentDateTime() << L"New game! Name: " << name << " (ID " << new_game->id << ") | Number of players: " << n_players << endl;
        glist.push_back(new_game);
        send_command("joined_game", player);
        send_int(player, get_utf8_length(name));
        send_wstring(player, name);
        send_int(player, new_game->chat->id);
        send_int(player, get_utf8_length(new_game->creator->name));
        send_wstring(player, new_game->creator->name);
        send_int(player, new_game->n_players);
        for (list<Player*>::iterator i = plist.begin() ; i != plist.end() ; ++i)
            SendGameList((*i), &glist);
    }
    else if (command == "join_game")
    {
        int bytes_received;
        int len_name = receive_int(player, &bytes_received);
        wstring name = receive_wstring(player, len_name, &bytes_received);
        Game* game = get_game_from_name(name, &glist);
        if (game == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        if (game->check_already_joined(player))
        {
            send_command("cant_join_already_joined", player);
            send_int(player, get_utf8_length(name));
            send_wstring(player, name);
        }
        else if (game->join(player))
        {
            send_command("joined_game", player);
            send_int(player, get_utf8_length(name));
            send_wstring(player, name);
            send_int(player, game->chat->id);
            send_int(player, get_utf8_length(game->creator->name));
            send_wstring(player, game->creator->name);
            send_int(player, game->n_players);
            Player* dest;
            for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
            {
                // We exclude joined player because the command is sent too quickly.
                // He will ask for player list just after joining
                dest = *i;
                if (dest != player)
                {
                    send_command("chat_userlist", dest);
                    send_int(dest, game->id);
                    send_int(dest, game->plist.size()); // Number of players
                    for (list<Player*>::iterator j = game->plist.begin() ; j != game->plist.end() ; ++j)
                    {
                        send_int(dest, get_utf8_length((*j)->name));
                        send_wstring(dest, (*j)->name);
                    }
                }
            }
            for (list<Player*>::iterator i = plist.begin() ; i != plist.end() ; i++)
                SendGameList((*i), &glist);
        }
        else if (game->started)
        {
            send_command("cant_join_game_started", player);
            send_int(player, get_utf8_length(name));
            send_wstring(player, name);
        }
        else if (game->ended)
        {
            send_command("cant_join_game_ended", player);
            send_int(player, get_utf8_length(name));
            send_wstring(player, name);
        }
        else
        {
            send_command("cant_join_game_full", player);
            send_int(player, get_utf8_length(name));
            send_wstring(player, name);
        }
    }
    else if (command == "leave_game")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        Player* old_creator = game->creator;
        game->leave(player);
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
        {
            dest = *i;
            send_command("chat_userlist", dest);
            send_int(dest, game->id);
            send_int(dest, game->plist.size()); // Number of players
            for (j = game->plist.begin() ; j != game->plist.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
            // If the creator is who left, send the new creator so he can start the game
            if (game->creator != old_creator)
            {
                send_command("game_creator_changed", dest);
                send_int(dest, game->id);
                send_int(dest, get_utf8_length(game->creator->name));
                send_wstring(dest, game->creator->name);
            }
        }
        if (delete_game_if_empty(game, &mutex_lists, &glist))
        {
            for (i = plist.begin() ; i != plist.end() ; ++i)
            {
                dest = *i;
                SendGameList(dest, &glist);
            }
        }
        for (i = plist.begin() ; i != plist.end() ; i++)
            SendGameList((*i), &glist);
    }
    else if (command == "create_chat")
    {
        int bytes_received;
        int len_quantity = receive_int(player, &bytes_received);
        int quantity = atoi(receive_string(player, len_quantity, &bytes_received).c_str());
        vector<wstring> player_list = vector<wstring>(quantity+1);
        vector<wstring>::iterator i;
        // Receive every player and add the player who sends the command
        int len_name;
        for (i = player_list.begin() ; i != --player_list.end() ; ++i)
        {
            len_name = receive_int(player, &bytes_received);
            (*i) = receive_wstring(player, len_name, &bytes_received);
        }
        (*i) = player->name;
        Chat* new_chat = new Chat(player, true, &mutex_ids, &id_count);
        chat_list.push_back(new_chat);
        wcout << currentDateTime() << L"New chat with ID " << new_chat->id << ". Number of players: " << player_list.size() << endl;
        // Send commands to selected players to ask them to join the chat
        Player* dest;
        for (i = player_list.begin() ; i != player_list.end() ; ++i)
        {
            dest = get_player_from_name(*i, &plist);
            // The requested player can disconnect while processing this command
            if (dest == NULL)
                continue;
            send_command("ask_join_chat", dest);
            send_int(dest, new_chat->id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
        }
    }
    else if (command == "join_global_chat")
    {
        global_chat_list.push_back(player);
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
        {
            dest = *i;
            send_command("global_chat_userlist", dest);
            send_int(dest, global_chat_list.size()); // Number of players
            for (j = global_chat_list.begin() ; j != global_chat_list.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
        }
    }
    else if (command == "join_chat")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Chat* chat = get_chat_from_id(id, &chat_list, &glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        if (chat->check_already_joined(player)) // Don't allow join a chat twice (hack)
            return kick_hacker(3, player);
        chat->join(player);
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
        {
            dest = *i;
            send_command("chat_userlist", dest);
            send_int(dest, chat->id);
            send_int(dest, chat->players.size()); // Number of players
            for (j = chat->players.begin() ; j != chat->players.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
        }
    }
    else if (command == "leave_global_chat")
    {
        global_chat_list.remove(player);
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
        {
            dest = *i;
            send_command("global_chat_userlist", dest);
            send_int(dest, global_chat_list.size()); // Number of players
            for (j = global_chat_list.begin() ; j != global_chat_list.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
        }
    }
    else if (command == "leave_chat")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Chat* chat = get_chat_from_id(id, &chat_list, &glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        if (!chat->leave(player)) // Possible hack
            return kick_hacker(4, player);
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
        {
            dest = *i;
            send_command("chat_userlist", dest);
            send_int(dest, chat->id);
            send_int(dest, chat->players.size()); // Number of players
            for (j = chat->players.begin() ; j != chat->players.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
        }
        delete_chat_if_empty(chat, &mutex_lists, &chat_list);
    }
    else if (command == "get_global_chat_users")
    {
        list<Player*>::iterator i, j;
        send_command("global_chat_userlist", player);
        send_int(player, global_chat_list.size()); // Number of players
        for (j = global_chat_list.begin() ; j != global_chat_list.end() ; ++j)
        {
            send_int(player, get_utf8_length((*j)->name));
            send_wstring(player, (*j)->name);
        }
    }
    else if (command == "get_chat_users")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Chat* chat = get_chat_from_id(id, &chat_list, &glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        list<Player*>::iterator i; 
        send_command("chat_userlist", player);
        send_int(player, id);
        send_int(player, chat->players.size()); // Number of players
        for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
        {
            send_int(player, get_utf8_length((*i)->name));
            send_wstring(player, (*i)->name);
        }
    }
    else if (command == "send_global_chat_msg")
    {
        int bytes_received;
        int long_msg = receive_int(player, &bytes_received);
        wstring msg = receive_wstring(player, long_msg, &bytes_received);
        list<Player*>::iterator i;
        Player* dest;
        for (i = global_chat_list.begin() ; i != global_chat_list.end() ; ++i)
        {
            dest = *i;
            send_command("new_global_chat_msg", dest);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, get_utf8_length(msg));
            send_wstring(dest, msg);
        }
    }
    else if (command == "send_chat_msg")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        int long_msg = receive_int(player, &bytes_received);
        wstring msg = receive_wstring(player, long_msg, &bytes_received);
        Chat* chat = get_chat_from_id(id, &chat_list, &glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        if (!chat->check_already_joined(player)) // hack, send messages to a chat the player hasn't joined: kick player
            return kick_hacker(5, player);
        if ((msg.length() > 1024) || (dlib::trim(msg).length() == 0)) // hack, send messages longer or shorter than limits
            return kick_hacker(6, player);
        list<Player*>::iterator i;
        Player* dest;
        for (i = chat->players.begin() ; i != chat->players.end() ; ++i)
        {
            dest = *i;
            send_command("new_chat_msg", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, get_utf8_length(msg));
            send_wstring(dest, msg);
        }
    }
    else if (command == "start_game")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if (game->creator != player) // Hack, trying to start a game not created by the player
            return kick_hacker(7, player);
        if (game->started || game->ended) // Hack, start a already started game or an ended game
            return kick_hacker(8, player);
        game->set_players_money(configuration);
        game->start();
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = game->plist.begin() ; i != game->plist.end() ; ++i)
        {
            dest = *i;
            send_command("game_started", dest);
            send_int(dest, id);
            send_int(dest, game->n_players);
            // Send configuration
            send_int(dest, config_content.size());
            send_string(dest, config_content);
            send_int(dest, game->starting_player);
            // Send player list
            send_int(dest, game->plist.size());
            for (j = game->plist.begin() ; j != game->plist.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
        }
        for (i = plist.begin() ; i != plist.end() ; i++)
            SendGameList((*i), &glist);
    }
    else if (command == "roll_dice")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        list<Player*>::iterator i;
        Player* dest;
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(10, player); 
        if ((state->rolled_last_turn) && ((game->last_dice_res) < 6)) // Hack, retire player
            return kick_hacker(11, player);
        if (state->debt_last_turn > 0) // Hack, retire player
            return kick_hacker(12, player);
        game->roll_dice();
        game->move_player(player, &debt_mutex);
        state->rolled_last_turn = true;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("rolled_dice", dest);
            send_int(dest, id);
            send_int(dest, game->last_dice_res);
            send_int(dest, game->last_auto_advance);
            send_int(dest, state->position->number);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
        }
    }
    else if (command == "roll_construction_dice")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(14, player);
        if (state->position->type != build) // Hack, retire player
            return kick_hacker(15, player);
        TBuild_dice_res construction_dice_res = game->roll_construction_dice();
        Player* dest;
        list<Player*>::iterator i;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; i++)
        {
            dest = (*i);
            send_command("rolled_construction_dice", dest);
            send_int(dest, id);
            send_int(dest, (int)construction_dice_res);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
        }
        if (construction_dice_res == Deny)
            state->built_last_turn = true; // Avoid hacking, because if construction is denied, the player can't try again in the same turn
    }
    else if (command == "turn_pass")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        list<Player*>::iterator i;
        Player* dest;
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(17, player);
        if ((!state->rolled_last_turn) && ((game->last_dice_res) < 6)) // Hack, retire player
            return kick_hacker(18, player);
        if (state->debt_last_turn > 0) // Hack, retire player
            return kick_hacker(19, player);
        Player* next_player = game->turn_pass(&debt_mutex);
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("turn_passed", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(next_player->name));
            send_wstring(dest, next_player->name);
        }
    }
    else if (command == "charge_bank")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        list<Player*>::iterator i;
        Player* dest;
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if ((game->current_player != player) || (game->can_charge_bank(player) == false) || (state->charged_bank_last_turn)) // Hack, retire player
            return kick_hacker(21, player);
        else
            player->Charge_bank(game->id);
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, state->n_50);
            send_int(dest, state->n_100);
            send_int(dest, state->n_500);
            send_int(dest, state->n_1000);
            send_int(dest, state->n_5000);
        }
    }
    else if (command == "buy_hotel")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        len_int = receive_int(player, &bytes_received);
        int n_5000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_1000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_500 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_100 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_50 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        // We have selected money by player, change needs to be calculated
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(23, player);
        if (state->position->type != buy)  // Hack, retire player
            return kick_hacker(24, player);
        if (state->bought_last_turn) // Hack, retire player
            return kick_hacker(25, player);
        Hotel* hotel = get_hotel_from_name(hotel_name, game);
        if (hotel->owner != NULL) // Hack, retire player
            return kick_hacker(26, player);
        if ((state->position->hotel_left != hotel->name) && (state->position->hotel_right != hotel->name)) // Hack, retire player
            return kick_hacker(27, player);
        int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
        if (total_selected < (hotel->price)) // Hack, retire player
            return kick_hacker(28, player);
        hotel->owner = player;
        player->Buy_hotel(game->id, hotel, n_5000, n_1000, n_500, n_100, n_50);
        // Calculate change
        if (total_selected > (hotel->price))
        {
            game->calculate_return(total_selected - hotel->price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
            player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
        }
        state->bought_last_turn = true;
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("hotel_purchased", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, get_utf8_length(hotel_name));
            send_wstring(dest, hotel_name);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, state->n_50);
            send_int(dest, state->n_100);
            send_int(dest, state->n_500);
            send_int(dest, state->n_1000);
            send_int(dest, state->n_5000);
        }
    }
    else if (command == "expropriate_hotel")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        len_int = receive_int(player, &bytes_received);
        int n_5000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_1000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_500 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_100 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int n_50 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        // We have selected money by player, change needs to be calculated
        // Needs checking: hotel has owner and is different than player, hotel can be expropriated (player position is next to the hotel, no phases built),
        // player total money is previous total - hotel expropriation price, previous owner total money is previous total + hotel_expropriation price
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(30, player);
        if (state->position->type != buy)  // Hack, retire player
            return kick_hacker(31, player);
        if (state->bought_last_turn) // Hack, retire player
            return kick_hacker(32, player);
        Hotel* hotel = get_hotel_from_name(hotel_name, game);
        if ((hotel->owner == NULL) || (hotel->owner == player) || ((hotel->n_built_phases) > 0)) // Hack, retire player
            return kick_hacker(33, player);
        if ((state->position->hotel_left != hotel->name) && (state->position->hotel_right != hotel->name)) // Hack, retire player
            return kick_hacker(34, player);
        int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
        if (total_selected < (hotel->expropriation_price)) // Hack, retire player
            return kick_hacker(35, player);
        Player* previous_owner = hotel->owner;
        hotel->owner = player;
        previous_owner->Expropriate_hotel(game->id, hotel);
        player->Buy_hotel(game->id, hotel, previous_owner, n_5000, n_1000, n_500, n_100, n_50);
        // Calculate change
        if (total_selected > (hotel->expropriation_price))
        {
            game->calculate_return(previous_owner->GetState(game->id), total_selected - hotel->expropriation_price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
            player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
        }
        state->bought_last_turn = true;
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("hotel_expropriated", dest);
            send_int(dest, id);
            send_int(dest, 0);// 0 = expropriation, 1 = auction
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, get_utf8_length(hotel_name));
            send_wstring(dest, hotel_name);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, state->n_50);
            send_int(dest, state->n_100);
            send_int(dest, state->n_500);
            send_int(dest, state->n_1000);
            send_int(dest, state->n_5000);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(previous_owner->name));
            send_wstring(dest, previous_owner->name);
            PlayerGameState* previous_owner_state = previous_owner->GetState(game->id);
            send_int(dest, previous_owner_state->n_50);
            send_int(dest, previous_owner_state->n_100);
            send_int(dest, previous_owner_state->n_500);
            send_int(dest, previous_owner_state->n_1000);
            send_int(dest, previous_owner_state->n_5000);
        }
    }
    else if (command == "build_phase")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
        len_int = receive_int(player, &bytes_received);
        int type = atoi(receive_string(player, len_int, &bytes_received).c_str());
        if (type == 2)
        {
            len_int = receive_int(player, &bytes_received);
            n_5000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_1000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_500 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_100 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_50 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        }
        // We have selected money by player, change needs to be calculated
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(37, player);
        // You can also buy the ground in any type of position
        if (state->built_last_turn) // Hack, retire player
            return kick_hacker(39, player);
        Hotel* hotel = get_hotel_from_name(hotel_name, game);
        if (hotel->owner != player) // Hack, retire player
            return kick_hacker(40, player);
        if ((type == 0) && (state->position->type != free_phase)) // Hack, retire player
            return kick_hacker(41, player);
        if (hotel->next_expansion_is_ground)
            game->rolled_construction_dice = true; // As it is not really rolled, simplify checks
        if ((type == 1) && (hotel->next_expansion_is_ground)) // Hack, retire player, ground can't be free (only in free phases positions) because construction dice is not rolled
            return kick_hacker(42, player);
        if ((type == 1) && (!game->rolled_construction_dice)) // Hack, retire player, he didn't ask for permission when building anything different than ground
            return kick_hacker(43, player);
        if ((type == 1) && (game->last_construction_dice_res != Free)) // Hack, retire player
            return kick_hacker(44, player);
        if ((type == 2) && (!game->rolled_construction_dice)) // Hack, retire player, didn't ask for permission when building anything different than ground
            return kick_hacker(45, player);
        int total_selected = 0;
        if (!hotel->Can_extend()) // Hack, retire player
            return kick_hacker(46, player);
        int total_price;
        switch (type)
        {
        case 0:
        case 1:  total_price = 0;
            break;
        case 2:  total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
            total_price = hotel->Price_next_expansion();
            if (game->last_construction_dice_res == Double)
                total_price = hotel->Price_next_expansion() * 2;
            if (total_selected < hotel->Price_next_expansion()) // Hack, retire player
                return kick_hacker(47, player);
            break;
        default: return kick_hacker(48, player); // Hack, retire player
        }
        hotel->Extend();
        if (type == 2)
        {
            player->Pay_phase_or_entrance(game->id, n_5000, n_1000, n_500, n_100, n_50);
            // Calculate change
            if (total_selected > total_price)
            {
                game->calculate_return(total_selected - total_price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
                player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
            }
        }
        state->built_last_turn = true;
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("phase_built", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(hotel_name));
            send_wstring(dest, hotel_name);
            if (type == 2)
            {
                send_command("update_player_money", dest);
                send_int(dest, id);
                send_int(dest, get_utf8_length(player->name));
                send_wstring(dest, player->name);
                send_int(dest, state->n_50);
                send_int(dest, state->n_100);
                send_int(dest, state->n_500);
                send_int(dest, state->n_1000);
                send_int(dest, state->n_5000);
            }
        }
    }
    else if (command == "buy_entrance")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        len_int = receive_int(player, &bytes_received);
        int position = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int n_5000 = 0, n_1000 = 0, n_500 = 0, n_100 = 0, n_50 = 0;
        len_int = receive_int(player, &bytes_received);
        int type = atoi(receive_string(player, len_int, &bytes_received).c_str());
        if (type == 1)
        {
            len_int = receive_int(player, &bytes_received);
            n_5000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_1000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_500 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_100 = atoi(receive_string(player, len_int, &bytes_received).c_str());
            len_int = receive_int(player, &bytes_received);
            n_50 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        }
        // We have selected money by player, change needs to be calculated
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player != player) // Hack, retire player
            return kick_hacker(50, player);
        if ((state->position->type != free_entrance) && (!game->can_buy_entrances(player))) // Hack, retire player
            return kick_hacker(51, player);
        Hotel* hotel = get_hotel_from_name(hotel_name, game);
        if (hotel->owner != player) // Hack, retire player
            return kick_hacker(52, player);
        if (hotel->n_built_phases == 0) // Hack, retire player
            return kick_hacker(53, player);
        if (hotel->entrance_bought_last_turn) // Hack, retire player
            return kick_hacker(54, player);
        if ((type == 0) && (state->position->type != free_entrance)) // Hack, retire player
            return kick_hacker(55, player);
        if ((type == 0) && state->free_entrance_used) // Hack, retire player
            return kick_hacker(56, player);
        if (hotel->Has_entrance_in_position(position)) // Hack, retire player
            return kick_hacker(57, player);
        if (!hotel->Is_a_valid_entrance_position(game->positions[position])) // Hack, retire player
            return kick_hacker(58, player);
        hotel->Add_entrance(position);
        if (type == 1)
        {
            int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
            player->Pay_phase_or_entrance(game->id, n_5000, n_1000, n_500, n_100, n_50);
            // Calculate change
            if (total_selected > (hotel->entrance_price))
            {
                game->calculate_return(total_selected - hotel->entrance_price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
                player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
            }
            hotel->entrance_bought_last_turn = true; // Only if it is not free
        }
        else if (type == 0)
            state->free_entrance_used = true;
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("entrance_added", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(hotel_name));
            send_wstring(dest, hotel_name);
            send_int(dest, position);
            if (type == 1)
            {
                send_command("update_player_money", dest);
                send_int(dest, id);
                send_int(dest, get_utf8_length(player->name));
                send_wstring(dest, player->name);
                send_int(dest, state->n_50);
                send_int(dest, state->n_100);
                send_int(dest, state->n_500);
                send_int(dest, state->n_1000);
                send_int(dest, state->n_5000);
            }
        }
    }
    else if (command == "retire")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int type = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Player* receiving_player = NULL;
        if (type == 1) // Player is automatically retired because he tried to create an auction and he doesn't have any hotel or enough money to pay
        {
            int len_name = receive_int(player, &bytes_received);
            wstring receiver_name = receive_wstring(player, len_name, &bytes_received);
            receiving_player = get_player_from_name(receiver_name, &plist);
        }
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (player == NULL) // Hack, retire player
            return kick_hacker(60, player);
        if (!game->is_active(player)) // Hack, already retired
            return kick_hacker(61, player);
        Player* next_player = NULL;
        if (game->get_active_players_count() > 2) // Game not finished yet
        {
            if (game->current_player == player)
                next_player = game->turn_pass(&debt_mutex);
        }
        game->eliminate_player(player, receiving_player);
        list<Player*>::iterator i;
        Player* dest, * winner;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i) // Avoid sending commands to retired players, because their forms can be already closed and could lead to a client crash
        {
            dest = (*i);
            send_command("player_retired", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            if (type == 1) // Update affected players money
            {
                send_command("update_player_money", dest);
                send_int(dest, id);
                send_int(dest, get_utf8_length(receiving_player->name));
                send_wstring(dest, receiving_player->name);
                PlayerGameState* receiving_player_state = receiving_player->GetState(game->id);
                send_int(dest, receiving_player_state->n_50);
                send_int(dest, receiving_player_state->n_100);
                send_int(dest, receiving_player_state->n_500);
                send_int(dest, receiving_player_state->n_1000);
                send_int(dest, receiving_player_state->n_5000);
                send_command("update_player_money", dest);
                send_int(dest, id);
                send_int(dest, get_utf8_length(player->name));
                send_wstring(dest, player->name);
                send_int(dest, state->n_50);
                send_int(dest, state->n_100);
                send_int(dest, state->n_500);
                send_int(dest, state->n_1000);
                send_int(dest, state->n_5000);
            }
            if (game->get_active_players_count() == 1)
            {
                game->ended = true;
                send_command("game_ended", dest);
                send_int(dest, id);
                winner = game->get_winner();
                send_int(dest, get_utf8_length(winner->name));
                send_wstring(dest, winner->name);
            }
            else
            {
                if (next_player != NULL) // Turn was passed
                {
                    send_command("turn_passed", dest);
                    send_int(dest, id);
                    send_int(dest, get_utf8_length(next_player->name));
                    send_wstring(dest, next_player->name);
                }
            }
        }
    }
    else if (command == "ask_nights")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        list<Player*>::iterator i;
        Player* dest;
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->current_player == player) // The client does not allow this
            return;
        int amount = 0, nights = 0;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i) // Check if players are in entrances of hotels of the asking player
        {
            dest = (*i);
            if (dest != player) // Player doesn't have to pay himself :D
            {
                wstring hotel_name;
                amount = game->get_money_for_nights(player, dest, &nights, &hotel_name);
                if (amount > 0) // The player is in a entrance and hasn't paid this turn
                {
                    debt_mutex.lock(); // To avoid creating a debt just when player is passing turn, because this command is asynchronous
                    PlayerGameState* dest_state = dest->GetState(game->id);
                    dest_state->debt_last_turn = amount;
                    dest_state->debt_nights_to_last_turn = player->GetState(game->id);
                    dest_state->paid_last_turn = false;
                    debt_mutex.unlock();
                    for (list<Player*>::iterator j = game->active_plist.begin() ; j != game->active_plist.end() ; ++j)
                    {
                        Player* destj = (*j);
                        send_command("ask_pay_nights", destj); // Will force the player to pay nights, if he hacks the game, in next turn pass he will be retired
                        send_int(destj, id);
                        send_int(destj, get_utf8_length(player->name));
                        send_wstring(destj, player->name);
                        send_int(destj, get_utf8_length(dest->name));
                        send_wstring(destj, dest->name);
                        send_int(destj, amount);
                        send_int(destj, nights);
                        send_int(destj, get_utf8_length(hotel_name));
                        send_wstring(destj, hotel_name);
                    }
                }
            }
        }
    }
    else if (command == "pay_nights")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int n_5000, n_1000, n_500, n_100, n_50;
        len_int = receive_int(player, &bytes_received);
        n_5000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_1000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_500 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_100 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_50 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        // We have selected money by player, change needs to be calculated
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if ((!game->started) || game->ended) // Hack, game not started yet or already ended
            return;
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (player == NULL) // Hack, retire player
            return kick_hacker(65, player);
        int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
        if (total_selected <= 0) // Hack, retire player, the command is only sent if player has something to pay
            return kick_hacker(66, player);
        if ((state->debt_last_turn == 0) || (state->debt_nights_to_last_turn == NULL)) // Hack, retire player
            return kick_hacker(67, player);
        player->Pay_nights(game->id, state->debt_nights_to_last_turn, n_5000, n_1000, n_500, n_100, n_50);
        state->paid_last_turn = true;
        // Calculate change
        if (total_selected > (state->debt_last_turn))
        {
            game->calculate_return(state->debt_nights_to_last_turn, total_selected - state->debt_last_turn, &n_5000, &n_1000, &n_500, &n_100, &n_50);
            player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
        }
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, state->n_50);
            send_int(dest, state->n_100);
            send_int(dest, state->n_500);
            send_int(dest, state->n_1000);
            send_int(dest, state->n_5000);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(state->debt_nights_to_last_turn->player->name));
            send_wstring(dest, state->debt_nights_to_last_turn->player->name);
            send_int(dest, state->debt_nights_to_last_turn->n_50);
            send_int(dest, state->debt_nights_to_last_turn->n_100);
            send_int(dest, state->debt_nights_to_last_turn->n_500);
            send_int(dest, state->debt_nights_to_last_turn->n_1000);
            send_int(dest, state->debt_nights_to_last_turn->n_5000);
        }
        debt_mutex.lock(); // To avoid creating a debt just when player is passing turn, because this command is asynchronous
        state->debt_last_turn = 0;
        state->debt_nights_to_last_turn = NULL;
        debt_mutex.unlock();
    }
    else if (command == "auction_start")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
            return kick_hacker(68, player);
        Hotel* hotel = get_hotel_from_name(hotel_name, game);
        if (hotel->owner != player) // Hack, retire player
            return kick_hacker(69, player);
        if (game->hotel_at_auction != NULL) // Hack, auction already in progress
            return kick_hacker(70, player);
        game->hotel_at_auction = hotel;
        game->best_bid = 0;
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("auction_started", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(hotel->name_txt));
            send_wstring(dest, hotel->name_txt);
        }
    }
    else if (command == "auction_bid")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int amount = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
            return kick_hacker(71, player);
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
            return kick_hacker(72, player);
        if (player == game->hotel_at_auction->owner) // Hack, retire player, he is trying to bid in his own auction
            return kick_hacker(73, player);
        if ((amount <= 0) || (amount < (game->best_bid)) || amount > (state->total_money) || ((amount % 50) != 0)) // Hack, retire player, invalid values
            return kick_hacker(74, player);
        game->best_bid = amount;
        game->best_bidder = player;
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("auction_bid_placed", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, amount);
        }
    }
    else if (command == "auction_sell")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
            return kick_hacker(75, player);
        if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
            return kick_hacker(76, player);
        if (player != game->hotel_at_auction->owner) // Hack, retire player, he is trying to end the auction without being the owner
            return kick_hacker(77, player);
        if (game->best_bid == 0) // Hack, retire player, no one has placed a bid yet
            return kick_hacker(78, player);
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("auction_sold", dest);
            send_int(dest, id);
            send_int(dest, game->best_bid);
        }
    }
    else if (command == "auction_pay")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int n_5000, n_1000, n_500, n_100, n_50;
        len_int = receive_int(player, &bytes_received);
        n_5000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_1000 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_500 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_100 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        n_50 = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
            return kick_hacker(79, player);
        PlayerGameState* state = player->GetState(game->id);
        if (state == NULL)
            return;
        if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
            return kick_hacker(80, player);
        if (game->best_bid == 0) // Hack, retire player, no one has placed a bid yet
            return kick_hacker(81, player);
        if (player != game->best_bidder) // Hack, retire player, the player who pays must be the best bidder
            return kick_hacker(82, player);
        int total_selected = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
        if ((total_selected <= 0) || (total_selected < (game->best_bid))) // Hack, retire player, the command is only sent if player has something to pay and >= than best bid
            return kick_hacker(83, player);
        Player* previous_owner = game->hotel_at_auction->owner;
        previous_owner->Expropriate_hotel(game->id, game->hotel_at_auction);
        player->Buy_hotel(game->id, game->hotel_at_auction, previous_owner, n_5000, n_1000, n_500, n_100, n_50);
        game->hotel_at_auction->owner = player;
        // Calculate change
        if (total_selected > (game->best_bid))
        {
            game->calculate_return(previous_owner->GetState(game->id), total_selected - game->best_bid, &n_5000, &n_1000, &n_500, &n_100, &n_50);
            player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
        }
        list<Player*>::iterator i;
        Player* dest;
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("hotel_expropriated", dest);
            send_int(dest, id);
            send_int(dest, 1);// 0 = expropriation, 1 = auction
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, get_utf8_length(game->hotel_at_auction->name_txt));
            send_wstring(dest, game->hotel_at_auction->name_txt);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(player->name));
            send_wstring(dest, player->name);
            send_int(dest, state->n_50);
            send_int(dest, state->n_100);
            send_int(dest, state->n_500);
            send_int(dest, state->n_1000);
            send_int(dest, state->n_5000);
            send_command("update_player_money", dest);
            send_int(dest, id);
            send_int(dest, get_utf8_length(previous_owner->name));
            send_wstring(dest, previous_owner->name);
            send_int(dest, previous_owner->GetState(game->id)->n_50);
            send_int(dest, previous_owner->GetState(game->id)->n_100);
            send_int(dest, previous_owner->GetState(game->id)->n_500);
            send_int(dest, previous_owner->GetState(game->id)->n_1000);
            send_int(dest, previous_owner->GetState(game->id)->n_5000);
        }
        // If this command is sent before updating hotels and money to everyone, the client doesn't know who owns the hotels
        for (i = game->active_plist.begin() ; i != game->active_plist.end() ; ++i)
        {
            dest = (*i);
            send_command("auction_ended", dest);
            send_int(dest, id);
        }
        game->best_bidder = NULL;
        game->best_bid = 0;
        game->hotel_at_auction = NULL;
    }
    else if (command == "save_game")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_password = receive_int(player, &bytes_received);
        wstring password = receive_wstring(player, len_password, &bytes_received);
        Game* game = get_game_from_id(id, &glist);
        game->password = password;
        if (savedgamesmgr->SaveGame(game, player))
        {
            for (list<Player*>::iterator i = game->active_plist.begin() ; i != game->active_plist.end() ; i++)
            {
                send_command("game_saved", (*i));
                send_int((*i), id);
                send_int((*i), game->bd_id);
                send_int((*i), get_utf8_length(player->name));
                send_wstring((*i), player->name);
                send_int((*i), get_utf8_length(game->password));
                send_wstring((*i), game->password);
            }
        }
        else
        {
            send_command("error_saving_game", player);
            send_int(player, id);
        }
    }
    else if (command == "load_game")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_password = receive_int(player, &bytes_received);
        wstring password = receive_wstring(player, len_password, &bytes_received);
        Game* game = get_game_from_bd_id(id, &glist);
        if (game != NULL) // Game already loaded
        {
            send_command("cannot_load_game", player);
            send_int(player, 1); // Error code = 1
            send_int(player, id);
            send_int(player, get_utf8_length(game->creator->name));
            send_wstring(player, game->creator->name);
        }
        else
        {
            int error_code = 0;
            game = savedgamesmgr->LoadGame(id, password, player, &mutex_ids, &id_count, random_gen, &error_code);
            if (game != NULL)
            {
                glist.push_back(game);
                send_command("game_loaded", player);
                for (list<Player*>::iterator i = plist.begin() ; i != plist.end() ; i++)
                    SendGameList((*i), &glist);
            }
            else
            {
                send_command("cannot_load_game", player);
                send_int(player, error_code);
                send_int(player, id);
            }
        }
    }
}

void Server::handle_client(void* arg)
{
    Player* p = (Player*) arg;
    int bytes_received;
    int len_version = receive_int(p, &bytes_received);
    string version = receive_string(p, len_version, &bytes_received);
    if (compatible_version != version)
    {
        send_int(p, compatible_version.length());
        send_string(p, compatible_version);
        wstring w_version;
        w_version.assign(version.begin(), version.end());
        wcout << currentDateTime() << L"Player rejected because of incompatible versions. It is using version " << w_version << endl;
        wcout << currentDateTime() << L"Disconnecting client" << endl;
    }
    else
    {
        send_int(p, 2);
        send_string(p, "ok");
    }
    int len_name = receive_int(p, &bytes_received);
    p->name = receive_wstring(p, len_name, &bytes_received);
    wcout << currentDateTime() << L"Handling new player. Player name: " << p->name << endl;
    if (p->name.length() > 20)
    {
        wcout << currentDateTime() << L"Player " << p->name << L" rejected because the name is too long (WARNING: possible hacked client)" << endl;
        p->socket->psend("login no", 8, 0);
        delete p;
        wcout << currentDateTime() << L"Disconnecting client" << endl;
    }
    else if (add_player_to_player_list(p, &mutex_lists, &plist) == false)
    {
        p->socket->psend("login ko", 8, 0);
        delete p;
        wcout << currentDateTime() << L"Disconnecting client" << endl;
    }
    else
    {
        p->socket->psend("login ok", 8, 0);
        bool online = true;
        // Send player list to all players, so they are notified about the new user
        // Send as much strings as connected players, with a count first
        list<Player*>::iterator i, j;
        Player* dest;
        for (i = plist.begin() ; i != plist.end() ; ++i)
        {
            dest = *i;
            send_command("player_list", dest);
            send_int(dest, plist.size()); // Number of players
            for (j = plist.begin() ; j != plist.end() ; ++j)
            {
                send_int(dest, get_utf8_length((*j)->name));
                send_wstring(dest, (*j)->name);
            }
        }
        int long_command;
        while (online)
        {
            long_command = receive_int(p, &bytes_received);
            string command = receive_string(p, long_command, &bytes_received);
            if (bytes_received > 0)
            {
                wstring w_command;
                w_command.assign(command.begin(), command.end());
                wcout << currentDateTime() << L"Received command from player " << p->name << ": " << w_command << endl;
                handle_command(command, p);
            }
            else
            {
                online = false;
                disconnect_client(p, false, &mutex_disconnects, &mutex_lists, &chat_list, &glist, &global_chat_list, &plist);
                delete p;
                wcout << currentDateTime() << L"Client disconnected" << endl;
            }
        }
    }
}

void Server::read_config(TiXmlDocument* config_xml)
{
    // For two players
    TiXmlNode* node = config_xml->LastChild()->FirstChild()->FirstChild()->FirstChild();
    configuration.two_players.n_5000 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.two_players.n_1000 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.two_players.n_500 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.two_players.n_100 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.two_players.n_50 = atoi(node->FirstChild()->Value());
    // Now for three or four players
    node = node->Parent()->NextSibling()->FirstChild();
    configuration.three_or_four_players.n_5000 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.three_or_four_players.n_1000 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.three_or_four_players.n_500 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.three_or_four_players.n_100 = atoi(node->FirstChild()->Value());
    node = node->NextSiblingElement();
    configuration.three_or_four_players.n_50 = atoi(node->FirstChild()->Value());
}

void Server::Run(int port)
{
#ifdef _WIN32
    SetConsoleOutputCP(CP_UTF8);
    //wcout.imbue(locale("Spanish_Spain.1256"));
#else
    setlocale(LC_ALL, "es_ES.UTF8");
#endif
    wcout << currentDateTime() << L"Starting Hotel server on port " << port << "..." << endl;
    socket_server = new Portable_socket();
    sockaddr_in server_info;
    sockaddr_in client_info;
    socklen_t addrlen;

    server_info.sin_family=AF_INET;
    server_info.sin_port=htons(port);
    server_info.sin_addr.s_addr=INADDR_ANY;
    int bind_retries = 0;
    while (socket_server->pbind((sockaddr*) &server_info,sizeof(server_info)) < 0)
    {
        wcout << currentDateTime() << L"bind error: " << socket_server->get_last_error() << endl;
        if (bind_retries == 5)
        {
            delete socket_server;
            return;
        }
        else
        {
            bind_retries++;
            #ifdef _WIN32
                Sleep(5000);
            #else
                sleep(5000);
            #endif
            wcout << currentDateTime() << L"bind retries: " << bind_retries << endl;
        }
    }
    if (socket_server->plisten(MAXCONN) < 0)
    {
        wcout << currentDateTime() << L"listen error: " << socket_server->get_last_error() << endl;
        delete socket_server;
        return;
    }
    wcout << currentDateTime() << L"Listening for connections" << endl;
    savedgamesmgr = new SavedgamesMgr();
    hook_signals();
    Player* p;
    random_gen = new CRandomMT();
    // Read Config.xml as string to send it to players
    ifstream config_file ("Config.xml");
    string line;
    config_content = "";
    while (getline(config_file, line))
    {
        config_content += line + '\n';
    }
    config_file.close();
    TiXmlDocument config_xml("Config.xml");
    if (!(config_xml.LoadFile()))
    {
        wcout << currentDateTime() << L"Error loading Config.xml" << endl;
        return;
    }
    // Load Config.xml to have configuration loaded for future checks
    read_config(&config_xml);
	ch = new CommandHandler();
    while (closing == 0)
    {
        addrlen = sizeof(client_info);
        socket_client = socket_server->paccept((sockaddr*) &client_info, &addrlen);
        if (socket_client != NULL)
            wcout << currentDateTime() << L"Client connection from " << inet_ntoa(client_info.sin_addr) << ":" << ntohs(client_info.sin_port) << endl;
        else
        {
            if (closing == 0)
                wcout << currentDateTime() << L"accept error: " << socket_server->get_last_error() << endl;
            else
            {
                // The server is closing
                delete savedgamesmgr;
				delete ch;
                empty_global_chat_list();
                wcout << currentDateTime() << L"Global Chat cleaned" << endl;
                empty_chat_list();
                wcout << currentDateTime() << L"Chat list cleaned" << endl;
                empty_glist();
                wcout << currentDateTime() << L"Game list cleaned" << endl;
                empty_plist();
                wcout << currentDateTime() << L"Player list cleaned" << endl;
                return;
            }
        }
        p = new Player(inet_ntoa(client_info.sin_addr), socket_client);
        dlib::create_new_thread(handle_client, (void*) p);
    }
}