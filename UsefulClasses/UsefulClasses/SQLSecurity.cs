using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Useful_Classes
{
    public static class SQLSecurity
    {
        private static int injectLimit = 6;

        ///<summary>
        ///Get/Set the SQL injection lomit
        ///</summary>
        ///<param name="set">Set Injection Limit</param>
        ///<param name="get">Get Injection Limit</param>
        public static int SQLInjectionLimit
        {
            get
            {
                return injectLimit;
            }
            set
            {
                injectLimit = value;
            }
        }

        ///<summary>
        ///Do DeSQL Injection on string
        ///</summary>
        ///<param name="unSqldString">String to DeSql</param>
        ///<returns>DeSql String</returns> / <returns>LOL if limit is reached</returns>
        public static string lightDeSQL(string unSqldString)
        {
            if (!Licencing.validate())
                return null;

            try
            {
                bool found = true;
                int count = -1;
                while (found)
                {
                    count++;
                    unSqldString = doLightREP(unSqldString);
                    if (unSqldString.Equals(doLightREP(unSqldString)))
                    {
                        found = false;
                    }

                    if (count > injectLimit)
                        throw new Exception("LIMIT REACHED");
                }
                return unSqldString;
            }
            catch
            {
                return "LOL";
            }
        }

        ///<summary>
        ///Do DeSQL Injection on string
        ///</summary>
        ///<param name="unSqldString">String to DeSql</param>
        ///<returns>DeSql String</returns> / <returns>LOL if limit is reached</returns>
        public static string deSQL(string unSqldString)
        {
            if (!Licencing.validate())
                return null;

            try
            {
                bool found = true;
                int count = -1;
                while (found)
                {
                    count++;
                    unSqldString = doREP(unSqldString);
                    if (unSqldString.Equals(doREP(unSqldString)))
                    {
                        found = false;
                    }

                    if (count > injectLimit)
                        throw new Exception("LIMIT REACHED");
                }
                return unSqldString;
            }
            catch
            {
                return "LOL";
            }
        }

        private static String doREP(String s)
        {
            try
            {
                s = s.Replace("[", "!");
                s = s.Replace("]", "!");
                s = s.Replace("%", "!");
                s = s.Replace("xp_", "!!!");
                //s = s.Replace("_", "!");
                s = s.Replace(";", "!");
                s = s.Replace("'", "`");
                s = s.Replace("--", "!!");
                s = s.Replace("/*", "!!");
                s = s.Replace("*/", "!!");
                s = s.Replace("<", "!");
                s = s.Replace(">", "!");

                return s;
            }
            catch
            { return "LOL"; }
        }

        private static String doLightREP(String s)
        {
            try
            {
                //s = s.Replace("[", "!");
                //s = s.Replace("]", "!");
                s = s.Replace("%", "!");
                s = s.Replace("xp_", "!!!");
                //s = s.Replace("_", "!");
                //s = s.Replace(";", "!");
                s = s.Replace("'", "`");
                s = s.Replace("--", "!!");
                s = s.Replace("/*", "!!");
                s = s.Replace("*/", "!!");
                //s = s.Replace("<", "!");
                //s = s.Replace(">", "!");

                return s;
            }
            catch
            { return "LOL"; }
        }
    }
}
