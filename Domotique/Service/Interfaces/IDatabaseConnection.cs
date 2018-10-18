using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service
{
    interface IDatabaseConnection
    {
        MySqlConnection GetConnection();
    }
}
