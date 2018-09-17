using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Domotique.Service
{
    class DatabaseConnection : IDatabaseConnection
    {
        string _dbConnectionstring { get; set; }


        public DatabaseConnection(string dbconnectionString)
        {
            _dbConnectionstring = dbconnectionString;
        }


        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_dbConnectionstring);
        }
    }
}
