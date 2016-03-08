using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoreLINQToSQL.Util
{
    public static class Display
    {
        #region Display Header

        public static void DisplayHeader(this string header)
        {
            Console.WriteLine("\n" + header + "\n============================================");
        }

        #endregion

        #region Display Results
        public static void DisplayResults(this string msg)
        {
            DisplayResults(msg, false);
        }

        public static void DisplayResults(this string msg, bool separator)
        {
            Console.WriteLine(msg);
            if (separator)
                Console.WriteLine();
        }

        public static void DisplayResults(this string msg, string item)
        {
            DisplayResults(msg, item, false);
        }

        public static void DisplayResults(this string msg, string item, bool addLine)
        {
            Console.WriteLine("\n" + msg + "\n============================================");

            Console.WriteLine(item);

            if(addLine)
                Console.WriteLine();

        }

        public static void DisplayResults(this string msg, IEnumerable items)
        {
           DisplayResults(msg, items, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="items"></param>
        /// <param name="SameLine"></param>
        public static void DisplayResults(this string msg, IEnumerable items, bool sameLine)
        {
            Console.WriteLine("\n" + msg + "\n============================================");

            foreach (var item in items)
            {
                if(sameLine)
                    Console.Write(item + " ");
                else
                    Console.WriteLine(item);
            }

            if(sameLine)
                Console.WriteLine();

            Console.WriteLine();
        }

        #endregion

        #region Display Orders

        public static void ShowOrderDetails(NorthwindDataContext dc, int orderId)
        {
            var orderDetails = dc.Order_Details.Where(o => o.OrderID == orderId);

            string.Format("Details for order {0}", orderId).DisplayHeader();

            foreach (var detail in orderDetails)
            {
                string.Format("{0} units of {1} at {2:C}",
                    detail.Quantity, detail.Product.ProductName, detail.UnitPrice).DisplayResults();
            }

            Console.WriteLine();
        }

        public static void ShowCustomerOrders(NorthwindDataContext dc, string custId)
        {
            var customerOrders =
                dc.Customers.Where(c => c.CustomerID == custId).Select(c => new { c.CompanyName, c.Orders });

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

            Console.WriteLine();
        }

        #endregion

    }
}
