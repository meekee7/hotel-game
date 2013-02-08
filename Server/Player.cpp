#include "Player.h"

Player::Player(string ip, Portable_socket* socket_client)
{
    this->ip = ip;
    this->socket = socket_client;
    this->connected = true;
    this->games_states = new map<int, PlayerGameState*>();
}

void Player::Join_Game(int game_id)
{
    if (this->games_states->find(game_id) == this->games_states->end())
        this->games_states->insert(pair<int, PlayerGameState*>(game_id, new PlayerGameState(this)));
}

PlayerGameState* Player::GetState(int game_id)
{
    map<int, PlayerGameState*>::iterator i = this->games_states->find(game_id);
    if (i != this->games_states->end())
        return i->second;
    else
        return NULL;
}

void Player::Calculate_total_money(int game_id)
{
    //this->total_money = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
    this->GetState(game_id)->Calculate_total_money();
}

void Player::Charge_bank(int game_id)
{
    this->GetState(game_id)->Charge_bank();
    /*this->n_1000 += 2;
    this->Calculate_total_money(game_id);
    this->charged_bank_last_turn = true;*/
}

void Player::Buy_hotel(int game_id, Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Buy_hotel(hotel, n_5000, n_1000, n_500, n_100, n_50);
    // Owner is set by caller thread because incomplete class Hotel
    /*this->hotels.push_back(hotel);
    this->n_5000 -= n_5000;
    this->n_1000 -= n_1000;
    this->n_500 -= n_500;
    this->n_100 -= n_100;
    this->n_50 -= n_50;
    this->Calculate_total_money(game_id);*/
}

void Player::Buy_hotel(int game_id, Hotel* hotel, Player* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Buy_hotel(hotel, previous_owner->GetState(game_id), n_5000, n_1000, n_500, n_100, n_50);
    // Owner is set by caller thread because incomplete class Hotel
    /*this->hotels.push_back(hotel);
    this->n_5000 -= n_5000;
    this->n_1000 -= n_1000;
    this->n_500 -= n_500;
    this->n_100 -= n_100;
    this->n_50 -= n_50;
    this->Calculate_total_money(game_id);
    previous_owner->n_5000 += n_5000;
    previous_owner->n_1000 += n_1000;
    previous_owner->n_500 += n_500;
    previous_owner->n_100 += n_100;
    previous_owner->n_50 += n_50;
    previous_owner->Calculate_total_money(game_id);*/
}

void Player::Expropriate_hotel(int game_id, Hotel* hotel)
{
    this->GetState(game_id)->Expropriate_hotel(hotel);
    // Money its already calculated, client does the math
    //this->hotels.remove(hotel);
}

void Player::Set_money(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Set_money(n_5000, n_1000, n_500, n_100, n_50);
    /*this->n_5000 = n_5000;
    this->n_1000 = n_1000;
    this->n_500 = n_500;
    this->n_100 = n_100;
    this->n_50 = n_50;
    this->Calculate_total_money(game_id);*/
}

void Player::Pay_nights (int game_id, PlayerGameState* to_player, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Pay_nights(to_player, n_5000, n_1000, n_500, n_100, n_50);
    // Transfiere los fondos del jugador que paga al que cobra, el precio viene calculado de la IU
    /*this->n_5000 -= n5000;
    this->n_1000 -= n1000;
    this->n_500 -= n500;
    this->n_100 -= n100;
    this->n_50 -= n50;
    this->Calculate_total_money(game_id);
    to_player->n_5000 += n5000;
    to_player->n_1000 += n1000;
    to_player->n_500 += n500;
    to_player->n_100 += n100;
    to_player->n_50 += n50;
    to_player->Calculate_total_money(game_id);*/
}

void Player::Return_change(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->games_states->find(game_id)->second->Return_change(n_5000, n_1000, n_500, n_100, n_50);
    /*this->n_5000 += n_5000;
    this->n_1000 += n_1000;
    this->n_500 += n_500;
    this->n_100 += n_100;
    this->n_50 += n_50;
    this->Calculate_total_money(game_id);*/
}

void Player::Pay_phase_or_entrance(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Pay_phase_or_entrance(n_5000, n_1000, n_500, n_100, n_50);
    /*this->n_5000 -= n5000;
    this->n_1000 -= n1000;
    this->n_500 -= n500;
    this->n_100 -= n100;
    this->n_50 -= n50;
    this->Calculate_total_money(game_id);*/
}

void Player::Take_5000_without_having_b5000(int game_id, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_5000_without_having_b5000(n_5000, n_1000, n_500, n_100, n_50);
    /*(*n_5000) = 0;
    (*n_1000) = 0;
    (*n_500) = 0;
    (*n_100) = 0;
    (*n_50) = 0;

    int accumulated = 0;

    while (accumulated < 5000)
    {
        if (this->n_5000 > 0)
        {
            accumulated += 5000;
            (*n_5000)++;
            this->n_5000--;
        }
        else if (this->n_1000 > 0)
        {
            accumulated += 1000;
            (*n_1000)++;
            this->n_1000--;
        }
        else if (this->n_500 > 0)
        {
            accumulated += 500;
            (*n_500)++;
            this->n_500--;
        }
        else if (this->n_100 > 0)
        {
            accumulated += 100;
            (*n_100)++;
            this->n_100--;
        }
        else if (this->n_50 > 0)
        {
            accumulated += 50;
            (*n_50)++;
            this->n_50--;
        }
    }
    this->Calculate_total_money(game_id);*/
}

void Player::Take_1000_without_having_b1000(int game_id, int* n_1000, int* n_500, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_1000_without_having_b1000(n_1000, n_500, n_100, n_50);
    /*(*n_1000) = 0;
    (*n_500) = 0;
    (*n_100) = 0;
    (*n_50) = 0;

    int accumulated = 0;

    while (accumulated < 1000)
    {
        if (this->n_1000 > 0)
        {
            accumulated += 1000;
            (*n_1000)++;
            this->n_1000--;
        }
        else if (this->n_500 > 0)
        {
            accumulated += 500;
            (*n_500)++;
            this->n_500--;
        }
        else if (this->n_100 > 0)
        {
            accumulated += 100;
            (*n_100)++;
            this->n_100--;
        }
        else if (this->n_50 > 0)
        {
            accumulated += 50;
            (*n_50)++;
            this->n_50--;
        }
        else // When there are not available bills, change them with the bank
        {
            if (this->n_5000 > 0)
            {
                this->n_5000--;
                this->n_1000 += 5;
            }
        }
    }
    this->Calculate_total_money(game_id);*/
}

void Player::Take_500_without_having_b500(int game_id, int* n_500, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_500_without_having_b500(n_500, n_100, n_50);
    /*(*n_500) = 0;
    (*n_100) = 0;
    (*n_50) = 0;

    int accumulated = 0;

    while (accumulated < 500)
    {
        if (this->n_500 > 0)
        {
            accumulated += 500;
            (*n_500)++;
            this->n_500--;
        }
        else if (this->n_100 > 0)
        {
            accumulated += 100;
            (*n_100)++;
            this->n_100--;
        }
        else if (this->n_50 > 0)
        {
            accumulated += 50;
            (*n_50)++;
            this->n_50--;
        }
        else // When there are not available bills, change them with the bank
        {
            if (this->n_1000 > 0)
            {
                this->n_1000--;
                this->n_500 += 2;
            }
            else if (this->n_5000 > 0)
            {
                this->n_5000--;
                this->n_1000 += 5;
            }
        }
    }
    this->Calculate_total_money(game_id);*/
}

void Player::Take_100_without_having_b100(int game_id, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_100_without_having_b100(n_100, n_50);
    /*(*n_100) = 0;
    (*n_50) = 0;

    int accumulated = 0;

    while (accumulated < 100)
    {
        if (this->n_100 > 0)
        {
            accumulated += 100;
            (*n_100)++;
            this->n_100--;
        }
        else if (this->n_50 > 0)
        {
            accumulated += 50;
            (*n_50)++;
            this->n_50--;
        }
        else // When there are not available bills, change them with the bank
        {
            if (this->n_500 > 0)
            {
                this->n_500--;
                this->n_100 += 5;
            }
            else if (this->n_1000 > 0)
            {
                this->n_1000--;
                this->n_500 += 2;
            }
            else if (this->n_5000 > 0)
            {
                this->n_5000--;
                this->n_1000 += 5;
            }
        }
    }
    this->Calculate_total_money(game_id);*/
}

void Player::Take_50_without_having_b50(int game_id, int* n_50)
{
    this->GetState(game_id)->Take_50_without_having_b50(n_50);
    /*(*n_50) = 0;

    int accumulated = 0;

    while (accumulated < 50)
    {
        if (this->n_50 > 0)
        {
            accumulated += 50;
            (*n_50)++;
            this->n_50--;
        }
        else // When there are not available bills, change them with the bank
        {
            if (this->n_100 > 0)
            {
                this->n_100--;
                this->n_50 += 2;
            }
            else if (this->n_500 > 0)
            {
                this->n_500--;
                this->n_100 += 5;
            }
            else if (this->n_1000 > 0)
            {
                this->n_1000--;
                this->n_500 += 2;
            }
            else if (this->n_5000 > 0)
            {
                this->n_5000--;
                this->n_1000 += 5;
            }
        }
    }
    this->Calculate_total_money(game_id);*/
}

Player::~Player(void)
{
    if (this->socket != NULL)
        delete this->socket;
    for (map<int, PlayerGameState*>::iterator i = this->games_states->begin () ; i != this->games_states->end() ; i++)
        delete i->second;
    this->games_states->clear();
    delete this->games_states;
}
