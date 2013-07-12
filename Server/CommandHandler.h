#pragma once
#include "Player.h"
#include "Game.h"
#include "Chat.h"
#include "PlayerGameState.h"
#include "Types.h"

class CommandHandler
{
public:
    CommandHandler(void);
    ~CommandHandler(void);

    void Disconnect(Player* player, list<Player*>* plist, list<Game*>* glist);
    void GetPlayers(Player* player, list<Player*>* plist);
    void GetGames(Player* player, list<Game*>* glist);
    void CreateGame(Player* player, list<Player*>* plist, list<Game*>* glist, wstring name, int n_players);
    void JoinGame(Player* player, list<Player*>* plist, list<Game*>* glist, wstring name);
    void StartGame(Player* player, list<Player*>* plist, list<Game*>* glist, int id);
    void LeaveGame(Player* player, list<Player*>* plist, list<Game*>* glist, int id);
    void JoinGlobalChat(Player* player, list<Player*>* global_chat_list);
    void LeaveGlobalChat(Player* player, list<Player*>* global_chat_list);
    void GetGlobalChatUsers(Player* player, list<Player*>* global_chat_list);
    void SendGlobalChatMsg(Player* player, list<Player*>* global_chat_list);
    void CreateChat(Player* player, list<Player*>* plist, list<Chat*>* clist, int quantity);
    void JoinChat(Player* player, list<Game*>* glist, list<Chat*>* clist, int id);
    void LeaveChat(Player* player, list<Game*>* glist, list<Chat*>* clist, int id);
    void GetChatUsers(Player* player, list<Game*>* glist, list<Chat*>* clist, int id);
    void SendChatMsg(Player* player, list<Game*>* glist, list<Chat*>* clist, int id, wstring msg);
    void Retire(Player* player, Game game, int type, wstring receiver_name);

    void RollDice(Player* player, Game game);
    void RollConstructionDice(Player* player, Game game);
    void PassTurn(Player* player, Game game);
    void ChargeBank(Player* player, Game* game);
    void BuyHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void ExpropriateHotel(Player* player, Game* game, wstring hotel_name, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void BuildPhase(Player* player, Game* game, wstring hotel_name, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void BuildPhase(Player* player, Game* game, wstring hotel_name, int position, int type, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void AskNights(Player* player, Game* game);
    void PayNights(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void AuctionStart(Player* player, Game* game, wstring hotel_name);
    void AuctionBid(Player* player, Game* game, int amount);
    void AuctionSell(Player* player, Game* game);
    void AuctionPay(Player* player, Game* game, int n_5000, int n_1000, int n_500, int n_100, int n_50);

    void SaveGame(Player* player, Game* game, wstring password);
    void LoadGame(Player* player, Game* game, wstring password);
};