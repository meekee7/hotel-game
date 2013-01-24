#pragma once
#include <string>

#ifdef _WIN32
	#include <mysql_driver.h>
	#include <mysql_connection.h>
	#include <cppconn\resultset.h>
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
};

class MySQLMgr
{
public:
	#ifdef _WIN32
		sql::mysql::MySQL_Driver *driver;
	#endif
	MySQLConnection* conn;
	MySQLConnection* Connect (string host, string username, string password, string database);
	MySQLMgr(void);
	~MySQLMgr(void);
};