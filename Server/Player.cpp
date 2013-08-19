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

void Player::Leave_Game(int game_id)
{
    // Delete the game state
    map<int, PlayerGameState*>::iterator i = this->games_states->find(game_id);
    this->games_states->erase(i);
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
    this->GetState(game_id)->Calculate_total_money();
}

void Player::Charge_bank(int game_id)
{
    this->GetState(game_id)->Charge_bank();
}

void Player::Buy_hotel(int game_id, Hotel* hotel, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Buy_hotel(hotel, n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Buy_hotel(int game_id, Hotel* hotel, Player* previous_owner, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Buy_hotel(hotel, previous_owner->GetState(game_id), n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Expropriate_hotel(int game_id, Hotel* hotel)
{
    this->GetState(game_id)->Expropriate_hotel(hotel);
}

void Player::Set_money(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Set_money(n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Pay_nights (int game_id, PlayerGameState* to_player, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Pay_nights(to_player, n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Return_change(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->games_states->find(game_id)->second->Return_change(n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Pay_phase_or_entrance(int game_id, int n_5000, int n_1000, int n_500, int n_100, int n_50)
{
    this->GetState(game_id)->Pay_phase_or_entrance(n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Take_5000_without_having_b5000(int game_id, int* n_5000, int* n_1000, int* n_500, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_5000_without_having_b5000(n_5000, n_1000, n_500, n_100, n_50);
}

void Player::Take_1000_without_having_b1000(int game_id, int* n_1000, int* n_500, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_1000_without_having_b1000(n_1000, n_500, n_100, n_50);
}

void Player::Take_500_without_having_b500(int game_id, int* n_500, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_500_without_having_b500(n_500, n_100, n_50);
}

void Player::Take_100_without_having_b100(int game_id, int* n_100, int* n_50)
{
    this->GetState(game_id)->Take_100_without_having_b100(n_100, n_50);
}

void Player::Take_50_without_having_b50(int game_id, int* n_50)
{
    this->GetState(game_id)->Take_50_without_having_b50(n_50);
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
