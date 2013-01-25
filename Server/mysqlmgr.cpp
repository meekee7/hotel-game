#include "mysqlmgr.h"

// Class MySQLMgr
MySQLConnection* MySQLMgr::Connect (string host, string username, string password, string database)
{
	#ifdef _WIN32
		try
		{
			this->connection->conn = this->driver->connect(host, username, password);
		}
		catch (sql::SQLException e)
		{
			wcout << "Error connecting to database: " << e.getSQLStateCStr() << " " << e.getErrorCode() << endl;
		}
        this->connection->conn->setSchema(database);
		return this->connection;
	#else
		this->connection->conn = mysql_init(NULL);
		mysql_real_connect(this->connection->conn, host.c_str(), username.c_str(), password.c_str(), database.c_str(), 0, NULL, 0);
      if (this->connection == NULL)
         wcout << "Error connecting to database" << endl;
      return this->connection;
	#endif
}

void MySQLMgr::Disconnect()
{
	delete this->connection;
}

MySQLResult* MySQLMgr::ExecuteQueryWithData(string query)
{
	return this->connection->ExecuteQueryWithData(query);
}

int MySQLMgr::ExecuteQueryWithOutData(string query)
{
    return this->connection->ExecuteQueryWithOutData(query);
}

MySQLMgr::MySQLMgr(void)
{
	#ifdef _WIN32
		try
		{
		    this->driver = sql::mysql::get_driver_instance();
			if (this->driver == NULL)
				throw sql::SQLException();
		}
		catch (sql::SQLException e)
		{
			wcout << "Error creating MySQL Driver: " << e.getSQLStateCStr() << " " << e.getErrorCode() << endl;
		}
	#endif
	this->connection = new MySQLConnection();
}

MySQLMgr::~MySQLMgr(void)
{
	delete this->connection;
}

// Class MySQLConnection
MySQLResult* MySQLConnection::ExecuteQueryWithData(string query)
{
	MySQLResult* result = new MySQLResult();
	#ifdef _WIN32
		sql::Statement* stmt = this->conn->createStatement();
		result->result = stmt->executeQuery(query);
		delete stmt;
	#else
      mysql_query(this->conn, query.c_str());
		result->result = mysql_store_result(this->conn);
	#endif
	return result;
}

int MySQLConnection::ExecuteQueryWithOutData(string query)
{
    int num_rows_modified;
	#ifdef _WIN32
		sql::Statement* stmt = this->conn->createStatement();
		num_rows_modified = stmt->executeUpdate(query);
		delete stmt;
	#else
        mysql_query(this->conn, query.c_str());
		result->result = mysql_store_result(this->conn);
	#endif
	return num_rows_modified;
}

MySQLConnection::~MySQLConnection(void)
{
	delete this->conn;
}
