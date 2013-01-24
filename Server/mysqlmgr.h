#pragma once
#include <string>

#ifdef _WIN32
	#include "mysql_driver.h"
#else
	#include "mysql/mysql.h"
#endif

using namespace std;

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
	MySQLConnection Connect (string host, string username, string password, string database);
	MySQLMgr(void);
	~MySQLMgr(void);
};