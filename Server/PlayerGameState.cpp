#include "PlayerGameState.h"

PlayerGameState::PlayerGameState(Player* player)
{
    this->player = player;
    this->bd_id = -1; // Will be overwritten when saved
    this->rolled_last_turn = false;
    this->charged_bank_last_turn = false;
    this->bought_last_turn = false;
    this->built_last_turn = false;
    this->paid_last_turn = false;
    this->free_entrance_used = false;
    this->debt_last_turn = 0;
    this->debt_to_last_turn = NULL;
    this->hotel_to_be_extended = NULL;
    this->n_5000 = 0;
    this->n_1000 = 0;
    this->n_500 = 0;
    this->n_100 = 0;
    this->n_50 = 0;
    this->position = new Position(0);
}

void PlayerGameState::Calculate_total_money()
{
    this->total_money = (n_5000 * 5000) + (n_1000 * 1000) + (n_500 * 500) + (n_100 * 100) + (n_50 * 50);
}

void PlayerGameState::Charge_bank()
{
    this->n_1000 += 2;
    this->Calculate_total_money();
    this->charged_bank_last_turn = true;
}

void PlayerGameState::Buy_hotel(Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    // Owner is set by caller thread because incomplete class Hotel
    this->hotels.push_back(hotel);
    this->n_5000 -= n_5000;
    this->n_1000 -= n_1000;
    this->n_500 -= n_500;
    this->n_100 -= n_100;
    this->n_50 -= n_50;
    this->Calculate_total_money();
}

void PlayerGameState::Buy_hotel(Hotel* hotel, PlayerGameState* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    // Owner is set by caller thread because incomplete class Hotel
    this->hotels.push_back(hotel);
    this->n_5000 -= n_5000;
    this->n_1000 -= n_1000;
    this->n_500 -= n_500;
    this->n_100 -= n_100;
    this->n_50 -= n_50;
    this->Calculate_total_money();
    previous_owner->n_5000 += n_5000;
    previous_owner->n_1000 += n_1000;
    previous_owner->n_500 += n_500;
    previous_owner->n_100 += n_100;
    previous_owner->n_50 += n_50;
    previous_owner->Calculate_total_money();
}

void PlayerGameState::Expropriate_hotel(Hotel* hotel)
{
    // Money its already calculated, client does the math
    this->hotels.remove(hotel);
}

void PlayerGameState::Set_money(int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->n_5000 = n_5000;
    this->n_1000 = n_1000;
    this->n_500 = n_500;
    this->n_100 = n_100;
    this->n_50 = n_50;
    this->Calculate_total_money();
}

void PlayerGameState::Pay_nights (PlayerGameState* to_player, int n5000, int n1000, int n500, int n100, int n50)
{
    // Transfiere los fondos del jugador que paga al que cobra, el precio viene calculado de la IU
    this->n_5000 -= n5000;
    this->n_1000 -= n1000;
    this->n_500 -= n500;
    this->n_100 -= n100;
    this->n_50 -= n50;
    this->Calculate_total_money();
    to_player->n_5000 += n5000;
    to_player->n_1000 += n1000;
    to_player->n_500 += n500;
    to_player->n_100 += n100;
    to_player->n_50 += n50;
    to_player->Calculate_total_money();
}

void PlayerGameState::Return_change(int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->n_5000 += n_5000;
    this->n_1000 += n_1000;
    this->n_500 += n_500;
    this->n_100 += n_100;
    this->n_50 += n_50;
    this->Calculate_total_money();
}

void PlayerGameState::Pay_phase_or_entrance(int n5000, int n1000, int n500, int n100, int n50)
{
    this->n_5000 -= n5000;
    this->n_1000 -= n1000;
    this->n_500 -= n500;
    this->n_100 -= n100;
    this->n_50 -= n50;
    this->Calculate_total_money();
}

void PlayerGameState::Take_5000_without_having_b5000(int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50)
{
    (*n_5000) = 0;
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
    this->Calculate_total_money();
}

void PlayerGameState::Take_1000_without_having_b1000(int* n_1000, int* n_500, int* n_100, int* n_50)
{
    (*n_1000) = 0;
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
    this->Calculate_total_money();
}

void PlayerGameState::Take_500_without_having_b500(int* n_500, int* n_100, int* n_50)
{
    (*n_500) = 0;
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
    this->Calculate_total_money();
}

void PlayerGameState::Take_100_without_having_b100(int* n_100, int* n_50)
{
    (*n_100) = 0;
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
    this->Calculate_total_money();
}

void PlayerGameState::Take_50_without_having_b50(int* n_50)
{
    (*n_50) = 0;

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
    this->Calculate_total_money();
}

PlayerGameState::~PlayerGameState(void)
{
    if (position != NULL)
        delete this->position;
}