#pragma once
#ifndef _WIN32
#include "mysql_client_simple_conn.h"
#endif

class DBMgr
{
public:
	DBMgr(void);
	~DBMgr(void);
};