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

enum TColor { red, blue, yellow, green, bank };
enum TPosition { start, buy, build, free_entrance, free_phase };
enum THotel_name { Fujiyama, Boomerang, Letoile, President, Royal, Waikiki, Taj_Mahal, Safari, None };
enum TBuild_dice_res { Allow, Free, Double, Deny };