using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Global;
using Global.Comparer;
using LinQDemo.Model;

namespace LinqToObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();

            prog.Menu();
        }

        #region Aggregate Calculations

        private void AggregateCalculations()
        {
            var products = Northwind.Products.Where(p => p.CategoryID == 1);

            /* Use the agregate method to perform a calculation of the sum of the squares
             * of the item prices, so you can calculate the variance in the prices, 
             * and take its square root to determine its standard deviation.
             * Calculate the sum of the squares of the difference between each price and 
             * the average price, and divide by the count of items - 1.
             * Take the square root of the result to calculate the
             * standard deviation.
             */

            var averagePrice = products.Average(p => p.UnitPrice);

            var sumSquares = products
                .Aggregate(0,
                    (decimal current, Product item) =>
                        current + (decimal) (Math.Pow((double) (item.UnitPrice - averagePrice), 2)));

            decimal variance = sumSquares/products.Count();

            var standardDeviation = Math.Sqrt((double)variance);

            string.Format("Standard Deviaton = {0:N2}", standardDeviation).DisplayResults();


        }
        #endregion

        #region Simple Calculations

        private void SimpleCalculations()
        {
            var products = Northwind.Products;
            var decimalResults = from product in products
                select product.UnitPrice;

            var averagePrice = decimalResults.Average();
            string.Format("Price average = {0:C}", averagePrice).DisplayResults();

            //You can pass a lambda expression to calculating methods
            averagePrice = products.Average(p => p.UnitPrice);
            string.Format("Price average (lambda) = {0:C}", averagePrice).DisplayResults();

            //Count
            var productCount = products.Count();
            string.Format("Count = {0}", productCount).DisplayResults();

            //Count of products starting with "B"
            productCount = products.Count(p => p.ProductName.StartsWith("B"));
            string.Format("Start with \"B\" Count = {0}", productCount).DisplayResults();

            //Retrieve Maximum Price
            var maxPrice = products.Max(p => p.UnitPrice);
            string.Format("Max price = {0:C}", maxPrice).DisplayResults();

            //Calculate Sum of Units in Stock
            var total = products.Sum(p => p.UnitsInStock);
            string.Format("Total Units in Stock = {0}", total).DisplayResults();

        }
        #endregion

        #region Positionning Sequences

        private void PositionWithSequences()
        {
            //Mixing Query Syntax with Functinal Syntax
            var products = (from prod in Northwind.Products
                orderby prod.ProductName
                select string.Format("{0}: {1}", prod.ProductID, prod.ProductName))
                .Skip(10)
                .Take(5);

            "Products Query (Skip 10 Take 5):".DisplayResults(products);

            //Using functional Syntax only
            products = Northwind.Products
                .OrderBy(p => p.ProductName)
                .Select(p => string.Format("{0}: {1}", p.ProductID, p.ProductName))
                .Skip(10)
                .Take(5);

            "Products Functional (Skip 10 Take 5):".DisplayResults(products);
        }

        #endregion

        #region Converting Sequences

        private void ConvertingSequences()
        {
            var products = Northwind.Products.Where(p => p.CategoryID == 2);

            var nameList = string.Join(", ", products.Select(p => p.ProductName).ToArray()); //ToArray is not necesary but we are testing conversions
            "Name List:".DisplayResults(nameList);

            //Convert Products to Dictionary
            var productsDictionary = products.ToDictionary(p => p.ProductID);

            var sw = new StringWriter();
            foreach (var kvp in productsDictionary)
            {
                sw.WriteLine("{0}: {1}", kvp.Key, kvp.Value.ProductName);
            }

            "Dictionary Contents:".DisplayResults(sw.ToString());

        }

        #endregion

        #region Verifying Sequences

        private void VerifyingSequences()
        {
            var products = Northwind.Products;

            var results = products.Where(prod => prod.CategoryID == 1);
            "Products Where CategoryID == 1".DisplayResults(results);

            //Determine is a list has any elements
            var anyElements = results.Any();
            string.Format("Are there Any elements: {0}", anyElements).DisplayResults();
            
            //Determine if any poducts are discontinued
            var anyDiscontinued = results.Any(p => p.Discontinued);
            string.Format("Any elements discontinued: {0}", anyDiscontinued).DisplayResults();

            // Do any products start with "M"
            var matchingElements = results.Any(p => p.ProductName.StartsWith("M"));
            string.Format("Any elements start with \"M\": {0}", matchingElements).DisplayResults();

            //Do all products have at least 5 units in stock?
            var unitsInStockOK = results.All(p => p.UnitsInStock >= 5);
            string.Format("All have 5 units in stock: {0}", unitsInStockOK).DisplayResults();

            //Check if collection contains a given product
            var item = new Product()
            {
                ProductID = 1,
                ProductName = "Chai"
            };

            var containsChai = results.Contains(item, new ProductComparer());

            string.Format("Contains Chai: {0}", containsChai).DisplayResults();

        }
        #endregion

        #region Filter With Where

        private void FilterWithWhere()
        {
            var files = Files.GetFiles(searchPath);

            var fileResult = files
                .Where(file => file.Length < 100)
                .Select(file => string.Format("{0} ({1})", file.Name, file.Length));

            "File Results:".DisplayResults(fileResult);

            //Can use index as well
            //In this case search only in the first 10 files of the original list
            fileResult = files
                .Where((file, idx) => (file.Length < 100) & (idx < 10))
                .Select(file => string.Format("{0} ({1})", file.Name, file.Length));
            "Indexed Results:".DisplayResults(fileResult);
        }

        #endregion

        #region Single Elements

        private void SingleElements()
        {
            var products = Northwind.Products;

            var query1 = from prod in products
                where prod.ProductID == 1
                select prod;

            var product = query1.SingleOrDefault();

            "Product 1: ".DisplayResults(product.ProductName);

            var prodInCat5 = from prod in products
                where prod.CategoryID == 5
                select prod;

            //Find the last product in category 5

            product = prodInCat5.LastOrDefault();

            "Last in Category 5:".DisplayResults(product.ProductName);

            //These raise exceptions

            var query3 = products.Where(p => p.ProductID == 999);

            try
            {
                //There is no product where ID is 999
                product = query3.Single(); //Use SingleOrDefault() to return null instead of raising an exception
            }
            catch (Exception e)
            {
                "First Exception: ".DisplayResults(e.Message);
            }

            //Use SingleOrDefault() to return null instead of raising an exception
            product = query3.SingleOrDefault();
            string.Format("Product ID 999 is null: {0}", product == null).DisplayResults();
            
            try
            {
                //There are't 200 elements in category 5
                product = prodInCat5.ElementAt(200); 
            }
            catch (Exception e)
            {
                "Second Exception: ".DisplayResults(e.Message);
            }

            // use ElementAtOrDefault() to return null instead of raising an exception
            product = prodInCat5.ElementAtOrDefault(200);
            string.Format("Product 200 in Category 5 is null: {0}", product == null).DisplayResults();

        }
        #endregion
    
        #region Selecting Sequences

        private void SelectingSequences()
        {
            var products = Northwind.Products.Where(p => p.CategoryID == 1);

            var productsCatId1 = products.Where(p => p.CategoryID == 1);

            "Select:".DisplayResults(productsCatId1);

            //Selecting a formatted string using the index in the collection
            var idxResult = productsCatId1.Select((prod, idx) => string.Format("{0}. {1}", idx + 1, prod.ProductName));
            "Formatted string:".DisplayResults(idxResult);

        }
        #endregion
        
        #region Creating Sequences

        private void CreatingSequences()
        {
            var items = Enumerable.Range(1, 10);
            "Enumerable.Range(1,10):".DisplayResults(items, true);

            var repeated = Enumerable.Repeat("AppDev", 5);
            "Enumerable.Repeat(\"AppDev\", 5)".DisplayResults(repeated, true);

            //Reverse extends the collection class
            var reversed = items.Reverse();
            "items.Reverse()".DisplayResults(reversed, true);

        }
        #endregion

        #region Query Array List

        private void QueryArrayList()
        {
            var files = new ArrayList();

            files.AddRange(Files.GetFiles(searchPath));

            var query = from item in files.Cast<FileInfo>()
                where (item.Attributes & FileAttributes.Archive) == FileAttributes.Archive
                select new {item.Name, item.Attributes};

            "Query ArrayList (Cast):".DisplayResults(query);

            //OR
            query = from FileInfo item in files
                    where (item.Attributes & FileAttributes.Archive) == FileAttributes.Archive
                    select new { item.Name, item.Attributes };

            "Query ArrayList (Expicit):".DisplayResults(query);
        }

        #endregion

        #region Query String

        private void QueryString()
        {
            var testString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var vowels = testString.Where(ch => ch.IsVowel());
            var vowelCount = vowels.Count();

            string.Format("The string \"{0}\" contains {1} vowel(s)", testString, vowelCount).DisplayResults();
            "The vowels are:".DisplayResults(vowels, true);

        }

        #endregion

        #region Query Generic Dictionary

        private void QueryGenericDictionary()
        {
            var filesDictionary = Files.GetFiles(searchPath).ToDictionary(x => x.Name, x => x);

            var query = from item in filesDictionary
                let size = item.Value.Length
                where size < 1024
                orderby size descending, item.Key
                select new {item.Key, size};

            "Generic Dictionary:".DisplayResults(query);

        }

        #endregion

        #region Query Generic List

        private void QueryGenericList()
        {
            var files = Files.GetFiles(searchPath);

            //SQl Syntax

            var query = from fi in files
                where fi.Length < 1024
                orderby fi.Length descending, fi.Name
                select new {fi.Name, fi.Length};
            
            "Generic List (SQL Syntax)".DisplayResults(query);

            //Functional Syntax
            var query2 = files.Where(fi => fi.Length < 1024)
                .OrderByDescending(fi => fi.Length)
                .ThenBy(fi => fi.Name)
                .Select(fi => new {fi.Name, fi.Length});

            "Generic List (Functional Syntax)".DisplayResults(query2);
        }
        #endregion

        #region Query Array of Different types

        private void QueryArrayOfDifTypes()
        {
            object[] array = {"Hello", 12, true, 'a', 123.45, DateTime.Parse("5/16/1956"), "Goodbye"};

            var query = from item in array
                let type = item.GetType().Name
                orderby type
                select new
                {
                    item,
                    type
                };

            "Arrays of different types:".DisplayResults(query);

            //Display type anme sorted
            //using SQl notation
            var query2 = from item in array
                let type = item.GetType().Name
                orderby type
                select type;

            "Sorted Types using SQL notation:".DisplayResults(query2);

            //Display type name sorted
            //using lambdas
            var query3 = array.Select(x => x.GetType().Name).OrderBy(x => x);
            "Sorted Types using Lambdas:".DisplayResults(query3);

            //Use the OfType extension Method to 
            //retrieve just strings
            var query4 = array.OfType<string>();
            "Elements of type String:".DisplayResults(query4);

            //Use the OfType extension Method to 
            //retrieve just ints
            var query5 = array.OfType<int>();
            "Elements of type Integer:".DisplayResults(query5);
        }
        #endregion

        #region Query Array

        private void QueryArray()
        {
            var files = new DirectoryInfo(searchPath).GetFiles();

            var query = from file in files
                where file.Length >128 && file.Length < 1024
                orderby file.Name
                select new {file.Name, file.Length };

            "Query Array:".DisplayResults(query);

            var formatted = from file in query
                            select String.Format("{0,15:N0}  {1}", file.Length, file.Name);

            "\nFormatted Query Array:".DisplayResults(formatted);

        }
        #endregion

        #region Menu

        private const string searchPath = @"C:\Windows";
        public void Menu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("A: Query Array               B: Query Array of Different Types");
                Console.WriteLine("C: Query Generic List        D: Query Generic Dictionary");
                Console.WriteLine("E: Query String              F: Query Array List");
                Console.WriteLine("G: Creating Sequences        H: Selecting Sequences");
                Console.WriteLine("I: Single Elements           J: Filter With Where");
                Console.WriteLine("K: Verifying Sequences       L: Converting Sequences");
                Console.WriteLine("M: Positionning Sequences    N: Simple Calculations");
                Console.WriteLine("O: Aggregate Calculations    P: Simple Set Operations");
                

                Console.WriteLine("\nEnter an Option (Press . to exit):");

                var key = Char.ToUpper(Console.ReadKey(true).KeyChar);
                Console.WriteLine(key);
                Thread.Sleep(200);

                Console.Clear();

                switch (key)
                {
                    case 'A':
                        QueryArray();
                        break;
                    case 'B':
                        QueryArrayOfDifTypes();
                        break;
                    case 'C':
                        QueryGenericList();
                        break;
                    case 'D':
                        QueryGenericDictionary();
                        break;
                    case 'E':
                        QueryString();
                        break;
                    case 'F':
                        QueryArrayList();
                        break;
                    case 'G':
                        CreatingSequences();
                        break;
                    case 'H':
                        SelectingSequences();
                        break;
                    case 'I':
                        SingleElements();
                        break;
                    case 'J':
                        FilterWithWhere();
                        break;
                    case 'K':
                        VerifyingSequences();
                        break;
                    case 'L':
                        ConvertingSequences();
                        break;
                    case 'M':
                        PositionWithSequences();
                        break;
                    case 'N':
                        SimpleCalculations();
                        break;
                    case 'O':
                        break;
                    case 'P':
                        AggregateCalculations();
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
