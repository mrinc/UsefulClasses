using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Useful_Classes
{
    class Licencing
    {
        public static bool validate()
        {
            return validateLicense();
        }

        protected static bool validateLicense()
        {
            try
            {
                //string FullName = Assembly.GetExecutingAssembly().FullName;

                //Console.WriteLine(FullName);

                return true;
            }
            catch
            { }
            return false;
        }
    }
}
