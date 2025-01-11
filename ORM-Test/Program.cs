// See https://aka.ms/new-console-template for more information
using System;

using ORMLibrary.QueryBuilders;
using ORMLibrary.Context;
using ORMLibrary.DataAccess;
using ORMLibrary.Logging;
using ORMLibrary.Iterator;
using ORMLibrary.Mapping;
using ORMTest.Domain;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;
namespace ORMTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
                Logger logger = Logger.GetInstance();

                logger.LogInfo("Aplikacja uruchomiona");
                logger.LogWarning("To jest ostrzeżenie");
                // try
                // {
                //     throw new InvalidOperationException("Wystąpił błąd w aplikacji");
                // }
                // catch (Exception ex)
                // {
                //     logger.LogError("Wystąpił błąd", ex);
                // }

                PostgreSqlConnectionFactory postgreSqlConnectionFactory = new PostgreSqlConnectionFactory();
                string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123m123m;";
                
                var dbContext = new CompaniesContext(connectionString, postgreSqlConnectionFactory);
                

        }
    }
}