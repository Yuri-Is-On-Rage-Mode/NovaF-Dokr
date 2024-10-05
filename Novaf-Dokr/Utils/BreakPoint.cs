using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nova.Utils
{
    internal class BreakPoint
    {
        public static void hit(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("@info ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("@breakpoint ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("@hit ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text + "\n");
        }
    }
}
