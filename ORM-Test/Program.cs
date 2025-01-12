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
using System.Data;
namespace ORMTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
                Logger logger = Logger.GetInstance();

                // logger.LogInfo("Aplikacja uruchomiona");
                // logger.LogWarning("To jest ostrzeżenie");
                // try
                // {
                //     throw new InvalidOperationException("Wystąpił błąd w aplikacji");
                // }
                // catch (Exception ex)
                // {
                //     logger.LogError("Wystąpił błąd", ex);
                // }

                var postgreSqlConnectionFactory = new PostgreSqlConnectionFactory();
                string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=123m123m;";

                var MySqlConnectionFactory = new MySqlConnectionFactory();
                string connectionString_mySql = "";
                
                var companiesContext = new CompaniesContext(connectionString, postgreSqlConnectionFactory);
                
                var companiesTable = new Table<Company>(companiesContext.GetDatabaseConnection());
                
                var company = new Company
                {
                    Company_id = 4,
                    Name = "Test3",
                    Address = "Test3" 
                };
                
                companiesTable.Add(company);
                companiesTable.Remove(company);
                var company2 = new Company
                {
                    Company_id = 5,
                    Name = "Test5",  
                    Address = "Test5" 
                };
                companiesTable.Update(company2);

                List<Company> list = companiesTable.ToList(); 
                foreach(var element in list)
                {
                    Console.WriteLine(element.Company_id + " " + element.Name + " " + element.Address);
                }
                                   
                Console.WriteLine("\n");
                var obj = companiesTable.First();
                Console.WriteLine(obj.Company_id + " " + obj.Name + " " + obj.Address);


                Console.WriteLine("\n");
                var includeTest = companiesTable.Include(r => r.Name == "Test3").Include(r => r.Company_id == 3).ToList();

                foreach(var element in includeTest)
                {
                    Console.WriteLine(element.Company_id + " " + element.Name + " " + element.Address);
                }
        }
    }
}