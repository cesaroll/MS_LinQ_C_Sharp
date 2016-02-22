using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

                Console.WriteLine("A: NumbersGreaterThan5");
                Console.WriteLine("B: MultipleReturnValues1");
                Console.WriteLine("C: MultipleReturnValues2");
                Console.WriteLine("D: DeferredExecution1");
                Console.WriteLine("E: DeferredExecution2");

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
                    default:
                        return;
                }

                Console.ReadKey(true);

            }
            
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

        private void DisplayResults(string msg)
        {
            Console.WriteLine(msg);
        }

        private void DisplayResults(string msg, IEnumerable items)
        {
            Console.WriteLine(msg + "\n============================================");

            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

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
