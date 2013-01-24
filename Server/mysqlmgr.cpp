#include "mysqlmgr.h"

MySQLConnection* MySQLMgr::Connect (string host, string username, string password, string database)
{
	#ifdef _WIN32
		this->conn->conn = this->driver->connect(host, username, password);
		return this->conn;
	#else
		this->conn->conn = mysql_init(NULL);
		mysql_real_connect(this->conn->conn, host.c_str(), username.c_str(), password.c_str(), database.c_str(), 0, NULL, 0);
      return this->conn;
	#endif
}

MySQLMgr::MySQLMgr(void)
{
	#ifdef _WIN32
		this->driver = sql::mysql::get_mysql_driver_instance();
	#endif
	this->conn = new MySQLConnection();
}

MySQLMgr::~MySQLMgr(void)
{
}
