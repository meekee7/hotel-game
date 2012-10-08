#include "aux_functions.h"

int get_utf8_length(wstring data)
{
   int res;
   #ifdef _WIN32
      res = WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
   #else
      res = wcstombs(NULL, data.c_str(), 0);
   #endif
   return res;
}

wstring utf8_to_utf16 (string data)
{
   if (data.empty())
      return L"";
   wstring res;
   #ifdef _WIN32
      res.resize(MultiByteToWideChar(CP_UTF8, 0, data.data(), data.length(), NULL, 0));
      MultiByteToWideChar(CP_UTF8, 0, data.data(), data.length(), &res[0], res.length());
   #else
      res.resize(mbstowcs(NULL, data.c_str(), 0)+1);
      mbstowcs((wchar_t*)&res.data()[0], data.c_str(), res.size());
   #endif
   return res;
}

string utf16_to_utf8 (wstring data)
{
   if (data.empty())
      return "";
   #ifdef _WIN32
      int size_utf8 = WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), NULL, 0, NULL, NULL);
   #else
      //setlocale(LC_ALL, "es_ES.utf8");
      int size_utf8 = wcstombs(NULL, data.data(), 0);
   #endif
   string data_utf8;
   data_utf8.resize(size_utf8);
   #ifdef _WIN32
      WideCharToMultiByte(CP_UTF8, 0, data.data(), data.length(), &data_utf8[0], data_utf8.length(), NULL, NULL);
   #else
      //setlocale(LC_ALL, "es_ES.utf8");
      wcstombs(&data_utf8[0], data.data(), size_utf8);
   #endif
   return data_utf8;
}

const wstring currentDateTime()
{
   time_t     now = time(0);
   struct tm  tstruct;
   char       buf[80];
   #ifdef _WIN32
      localtime_s(&tstruct, &now);
   #else
      tstruct = *localtime(&now);
   #endif
   // Visit http://www.cplusplus.com/reference/clibrary/ctime/strftime/
   // for more information about date/time format
   strftime(buf, sizeof(buf), "%Y-%m-%d %X -> ", &tstruct);
   return utf8_to_utf16(buf);
}
