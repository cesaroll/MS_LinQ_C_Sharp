using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Global;

namespace LINQToSQLDemo
{
    class Program
    {
        static void Main(string[] args)
        {


            var prog = new Program();

            prog.Menu();

        }

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
