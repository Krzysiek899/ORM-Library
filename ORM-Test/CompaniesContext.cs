using ORMLibrary.Context;
using ORMTest.Domain;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;


namespace ORMTest
{

    public class CompaniesContext : DatabaseContext
    {
        public Table<Company> companies {get; set;}
        public Table<Worker> workers {get; set;}
        public Table<Product> products {get; set;}
        public Table<Orders> orders {get; set;}
        public Table<Customer> customers {get; set;}

        public CompaniesContext(string connectionString, IDbConnectionFactory dbConnectionFactory) : base(connectionString, dbConnectionFactory)
        {
            companies = Set<Company>();
            workers = Set<Worker>();
            products = Set<Product>();
            orders = Set<Orders>();
            customers = Set<Customer>();
        }

        
    }
}