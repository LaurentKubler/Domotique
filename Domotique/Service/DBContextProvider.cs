using Domotique.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;


namespace Domotique.Service
{
    public class DBContextProvider : IDBContextProvider
    {
        string _dbConnectionString;

        ILoggerFactory _loggerFactory;


        public DBContextProvider(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }

        public DomotiqueContext getContext()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseMySql(_dbConnectionString, mysqlOptions =>
            {
                mysqlOptions.ServerVersion(new Version(5, 7, 17), ServerType.MySql); // replace with your Server Version and Type
            });
            return new DomotiqueContext(builder.Options);
        }
    }
}
