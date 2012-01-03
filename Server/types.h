#pragma once
struct config_bills
{
   int n_5000;
   int n_1000;
   int n_500;
   int n_100;
   int n_50;
};

struct config
{
   struct config_bills two_players;
   struct config_bills three_or_four_players;
};