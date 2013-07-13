#pragma once
#include "Player.h"
#include "Game.h"
#include "Chat.h"
#include "PlayerGameState.h"
#include "Types.h"
#include "ServerState.h"
#include "Aux_Functions.h"

class CommandHandler
{
public:
    CommandHandler(void);
    ~CommandHandler(void);
    ServerState* serverState;

    void kick_hacker(int reason, Player* p);
    void Disconnect(Player* player);
    void GetPlayers(Player* player);
    void GetGames(Player* player);
    void CreateGame(Player* player, wstring name, int n_players);
    void JoinGame(Player* player, Game* game);
    void StartGame(Player* player, Game* game, struct config configuration);
    void LeaveGame(Player* player, Game* game);
    void JoinGlobalChat(Player* player);
    void LeaveGlobalChat(Player* player);
    void GetGlobalChatUsers(Player* player);
    void SendGlobalChatMsg(Player* player);
    void CreateChat(Player* player, int quantity, vector<wstring> player_list);
    void JoinChat(Player* player, Chat* chat);
    void LeaveChat(Player* player, Chat* chat);
    void GetChatUsers(Player* player, Chat* chat);
    void SendChatMsg(Player* player, Chat* chat, wstring msg);
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