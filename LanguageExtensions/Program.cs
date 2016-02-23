using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LinQDemo;

namespace LanguageExtensions
{
    class Program
    {
        private static string _searchPath = @"C:\Windows"; // "C:\\Windows";

        static void Main(string[] args)
        {


            var prog = new Program();

            prog.Menu();

        }

        #region Anonymous Type

        private void AnonymousType()
        {
            DisplayResults("Anonymous Type");
            SearchForFiles9(_searchPath, fi => fi.Length > 1024);
        }

        private void SearchForFiles9(string path, Predicate<FileInfo> condition)
        {
            var items = new DirectoryInfo(path).GetFiles();
            var files = from item in items
                where condition(item)
                orderby item.Length descending
                select new
                {
                    Name = item.Name,
                    Size = item.Length,
                    CreationTime = item.CreationTime
                };

            

            //Notice you do not have the override ToString on MyFileInfo
            //since we are not using MyFileInfo class
            DisplayResults("Selected Files in anonymous type:", files);


            //Make one collection for display only
            var display = from item in files
                select String.Format("{0} {1,15:N0}  {2}",
                    item.CreationTime.ToString("g", DateTimeFormatInfo.InvariantInfo),
                    item.Size,
                    item.Name);

            DisplayResults("\nMerge anonymous type for better display:", display);

        }

        #endregion

        #region Enumerable methods
        /// <summary>
        /// Enumerable methods
        /// More COmplicated than Query Methods
        /// Do Not use ...
        /// </summary>
        private void EnumerableMethods()
        {
            DisplayResults("Enumerable Methods");
            SearchForFiles8a(_searchPath, fi => fi.Length > 512, mfi => -mfi.Length);
        }

        private void SearchForFiles8a(string path, Predicate<FileInfo> condition, Func<MyFileInfo, long> sorting)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo() { Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime });
                }
            }

            DisplayResults("Filter using function parameter: " + path, files.OrderBy(sorting));

            DisplayResults("\nTotal Size: " + files.TotalSize().ToString("N0"));

            DisplayResults("\nTop 5 Total Size: " + 
                Enumerable.Sum(
                    Enumerable.Take(
                        Enumerable.OrderByDescending(files, mfi => mfi.Length), 5), 
                mfi => mfi.Length).ToString("N0"));

        }

        #endregion

        #region Query methods

        private void QeryMethods()
        {
            DisplayResults("Query methods:");
            SearchForFiles8(_searchPath, fi => fi.Length > 512, mfi => -mfi.Length);
        }
        private void SearchForFiles8(string path, Predicate<FileInfo> condition, Func<MyFileInfo, long> sorting)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo() { Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime });
                }
            }

            DisplayResults("Filter using function parameter: " + path, files.OrderBy(sorting));

            DisplayResults("\nTotal Size: " + files.TotalSize().ToString("N0"));

            DisplayResults("\nTop 5 Total Size: " + files.OrderByDescending(mfi => mfi.Length).Take(5).Sum(mfi => mfi.Length).ToString("N0") );

        }
        #endregion

        #region Extension Method

        private void ExtensionMethod()
        {
            DisplayResults("Extension Method:");
            SearchForFiles7(_searchPath,
                fi => fi.Length > 512,
                sfi => -sfi.Length);
        }

        private void SearchForFiles7(string path, Predicate<FileInfo> condition, Func<MyFileInfo, long> sorting)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo() { Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime });
                }
            }

            DisplayResults("Filter using function parameter: " + path, files.OrderBy(sorting));

            DisplayResults("\nTotal Size: " + files.TotalSize().ToString("N0"));

        }

        #endregion

        #region Lambda parameters for sorting

        private void LambdaParametersForSorting()
        {
            DisplayResults("Lambda parameters for sorting:");
            SearchForFiles6c(_searchPath, 
                fi => fi.Length > 512,
                sfi => -sfi.Length);
        }

        private void SearchForFiles6c(string path, Predicate<FileInfo> condition, Func<MyFileInfo, long> sorting)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo() { Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime });
                }
            }

            DisplayResults("Filter using function parameter: " + path, files.OrderBy(sorting));

        }
        #endregion

        #region Lambda expressions for sorting

        private void LambdaExpressionsForSorting()
        {
            DisplayResults("Lambda expressions for sorting:");
            SearchForFiles6b(_searchPath, fi => fi.Length > 3000);
        }

        private void SearchForFiles6b(string path, Predicate<FileInfo> condition)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo() { Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime });
                }
            }

            DisplayResults("Filter using function parameter: " + path, files.OrderBy(fi => fi.Length));

        }

        #endregion

        #region Lambda Expressions

        private void LambdaExpressions()
        {
            DisplayResults("Lambda Expressions:");
            SearchForFiles6a(_searchPath, fi => fi.Length > 100 && fi.Length < 1000);
        }
        #endregion

        #region Anonymous Delegates
        private void AnonymousDelegates()
        {
            DisplayResults("Anonymous Delegates:");
            SearchForFiles6a(_searchPath, delegate(FileInfo fi)
            {
                return fi.Length > 100 && fi.Length < 10000;
            });
        }
        #endregion

        #region Test Predicate

        private void TestPredicate()
        {
            DisplayResults("Use Predicate Instance:");
            DisplayResults("Large Files:");
            SearchForFiles6a(_searchPath, LargeFiles);
            DisplayResults("Small Files:");
            SearchForFiles6a(_searchPath, SmallFiles);

        }

        private bool SmallFiles(FileInfo fi)
        {
            return fi.Length < 10000;
        }

        private void SearchForFiles6a(string path, Predicate<FileInfo> condition)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo() { Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime });
                }
            }

            DisplayResults("Filter using function parameter: " + path, files);

        }

        #endregion

        #region Test Delegates

        private delegate Boolean FileFilterDelegate(FileInfo fi);
        private void TestDelegates()
        {
            DisplayResults("Use Delegate instance:");
            SearchForFiles6(_searchPath, new FileFilterDelegate(LargeFiles));
        }

        private bool LargeFiles(FileInfo fi)
        {
            return fi.Length >= 10000;
        }

        private void SearchForFiles6(string path, FileFilterDelegate condition)
        {
            var files = new List<MyFileInfo>();

            foreach (var fi in new DirectoryInfo(path).GetFiles())
            {
                if (condition(fi))
                {
                    files.Add(new MyFileInfo(){Name = fi.Name, Length = fi.Length, CreationTime = fi.CreationTime});
                }
            }

            DisplayResults("Filter using function parameter: " + path, files);
        }

        #endregion

        #region Implicit declaration
        /// <summary>
        /// Introduce and use Implicit declaration
        /// </summary>
        private void UseImplicitDeclaration()
        {
            
        }
        #endregion

        #region Basic File information

        /// <summary>
        /// Use declared type to store information
        /// Manually copy the information to a new
        /// instance of MyFileInfo class
        /// </summary>
        /// <param name="path"></param>
        private void SearchForFiles2(string path)
        {
            var files = new List<MyFileInfo>();
            
            foreach (var fileInfo in new DirectoryInfo(path).GetFiles())
            {
                files.Add(new MyFileInfo()
                {
                    Name = fileInfo.Name,
                    Length = fileInfo.Length,
                    CreationTime = fileInfo.CreationTime
                });
            }

            DisplayResults(string.Format("Use a custom type: {0}", path), files);
        }

        /// <summary>
        /// Regular NO linq method
        /// </summary>
        /// <param name="path"></param>
        private void SearchForFiles1(string path)
        {
            var files = new List<string>();

            foreach (var fileInfo in new DirectoryInfo(path).GetFiles())
            {
                files.Add(fileInfo.Name);
            }

            DisplayResults(string.Format("Files in {0}", path), files);

        }
        #endregion

        #region Menu
        public void Menu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("A: Basic File information                J: Test Predicate");
                Console.WriteLine("B: File information with Class           K: Anonymous Delegates");
                Console.WriteLine("C:                                       L: Lambda Expressions");
                Console.WriteLine("D:                                       M: Lambda expressions for sorting");
                Console.WriteLine("E: Object initialization                 N: Lambda parameters for sorting");
                Console.WriteLine("F: Object initialization (constructor)   O: ");
                Console.WriteLine("G: Object initialization (revisited)     P: Extension Method");
                Console.WriteLine("H: Add specific files                    Q: Query methods");
                Console.WriteLine("I: Test Delegates                        R: Enumerable methods");
                Console.WriteLine("                                         S: Anonymous Type");

                Console.WriteLine("\nEnter an Option (Press . to exit):");

                var key = Char.ToUpper(Console.ReadKey(true).KeyChar);
                Console.WriteLine(key);
                Thread.Sleep(200);

                Console.Clear();

                switch (key)
                {
                    case 'A':
                        SearchForFiles1(_searchPath);
                        break;
                    case 'B':
                        SearchForFiles2(_searchPath);
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
                        TestDelegates();
                        break;
                    case 'J':
                        TestPredicate();
                        break;
                    case 'K':
                        AnonymousDelegates();
                        break;
                    case 'L':
                        LambdaExpressions();
                        break;
                    case 'M':
                        LambdaExpressionsForSorting();
                        break;
                    case 'N':
                        LambdaParametersForSorting();
                        break;
                    case 'O':
                        break;
                    case 'P':
                        ExtensionMethod();
                        break;
                    case 'Q':
                        QeryMethods();
                        break;
                    case 'R':
                        EnumerableMethods();
                        break;
                    case 'S':
                        AnonymousType();
                        break;
                    default:
                        return;
                }

                Console.ReadKey(true);

            }

        }
        #endregion

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
    }
}
