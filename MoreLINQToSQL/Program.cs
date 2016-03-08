using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoreLINQToSQL.Util;

namespace MoreLINQToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();

            prog.Menu();

        }

        #region Read Only Queries

        private void ReadOnlyQueries()
        {
            var dc = new NorthwindDataContext();


            //Beware that disabling tracking also disables deferred execution
            dc.ObjectTrackingEnabled = false;

            //Retrieve Customers in USA
            var usaCustomers = from customer in dc.Customers
                where customer.Country == "USA"
                select new
                {
                    customer.CompanyName,
                    customer.City,
                    customer.Region
                };

            "Customers in the USA".DisplayHeader();

            foreach (var customer in usaCustomers)
            {
                string.Format("{0} in {1}, {2}", customer.CompanyName, customer.City, customer.Region).DisplayResults();
            }

        }

        #endregion

        #region Deferred Loading

        private void DeferredLoading()
        {
            Console.Clear();
            Console.WriteLine("What report do you want to run?");
            Console.WriteLine("  1. Default");
            Console.WriteLine("  2. Just Customer");
            Console.WriteLine("  3. Customer and Order count");

            var key = Console.ReadKey().KeyChar;
            Console.Clear();

            var dc = new NorthwindDataContext();

            switch (key)
            {
                case '1':
                    break;
                case '2':
                    //Turn off deferred loading
                    dc.DeferredLoadingEnabled = false;
                    break;
                case '.':
                    return;
                default:
                    var loadOptions = new DataLoadOptions();
                    loadOptions.LoadWith<Customer>(c => c.Orders);
                    dc.LoadOptions = loadOptions;
                    break;
            }

            //Retrieve a Customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == "ALFKI");

            if (customer == null)
            {
                Console.WriteLine("Customer ALFKI NOT found!");
                return;
            }

            Console.WriteLine("Company Name {0}", customer.CompanyName);
            Console.WriteLine("City {0}", customer.City);
            Console.WriteLine("Region {0}", customer.Region);
            
            Console.WriteLine("\nOrders Placed: {0}", customer.Orders.Count);

        }

        #endregion

        #region Menu
        public void Menu()
        {
            bool isDefault;

            while (true)
            {
                isDefault = false;
                Console.Clear();

                Console.WriteLine("A: Deferred Loading");
                Console.WriteLine("B: Read Only Queries");

                Console.WriteLine("\nEnter an Option ('.' to exit):");

                var key = Char.ToUpper(Console.ReadKey(true).KeyChar);
                Console.WriteLine(key);
                Thread.Sleep(200);

                Console.Clear();

                switch (key)
                {
                    case 'A':
                        DeferredLoading();
                        break;
                    case 'B':
                        ReadOnlyQueries();
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
                    case '.':
                        return;
                    default:
                        isDefault = true;
                        break;
                }

                if (!isDefault & Console.ReadKey(true).KeyChar == '.')
                    return;
            }

        }
        #endregion
    }
}
