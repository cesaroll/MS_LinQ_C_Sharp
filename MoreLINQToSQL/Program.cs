using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        #region Data Validation

        /// <summary>
        /// Test Validation of Invalid Update
        /// Required Date
        /// This is executed when SubmitChanges() to DB
        /// </summary>
        private void DataValidation3()
        {
            //Try to Change the required date of an existing order, prior to orderd date

            var dc = new NorthwindDataContext();

            var custId = "ALFKI";

            //Retrieve customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custId);

            if (customer == null)
            {
                string.Format("Customer: [{0}] does NOT exist!").DisplayHeader();
                return;
            }

            //Create a valid new order
            var newOrder = new Order
            {
                CustomerID = custId,
                OrderDate = DateTime.Today,
                RequiredDate = DateTime.Today.AddDays(7),
                EmployeeID = 4
            };

            customer.Orders.Add(newOrder);

            //Try to save order
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }


            //Update order
            try
            {
                newOrder.RequiredDate = newOrder.OrderDate.Value.Subtract(new TimeSpan(1, 0, 0, 0));
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }

            //Delete Order (Clean Up)
            try
            {
                customer.Orders.Remove(newOrder);
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }

        }

        /// <summary>
        /// Test Validation of Invalid OrderDate Changing
        /// This happens even before submitting to the DB
        /// </summary>
        private void DataValidation2()
        {
            //Try to Change the order date of an existing order

            var dc = new NorthwindDataContext();

            var custId = "ALFKI";

            //Retrieve customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custId);

            if (customer == null)
            {
                string.Format("Customer: [{0}] does NOT exist!").DisplayHeader();
                return;
            }

            //Create a valid new order
            var newOrder = new Order
            {
                CustomerID = custId,
                OrderDate = DateTime.Today,
                RequiredDate = DateTime.Today.AddDays(7),
                EmployeeID = 4
            };

            customer.Orders.Add(newOrder);

            //Try to save order
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }


            //Update order
            try
            {
                newOrder.OrderDate = DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0));
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }
            
            //Delete Order (Clean Up)
            try
            {
                customer.Orders.Remove(newOrder);
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }

        }

        /// <summary>
        /// Test Validation of Invalid Insert to DB
        /// Invalid OrderDate
        /// </summary>
        private void DataValidation1()
        {
            var dc = new NorthwindDataContext();

            var custId = "ALFKI";

            //Retrieve customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custId);

            if (customer == null)
            {
                string.Format("Customer: [{0}] does NOT exist!").DisplayHeader();
                return;
            }

            //Create an invalid new order
            var newOrder = new Order
            {
                CustomerID = custId,
                OrderDate = DateTime.Today.Subtract(new TimeSpan(1,0,0,0)),
                RequiredDate = DateTime.Today.AddDays(7),
                EmployeeID = 4
            };

            customer.Orders.Add(newOrder);

            //Try to save order
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ex.Message.DisplayResults();
            }

        }

        #endregion

        #region Transactions

        /// <summary>
        /// Using Transaction Scope with Exception Example
        /// </summary>
        private void Transactions3()
        {
            var dc = new NorthwindDataContext();

            var custId = "ALFKI";

            //Retrieve Customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custId);

            if (customer == null)
            {
                string.Format("Customer: [{0}] does NOT exist!").DisplayResults();
                return;
            }

            AddOrderResult newOrderResult = null;
            //Open transaction Scope
            using (var transScope = new System.Transactions.TransactionScope())
            {
                try
                {
                    //Create Customer Order
                    var result = dc.AddOrder(custId,
                        4,
                        DateTime.Today,
                        DateTime.Today.AddDays(7),
                        null,
                        1,
                        20.5m,
                        "Costa Concordia",
                        customer.Address,
                        customer.City,
                        customer.Region,
                        customer.PostalCode,
                        customer.Country).ToList();

                    newOrderResult = result[0];

                    //Create Details
                    dc.AddOrderDetail(newOrderResult.OrderID, 7, 25, 10, 0);
                    dc.AddOrderDetail(newOrderResult.OrderID, 99, 10, 15, 0);
                    dc.AddOrderDetail(newOrderResult.OrderID, 12, 20, 25, 0);

                    //Commit Transaction
                    transScope.Complete();
                }
                catch (Exception ex)
                {
                    "Exception".DisplayHeader();
                    ex.Message.DisplayResults();
                }
            }

            
            //Show Customer Orders
            Display.ShowCustomerOrders(dc, custId);
            //Show Order Details
            Display.ShowOrderDetails(dc, newOrderResult.OrderID);
        }

        /// <summary>
        /// Using Transaction Scope. OK
        /// </summary>
        private void Transactions2()
        {
            var dc = new NorthwindDataContext();

            var custId = "ALFKI";

            //Retrieve Customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custId);

            if(customer == null)
            {
                string.Format("Customer: [{0}] does NOT exist!").DisplayResults();
                return;
            }

            //Get Latest Customer Order
            var order = customer.Orders.OrderByDescending(o => o.OrderID).FirstOrDefault();

            //Open transaction Scope
            using (var transScope = new System.Transactions.TransactionScope())
            {
                try
                {
                    //Delete Details
                    foreach (var detail in order.Order_Details)
                    {
                        dc.DeleteOrderDetail(detail.OrderID, detail.ProductID);
                    }

                    //Delete Order
                    dc.DeleteOrder(order.OrderID);

                    //Commit Transaction
                    transScope.Complete();
                }
                catch (Exception ex)
                {
                    "Exception".DisplayHeader();
                    ex.Message.DisplayResults();
                }
            }

            //Show Customer Orders
            Display.ShowCustomerOrders(dc, custId);

        }

        /// <summary>
        /// Implicit Transaction
        /// </summary>
        private void Transactions1()
        {
            var dc = new NorthwindDataContext();

            var custID = "ALFKI";

            //Retrieve Customer
            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custID);

            if (customer == null)
            {
                string.Format("Customer: [{0}] does NOT exist!").DisplayResults();
                return;
            }

            //Crete new Customer Order
            var newOrder = new Order
            {
                CustomerID = custID,
                OrderDate = DateTime.Today,
                RequiredDate = DateTime.Today.AddDays(7),
                EmployeeID = 4
            };
            
            //Add the order
            customer.Orders.Add(newOrder);

            //Create and add order details
            newOrder.Order_Details.Add(new Order_Detail {ProductID = 7, UnitPrice = 25, Quantity = 10, Discount = 0});
            newOrder.Order_Details.Add(new Order_Detail {ProductID = 8, UnitPrice = 10, Quantity = 15, Discount = 0});
            newOrder.Order_Details.Add(new Order_Detail {ProductID = 12, UnitPrice = 20, Quantity = 25, Discount = 0});

            //Save Changes in the implicit transaction
            dc.SubmitChanges();

            //Show Customer Orders
            Display.ShowCustomerOrders(dc, custID);
            //Show Order Details
            Display.ShowOrderDetails(dc, newOrder.OrderID);


        }

        #endregion

        #region Direct Execution

        /// <summary>
        /// Talk Direct to SQL Server
        /// </summary>
        private void DirectExecution()
        {
            var dc = new NorthwindDataContext();

            var usaCustomers = dc.ExecuteQuery<Customer>(@"SELECT CustomerID, CompanyName, City, Region  
                    FROM Customers WHERE Country = 'USA' ");

            "Customers in the USA".DisplayHeader();

            foreach (var cust in usaCustomers)
            {
                string.Format("{0} in {1}, {2}", cust.CompanyName, cust.City, cust.Region).DisplayResults();
            }

        }

        #endregion

        #region Compiled Static Queries

        private void CompiledStaticQueries()
        {
            var dc = new NorthwindDataContext();

            "Customers in the USA".DisplayHeader();
            foreach (var customer in NorthWindQueries.CustomersInTheUSA(dc))
            {
                string.Format("{0} in {1}, {2}", customer.CompanyName, customer.City, customer.Region).DisplayResults();
            }
            
            "Customers in Washington".DisplayHeader();

            foreach (var customer in NorthWindQueries.CustomersInARegion(dc, "WA"))
            {
                string.Format("{0} in {1}, {2}", customer.CompanyName, customer.City, customer.Region).DisplayResults();
            }

        }

        #endregion

        #region Compiled Queries

        /// <summary>
        /// Precompiled Queries example
        /// </summary>
        private void CompiledQueries()
        {
            var dc = new NorthwindDataContext();

            //Precompile Query to retrieve customers in USA
            var usaCustomers = CompiledQuery.Compile((NorthwindDataContext ctx) => ctx.Customers
                .Where(c => c.Country == "USA")
                .Select(c => new
                {
                    c.CompanyName, 
                    c.City, 
                    c.Region
                }));

            "Customers in the USA".DisplayHeader();

            foreach (var cust in usaCustomers(dc))
            {
                string.Format("{0} in {1}, {2}", cust.CompanyName, cust.City, cust.Region).DisplayResults();
            }

        }

        #endregion

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
                Console.WriteLine("C: Compiled Queries");
                Console.WriteLine("D: Compiled Static Queries");
                Console.WriteLine("E: Direct Execution");
                Console.WriteLine("F: Transactions");
                Console.WriteLine("G: Data Validation");

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
                        CompiledQueries();
                        break;
                    case 'D':
                        CompiledStaticQueries();
                        break;
                    case 'E':
                        DirectExecution();
                        break;
                    case 'F':
                        Transactions1();
                        Transactions2();
                        Transactions3();
                        break;
                    case 'G':
                        DataValidation1();
                        DataValidation2();
                        DataValidation3();
                        break;
                    case 'H':
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
