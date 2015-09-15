using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Useful_Classes
{
    public static class MyConsole
    {
        public static void Write(string message)
        {
            Console.WriteLine(String.Format("{0, -50}", message));
        }
        public static void Write(string message, bool status)
        {
            Console.ForegroundColor = ((status) ? Console.ForegroundColor : ConsoleColor.Red);
            Console.Write(String.Format("{0, -50}", message));
            Console.ForegroundColor = ((status) ? ConsoleColor.Green : ConsoleColor.Red);
            Console.Write(String.Format("{0, 10}", ((status) ? "[OKAY]" : "[FAIL]")));
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void Write()
        {
            Console.WriteLine();
        }

        public static void Break()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(false);
        }
    }
}
