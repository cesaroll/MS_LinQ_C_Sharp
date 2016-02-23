using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                Console.WriteLine("F: Object initialization (constructor)   O: Static Method");
                Console.WriteLine("G: Object initialization (revisited)     P: Extension method");
                Console.WriteLine("H: Add specific files                    Q: Query methods");
                Console.WriteLine("I: Test Delegates                        R: Enumerable methods");
                Console.WriteLine("                                         S: Anonymous type");

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
                        
                        break;
                    case 'M':
                        
                        break;
                    case 'N':
                        
                        break;
                    case 'O':
                        
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
