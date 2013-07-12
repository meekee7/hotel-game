#include "CommandHandler.h"
#include "Aux_Functions.h"

void CommandHandler::Disconnect(Player* player, list<Player*>* plist, list<Game*>* glist)
{
    send_command("#disconnect#", player);
    // Send player list to all players, so they are notified about the diconnected user
    // Send as much strings as connected players, with a count first
    list<Player*>::iterator i, j;
    Player* dest;
    for (i = plist->begin() ; i != plist->end() ; ++i)
    {
        dest = *i;
        send_command("player_list", dest);
        send_int(dest, plist->size()); // Number of players
        for (j = plist->begin() ; j != plist->end() ; ++j)
        {
            send_int(dest, get_utf8_length((*j)->name));
            send_wstring(dest, (*j)->name);
        }
        SendGameList(dest, glist);
    }
}

void CommandHandler::GetPlayers(Player* player, list<Player*>* plist){}
void CommandHandler::GetGames(Player* player, list<Game*>* glist){}
void CommandHandler::CreateGame(Player* player, list<Player*>* plist, list<Game*>* glist, wstring name, int n_players){}
void CommandHandler::JoinGame(Player* player, list<Player*>* plist, list<Game*>* glist, wstring name){}
void CommandHandler::StartGame(Player* player, list<Player*>* plist, list<Game*>* glist, int id){}
void CommandHandler::LeaveGame(Player* player, list<Player*>* plist, list<Game*>* glist, int id){}
void CommandHandler::JoinGlobalChat(Player* player, list<Player*>* global_chat_list){}
void CommandHandler::LeaveGlobalChat(Player* player, list<Player*>* global_chat_list){}
void CommandHandler::GetGlobalChatUsers(Player* player, list<Player*>* global_chat_list){}
void CommandHandler::SendGlobalChatMsg(Player* player, list<Player*>* global_chat_list){}
void CommandHandler::CreateChat(Player* player, list<Player*>* plist, list<Chat*>* clist, int quantity){}
void CommandHandler::JoinChat(Player* player, list<Game*>* glist, list<Chat*>* clist, int id){}
void CommandHandler::LeaveChat(Player* player, list<Game*>* glist, list<Chat*>* clist, int id){}
void CommandHandler::GetChatUsers(Player* player, list<Game*>* glist, list<Chat*>* clist, int id){}
void CommandHandler::SendChatMsg(Player* player, list<Game*>* glist, list<Chat*>* clist, int id, wstring msg){}
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