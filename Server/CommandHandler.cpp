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

void CommandHandler::GetPlayers(Player* player){}
void CommandHandler::GetGames(Player* player){}
void CommandHandler::CreateGame(Player* player, wstring name, int n_players){}
void CommandHandler::JoinGame(Player* player, wstring name){}
void CommandHandler::StartGame(Player* player, int id){}
void CommandHandler::LeaveGame(Player* player, int id){}
void CommandHandler::JoinGlobalChat(Player* player){}
void CommandHandler::LeaveGlobalChat(Player* player){}
void CommandHandler::GetGlobalChatUsers(Player* player){}
void CommandHandler::SendGlobalChatMsg(Player* player){}
void CommandHandler::CreateChat(Player* player, int quantity){}
void CommandHandler::JoinChat(Player* player, int id){}
void CommandHandler::LeaveChat(Player* player, int id){}
void CommandHandler::GetChatUsers(Player* player, int id){}
void CommandHandler::SendChatMsg(Player* player, int id, wstring msg){}
void CommandHandler::Retire(Player* player, Game game, int type, wstring receiver_name){}

void CommandHandler::RollDice(Player* player, Game game){}
void CommandHandler::RollConstructionDice(Player* player, Game game){}
void CommandHandler::PassTurn(Player* player, Game game){}
void CommandHandler::ChargeBank(Player* player, Game* game){}
void CommandHandler::BuyHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50){}
void CommandHandler::ExpropriateHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50){}
void CommandHandler::BuildPhase(Player* player, Game* game, wstring hotel_name, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50){}
void CommandHandler::BuildPhase(Player* player, Game* game, wstring hotel_name, int position, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50){}
void CommandHandler::AskNights(Player* player, Game* game){}
void CommandHandler::PayNights(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50){}
void CommandHandler::AuctionStart(Player* player, Game* game, wstring hotel_name){}
void CommandHandler::AuctionBid(Player* player, Game* game, int amount){}
void CommandHandler::AuctionSell(Player* player, Game* game){}
void CommandHandler::AuctionPay(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50){}

void CommandHandler::SaveGame(Player* player, Game* game, wstring password){}
void CommandHandler::LoadGame(Player* player, Game* game, wstring password){}


CommandHandler::CommandHandler(void){}

CommandHandler::~CommandHandler(void){}