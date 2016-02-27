using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LINQToSQLDemo.Util;


namespace LINQToSQLDemo
{
    class Program
    {

        #region Data Context

        private NorthwindDataContext GetDataContext(bool logging = false)
        {
            var nwDataContext = new NorthwindDataContext();

            //Display the generated SQL statement in the Console window
            if (logging)
                nwDataContext.Log = Console.Out;

            return nwDataContext;
        }
        #endregion

        static void Main(string[] args)
        {
            var prog = new Program();

            prog.Menu();

        }

        #region Scalar Functions

        private void ScalarFunction1()
        {
            //Products that start with "A"
            var aProducts = from product in GetDataContext(true).Products
                where product.ProductName.StartsWith("A")
                select new
                {
                    prodName = product.ProductName
                };
            
            "Products that start with \"A\"".DisplayHeader();

            foreach (var item in aProducts)
            {
                item.prodName.DisplayResults();
            }

        }

        private void ScalarFunction2()
        {
            //Category names and abreviations
            var categories = from category in GetDataContext().Categories
                select new
                {
                    Name = category.CategoryName,
                    Abbrev = category.CategoryName.Substring(0, 3).ToUpper()
                };

            "Category names and Abreviations:".DisplayHeader();

            foreach (var item in categories)
            {
                string.Format("{0} ({1})", item.Name, item.Abbrev).DisplayResults();
            }

        }

        private void ScalarFunction3()
        {
            //Contacts whose name contains Carlos
            var carlosCustomers = from customer in GetDataContext(true).Customers
                where customer.ContactName.Contains("Carlos")
                select new
                {
                    customer.CompanyName,
                    customer.ContactName,
                    customer.ContactTitle
                };

            "Contacts anmed \"Carlos\"".DisplayHeader();

            foreach (var item in carlosCustomers)
            {
                string.Format("{0}, {1} at {2}", item.ContactName, item.ContactTitle, item.CompanyName).DisplayResults();
            }

        }

        #endregion

        #region Simple Queries

        private void SimpleQuery()
        {
            var nwDataContext = new NorthwindDataContext();

            //Display the generated SQL statement in the Console window
            nwDataContext.Log = Console.Out;

            //Customers in the US
            var usaCustomers = from customer in nwDataContext.Customers
                where customer.Country == "USA"
                select customer.CompanyName;

            "Customers in the USA:".DisplayResults(usaCustomers);

        }

        private void SimpleQuery2()
        {
            var nwdc = new NorthwindDataContext();

            //Customers in the USA ordered by region and City
            var usaCustomers = from cust in nwdc.Customers
                where cust.Country == "USA"
                orderby cust.Region, cust.City
                select new
                {
                    cust.CompanyName,
                    cust.City,
                    cust.Region
                };

            Console.WriteLine();
            Console.WriteLine(usaCustomers.ToString());

            foreach (var cust in usaCustomers)
            {
                Console.WriteLine("{0} in {1}, {2}",
                    cust.CompanyName, cust.City, cust.Region);
            }

        }

        private void SimpleQuery3()
        {
            var nwdc = new NorthwindDataContext();

            //Cities in Spain with Customers
            var spainCities = (from cust in nwdc.Customers
                where cust.Country == "Spain"
                select cust.City).Distinct();

            var dataCommand = nwdc.GetCommand(spainCities);
            Console.WriteLine("\n" + dataCommand.CommandText + "\n");

            "Cities in Spain with Customers".DisplayResults(spainCities);

        }

        #endregion

        #region Menu
        public void Menu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("A: Simple Queries                        B: Scalar Functions");

                Console.WriteLine("\nEnter an Option (any other to exit):");

                var key = Char.ToUpper(Console.ReadKey(true).KeyChar);
                Console.WriteLine(key);
                Thread.Sleep(200);

                Console.Clear();

                switch (key)
                {
                    case 'A':
                        SimpleQuery();
                        SimpleQuery2();
                        SimpleQuery3();
                        break;
                    case 'B':
                        ScalarFunction1();
                        ScalarFunction2();
                        ScalarFunction3();
                        break;
                    case 'C':
                        break;
                    case 'D':
                        break;
                    case 'E':

                        break;
                    case 'F':

                        break;
                    case 'G':

                        break;
                    case 'H':

                        break;
                    case 'I':
                        
                        break;
                    case 'J':
                        
                        break;
                    case 'K':
                        
                        break;
                    case 'L':
                        
                        break;
                    case 'M':
                        
                        break;
                    case 'N':
                        
                        break;
                    case 'O':
                        break;
                    case 'P':
                        
                        break;
                    case 'Q':
                        
                        break;
                    case 'R':
                        
                        break;
                    case 'S':
                        
                        break;
                    default:
                        return;
                }

                Console.ReadKey(true);

            }

        }
        #endregion
    }
}
