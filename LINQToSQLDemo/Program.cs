using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net.Sockets;
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

        #region Stored Procedures 2

        private void StoredProcedures2_3()
        {
            var dc = GetDataContext(true);

            var custID = "AAAAA";

            var customer = dc.Customers.SingleOrDefault(c => c.CustomerID == custID);

            if(customer == null)
            {
                string.Format("Customer: {0} NOT found!", custID).DisplayHeader();
                return;
            }

            //Delete the customer
            dc.Customers.DeleteOnSubmit(customer);
            dc.SubmitChanges();

            dc.Log = null;

            //Show Customers in TX
            var texasCustomers = dc.CustomersInRegion("TX");

            DisplayCustomers(texasCustomers);

        }

        private void StoredProcedures2_1()
        {
            // Add new customer
            var newCustomer = new Customer
            {
                CustomerID = "AAAAA",
                CompanyName = "AAAAA Consulting",
                ContactName = "Cesar Lopez",
                ContactTitle = "CEO",
                Address = "123 Main St.",
                City = "El Paso",
                Region = "TX",
                PostalCode = "55555",
                Country = "USA",
                Phone = "955-555-5555",
                Fax = "955-555-5555"
            };

            var dc = GetDataContext(true);

            dc.Customers.InsertOnSubmit(newCustomer);
            dc.SubmitChanges();

            dc.Log = null;

            //Show Customers in TX
            var texasCustomers = dc.CustomersInRegion("TX");

            DisplayCustomers(texasCustomers);


        }

        #endregion

        #region Stored Procedures 1

        private void StoredProcedures1_3()
        {
            var dc = GetDataContext(true);

            //Delete the customer
            dc.DeleteCustomer("AAAAA");

            dc.Log = null;

            //Show Customers in TX
            var texasCustomers = dc.CustomersInRegion("TX");

            DisplayCustomers(texasCustomers);

        }

        private void StoredProcedures1_1()
        {
            var dc = GetDataContext(true);

            // Add new customer
            dc.AddCustomer("AAAAA", "AAAAA Consulting", "Cesar Lopez", "CEO", "123 Main St.", "El Paso", "TX", "55555",
                "USA", "955-555-5555", "955-555-5555");

            dc.Log = null;

            //Show Customers in TX
            var texasCustomers = dc.CustomersInRegion("TX");
            
            DisplayCustomers(texasCustomers);

        }

        private void DisplayCustomers(ISingleResult<CustomersInRegionResult> cuir)
        {
            "Customers:".DisplayHeader();

            foreach (var cust in cuir)
            {
                string.Format("{0} in {1}, {2} - Contact: {3}", cust.CompanyName, cust.City, cust.Region, cust.ContactName).DisplayResults();
            }
            
            " ".DisplayResults();
            " ".DisplayResults();
        }

        #endregion

        #region Modify Related Data

        private void ModifyRelatedData()
        {
            var dc = GetDataContext();

            var customerID = "ALFKI";

            //Show Orders for a Cutomer
            ShowCustomerOrders(dc, customerID);


            //Retrieve full Customer
            var customerALFIKI = dc.Customers.FirstOrDefault(c => c.CustomerID == customerID);

            if (customerALFIKI == null)
                return;

            //Create new customer Order
            var newOrder = new Order
            {
                CustomerID   = customerID,
                OrderDate = DateTime.Today,
                RequiredDate = DateTime.Today.AddDays(7),
                EmployeeID = 4
            };

            //Add the order
            customerALFIKI.Orders.Add(newOrder);

            //Create and Add Order Details for this order
            newOrder.Order_Details.Add(new Order_Detail{ProductID = 7, UnitPrice = 25, Quantity = 10, Discount = 0});
            newOrder.Order_Details.Add(new Order_Detail{ProductID = 8, UnitPrice = 10, Quantity = 15, Discount = 0});
            newOrder.Order_Details.Add(new Order_Detail{ProductID = 12, UnitPrice = 20, Quantity = 25, Discount = 0});

            //Save Changes
            dc.SubmitChanges();

            //Record OrderID of the new order
            var lastOrderId = newOrder.OrderID;

            //Show Customer Orders after
            ShowCustomerOrders(dc, customerID);

            //Display Order Details
            ShowOrderDetails(dc, lastOrderId);

            /*
             *  SELECT * FROM Orders WHERE CustomerID = 'ALFKI' ORDER BY OrderDate DESC;
                SELECT * FROM [Order Details] WHERE OrderID = 11080;

                DELETE [Order Details] WHERE OrderID = 11080;
                DELETE Orders WHERE OrderID = 11080;
             * 
             */

        }

        private void ShowOrderDetails(NorthwindDataContext dc, int orderId)
        {
            var orderDetails = dc.Order_Details.Where(o => o.OrderID == orderId);
            
            string.Format("Details for order {0}", orderId).DisplayHeader();

            foreach (var detail in orderDetails)
            {
                string.Format("{0} units of {1} at {2:C}", 
                    detail.Quantity, detail.Product.ProductName, detail.UnitPrice).DisplayResults();
            }

        }

        private void ShowCustomerOrders(NorthwindDataContext dc,  string custId)
        {
            var customerOrders =
                dc.Customers.Where(c => c.CustomerID == custId).Select(c => new {c.CompanyName, c.Orders});

            var customer = customerOrders.FirstOrDefault();
            if (customer == null)
                return;

            string.Format("Orders for {0}", customer.CompanyName).DisplayHeader();

            foreach (var cust in customerOrders)
            {
                foreach (var order in cust.Orders.OrderByDescending(o => o.OrderDate))
                {
                    string.Format("Order {0} placed {1:d} and required {2:d}",
                        order.OrderID, order.OrderDate, order.RequiredDate).DisplayResults();
                }
            }
        }

        #endregion

        #region Modify Data

        private void ModifyData()
        {
            var dc = GetDataContext();

//            var beveragesBefore = from product in dc.Products
//                where product.Category.CategoryName == "Beverages"
//                select product;
            var beveragesBefore = dc.Products.Where(p => p.Category.CategoryName == "Beverages");

            "Current Prices:".DisplayHeader();

            foreach (var product in beveragesBefore)
            {
                string.Format("{0, 25} currently costs {1:C}", product.ProductName, product.UnitPrice).DisplayResults();
            }

            //Reduce Prices by 10% for Beverages
            foreach (var product in beveragesBefore)
            {
                product.UnitPrice = product.UnitPrice*0.9M;
            }

            //Save Changes
            dc.SubmitChanges();

            //Retrieve again and display new prices
            var beveragesAfter = dc.Products.Where(p => p.Category.CategoryName == "Beverages");

            "New Prices:".DisplayHeader();
            foreach (var product in beveragesBefore)
            {
                string.Format("{0, 25} now costs {1:C}", product.ProductName, product.UnitPrice).DisplayResults();
            }
        }

        private void ModifyData2()
        {
            var dc = GetDataContext();

            //Retrieve a list of customers in Whashington
            var waCustomersBefore =
                dc.Customers.Where(c => c.Region == "WA").Select(c => new {c.CompanyName, c.City, c.Region});

            "WA Customers Before:".DisplayHeader();

            foreach (var item in waCustomersBefore)
            {
                string.Format("{0,15} in {1}, {2}", item.CompanyName, item.City, item.Region).DisplayResults();
            }

            var newCustomer = new Customer()
            {
                CustomerID = "BIGGE",
                CompanyName = "Bigge Industries",
                ContactName = "Robert Green",
                ContactTitle = "President",
                Address = "1234 NE Some St.",
                City = "Redmond",
                Region = "WA",
                PostalCode = "98052",
                Country = "USA",
                Phone = "425-555-5555",
                Fax = "425-555-5556"
            };


            //Add the customer
            dc.Customers.InsertOnSubmit(newCustomer);

            //Save Changes
            dc.SubmitChanges();

            //Retrieve a list of customers in Whashington
            var waCustomersAfter =
                dc.Customers.Where(c => c.Region == "WA").Select(c => new { c.CompanyName, c.City, c.Region });

            "WA Customers After:".DisplayHeader();

            foreach (var item in waCustomersAfter)
            {
                string.Format("{0,15} in {1}, {2}", item.CompanyName, item.City, item.Region).DisplayResults();
            }


        }

        private void ModifyData3()
        {
            var dc = GetDataContext();

            //Retrieve customer
            var customer = dc.Customers.FirstOrDefault(c => c.CustomerID == "BIGGE");

            if (customer == null)
                return;

            "Contact information for Bigge Industries:".DisplayHeader();

            string.Format("Current contact is {0}, {1}", customer.ContactName, customer.ContactTitle).DisplayResults();

            //Modify Customer Information

            customer.ContactName = "Ken Getz";
            customer.ContactTitle = "Owner";

            //Save Changes
            dc.SubmitChanges();

            //Rettreive information ona customer
            //Retrieve customer
            var customer2 = dc.Customers.FirstOrDefault(c => c.CustomerID == "BIGGE");

            if (customer2 == null)
                return;

            string.Format("Current contact is {0}, {1}", customer2.ContactName, customer2.ContactTitle).DisplayResults();

        }

        private void ModifyData4()
        {
            var dc = GetDataContext();

            var customerBigge = dc.Customers.FirstOrDefault(c => c.CustomerID == "BIGGE");

            if (customerBigge == null)
            {
                "BIGGE NOT found!".DisplayHeader();
                return;
            }
                

            //Delete Customer
            dc.Customers.DeleteOnSubmit(customerBigge);

            //Save Changes
            dc.SubmitChanges();

            //Retrieve a list of customers in Whashington
            var waCustomersAfter =
                dc.Customers.Where(c => c.Region == "WA").Select(c => new { c.CompanyName, c.City, c.Region });

            "WA Customers After:".DisplayHeader();

            foreach (var item in waCustomersAfter)
            {
                string.Format("{0,15} in {1}, {2}", item.CompanyName, item.City, item.Region).DisplayResults();
            }
        }

        #endregion

        #region Joins

        private void Joins1()
        {
            var dc = GetDataContext();

            //Products in the Beverages category
            var beverages = from category in dc.Categories
                            join product in dc.Products
                    on category.CategoryID equals product.CategoryID
                where category.CategoryName == "Beverages"
                select new
                {
                    product.ProductName,
                    product.UnitPrice
                };

            "Products in the Beverages category".DisplayResults();

            foreach (var beverage in beverages)
            {
                string.Format("{0} ({1:C})", beverage.ProductName, beverage.UnitPrice).DisplayResults();
            }

        }

        private void Joins2()
        {
            var dc = GetDataContext();

            //Customers in Spain and how many orders they have placed.
            //Inner Join
            var spainCustomers = from cust in dc.Customers
                where cust.Country == "Spain"
                join order in dc.Orders
                    on cust equals order.Customer
                group order by order.Customer.CompanyName
                into groupedOrders
                select new
                {
                    CompanyName = groupedOrders.Key,
                    Orders = groupedOrders
                };

            string.Format("{0} customers in Spain have placed orders", spainCustomers.Count()).DisplayResults();

            foreach (var custOrd in spainCustomers)
            {
                string.Format("{0} placed {1} orders", custOrd.CompanyName, custOrd.Orders.Count()).DisplayResults();
            }

        }

        private void Joins3()
        {
            var dc = GetDataContext();

            //Customers in Spain and how many orders they have placed.
            //Outer Join
            var spainCustomers = from cust in dc.Customers
                                 where cust.Country == "Spain"
                                 join order in dc.Orders
                                     on cust equals order.Customer
                                 into groupedOrders
                                 select new
                                     {
                                         CompanyName = cust.CompanyName,
                                         Orders = groupedOrders
                                     };

            string.Format("There are {0} customers in Spain", spainCustomers.Count()).DisplayResults();

            foreach (var custOrd in spainCustomers)
            {
                string.Format("{0} placed {1} orders", custOrd.CompanyName, custOrd.Orders.Count()).DisplayResults();
            }


        }

        #endregion

        #region Grouping

        private void Grouping1()
        {
            //Customers in Spain grouped by City
            var spainCustomers = from customer in GetDataContext().Customers
                where customer.Country == "Spain"
                group customer by customer.City
                into groupedCustomers
                select new
                {
                    City = groupedCustomers.Key,
                    Customers = groupedCustomers
                };

            "Customers in Spain:".DisplayHeader();

            foreach (var groupedCity in spainCustomers)
            {
                string.Format("{0} [{1} customer(s)]", groupedCity.City, groupedCity.Customers.Count()).DisplayResults();

                foreach (var customer in groupedCity.Customers)
                {
                    string.Format("  {0} at {1}", customer.CompanyName, customer.Address).DisplayResults();
                }

            }

        }

        private void Grouping2()
        {
            
            //Categories with more than 10 products
            var categories = from product in GetDataContext().Products
                group product by product.Category.CategoryName
                into groupedProducts
                where groupedProducts.Count() > 10
                select new
                {
                    CategoryName = groupedProducts.Key,
                    Products = groupedProducts
                };

            "Categories with more than 10 products".DisplayResults();

            foreach (var category in categories)
            {
                string.Format("{0} [{1} products]", category.CategoryName, category.Products.Count()).DisplayResults();

                foreach (var product in category.Products)
                {
                    string.Format("   {0}", product.ProductName).DisplayResults();
                }

            }

        }

        private void Grouping3()
        {
            //Annual revenue
            var revenueByYear = from detail in GetDataContext().Order_Details
                group detail by detail.Order.OrderDate.Value.Year
                into groupedOrders
                orderby groupedOrders.Key descending
                select new
                {
                    Year = groupedOrders.Key,
                    Revenue = groupedOrders.Sum(o => o.UnitPrice * o.Quantity)
                };

            "Annual Revenue".DisplayHeader();

            foreach (var revenue in revenueByYear)
            {
                string.Format("{0} ({1:C})", revenue.Year, revenue.Revenue).DisplayResults();
            }

        }

        #endregion

        #region Extension Methods

        private void ExtensionMethods()
        {
            //Value of orders for product 7
            var valueDollars =
                GetDataContext().Order_Details.Where(o => o.ProductID == 7).Sum(o => o.Quantity*o.UnitPrice);

            "Orders Summary for product 7".DisplayHeader();

            string.Format("The total value or orders was {0:C} dollars", valueDollars).DisplayResults();

            //Reeturn the value or orders in euros in the linq expression

            var valueEuros =
                GetDataContext().Order_Details.Where(o => o.ProductID == 7).Sum(o => o.Quantity*o.UnitPrice).ToEuros();


            string.Format("The total value or orders was {0:C} Euros", valueEuros).DisplayResults();
        }

        #endregion

        #region Lambda Expressions

        private void LambdaExpressions1()
        {
            //Revenue by beverage product

            var beverages = (from product in GetDataContext().Products
                where product.Category.CategoryName == "Beverages"
                select new
                {
                    product.ProductName,
                    Number = product.Order_Details.Count,
                    Revenue = product.Order_Details.Sum(o => o.Quantity*o.UnitPrice)
                }).OrderByDescending(b => b.Revenue);

            "Revenue by beverage product:".DisplayHeader();

            foreach (var beverage in beverages)
            {
                string.Format("{0} was ordered {1} times for a total of {2:C}",
                    beverage.ProductName, beverage.Number, beverage.Revenue).DisplayResults();
            }

        }

        private void LambdaExpressions2()
        {
            //return the toal number of orders for product 7
            var orders = GetDataContext().Order_Details.Count(o => o.ProductID == 7);

            string.Format("Product 7 was ordered {0} times", orders).DisplayResults();

        }

        private void LambdaExpressions3()
        {
            // Retrieve order 10530
            var order = GetDataContext().Orders.FirstOrDefault(o => o.OrderID == 10530);

            if (order != null)
            {
                "Order 10530".DisplayHeader();
                string.Format("Order Date: {0}\nRequired Date: {1}\nShipped Date: {2}",
                    order.OrderDate, order.RequiredDate, order.ShippedDate).DisplayResults();
            }
            else
            {
                "Order 10530 does not exist!".DisplayHeader();
            }

            

        }

        #endregion

        #region Querying related tables

        private void QueryRelatedTables1()
        {
            //Products in the Beverages category
            var beverages = from product in GetDataContext(true).Products
                where product.Category.CategoryName == "Beverages"
                select new
                {
                    product.ProductName,
                    product.UnitPrice
                };

            "Products in the Beverages Category:".DisplayHeader();

            foreach (var item in beverages)
            {
                string.Format("{0} {1:C}", item.ProductName, item.UnitPrice).DisplayResults();
            }

        }

        private void QueryRelatedTables2()
        {
            //Customers with 20 or more orders
            var highOrdercustomers = from customer in GetDataContext().Customers
                where customer.Orders.Count >= 20
                select new
                {
                    Name = customer.CompanyName,
                    Count = customer.Orders.Count
                };

            "Customer with 20 or more orders".DisplayHeader();

            foreach (var item in highOrdercustomers)
            {
                string.Format("{0} placed {1} orders", item.Name, item.Count).DisplayResults();
            }

        }

        private void QueryRelatedTables3()
        {
            // Customers with 3 or fewer orders
            var lowOrderCustomers = from customer in GetDataContext().Customers
                where customer.Orders.Count <= 3
                select new
                {
                    customer.CompanyName,
                    customer.City,
                    customer.Region,
                    customer.Orders
                };

            "Customers with 2 or fewer orders:".DisplayHeader();

            foreach (var cust in lowOrderCustomers)
            {
                string.Format("{0} in {1}, {2} placed {3} orders",
                    cust.CompanyName, cust.City, cust.Region, cust.Orders.Count).DisplayResults();
            }

        }

        private void QueryRelatedTables4()
        {
            //Return total value of orders for
            //Blauer See Delikatessen

            var total =
                GetDataContext()
                    .Order_Details.Where(od => od.Order.Customer.CompanyName == "Blauer See Delikatessen")
                    .Select(od => od.UnitPrice*od.Quantity)
                    .Sum();

            "Orders summary for Blauer See Delikatessen".DisplayHeader();
            string.Format("Total value of orders: {0:C}", total).DisplayResults();

        }
        #endregion

        #region Aggregate Functions

        private void AggregateFunction1()
        {
            var totalOrders = GetDataContext(true).Orders.Count();

            string.Format("There are {0} total orders.", totalOrders).DisplayResults();
        }

        private void AggregateFunction2()
        {
            //Total orders for product 7
            //var totalOrdersForSeven = GetDataContext(true).Order_Details.Where(od => od.ProductID == 7).Count();
            var totalOrdersForSeven = GetDataContext(true).Order_Details.Count(od => od.ProductID == 7);

            string.Format("There are {0} orders for product \"7\"", totalOrdersForSeven).DisplayResults();

            //Return value of orders for product 7
            var valueOrdersForSeven =
                GetDataContext(true).Order_Details.Where(od => od.ProductID == 7).Sum(od => od.Quantity*od.UnitPrice);

            string.Format("Value of orders for product \"7\" is: {0:C}", valueOrdersForSeven).DisplayResults();

            //Return average value of orders for product 7
            var average =
                GetDataContext(true)
                    .Order_Details.Where(od => od.ProductID == 7)
                    .Select(od => od.Quantity*od.UnitPrice)
                    .Average();

            string.Format("Average value of orders for product \"7\": {0:C}", average).DisplayResults();

            //Return max value of orders for product 7
            var max =
                GetDataContext(true)
                    .Order_Details.Where(od => od.ProductID == 7)
                    .Select(od => od.Quantity*od.UnitPrice)
                    .Max();

            string.Format("Max value of order for product \"7\": {0:C}", max).DisplayResults();

            //Return min value of orders for product 7
            var min =
                GetDataContext(true)
                    .Order_Details.Where(od => od.ProductID == 7)
                    .Select(od => od.Quantity * od.UnitPrice)
                    .Min();

            string.Format("Min value of order for product \"7\": {0:C}", min).DisplayResults();

        }

        #endregion

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
                Console.WriteLine("C: Aggregate Functions                   D: Querying related tables");
                Console.WriteLine("E: Lambda Expressions                    F: Extension Methods");
                Console.WriteLine("G: Grouping                              H: Joins");
                Console.WriteLine("I: Modify Data                           J: Modify Related Data");
                Console.WriteLine("K: Stored Procedures 1                   L: Stored Procedures 2");

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
                        AggregateFunction1();
                        AggregateFunction2();
                        break;
                    case 'D':
                        QueryRelatedTables1();
                        QueryRelatedTables2();
                        QueryRelatedTables3();
                        QueryRelatedTables4();
                        break;
                    case 'E':
                        LambdaExpressions1();
                        LambdaExpressions2();
                        LambdaExpressions3();
                        break;
                    case 'F':
                        ExtensionMethods();
                        break;
                    case 'G':
                        Grouping1();
                        Grouping2();
                        Grouping3();
                        break;
                    case 'H':
                        Joins1();
                        Joins2();
                        Joins3();
                        break;
                    case 'I':
                        //ModifyData();
                        ModifyData2();
                        //ModifyData3();
                        ModifyData4();
                        break;
                    case 'J':
                        ModifyRelatedData();
                        break;
                    case 'K':
                        StoredProcedures1_1();
                        StoredProcedures1_3();
                        break;
                    case 'L':
                        StoredProcedures2_1();
                        StoredProcedures2_3();
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
