#include "mysqlmgr.h"

// Class MySQLMgr
bool MySQLMgr::Connect (string host, int port, string username, string password, string database)
{
    #ifdef _WIN32
        try
        {
            stringstream full_host;
            full_host << "tcp://" << host << ":" << port;
            this->connection->conn = this->driver->connect(full_host.str(), username, password);
        }
        catch (sql::SQLException e)
        {
            wcout << "Error connecting to database. Error codes: " << e.getSQLStateCStr() << " (" << e.getErrorCode() << ")" << endl;
            return false;
        }
        wcout << "Database connection succesful!" << endl;
        this->connection->conn->setSchema(database);
        return true;
    #else
        this->connection->conn = mysql_init(NULL);
        mysql_real_connect(this->connection->conn, host.c_str(), username.c_str(), password.c_str(), database.c_str(), port, NULL, 0);
        if (this->connection == NULL)
        {
            wcout << "Error connecting to database" << endl;
            return false;
        }
        else
        {
            wcout << "Database connection succesful!" << endl;
            return true;
        }
    #endif
}

void MySQLMgr::KeepAlive()
{
    if (this->connection != NULL)
    {
        this->connection->ExecuteQueryWithoutData("DO 1;");
        wcout << L"MySQL keep alive" << endl;
    }
}

void MySQLMgr::Disconnect()
{
    if (this->connection != NULL)
    {
        delete this->connection;
        this->connection = NULL;
    }
}

#ifdef _WIN32
bool MySQLMgr::IsClosed()
{
    return this->connection->IsClosed();
}
#endif

MySQLResult* MySQLMgr::ExecuteQueryWithData(string query)
{
    return this->connection->ExecuteQueryWithData(query);
}

int MySQLMgr::ExecuteQueryWithoutData(string query)
{
    return this->connection->ExecuteQueryWithoutData(query);
}

int MySQLMgr::GetLastInsertId()
{
    return this->connection->GetLastInsertId();
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
            wcout << "Error creating MySQL Driver. Error codes: " << e.getSQLStateCStr() << " (" << e.getErrorCode() << ")" << endl;
        }
    #endif
    this->connection = new MySQLConnection();
}

MySQLMgr::~MySQLMgr(void)
{
    if (this->connection != NULL)
    {
        delete this->connection;
        this->connection = NULL;
    }
}

// Class MySQLConnection
MySQLResult* MySQLConnection::ExecuteQueryWithData(string query)
{
//    this->query_mutex.lock();
    MySQLResult* result = new MySQLResult();
    #ifdef _WIN32
        try
        {
            sql::Statement* stmt = this->conn->createStatement();
            result->result = stmt->executeQuery(query);
            result->row_count = result->result->rowsCount();
            delete stmt;
        }
        catch (sql::SQLException e)
        {
            wcout << "Error executing query '" << query.c_str() << "'. Error codes: " << e.getSQLStateCStr() << " (" << e.getErrorCode() << ")" << endl;
        }
    #else
        mysql_query(this->conn, query.c_str());
        result->result = mysql_store_result(this->conn);
        result->row_count = mysql_num_rows(result->result);
    #endif
//    this->query_mutex.unlock();
    return result;
}

int MySQLConnection::ExecuteQueryWithoutData(string query)
{
    //this->query_mutex.lock();
    int num_rows_modified = 0;
    #ifdef _WIN32
        try
        {
            sql::Statement* stmt = this->conn->createStatement();
            num_rows_modified = stmt->executeUpdate(query);
            delete stmt;
        }
        catch (sql::SQLException e)
        {
            wcout << "Error executing query '" << query.c_str() << "'. Error codes: " << e.getSQLStateCStr() << " (" << e.getErrorCode() << ")" << endl;
        }
    #else
        if (mysql_query(this->conn, query.c_str()) > 0)
            wcout << mysql_error(this->conn) << endl;
        num_rows_modified = mysql_affected_rows(this->conn);
    #endif
    //this->query_mutex.unlock();
    return num_rows_modified;
}

int MySQLConnection::GetLastInsertId()
{
    #ifdef _WIN32
        sql::Statement* stmt = this->conn->createStatement();
        sql::ResultSet* result = NULL;
        int id;
        try
        {
            result = stmt->executeQuery("SELECT LAST_INSERT_ID();");
            result->next();
            id = result->getInt(1);
        }
        catch (sql::SQLException e)
        {
            wcout << "Error obtanining last insert id. Error codes: " << e.getSQLStateCStr() << " (" << e.getErrorCode() << ")" << endl;
        }
        if (result != NULL)
            delete result;
        delete stmt;
        return id;
    #else
        return mysql_insert_id(this->conn);
    #endif
}

#ifdef _WIN32
bool MySQLConnection::IsClosed()
{
    return this->conn->isClosed();
}
#endif

MySQLConnection::~MySQLConnection(void)
{
    #ifdef _WIN32
        this->conn->close();
        delete this->conn;
    #else
        // This function deletes the object
        mysql_close(this->conn);
    #endif
    wcout << L"DB connnection closed" << endl;
}

//Class MySQLResult
bool MySQLResult::fetch_row()
{
    #ifdef _WIN32
        return this->result->next();
    #else
        this->current_row = mysql_fetch_row(this->result);
        if (this->current_row == NULL)
            return false;
        else
            return true;
    #endif
}

int MySQLResult::get_int_field(string name)
{
    #ifdef _WIN32
        return this->result->getInt(name);
    #else
        MYSQL_FIELD *field;
        int field_index = -1;
        for(unsigned int i = 0; (field = mysql_fetch_field(result)); i++)
        {
            if (strcmp(name.c_str(), field->name) == 0)
            {
                field_index = i;
                break;
            }
        }
        mysql_field_seek(this->result, 0);
        if (field_index < 0)
            return 0;
        else
            return atoi(this->current_row[field_index]);
    #endif
}

int MySQLResult::get_int_field(int index)
{
    #ifdef _WIN32
        return this->result->getInt(index);
    #else
        if (index < 0)
            return 0;
        else
            return atoi(this->current_row[index]);
    #endif
}

string MySQLResult::get_string_field(string name)
{
    #ifdef _WIN32
        return this->result->getString(name);
    #else
        MYSQL_FIELD *field;
        int field_index = -1;
        for(unsigned int i = 0; (field = mysql_fetch_field(result)); i++)
        {
            if (strcmp(name.c_str(), field->name) == 0)
            {
                field_index = i;
                break;
            }
        }
        mysql_field_seek(this->result, 0);
        if (field_index < 0)
            return 0;
        else
            return this->current_row[field_index];
    #endif
}

string MySQLResult::get_string_field(int index)
{
    #ifdef _WIN32
        return this->result->getString(index);
    #else
        if (index < 0)
            return 0;
        else
            return this->current_row[index];
    #endif
}

bool MySQLResult::get_bool_field(string name)
{
    #ifdef _WIN32
        return this->result->getBoolean(name);
    #else
        MYSQL_FIELD* field;
        int field_index = -1;
        for(unsigned int i = 0; (field = mysql_fetch_field(result)); i++)
        {
            if (strcmp(name.c_str(), field->name) == 0)
            {
                field_index = i;
                break;
            }
        }
        mysql_field_seek(this->result, 0);
        if (field_index < 0)
            return false;
        else
        {
            string field_value = this->current_row[field_index];
            if ((field_value == "TRUE") || (field_value == "true") || (field_value == "True"))
                return true;
            else
                return false;
        }
    #endif
}

bool MySQLResult::get_bool_field(int index)
{
    #ifdef _WIN32
        return this->result->getBoolean(index);
    #else
        if (index < 0)
            return false;
        else
        {
            string field_value = this->current_row[index];
            if ((field_value == "TRUE") || (field_value == "true") || (field_value == "True"))
                return true;
            else
                return false;
        }
    #endif
}

MySQLResult::~MySQLResult(void)
{
    #ifdef _WIN32
        delete this->result;
    #else
        mysql_free_result(this->result);
    #endif
}
