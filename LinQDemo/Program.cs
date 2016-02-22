using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LinQDemo.Model;

namespace LinQDemo
{
    class Program
    {
        static void Main(string[] args)
        {


            var prog = new Program();

            prog.Menu();
            
        }

        public void Menu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("A: NumbersGreaterThan5       I: NumberQueryWithExtensionMethod");
                Console.WriteLine("B: MultipleReturnValues1     J: NumbersGrouping");
                Console.WriteLine("C: MultipleReturnValues2     K: DemoLinqToSql");
                Console.WriteLine("D: DeferredExecution1        L: DemoLinqToSqlWithDebugging");
                Console.WriteLine("E: DeferredExecution2        M: DemoLinqToSqlWithAnonymousType");
                Console.WriteLine("F: ForcingExecution          N: CreateXmlContent");
                Console.WriteLine("G: NumberQuery0              O: DemoLinqToXML");
                Console.WriteLine("H: NumberQuery1");

                Console.WriteLine("\nEnter an Option (Press . to exit):");

                var key = Char.ToUpper(Console.ReadKey(true).KeyChar);
                Console.WriteLine(key);
                Thread.Sleep(200);

                Console.Clear();

                switch (key)
                {
                    case 'A':
                        NumbersGreatherThan5();
                        break;
                    case 'B':
                        MultipleReturnValues1();
                        break;
                    case 'C':
                        MultipleReturnValues2();
                        break;
                    case 'D':
                        DeferredExecution1();
                        break;
                    case 'E':
                        DeferredExecution2();
                        break;
                    case 'F':
                        ForcingExecution();
                        break;
                    case 'G':
                        NumberQuery0();
                        break;
                    case 'H':
                        NumberQuery1();
                        break;
                    case 'I':
                        NumberQueryWithExtensionMethod();
                        break;
                    case 'J':
                        NumbersGrouping();
                        break;
                    case 'K':
                        DemoLinqToSql();
                        break;
                    case 'L':
                        DemoLinqToSqlWithDebugging();
                        break;
                    case 'M':
                        DemoLinqToSqlWithAnonymousType();
                        break;
                    case 'N':
                        CreateXmlContent();
                        break;
                    case 'O':
                        DemoLinqToXML();
                        break;
                    default:
                        return;
                }

                Console.ReadKey(true);

            }
            
        }

        #region LINQ to XML

        private void DemoLinqToXML()
        {
            var courses = CreateCourseList();

            var xml = new XElement("courses",
                    from course in courses
                    where course.Author.Contains("Robert Green")
                    select 
                        new XElement("course",
                            new XAttribute("year", course.Year),
                            new XElement("Title", course.Title),
                            new XElement("Auhtor", course.Author)));

            DisplayResults("Resulting XML", xml.ToString());
        }

        private void CreateXmlContent()
        {
            XElement xml = new XElement("courses",
                new XElement("course",
                    new XAttribute("Year", 2005),
                    new XElement("Title", "Introduction to Programming"),
                    new XElement("Author", "Ken Getz/Robert Green")));

            DisplayResults("Resultig XML:", xml.ToString());
        }
        
        private Course[] CreateCourseList()
        {
            Course[] courses =
            {
                new Course(){Title = "Introduction to LINQ", Year = 2008, Author = "Ken Getz/Robert Green"}, 
                new Course(){Title = "ASP.NET 3.5", Year = 2008, Author = "Don Kiely/Ken Getz"}, 
                new Course(){Title = "Introduction to Programming", Year = 2005, Author = "Ken Getz/Robert Green"} 
            };

            return courses;
        }

        class Course
        {
            public string Title;
            public int Year;
            public string Author;
        }
        #endregion

        private void DemoLinqToSqlWithAnonymousType()
        {
            //Create Data Context
            DataContext dc = new DataContext(NorthwindConnectionString);

            //use a StringWriter as an output stream for logging:
            var sw = new StringWriter();
            dc.Log = sw;

            var customers = from customer in dc.GetTable<Customer>()
                where customer.Country == "USA"
                select new
                {
                    customer.CompanyName,
                    customer.State
                };
            
            //Customer is a collection of a new type altogheter
            //That is why the override TpString method in Customer is not called
            DisplayResults("LINQ to SQL:", customers);
            

        }

        private void DemoLinqToSqlWithDebugging()
        {
            //Create Data Context
            DataContext dc = new DataContext(NorthwindConnectionString);

            //use a StringWriter as an output stream for logging:
            var sw = new StringWriter();
            dc.Log = sw;

            var customers = from cust in dc.GetTable<Customer>()
                            where cust.Country == "USA"
                            select cust;

            DisplayResults("Query: ", customers.ToString());
            DisplayResults("LINQ to SQL:", customers);
            DisplayResults("Log Output: ", sw.ToString());


        }
        private void DemoLinqToSql()
        {
            //Create Data Context
            DataContext dc = new DataContext(NorthwindConnectionString);

            //Returns a collection of objects of Customer type (Deferred execution)
            var customerTable = dc.GetTable<Customer>();

            var customers = from cust in customerTable
                where cust.Country == "USA"
                select cust;

            DisplayResults("LINQ to SQL:", customers);


        }

        public string NorthwindConnectionString {
            get
            {
                return ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
            }
        }

        private void NumbersGrouping()
        {
            int[] numbers = { 1, 5, 3, 6, 10, 12, 8 };

            var groups = from number in numbers
                orderby number ascending
                group number by number.IsOdd()
                into groupedNumbers
                orderby groupedNumbers.Key descending
                select new
                {
                    isOdd = groupedNumbers.Key,
                    groupedNumbers
                };

            foreach (var group in groups)
            {
                DisplayResults("IsOdd: " + group.isOdd, group.groupedNumbers);
                Console.WriteLine();
            }

        }

        private void NumberQueryWithExtensionMethod()
        {
            int[] numbers = { 1, 5, 3, 6, 10, 12, 8 };

            var oddNumbers = from number in numbers
                             where number.IsOdd()
                             orderby number descending
                             select number;

            DisplayResults("Odd numbers (using extension method)", oddNumbers);
        }
        
        private void NumberQuery1()
        {
            int[] numbers = { 1, 5, 3, 6, 10, 12, 8 };

            var oddNumbers = from number in numbers
                                where number % 2 != 0
                                orderby number descending 
                                select number;

            DisplayResults("Odd numbers", oddNumbers);
        }

        private void NumberQuery0()
        {
            int[] numbers = { 1, 5, 3, 6, 10, 12, 8 };

            var sortedNumbers = from number in numbers
                        orderby number
                        select number;

            DisplayResults("Sorted numbers", sortedNumbers);
        }

        private void ForcingExecution()
        {
            int[] numbers = { 1, 5, 3, 6, 10, 12, 8 };

            var query = from number in numbers
                        select DoubleIt(number);

            var items = query.ToList();

            query = from number in numbers
                        select DoubleIt(number + 10);

            DisplayResults("Converted to List:", items);
            DisplayResults("Values in query", query);

        }

        private void DeferredExecution2()
        {
            int[] numbers = { 1, 5, 3, 6, 10, 12, 8 };

            var query = from number in numbers
                orderby number descending
                select number;

            var query1 = from number in query
                where number < 8
                select number;

            var query2 = from number in query1
                select DoubleIt(number);

            DisplayResults("Deferred Execution revisited", query2);
        }
        
        private void DeferredExecution1()
        {
            int[] numbers = {1,5,3,6,10,12,8};

            var query = from number in numbers
                select DoubleIt(number);

            DisplayResults("Deferred Execution", query);
        }
        private long DoubleIt(int value)
        {
            DisplayResults("About to double the number " + value);
            return value*2;
        }

        #region Display Results
        private void DisplayResults(string msg)
        {
            Console.WriteLine(msg);
        }

        private void DisplayResults(string msg, string item)
        {
            Console.WriteLine(msg + "\n============================================");

            Console.WriteLine(item);
        }

        private void DisplayResults(string msg, IEnumerable items)
        {
            Console.WriteLine(msg + "\n============================================");

            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        #endregion

        private void NumbersGreatherThan5()
        {
            int[] numbers = {1,5,3,6,10,12,8};
            var outputNumbers = from number in numbers
                where number > 5
                orderby number
                select number;

            foreach (int number in outputNumbers)
            {
                Console.WriteLine(number);
            }

        }

        class MyFileInfo
        {
            public string Name;
            public long Length;
            public DateTime CreationTime;
        }
        private void MultipleReturnValues1()
        {
            //Globals.BreakForDebugging();

            var allFiles = new DirectoryInfo("C:\\").GetFiles();
            var files = from file in allFiles
                where file.Length > 10000
                orderby file.Length, file.Name
                select new MyFileInfo
                {
                    Name = file.Name,
                    Length = file.Length,
                    CreationTime = file.CreationTime
                };

            foreach (var fileInfo in files)
            {
                Console.WriteLine("{0} contains {1} bytes, created at {2}",
                    fileInfo.Name, fileInfo.Length, fileInfo.CreationTime);
            }
        }

        private void MultipleReturnValues2()
        {
            //Globals.BreakForDebugging();

            var allFiles = new DirectoryInfo("C:\\").GetFiles();
            var files = from file in allFiles
                        where file.Length > 10000
                        orderby file.Length, file.Name
                        select new
                        {
                            Name = file.Name,
                            Length = file.Length,
                            CreationTime = file.CreationTime
                        };

            foreach (var fileInfo in files)
            {
                Console.WriteLine("{0} contains {1} bytes, created at {2}",
                    fileInfo.Name, fileInfo.Length, fileInfo.CreationTime);
            }
        }
    }
}
