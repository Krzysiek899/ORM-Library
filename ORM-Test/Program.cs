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
using System.Collections.Generic;
namespace ORMTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger logger = Logger.GetInstance();

            string mySqlConnectionString = "Host=localhost;Port=3306;Database=ORMTest;Username=root;Password=root";
            var mySqlConnectionFactory = new MySqlConnectionFactory();

            var companiesContext = new CompaniesContext(mySqlConnectionString, mySqlConnectionFactory);
            var companiesTable = new Table<Company>(companiesContext.GetDatabaseConnection());

            try
            {
                logger.LogInfo("Starting ORM Tests...");

                TestAddOperation(companiesTable);
                TestUpdateOperation(companiesTable);
                TestIncludeOperation(companiesTable);
                TestQueryingFirstElement(companiesTable);
                TestRetrieveAll(companiesTable);
                TestRemoveOperation(companiesTable);


                logger.LogInfo("All tests completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred during tests.", ex);
            }
        }

        private static void TestAddOperation(Table<Company> companiesTable)
        {
            Console.WriteLine("\n--- Testing Add Operation ---\n");

            var newCompany = new Company
            {
                Company_id = 100,
                Name = "NewCompany",
                Address = "123 Test Street"
            };

            companiesTable.Add(newCompany);
            Console.WriteLine($"Added: {newCompany.Company_id} {newCompany.Name} {newCompany.Address}");
        }

        private static void TestUpdateOperation(Table<Company> companiesTable)
        {
            Console.WriteLine("\n--- Testing Update Operation ---\n");

            var updatedCompany = new Company
            {
                Company_id = 100, // Ensure this matches an existing ID
                Name = "UpdatedCompany",
                Address = "456 Updated Street"
            };

            companiesTable.Update(updatedCompany);
            Console.WriteLine($"Updated: {updatedCompany.Company_id} {updatedCompany.Name} {updatedCompany.Address}");
        }

        private static void TestRemoveOperation(Table<Company> companiesTable)
        {
            Console.WriteLine("\n--- Testing Remove Operation ---\n");

            var companyToRemove = new Company
            {
                Company_id = 100 // Ensure this matches an existing ID
            };

            companiesTable.Remove(companyToRemove);
            Console.WriteLine($"Removed: {companyToRemove.Company_id}");
        }

        private static void TestIncludeOperation(Table<Company> companiesTable)
        {
            Console.WriteLine("\n--- Testing Include Operation ---\n");

            var results = companiesTable
                .Include(c => c.Name.Contains("Company"))
                .Include(c => c.Address.Contains("Street"))
                .ToList();

            foreach (var company in results)
            {
                Console.WriteLine($"{company.Company_id} {company.Name} {company.Address}");
            }
        }

        private static void TestQueryingFirstElement(Table<Company> companiesTable)
        {
            Console.WriteLine("\n--- Testing Querying First Element ---\n");

            var firstCompany = companiesTable.First();

            if (firstCompany != null)
            {
                Console.WriteLine($"First: {firstCompany.Company_id} {firstCompany.Name} {firstCompany.Address}");
            }
            else
            {
                Console.WriteLine("No companies found.");
            }
        }

        private static void TestRetrieveAll(Table<Company> companiesTable)
        {
            Console.WriteLine("\n--- Testing Retrieve All Companies ---\n");

            var companies = companiesTable.ToList();

            foreach (var company in companies)
            {
                Console.WriteLine($"{company.Company_id} {company.Name} {company.Address}");
            }
        }
    }
}
