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
        result->row_count = result->result->rowsCount();
        delete stmt;
    #else
        mysql_query(this->conn, query.c_str());
        result->result = mysql_store_result(this->conn);
        result->row_count = mysql_num_rows(result->result);
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
        num_rows_modified = mysql_affected_rows(this->conn);
    #endif
    return num_rows_modified;
}

MySQLConnection::~MySQLConnection(void)
{
    delete this->conn;
}

//Class MySQLResult
void MySQLResult::fetch_row()
{
    #ifdef _WIN32
        this->result->next();
    #else
        this->current_row = mysql_fetch_row(this->result);
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
