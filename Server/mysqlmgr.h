#pragma once
#include <string>

#ifdef _WIN32
	#include "mysql_driver.h"
#else
	#include "mysql/mysql.h"
#endif

using namespace std;

class Connection
{
};

class MysqlMgr
{
public:
	Connection Connect (string host, string username, string password, string database);
	MysqlMgr(void);
	~MysqlMgr(void);
};