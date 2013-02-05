#pragma once
#include <string>
#include <list>
#include <map>
#include "portable_socket.h"
#include "types.h"
#include "position.h"
#include "playergamestate.h"
#include "hotel.h"

using namespace std;

class Player
{
public:
    wstring name;
    string ip;
    Portable_socket* socket;
    bool connected;
    //bool active;
    /*bool rolled_last_turn, bought_last_turn, built_last_turn, charged_bank_last_turn, paid_last_turn, free_entrance_used;
    int debt_last_turn;
    Player* debt_nights_to_last_turn;
    int n_5000, n_1000, n_500, n_100, n_50;
    int total_money;
    int num;
    Position* position;
    list<Hotel*> hotels;*/
    map<int, PlayerGameState*>* games_states;

    Player(string ip, Portable_socket* socket_client);
    void Join_Game(int game_id);
    PlayerGameState* GetState(int game_id);
    void Calculate_total_money(int game_id);
    void Charge_bank(int game_id);
    void Buy_hotel(int game_id, Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Buy_hotel(int game_id, Hotel* hotel, Player* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Expropriate_hotel(int game_id, Hotel* hotel);
    void Set_money(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Pay_nights (int game_id, PlayerGameState* to_player, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Return_change(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Pay_phase_or_entrance(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Take_5000_without_having_b5000(int game_id, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50);
    void Take_1000_without_having_b1000(int game_id, int* n_1000, int* n_500, int* n_100, int* n_50);
    void Take_500_without_having_b500(int game_id, int* n_500, int* n_100, int* n_50);
    void Take_100_without_having_b100(int game_id, int* n_100, int* n_50);
    void Take_50_without_having_b50(int game_id, int* n_50);
    ~Player(void);
};

