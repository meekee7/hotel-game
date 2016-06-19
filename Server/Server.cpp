#include "Server.h"
#include <iostream>
#include <list>
#include <string>
#include <ctime>
#include <cstdlib>

#include "dlib/threads.h"
#include "dlib/string.h"

#define MAXCONN 100

using namespace std;

Server::Server()
{
    this->ch = new CommandHandler();
    this->compatible_version = "2.3.0";
    this->closing = 0;
}

Server::~Server()
{
    delete this->ch;
}

void Server::empty_lists()
{
    for (list<Player*>::iterator i = this->ch->serverState->global_chat_list.begin() ; i != this->ch->serverState->global_chat_list.end() ; ++i)
    {
        *i = NULL;
    }
    wcout << currentDateTime() << L"Global Chat list cleaned" << endl;
    for (list<Chat*>::iterator j = this->ch->serverState->chat_list.begin() ; j != this->ch->serverState->chat_list.end() ; ++j)
    {
        Chat* c = *j;
        delete c;
    }
    wcout << currentDateTime() << L"Chat list cleaned" << endl;
    for (list<Game*>::iterator k = this->ch->serverState->glist.begin() ; k != this->ch->serverState->glist.end() ; ++k)
    {
        Game* g = *k;
        delete g;
    }
    wcout << currentDateTime() << L"Game list cleaned" << endl;
    for (list<Player*>::iterator i = this->ch->serverState->plist.begin() ; i != this->ch->serverState->plist.end() ; ++i)
    {
        Player* p = *i;
        delete p;
    }
    wcout << currentDateTime() << L"Player list cleaned" << endl;
}

void Server::handle_command(string command, Player* player)
{
    if (command == "#disconnect#")
		ch->Disconnect(player);
    else if (command == "get_players")
        ch->GetPlayers(player);
    else if (command == "get_games")
        ch->GetGames(player);
    else if (command == "create_game")
    {
        int bytes_received;
        int len_name = receive_int(player, &bytes_received);
        wstring name = receive_wstring(player, len_name, &bytes_received);
        int long_n_players = receive_int(player, &bytes_received);
        int n_players = atoi(receive_string(player, long_n_players, &bytes_received).c_str());
        ch->CreateGame(player, name, n_players);
    }
    else if (command == "join_game")
    {
        int bytes_received;
        int len_name = receive_int(player, &bytes_received);
        wstring name = receive_wstring(player, len_name, &bytes_received);
        Game* game = get_game_from_name(name, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        ch->JoinGame(player, game);        
    }
    else if (command == "start_game")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        ch->StartGame(player, game, this->configuration);
    }
    else if (command == "leave_game")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        ch->LeaveGame(player, game);
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
        ch->CreateChat(player, quantity, player_list);
    }
    else if (command == "join_global_chat")
    {
        ch->JoinGlobalChat(player);
    }
    else if (command == "join_chat")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Chat* chat = get_chat_from_id(id, &this->ch->serverState->chat_list, &this->ch->serverState->glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        ch->JoinChat(player, chat);
    }
    else if (command == "leave_global_chat")
    {
        ch->LeaveGlobalChat(player);
    }
    else if (command == "leave_chat")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Chat* chat = get_chat_from_id(id, &this->ch->serverState->chat_list, &this->ch->serverState->glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        ch->LeaveChat(player, chat);
    }
    else if (command == "get_global_chat_users")
    {
        ch->GetGlobalChatUsers(player);
    }
    else if (command == "get_chat_users")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Chat* chat = get_chat_from_id(id, &this->ch->serverState->chat_list, &this->ch->serverState->glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        ch->GetChatUsers(player, chat);
    }
    else if (command == "send_global_chat_msg")
    {
        int bytes_received;
        int long_msg = receive_int(player, &bytes_received);
        wstring msg = receive_wstring(player, long_msg, &bytes_received);
        ch->SendGlobalChatMsg(player, msg);
    }
    else if (command == "send_chat_msg")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        int long_msg = receive_int(player, &bytes_received);
        wstring msg = receive_wstring(player, long_msg, &bytes_received);
        Chat* chat = get_chat_from_id(id, &this->ch->serverState->chat_list, &this->ch->serverState->glist);
        if (chat == NULL) // To avoid commands sent when chat does not exist anymore
            return;
        ch->SendChatMsg(player, chat, msg);
    }
    else if (command == "roll_dice")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->RollDice(player, game);        
    }
    else if (command == "roll_construction_dice")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        Hotel* selected_hotel = get_hotel_from_name(hotel_name, game);
        ch->RollConstructionDice(player, game, selected_hotel);
    }
    else if (command == "turn_pass")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->PassTurn(player, game);
    }
    else if (command == "charge_bank")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->ChargeBank(player, game);        
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
        len_int = receive_int(player, &bytes_received);
        int mode = atoi(receive_string(player, len_int, &bytes_received).c_str());
        // We have selected money by player, change needs to be calculated
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->BuyHotel(player, game, hotel_name, n_5000, n_1000, n_500, n_100, n_50, mode);
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
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->ExpropriateHotel(player, game, hotel_name, n_5000, n_1000, n_500, n_100, n_50);
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
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->BuildPhase(player, game, hotel_name, type, n_5000, n_1000, n_500, n_100, n_50);
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
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->BuyEntrance(player, game, hotel_name, position, type, n_5000, n_1000, n_500, n_100, n_50);
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
            receiving_player = get_player_from_name(receiver_name, &this->ch->serverState->plist);
        }
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->Retire(player, game, type, receiving_player);
    }
    else if (command == "ask_nights")
    {
        int bytes_received;
        int len_id = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_id, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->AskNights(player, game);
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
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->PayNights(player, game, n_5000, n_1000, n_500, n_100, n_50);
    }
    else if (command == "auction_start")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_name = receive_int(player, &bytes_received);
        wstring hotel_name = receive_wstring(player, len_name, &bytes_received);
        len_int = receive_int(player, &bytes_received);
        int minimum_price = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->AuctionStart(player, game, hotel_name, minimum_price);
    }
    else if (command == "auction_bid")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        len_int = receive_int(player, &bytes_received);
        int amount = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->AuctionBid(player, game, amount);
    }
    else if (command == "auction_sell")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->AuctionSell(player, game);
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
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        if (game == NULL) // To avoid commands sent when game does not exist anymore
            return;
        game->seconds_elapsed_last_command = 0;
        ch->AuctionPay(player, game, n_5000, n_1000, n_500, n_100, n_50);
    }
    else if (command == "save_game")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_password = receive_int(player, &bytes_received);
        wstring password = receive_wstring(player, len_password, &bytes_received);
        Game* game = get_game_from_id(id, &this->ch->serverState->glist);
        ch->SaveGame(player, game, password);
    }
    else if (command == "load_game")
    {
        int bytes_received;
        int len_int = receive_int(player, &bytes_received);
        int id = atoi(receive_string(player, len_int, &bytes_received).c_str());
        int len_password = receive_int(player, &bytes_received);
        wstring password = receive_wstring(player, len_password, &bytes_received);
        Game* game = get_game_from_bd_id(id, &this->ch->serverState->glist);
        ch->LoadGame(player, id, game, password);
    }
}

void handle_client_wrapper(void* arg)
{
    struct thread_param* tp = (struct thread_param*)arg;
    tp->server->handle_client(tp->player);
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
        delete p;
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
    else if (add_player_to_player_list(p, &this->ch->serverState->mutex_lists, &this->ch->serverState->plist) == false)
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
        Player* dest;
        for (list<Player*>::iterator i = this->ch->serverState->plist.begin() ; i != this->ch->serverState->plist.end() ; ++i)
        {
            dest = *i;
            send_command("player_list", dest);
            send_int(dest, this->ch->serverState->plist.size()); // Number of players
            for (list<Player*>::iterator j = this->ch->serverState->plist.begin() ; j != this->ch->serverState->plist.end() ; ++j)
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
                disconnect_client(p, false, this->ch->serverState);
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
    wcout << currentDateTime() << L"Allowed version: " << utf8_to_utf16(this->compatible_version) << endl;
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
                sleep(5);
            #endif
            wcout << currentDateTime() << L"bind retries: " << bind_retries << endl;
        }
    }
    int reuse_socket = 1;
    if (setsockopt(socket_server->get_fd(), SOL_SOCKET, SO_REUSEADDR, (char*) &reuse_socket, sizeof(int)) == -1) {
        wcout << currentDateTime() << L"setsockopt error: " << socket_server->get_last_error() << endl;
        delete socket_server;
        return;
    }
    if (socket_server->plisten(MAXCONN) < 0)
    {
        wcout << currentDateTime() << L"listen error: " << socket_server->get_last_error() << endl;
        delete socket_server;
        return;
    }
    wcout << currentDateTime() << L"Listening for connections" << endl;
    // Read Config.xml as string to send it to players
    ifstream config_file ("Config.xml");
    string line;
    configuration.config_content = "";
    while (getline(config_file, line))
    {
        configuration.config_content += line + '\n';
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
                empty_lists();                
                return;
            }
        }
        Player* p = new Player(inet_ntoa(client_info.sin_addr), socket_client);
        struct thread_param tp;
        tp.player = p;
        tp.server = this;
        dlib::create_new_thread(handle_client_wrapper, (void*) &tp);
    }
}

void Server::Stop(int signum)
{
    wcout << currentDateTime() << endl << L"Closing server due to signal " << signum << endl;
    this->closing = 1;
    delete this->socket_server;
}
