#include "CommandHandler.h"
#include "Aux_Functions.h"

// Retire from all games and disconnect him using existing function
void CommandHandler::kick_hacker(int reason, Player* p)
{
    wcout << currentDateTime() << "Kicking player " << p->name << " for cheating. Reason code: " << reason << " (See source code for code correspondence)" << endl;
    wofstream kick_log;
    kick_log.open("kick_log.log", wofstream::app);
    kick_log << L"Player " << p->name << L" kicked. Reason: " << reason << L". Date and time: " << currentDateTime() << endl;
    kick_log.close();
    send_command("#disconnect#", p);
    disconnect_client(p, true, this->serverState);
}

void CommandHandler::Disconnect(Player* player)
{
    disconnect_client(player, false, this->serverState);
    send_command("#disconnect#", player);
    // Send player list to all players, so they are notified about the diconnected user
    // Send as much strings as connected players, with a count first
    list<Player*>::iterator i, j;
    Player* dest;
    for (i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
    {
        dest = *i;
        send_command("player_list", dest);
        send_int(dest, this->serverState->plist.size()); // Number of players
        for (j = this->serverState->plist.begin() ; j != this->serverState->plist.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
        SendGameList(dest, &this->serverState->glist);
    }
}

void CommandHandler::GetPlayers(Player* player)
{
    list<Player*>::iterator i;
    send_command("player_list", player);
    send_int(player, this->serverState->plist.size()); // Number of players
    for (i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
    {
        send_int(player, get_utf8_length((*i)->name));
        send_wstring(player, (*i)->name);
    }
}

void CommandHandler::GetGames(Player* player)
{
    SendGameList(player, &this->serverState->glist);
}

void CommandHandler::CreateGame(Player* player, wstring name, int n_players)
{
    if (name.empty()) // Hack, kick player
        return kick_hacker(1, player);
    if (get_game_from_name(name, &this->serverState->glist) != NULL) // Repeated name
        return;
    Game* new_game = new Game(name, n_players, player, &this->serverState->mutex_ids, &this->serverState->id_count, this->serverState->random_gen, false);
    wcout << currentDateTime() << L"New game! Name: " << name << " (ID " << new_game->id << ") | Number of players: " << n_players << endl;
    this->serverState->glist.push_back(new_game);
    send_command("joined_game", player);
    send_int(player, get_utf8_length(name));
    send_wstring(player, name);
    send_int(player, new_game->chat->id);
    send_int(player, get_utf8_length(new_game->creator->name));
    send_wstring(player, new_game->creator->name);
    send_int(player, new_game->n_players);
    for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
        SendGameList((*i), &this->serverState->glist);
}

void CommandHandler::JoinGame(Player* player, Game* game)
{
    if (game->check_already_joined(player))
    {
        send_command("cant_join_already_joined", player);
        send_int(player, get_utf8_length(game->name));
        send_wstring(player, game->name);
    }
    else if (game->join(player))
    {
        send_command("joined_game", player);
        send_int(player, get_utf8_length(game->name));
        send_wstring(player, game->name);
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
        for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; i++)
            SendGameList((*i), &this->serverState->glist);
    }
    else if (game->started)
    {
        send_command("cant_join_game_started", player);
        send_int(player, get_utf8_length(game->name));
        send_wstring(player, game->name);
    }
    else if (game->ended)
    {
        send_command("cant_join_game_ended", player);
        send_int(player, get_utf8_length(game->name));
        send_wstring(player, game->name);
    }
    else
    {
        send_command("cant_join_game_full", player);
        send_int(player, get_utf8_length(game->name));
        send_wstring(player, game->name);
    }
}

void CommandHandler::StartGame(Player* player, Game* game, struct config configuration)
{
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
        send_int(dest, game->id);
        send_int(dest, game->n_players);
        // Send configuration
        send_int(dest, configuration.config_content.size());
        send_string(dest, configuration.config_content);
        send_int(dest, game->starting_player);
        // Send player list
        send_int(dest, game->plist.size());
        for (j = game->plist.begin() ; j != game->plist.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
    }
    for (i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; i++)
        SendGameList((*i), &this->serverState->glist);
}

void CommandHandler::LeaveGame(Player* player, Game* game)
{
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
    if (delete_game_if_empty(game, &this->serverState->mutex_lists, &this->serverState->glist))
    {
        for (i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
        {
            dest = *i;
            SendGameList(dest, &this->serverState->glist);
        }
    }
    for (i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; i++)
        SendGameList((*i), &this->serverState->glist);
}

void CommandHandler::JoinGlobalChat(Player* player)
{
    this->serverState->global_chat_list.push_back(player);
    list<Player*>::iterator i, j;
    Player* dest;
    for (i = this->serverState->global_chat_list.begin() ; i != this->serverState->global_chat_list.end() ; ++i)
    {
        dest = *i;
        send_command("global_chat_userlist", dest);
        send_int(dest, this->serverState->global_chat_list.size()); // Number of players
        for (j = this->serverState->global_chat_list.begin() ; j != this->serverState->global_chat_list.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
    }
}

void CommandHandler::LeaveGlobalChat(Player* player)
{
}

void CommandHandler::GetGlobalChatUsers(Player* player)
{
}

void CommandHandler::SendGlobalChatMsg(Player* player)
{
}

void CommandHandler::CreateChat(Player* player, int quantity, vector<wstring> player_list)
{
    Chat* new_chat = new Chat(player, true, &this->serverState->mutex_ids, &this->serverState->id_count);
    this->serverState->chat_list.push_back(new_chat);
    wcout << currentDateTime() << L"New chat with ID " << new_chat->id << ". Number of players: " << player_list.size() << endl;
    // Send commands to selected players to ask them to join the chat
    Player* dest;
    for (vector<wstring>::iterator i = player_list.begin() ; i != player_list.end() ; ++i)
    {
        dest = get_player_from_name(*i, &this->serverState->plist);
        // The requested player can disconnect while processing this command
        if (dest == NULL)
            continue;
        send_command("ask_join_chat", dest);
        send_int(dest, new_chat->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
    }
}

void CommandHandler::JoinChat(Player* player, Chat* chat)
{
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

void CommandHandler::LeaveChat(Player* player, Chat* chat)
{
}

void CommandHandler::GetChatUsers(Player* player, Chat* chat)
{
}

void CommandHandler::SendChatMsg(Player* player, Chat* chat, wstring msg)
{
}

void CommandHandler::Retire(Player* player, Game game, int type, wstring receiver_name)
{
}

void CommandHandler::RollDice(Player* player, Game game)
{
}

void CommandHandler::RollConstructionDice(Player* player, Game game)
{
}

void CommandHandler::PassTurn(Player* player, Game game)
{
}

void CommandHandler::ChargeBank(Player* player, Game* game)
{
}

void CommandHandler::BuyHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
}

void CommandHandler::ExpropriateHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
}

void CommandHandler::BuildPhase(Player* player, Game* game, wstring hotel_name, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
}

void CommandHandler::BuildPhase(Player* player, Game* game, wstring hotel_name, int position, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
}

void CommandHandler::AskNights(Player* player, Game* game)
{
}

void CommandHandler::PayNights(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
}

void CommandHandler::AuctionStart(Player* player, Game* game, wstring hotel_name)
{
}

void CommandHandler::AuctionBid(Player* player, Game* game, int amount)
{
}

void CommandHandler::AuctionSell(Player* player, Game* game)
{
}

void CommandHandler::AuctionPay(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
}


void CommandHandler::SaveGame(Player* player, Game* game, wstring password)
{
}

void CommandHandler::LoadGame(Player* player, Game* game, wstring password)
{
}

CommandHandler::CommandHandler(void)
{
}

CommandHandler::~CommandHandler(void)
{
}