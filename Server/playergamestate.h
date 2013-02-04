#pragma once
#include <list>
#include "position.h"

using namespace std;

class Hotel;
class Player;

class PlayerGameState
{
public:
    //bool active;
    bool rolled_last_turn, bought_last_turn, built_last_turn, charged_bank_last_turn, paid_last_turn, free_entrance_used;
    int debt_last_turn;
    Player* debt_nights_to_last_turn;
    int n_5000, n_1000, n_500, n_100, n_50;
    int total_money;
    int num;
    Position* position;
    list<Hotel*> hotels;

    PlayerGameState(void);
    ~PlayerGameState(void);
};