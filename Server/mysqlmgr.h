#pragma once
#include <string>

#ifdef _WIN32
	#include <mysql_driver.h>
	#include <mysql_connection.h>
	#include <cppconn/exception.h>
	#include <cppconn/resultset.h>
	#include <cppconn/statement.h>
	#pragma comment(lib, "mysqlcppconn.lib")
	#pragma comment(lib, "libmysql.lib")
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
	#endif
};

class MySQLConnection
{
public:
	#ifdef _WIN32
		sql::Connection* conn;
	#else
		MYSQL* conn;
	#endif
	MySQLResult* ExecuteQuery(string query);
	MySQLConnection(void);
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
	MySQLConnection* Connect (string host, string username, string password, string database);
	void Disconnect();
	MySQLResult* ExecuteQuery(string query);
	MySQLMgr(void);
	~MySQLMgr(void);
};