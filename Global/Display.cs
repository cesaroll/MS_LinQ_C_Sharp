using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global
{
    public static class Display
    {
        #region Display Results
        public static void DisplayResults(this string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine();
        }

        public static void DisplayResults(this string msg, string item)
        {
            Console.WriteLine(msg + "\n============================================");

            Console.WriteLine(item);
            Console.WriteLine();
            
        }

        public static void DisplayResults(this string msg, IEnumerable items)
        {
            Console.WriteLine(msg + "\n============================================");

            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }

        #endregion
    }
}
