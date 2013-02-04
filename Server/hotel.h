#pragma once
#include <vector>
#include <list>
#include <algorithm>
#include <string>
#include "types.h"
#include "position.h"

using namespace std;

class Player;

class Hotel
{
public:
    THotel_name name;
    wstring name_txt;
    int price, expropriation_price;
    bool ground_bought;
    bool entrance_bought_last_turn;
    bool next_expansion_is_ground;
    int n_max_phases;
    int n_built_phases;
    int entrance_price;
    int n_entrances;
    list<int> entrances;
    Player* owner;
    vector<vector<int> > prices_matrix;
    vector<int> phases_prices;

    Hotel(THotel_name real_name);
    void Extend();
    void Add_entrance(int position);
    bool Has_entrance_in_position(int position);
    bool Is_a_valid_entrance_position(Position* position);
    int Price_next_expansion();
    bool Can_extend();
    int Calculate_nights(int number);
    void Return_to_bank();
    ~Hotel(void);
};

