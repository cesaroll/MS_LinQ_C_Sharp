using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using LinQDemo.Model;

namespace Global
{
    public static class Northwind
    {
        private static string NorthwindConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
            }
        }

        public static IEnumerable<Product> Products
        {
            get
            {
                //Create Data Context
                DataContext dc = new DataContext(NorthwindConnectionString);

                return dc.GetTable<Product>();

            }
        }

    }
}