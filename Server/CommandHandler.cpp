#include "CommandHandler.h"
#include "Aux_Functions.h"
#include "dlib/string.h"
#include <boost/format.hpp>

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
    Player* dest;
    for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
    {
        dest = *i;
        send_command("player_list", dest);
        send_int(dest, this->serverState->plist.size()); // Number of players
        for (list<Player*>::iterator j = this->serverState->plist.begin() ; j != this->serverState->plist.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
        SendGameList(dest, &this->serverState->glist);
    }
}

void CommandHandler::GetPlayers(Player* player)
{
    send_command("player_list", player);
    send_int(player, this->serverState->plist.size()); // Number of players
    for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
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
    send_int(player, (new_game->loaded ? 1 : 0));
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
    else if ((int) game->plist.size() == game->n_players)
    {
        wcout << currentDateTime() << L"Player " << player->name << L" cant join game " << game->name << L" because is full" << endl;
        send_command("cant_join_game_full", player);
        send_int(player, get_utf8_length(game->name));
        send_wstring(player, game->name);
    }
    else
    {
        int error_code = (game->loaded ? savedgamesmgr->LoadPlayerData(game, player) : 0);
        if (error_code > 0)
        {
            send_command("cant_join_not_part_of_saved_game", player);
            send_int(player, get_utf8_length(game->name));
            send_wstring(player, game->name);
            send_int(player, error_code);
        }
        else
        {
            game->join(player);
            send_command("joined_game", player);
            send_int(player, get_utf8_length(game->name));
            send_wstring(player, game->name);
            send_int(player, game->chat->id);
            send_int(player, get_utf8_length(game->creator->name));
            send_wstring(player, game->creator->name);
            send_int(player, game->n_players);
            send_int(player, (game->loaded ? 1 : 0));
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
    }
}

void CommandHandler::StartGame(Player* player, Game* game, struct config configuration)
{
    if (game->creator != player) // Hack, trying to start a game not created by the player
        return kick_hacker(7, player);
    if (game->started || game->ended) // Hack, start a already started game or an ended game
        return kick_hacker(8, player);
    if (!game->loaded)
      game->set_players_money(configuration);
    game->start();
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
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
        for (list<Player*>::iterator j = game->plist.begin() ; j != game->plist.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
        if (game->loaded)
        {
            send_int(dest, get_utf8_length(game->current_player->name));
            send_wstring(dest, game->current_player->name);
            send_int(dest, game->last_auto_advance);
            send_int(dest, game->last_dice_res);
            PlayerGameState* state;
            for (list<Player*>::iterator j = game->plist.begin() ; j != game->plist.end() ; ++j)
            {
                state = (*j)->GetState(game->id);
                string player_data = str(boost::format("%d;%d;%d;%d;%d;%d;%d;%d") % state->num % state->position->number % (state->paid_last_turn ? 1 : 0)
                    % state->n_5000 % state->n_1000 % state->n_500 % state->n_100 % state->n_50);
                send_int(dest, player_data.length());
                send_string(dest, player_data);
            }
            for (vector<Hotel*>::iterator k = game->hlist.begin() ; k != game->hlist.end() ; ++k)
            {
                string hotel_data = str(boost::format("%s;%d;%d;%d;%d;%s") % utf16_to_utf8((*k)->name_txt) % ((*k)->owner != NULL ? (*k)->owner->GetState(game->id)->num : 0)
                    % (*k)->n_built_phases % ((*k)->entrance_bought_last_turn ? 1 : 0) % ((*k)->ground_bought ? 1 : 0) % (*k)->Create_entrance_list());
                send_int(dest, hotel_data.length());
                send_string(dest, hotel_data);
            }
        }
    }
    for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; i++)
        SendGameList((*i), &this->serverState->glist);
}

void CommandHandler::LeaveGame(Player* player, Game* game)
{
    Player* old_creator = game->creator;
    game->leave(player);
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = *i;
        send_command("chat_userlist", dest);
        send_int(dest, game->id);
        send_int(dest, game->plist.size()); // Number of players
        for (list<Player*>::iterator j = game->plist.begin() ; j != game->plist.end() ; ++j)
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
        for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; ++i)
        {
            dest = *i;
            SendGameList(dest, &this->serverState->glist);
        }
    }
    for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; i++)
        SendGameList((*i), &this->serverState->glist);
}

void CommandHandler::JoinGlobalChat(Player* player)
{
    this->serverState->global_chat_list.push_back(player);
    Player* dest;
    for (list<Player*>::iterator i = this->serverState->global_chat_list.begin() ; i != this->serverState->global_chat_list.end() ; ++i)
    {
        dest = *i;
        send_command("global_chat_userlist", dest);
        send_int(dest, this->serverState->global_chat_list.size()); // Number of players
        for (list<Player*>::iterator j = this->serverState->global_chat_list.begin() ; j != this->serverState->global_chat_list.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
    }
}

void CommandHandler::LeaveGlobalChat(Player* player)
{
    this->serverState->global_chat_list.remove(player);
    Player* dest;
    for (list<Player*>::iterator i = this->serverState->global_chat_list.begin() ; i != this->serverState->global_chat_list.end() ; ++i)
    {
        dest = *i;
        send_command("global_chat_userlist", dest);
        send_int(dest, this->serverState->global_chat_list.size()); // Number of players
        for (list<Player*>::iterator j = this->serverState->global_chat_list.begin() ; j != this->serverState->global_chat_list.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
    }
}

void CommandHandler::GetGlobalChatUsers(Player* player)
{
    send_command("global_chat_userlist", player);
    send_int(player, this->serverState->global_chat_list.size()); // Number of players
    for (list<Player*>::iterator i = this->serverState->global_chat_list.begin() ; i != this->serverState->global_chat_list.end() ; ++i)
    {
        send_int(player, get_utf8_length((*i)->name));
        send_wstring(player, (*i)->name);
    }
}

void CommandHandler::SendGlobalChatMsg(Player* player, wstring msg)
{
    Player* dest;
    for (list<Player*>::iterator i = this->serverState->global_chat_list.begin() ; i != this->serverState->global_chat_list.end() ; ++i)
    {
        dest = *i;
        send_command("new_global_chat_msg", dest);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, get_utf8_length(msg));
        send_wstring(dest, msg);
    }
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
    Player* dest;
    for (list<Player*>::iterator i = chat->players.begin() ; i != chat->players.end() ; ++i)
    {
        dest = *i;
        send_command("chat_userlist", dest);
        send_int(dest, chat->id);
        send_int(dest, chat->players.size()); // Number of players
        for (list<Player*>::iterator j = chat->players.begin() ; j != chat->players.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
    }
}

void CommandHandler::LeaveChat(Player* player, Chat* chat)
{
    if (!chat->leave(player)) // Possible hack
        return kick_hacker(4, player);
    Player* dest;
    for (list<Player*>::iterator i = chat->players.begin() ; i != chat->players.end() ; ++i)
    {
        dest = *i;
        send_command("chat_userlist", dest);
        send_int(dest, chat->id);
        send_int(dest, chat->players.size()); // Number of players
        for (list<Player*>::iterator j = chat->players.begin() ; j != chat->players.end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
    }
    delete_chat_if_empty(chat, &this->serverState->mutex_lists, &this->serverState->chat_list);
}

void CommandHandler::GetChatUsers(Player* player, Chat* chat)
{
    send_command("chat_userlist", player);
    send_int(player, chat->id);
    send_int(player, chat->players.size()); // Number of players
    for (list<Player*>::iterator i = chat->players.begin() ; i != chat->players.end() ; ++i)
    {
        send_int(player, get_utf8_length((*i)->name));
        send_wstring(player, (*i)->name);
    }
}

void CommandHandler::SendChatMsg(Player* player, Chat* chat, wstring msg)
{
    if (!chat->check_already_joined(player)) // hack, send messages to a chat the player hasn't joined: kick player
        return kick_hacker(5, player);
    if ((msg.length() > 1024) || (dlib::trim(msg).length() == 0)) // hack, send messages longer or shorter than limits
        return kick_hacker(6, player);
    Player* dest;
    for (list<Player*>::iterator i = chat->players.begin() ; i != chat->players.end() ; ++i)
    {
        dest = *i;
        send_command("new_chat_msg", dest);
        send_int(dest, chat->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, get_utf8_length(msg));
        send_wstring(dest, msg);
    }
}

void CommandHandler::Retire(Player* player, Game* game, int type, Player* receiving_player)
{
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
            next_player = game->turn_pass(&this->serverState->debt_mutex);
    }
    game->eliminate_player(player, receiving_player);
    Player* dest, * winner;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i) // Avoid sending commands to retired players, because their forms can be already closed and could lead to a client crash
    {
        dest = (*i);
        send_command("player_retired", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        if (type == 1) // Update affected players money
        {
            send_command("update_player_money", dest);
            send_int(dest, game->id);
            send_int(dest, get_utf8_length(receiving_player->name));
            send_wstring(dest, receiving_player->name);
            PlayerGameState* receiving_player_state = receiving_player->GetState(game->id);
            send_int(dest, receiving_player_state->n_50);
            send_int(dest, receiving_player_state->n_100);
            send_int(dest, receiving_player_state->n_500);
            send_int(dest, receiving_player_state->n_1000);
            send_int(dest, receiving_player_state->n_5000);
            send_command("update_player_money", dest);
            send_int(dest, game->id);
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
            send_int(dest, game->id);
            winner = game->get_winner();
            send_int(dest, get_utf8_length(winner->name));
            send_wstring(dest, winner->name);
        }
        else
        {
            if (next_player != NULL) // Turn was passed
            {
                send_command("turn_passed", dest);
                send_int(dest, game->id);
                send_int(dest, get_utf8_length(next_player->name));
                send_wstring(dest, next_player->name);
                send_int(dest, 0);
            }
        }
    }
}

void CommandHandler::RollDice(Player* player, Game* game, bool automatically)
{
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
    game->move_player(player, &this->serverState->debt_mutex);
    state->rolled_last_turn = true;
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("rolled_dice", dest);
        send_int(dest, game->id);
        send_int(dest, game->last_dice_res);
        send_int(dest, game->last_auto_advance);
        send_int(dest, state->position->number);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, (automatically ? 1 : 0));
    }
}

void CommandHandler::RollConstructionDice(Player* player, Game* game, Hotel* selected_hotel)
{
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
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; i++)
    {
        dest = (*i);
        send_command("rolled_construction_dice", dest);
        send_int(dest, game->id);
        send_int(dest, (int)construction_dice_res);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
    }
    if (construction_dice_res == Deny)
        state->built_last_turn = true; // Avoid hacking, because if construction is denied, the player can't try again in the same turn
    else if (construction_dice_res != Free)
    {
        // Create a debt for turn auto-passing
        if (construction_dice_res == Double)
            state->debt_last_turn = selected_hotel->Price_next_expansion() * 2;
        else
            state->debt_last_turn = selected_hotel->Price_next_expansion();
        state->hotel_to_be_extended = selected_hotel;
    }
}

void CommandHandler::PassTurn(Player* player, Game* game, bool automatically)
{
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
    Player* next_player = game->turn_pass(&this->serverState->debt_mutex);
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("turn_passed", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(next_player->name));
        send_wstring(dest, next_player->name);
        send_int(dest, (automatically ? 1 : 0));
    }
}

void CommandHandler::ChargeBank(Player* player, Game* game)
{
    if ((!game->started) || game->ended) // Hack, game not started yet or already ended
        return;
    PlayerGameState* state = player->GetState(game->id);
    if (state == NULL)
        return;
    if ((game->current_player != player) || (game->can_charge_bank(player) == false) || (state->charged_bank_last_turn)) // Hack, retire player
        return kick_hacker(21, player);
    else
        player->Charge_bank(game->id);
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, state->n_50);
        send_int(dest, state->n_100);
        send_int(dest, state->n_500);
        send_int(dest, state->n_1000);
        send_int(dest, state->n_5000);
    }
}

void CommandHandler::BuyHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50, int mode)
{
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
    int needed_money = (mode == 1 ? hotel->Calculate_price_with_everything() : hotel->price);
    if (total_selected < needed_money) // Hack, retire player
        return kick_hacker(28, player);
    if (mode == 0)
        hotel->Clear_phases_and_entrances();
    hotel->owner = player;
    player->Buy_hotel(game->id, hotel, n_5000, n_1000, n_500, n_100, n_50);
    // Calculate change
    if (total_selected > (hotel->price))
    {
        game->calculate_return(total_selected - hotel->price, &n_5000, &n_1000, &n_500, &n_100, &n_50);
        player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
    }
    state->bought_last_turn = true;
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("hotel_purchased", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, get_utf8_length(hotel_name));
        send_wstring(dest, hotel_name);
        send_int(dest, mode);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, state->n_50);
        send_int(dest, state->n_100);
        send_int(dest, state->n_500);
        send_int(dest, state->n_1000);
        send_int(dest, state->n_5000);
    }
}

void CommandHandler::ExpropriateHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
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
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("hotel_expropriated", dest);
        send_int(dest, game->id);
        send_int(dest, 0);// 0 = expropriation, 1 = auction
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, get_utf8_length(hotel_name));
        send_wstring(dest, hotel_name);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, state->n_50);
        send_int(dest, state->n_100);
        send_int(dest, state->n_500);
        send_int(dest, state->n_1000);
        send_int(dest, state->n_5000);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
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

void CommandHandler::BuildPhase(Player* player, Game* game, wstring hotel_name, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
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
    state->hotel_to_be_extended = NULL;
    state->built_last_turn = true;
    state->debt_last_turn = 0;
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("phase_built", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(hotel_name));
        send_wstring(dest, hotel_name);
        if (type == 2)
        {
            send_command("update_player_money", dest);
            send_int(dest, game->id);
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

void CommandHandler::BuyEntrance(Player* player, Game* game, wstring hotel_name, int position, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
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
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("entrance_added", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(hotel_name));
        send_wstring(dest, hotel_name);
        send_int(dest, position);
        if (type == 1)
        {
            send_command("update_player_money", dest);
            send_int(dest, game->id);
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

void CommandHandler::AskNights(Player* player, Game* game)
{
    if ((!game->started) || game->ended) // Hack, game not started yet or already ended
        return;
    PlayerGameState* state = player->GetState(game->id);
    if (state == NULL)
        return;
    if (game->current_player == player) // The client does not allow this
        return;
    int amount = 0, nights = 0;
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i) // Check if players are in entrances of hotels of the asking player
    {
        dest = (*i);
        if (dest != player) // Player doesn't have to pay himself :D
        {
            wstring hotel_name;
            amount = game->get_money_for_nights(player, dest, &nights, &hotel_name);
            if (amount > 0) // The player is in a entrance and hasn't paid this turn
            {
                this->serverState->debt_mutex.lock(); // To avoid creating a debt just when player is passing turn, because this command is asynchronous
                PlayerGameState* dest_state = dest->GetState(game->id);
                dest_state->debt_last_turn = amount;
                dest_state->debt_to_last_turn = player->GetState(game->id);
                dest_state->paid_last_turn = false;
                this->serverState->debt_mutex.unlock();
                for (list<Player*>::iterator j = game->plist.begin() ; j != game->plist.end() ; ++j)
                {
                    Player* destj = (*j);
                    send_command("ask_pay_nights", destj); // Will force the player to pay nights, if he hacks the game, in next turn pass he will be retired
                    send_int(destj, game->id);
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

void CommandHandler::PayNights(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
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
    if ((state->debt_last_turn == 0) || (state->debt_to_last_turn == NULL)) // Hack, retire player
        return kick_hacker(67, player);
    player->Pay_nights(game->id, state->debt_to_last_turn, n_5000, n_1000, n_500, n_100, n_50);
    state->paid_last_turn = true;
    // Calculate change
    if (total_selected > (state->debt_last_turn))
    {
        game->calculate_return(state->debt_to_last_turn, total_selected - state->debt_last_turn, &n_5000, &n_1000, &n_500, &n_100, &n_50);
        player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
    }
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, state->n_50);
        send_int(dest, state->n_100);
        send_int(dest, state->n_500);
        send_int(dest, state->n_1000);
        send_int(dest, state->n_5000);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(state->debt_to_last_turn->player->name));
        send_wstring(dest, state->debt_to_last_turn->player->name);
        send_int(dest, state->debt_to_last_turn->n_50);
        send_int(dest, state->debt_to_last_turn->n_100);
        send_int(dest, state->debt_to_last_turn->n_500);
        send_int(dest, state->debt_to_last_turn->n_1000);
        send_int(dest, state->debt_to_last_turn->n_5000);
    }
    this->serverState->debt_mutex.lock(); // To avoid creating a debt just when player is passing turn, because this command is asynchronous
    state->debt_last_turn = 0;
    state->debt_to_last_turn = NULL;
    this->serverState->debt_mutex.unlock();
}

void CommandHandler::AuctionStart(Player* player, Game* game, wstring hotel_name, int minimum_price)
{
    if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
        return kick_hacker(68, player);
    Hotel* hotel = get_hotel_from_name(hotel_name, game);
    if (hotel->owner != player) // Hack, retire player
        return kick_hacker(69, player);
    if (game->hotel_at_auction != NULL) // Hack, auction already in progress
        return kick_hacker(70, player);
    game->hotel_at_auction = hotel;
    game->best_bid = 0;
    game->auction_min_price = minimum_price;
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("auction_started", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(hotel->name_txt));
        send_wstring(dest, hotel->name_txt);
        send_int(dest, game->auction_min_price);
    }
}

void CommandHandler::AuctionBid(Player* player, Game* game, int amount)
{
    if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
        return kick_hacker(71, player);
    PlayerGameState* state = player->GetState(game->id);
    if (state == NULL)
        return;
    if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
        return kick_hacker(72, player);
    if (player == game->hotel_at_auction->owner) // Hack, retire player, he is trying to bid in his own auction
        return kick_hacker(73, player);
    if ((amount <= 0) || (amount < (game->best_bid)) || amount > (state->total_money)
       || ((amount % 50) != 0) || (amount < game->auction_min_price)) // Hack, retire player, invalid values
        return kick_hacker(74, player);
    game->best_bid = amount;
    game->best_bidder = player;
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("auction_bid_placed", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, amount);
    }
}

void CommandHandler::AuctionSell(Player* player, Game* game)
{
    if (!game->check_already_joined(player)) // Hack, retire player, he is not part of the game
        return kick_hacker(75, player);
    if (game->hotel_at_auction == NULL) // Hack, retire player, no auction in progress
        return kick_hacker(76, player);
    if (player != game->hotel_at_auction->owner) // Hack, retire player, he is trying to end the auction without being the owner
        return kick_hacker(77, player);
    if (game->best_bid == 0) // Hack, retire player, no one has placed a bid yet
        return kick_hacker(78, player);
    // Create a debt
    this->serverState->debt_mutex.lock();
    game->best_bidder->GetState(game->id)->debt_last_turn = game->best_bid;
    game->best_bidder->GetState(game->id)->debt_to_last_turn = game->hotel_at_auction->owner->GetState(game->id);
    this->serverState->debt_mutex.unlock();
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("auction_sold", dest);
        send_int(dest, game->id);
        send_int(dest, game->best_bid);
    }
}

void CommandHandler::AuctionPay(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50, bool automatically)
{
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
    player->Buy_hotel(game->id, game->hotel_at_auction, previous_owner, n_5000, n_1000, n_500, n_100, n_50, automatically);
    game->hotel_at_auction->owner = player;
    // Calculate change
    if (total_selected > (game->best_bid))
    {
        game->calculate_return(previous_owner->GetState(game->id), total_selected - game->best_bid, &n_5000, &n_1000, &n_500, &n_100, &n_50);
        player->Return_change(game->id, n_5000, n_1000, n_500, n_100, n_50);
    }
    Player* dest;
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("hotel_expropriated", dest);
        send_int(dest, game->id);
        send_int(dest, 1);// 0 = expropriation, 1 = auction
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, get_utf8_length(game->hotel_at_auction->name_txt));
        send_wstring(dest, game->hotel_at_auction->name_txt);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(player->name));
        send_wstring(dest, player->name);
        send_int(dest, state->n_50);
        send_int(dest, state->n_100);
        send_int(dest, state->n_500);
        send_int(dest, state->n_1000);
        send_int(dest, state->n_5000);
        send_command("update_player_money", dest);
        send_int(dest, game->id);
        send_int(dest, get_utf8_length(previous_owner->name));
        send_wstring(dest, previous_owner->name);
        send_int(dest, previous_owner->GetState(game->id)->n_50);
        send_int(dest, previous_owner->GetState(game->id)->n_100);
        send_int(dest, previous_owner->GetState(game->id)->n_500);
        send_int(dest, previous_owner->GetState(game->id)->n_1000);
        send_int(dest, previous_owner->GetState(game->id)->n_5000);
    }
    // If this command is sent before updating hotels and money to everyone, the client doesn't know who owns the hotels
    for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
    {
        dest = (*i);
        send_command("auction_ended", dest);
        send_int(dest, game->id);
        send_int(dest, (automatically ? 1 : 0));
    }
    // Clear debt of player
    this->serverState->debt_mutex.lock();
    game->best_bidder->GetState(game->id)->debt_last_turn = 0;
    game->best_bidder->GetState(game->id)->debt_to_last_turn = NULL;
    this->serverState->debt_mutex.unlock();
    game->best_bidder = NULL;
    game->best_bid = 0;
    game->hotel_at_auction = NULL;
}

void CommandHandler::SaveGame(Player* player, Game* game, wstring password)
{
    game->password = password;
    if (savedgamesmgr->SaveGame(game, player))
    {
        for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; i++)
        {
            send_command("game_saved", (*i));
            send_int((*i), game->id);
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
        send_int(player, game->id);
    }
}

void CommandHandler::LoadGame(Player* player, int id, Game* game, wstring password)
{
    if (game != NULL) // Game already loaded
    {
        send_command("cannot_load_game", player);
        send_int(player, 1); // Error code = 1
        send_int(player, game->id);
        send_int(player, get_utf8_length(game->creator->name));
        send_wstring(player, game->creator->name);
    }
    else
    {
        int error_code = 0;
        game = savedgamesmgr->LoadGame(id, password, player, &this->serverState->mutex_ids, &this->serverState->id_count, this->serverState->random_gen, &error_code);
        if (game != NULL)
        {
            this->serverState->glist.push_back(game);
            send_command("game_loaded", player);
            for (list<Player*>::iterator i = this->serverState->plist.begin() ; i != this->serverState->plist.end() ; i++)
                SendGameList((*i), &this->serverState->glist);
            send_command("joined_game", player);
            send_int(player, get_utf8_length(game->name));
            send_wstring(player, game->name);
            send_int(player, game->chat->id);
            send_int(player, get_utf8_length(game->creator->name));
            send_wstring(player, game->creator->name);
            send_int(player, game->n_players);
            send_int(player, (game->loaded ? 1 : 0));
        }
        else
        {
            send_command("cannot_load_game", player);
            send_int(player, error_code);
            send_int(player, id);
        }
    }
}

void CommandHandler::CheckForTurnExpirations()
{
    while (this->serverState->running)
    {
        this->serverState->mutex_lists.lock();
        this->serverState->debt_mutex.lock();
        Game* game;
        for (list<Game*>::iterator i = this->serverState->glist.begin() ; i != this->serverState->glist.end() ; i++)
        {
            // Start a timer for this turn. It will be passed automatically in case the player is AFK
            game = (*i);
            if (game->started && !game->ended)
            {
                game->seconds_elapsed_last_command++;
                if (game->seconds_elapsed_last_command > 300)
                {
                    game->seconds_elapsed_last_command = 0;
                    wcout << currentDateTime() << L"Player " << game->current_player->name << " is AFK. Passing the turn automatically for game " << game->id << endl;
                    PlayerGameState* curr_p_state = game->current_player->GetState(game->id);
                    if (!curr_p_state->rolled_last_turn)
                        this->RollDice(game->current_player, game, true);
                    // Check for auctions
                    if (game->hotel_at_auction != NULL)
                    {
                        wcout << currentDateTime() << L"Player " << game->current_player->name << " has an auction in progress, ending it." << endl;
                        // If there is a best bidder, make him pay before ending the auction
                        if (game->best_bidder != NULL)
                        {
                            wcout << currentDateTime() << L"Player " << game->best_bidder->name << " will pay " << game->best_bid << " to player " << game->current_player->name << " automatically for the auction" << endl;
                            int n_5000, n_1000, n_500, n_100, n_50;
                            game->calculate_return(game->best_bidder->GetState(game->id), game->best_bid, &n_5000, &n_1000, &n_500, &n_100, &n_50);
                            this->AuctionPay(game->best_bidder, game, n_5000, n_1000, n_500, n_100, n_50, true);
                        }
                        else
                        {
                            // If there is no best bidder, the auction can be cancelled directly
                            for (list<Player*>::iterator i = game->plist.begin() ; i != game->plist.end() ; ++i)
                            {
                                send_command("auction_ended", (*i));
                                send_int((*i), game->id);
                                // Ended automatically
                                send_int((*i), 1);
                            }
                            game->hotel_at_auction = NULL;
                        }
                    }
                    if (curr_p_state->debt_last_turn > 0)
                    {
                        if (curr_p_state->debt_to_last_turn != NULL)
                            wcout << currentDateTime() << L"Player " << game->current_player->name << " will pay " << curr_p_state->debt_last_turn << " to " << curr_p_state->debt_to_last_turn->player->name << " automatically" << endl;
                        else
                            wcout << currentDateTime() << L"Player " << game->current_player->name << " will pay " << curr_p_state->debt_last_turn << " automatically because of pending debt to the bank" << endl;
                        if (curr_p_state->total_money < curr_p_state->debt_last_turn)
                        {
                            // Retire player, he cannot pay the debt. The turn is automatically passed when the current player retires
                            this->Retire(curr_p_state->player, game, 1, curr_p_state->debt_to_last_turn->player);
                        }
                        else
                        {
                            int n_5000, n_1000, n_500, n_100, n_50;
                            game->calculate_return(curr_p_state, curr_p_state->debt_last_turn, &n_5000, &n_1000, &n_500, &n_100, &n_50);
                            if (curr_p_state->debt_to_last_turn != NULL)
                                curr_p_state->debt_to_last_turn->Return_change(n_5000, n_1000, n_500, n_100, n_50);
                            // Check if an hotel is about to be extended and extend it
                            if (curr_p_state->hotel_to_be_extended != NULL)
                            {
                                if (curr_p_state->hotel_to_be_extended->Can_extend())
                                    curr_p_state->hotel_to_be_extended->Extend();
                            }
                            Player* dest;
                            for (list<Player*>::iterator j = game->plist.begin() ; j != game->plist.end() ; ++j)
                            {
                                dest = (*j);
                                if (curr_p_state->hotel_to_be_extended != NULL)
                                {
                                    wcout << currentDateTime() << L"Phase of hotel " << curr_p_state->hotel_to_be_extended->name_txt << " built automatically because of pending debt to the bank" << endl;
                                    send_command("phase_built", dest);
                                    send_int(dest, game->id);
                                    send_int(dest, get_utf8_length(curr_p_state->hotel_to_be_extended->name_txt));
                                    send_wstring(dest, curr_p_state->hotel_to_be_extended->name_txt);
                                }
                                // Update the money of current player
                                send_command("update_player_money", dest);
                                send_int(dest, game->id);
                                send_int(dest, get_utf8_length(curr_p_state->player->name));
                                send_wstring(dest, curr_p_state->player->name);
                                send_int(dest, curr_p_state->n_50);
                                send_int(dest, curr_p_state->n_100);
                                send_int(dest, curr_p_state->n_500);
                                send_int(dest, curr_p_state->n_1000);
                                send_int(dest, curr_p_state->n_5000);
                                if (curr_p_state->debt_to_last_turn != NULL)
                                {
                                    // Update the money of the player that received the debt quantity
                                    send_command("update_player_money", dest);
                                    send_int(dest, game->id);
                                    send_int(dest, get_utf8_length(curr_p_state->debt_to_last_turn->player->name));
                                    send_wstring(dest, curr_p_state->debt_to_last_turn->player->name);
                                    send_int(dest, curr_p_state->debt_to_last_turn->n_50);
                                    send_int(dest, curr_p_state->debt_to_last_turn->n_100);
                                    send_int(dest, curr_p_state->debt_to_last_turn->n_500);
                                    send_int(dest, curr_p_state->debt_to_last_turn->n_1000);
                                    send_int(dest, curr_p_state->debt_to_last_turn->n_5000);
                                }
                            }
                            curr_p_state->hotel_to_be_extended = NULL;
                            curr_p_state->debt_to_last_turn = NULL;
                            curr_p_state->debt_last_turn = 0;
                            this->PassTurn(game->current_player, game, true);
                        }
                    }
                    else
                    {
                        this->PassTurn(game->current_player, game, true);
                    }
                }
            }
        }
        this->serverState->mutex_lists.unlock();
        this->serverState->debt_mutex.unlock();
        #ifdef _WIN32
            Sleep(1000);
        #else
            sleep(1);
        #endif
    }
}

CommandHandler::CommandHandler(void)
{
    this->savedgamesmgr = new SavedgamesMgr();
    this->serverState = new ServerState();
    dlib::create_new_thread(CheckForTurnExpirationsWrapper, (void*) this);
}

CommandHandler::~CommandHandler(void)
{
    delete this->savedgamesmgr;
    delete this->serverState;
}