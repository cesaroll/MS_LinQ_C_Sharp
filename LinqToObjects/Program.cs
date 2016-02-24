using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Global;

namespace LinqToObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();

            prog.Menu();
        }

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
