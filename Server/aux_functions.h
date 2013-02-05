#pragma once
#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include <ctime>
#include <cstdlib>
#include <cstring>
#include "portable_socket.h"
#include "player.h"
#include "game.h"
#include "hotel.h"
#include "chat.h"

using namespace std;

int get_utf8_length(wstring data);
wstring utf8_to_utf16 (string data);
string utf16_to_utf8 (wstring data);
const wstring currentDateTime();
Player* get_player_from_name(wstring name, list<Player*>* plist);
Player* get_player_from_game(wstring name, Game* game);
Hotel* get_hotel_from_name(wstring name_txt, Game* game);
Game* get_game_from_name(wstring name, list<Game*>* glist);
Game* get_game_from_id(int id, list<Game*>* glist);
Chat* get_chat_from_id(int id, list<Chat*>* chat_list, list<Game*>* glist);
string receive_string (Player* p, int length, int* bytes_received);
wstring receive_wstring (Player* p, int length, int* bytes_received);
int receive_int (Player* p, int* bytes_received);
int send_string (Player* p, string data);
int send_wstring (Player* p, wstring data);
int send_int (Player* p, int data);
void send_command(string command, Player* p);
void SendGameList(Player* p, list<Game*>* glist);