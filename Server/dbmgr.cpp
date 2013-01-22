#include "dbmgr.h"
#ifdef _WIN32
	#include "mysql_driver.h"
#else
	#include "mysql/mysql.h"
#endif

DBMgr::DBMgr(void)
{
}

DBMgr::~DBMgr(void)
{
}
