using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Useful_Classes.MyComputer.GetValidFilePathInWindows(@"C:\Users\hhh\Documents\FTPbox\bbb.eee.net|xxx@bbb.eee.net"));
            Console.ReadLine();
        }
    }
}
