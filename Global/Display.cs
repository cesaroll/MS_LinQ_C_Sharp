using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            Console.WriteLine(msg + "\n============================================");

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
    }
}
