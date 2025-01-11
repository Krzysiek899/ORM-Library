using ORMLibrary.Context;
using ORMTest.Domain;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;


namespace ORMTest
{

    public class CompaniesContext : DatabaseContext
    {
        public Table<Company> companies {get; set;}
        public Table<Worker> users {get; set;}
        public Table<Product> products {get; set;}

        public CompaniesContext(string connectionString, IDbConnectionFactory dbConnectionFactory) : base(connectionString, dbConnectionFactory)
        {
            companies = Set<Company>();
            users = Set<Worker>();
            products = Set<Product>();
        }

        
    }
}