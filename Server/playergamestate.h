#pragma once
#include <list>
#include "hotel.h"
#include "position.h"

using namespace std;

class Player;

class PlayerGameState
{
public:
    Player* player;
    int bd_id;
    bool active, rolled_last_turn, bought_last_turn, built_last_turn, charged_bank_last_turn, paid_last_turn, free_entrance_used;
    int debt_last_turn;
    PlayerGameState* debt_nights_to_last_turn;
    int n_5000, n_1000, n_500, n_100, n_50;
    int total_money;
    int num;
    Position* position;
    list<Hotel*> hotels;

    PlayerGameState(Player* player);
    void Calculate_total_money();
    void Charge_bank();
    void Buy_hotel(Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Buy_hotel(Hotel* hotel, PlayerGameState* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Expropriate_hotel(Hotel* hotel);
    void Set_money(int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Pay_nights(PlayerGameState* to_player, int n5000, int n1000, int n500, int n100, int n50);
    void Return_change(int n_5000, int n_1000, int n_500, int n_100, int n_50);
    void Pay_phase_or_entrance(int n5000, int n1000, int n500, int n100, int n50);
    void Take_5000_without_having_b5000(int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50);
    void Take_1000_without_having_b1000(int* n_1000, int* n_500, int* n_100, int* n_50);
    void Take_500_without_having_b500(int* n_500, int* n_100, int* n_50);
    void Take_100_without_having_b100(int* n_100, int* n_50);
    void Take_50_without_having_b50(int* n_50);
    ~PlayerGameState(void);
};