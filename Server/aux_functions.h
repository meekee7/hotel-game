#pragma once
#include <iostream>
#include <list>
#include <string>
#include <stdio.h>
#include <signal.h>
#include <ctime>
#include <cstdlib>
#include <cstring>
#ifdef _WIN32
   #include <windows.h>
#endif

using namespace std;

int get_utf8_length(wstring data);
wstring utf8_to_utf16 (string data);
string utf16_to_utf8 (wstring data);
const wstring currentDateTime();
