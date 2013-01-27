#pragma once
#include <string>
#include <sstream>

#ifdef _WIN32
    #include <mysql_driver.h>
    #include <mysql_connection.h>
    #include <cppconn/exception.h>
    #include <cppconn/resultset.h>
    #include <cppconn/statement.h>
#else
    #include "mysql/mysql.h"
#endif

using namespace std;

class MySQLResult
{
public:
    #ifdef _WIN32
        sql::ResultSet* result;
    #else
        MYSQL_RES* result;
        MYSQL_ROW current_row;
    #endif
    int row_count;
    bool fetch_row();
    int get_int_field(string name);
    int get_int_field(int index);
    string get_string_field(string name);
    string get_string_field(int index);
    bool get_bool_field(string name);
    bool get_bool_field(int index);
    ~MySQLResult(void);
};

class MySQLConnection
{
public:
    #ifdef _WIN32
        sql::Connection* conn;
    #else
        MYSQL* conn;
    #endif
    MySQLResult* ExecuteQueryWithData(string query);
    int ExecuteQueryWithOutData(string query);
    int GetLastInsertId();
    ~MySQLConnection(void);
};

class MySQLMgr
{
private:
    MySQLConnection* connection;
public:
    #ifdef _WIN32
        sql::mysql::MySQL_Driver *driver;
    #endif
    bool Connect (string host, int port, string username, string password, string database);
    void Disconnect();
    MySQLResult* ExecuteQueryWithData(string query);
    int ExecuteQueryWithOutData(string query);
    int GetLastInsertId();
    MySQLMgr(void);
    ~MySQLMgr(void);
};
