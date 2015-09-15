using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Useful_Classes
{
    public static class StringManipulation
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static string GenerateRandomString(int size)
        {
            if (!Licencing.validate())
                return null;

            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string GenerateRandomString()
        {
            if (!Licencing.validate())
                return null;

            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < 125; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string UpperCaseFirstLetter(string data)
        {
            try
            {
                return data.ToCharArray()[0].ToString().ToUpper() + data.ToLower().Remove(0, 1);
            }
            catch
            { }

            return data;
        }

        public static string UpperCaseEveryWord(string data)
        {
            try
            {
                string newLabel = "";
                foreach (string text in data.Split(' '))
                {
                    int ic = 0;
                    while (!char.IsLetter(text.ToCharArray()[ic]))
                    {
                        ic++;
                        if (text.Length == ic)
                        {
                            ic = 0;
                            break;
                        }
                    }

                    if (ic == 0)
                        newLabel += text.ToCharArray()[ic].ToString().ToUpper() + text.ToLower().Remove(0, 1) + " ";
                    else
                    {
                        string newString = "";
                        char[] charsInTxt = text.ToString().ToLower().ToCharArray();

                        charsInTxt[ic] = charsInTxt[ic].ToString().ToUpper().ToCharArray()[0];

                        for (int icc = 0; icc < charsInTxt.Length; icc++)
                        {
                            newString += charsInTxt[icc];
                        }

                        newLabel += newString + " ";
                    }
                }

                return newLabel.Remove(newLabel.Length - 1, 1);
            }
            catch
            { }

            return data;
        }

        public static string TruncateAtWord(this string value, int length)
        {
            if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
                return value;

            return value.Substring(0, value.IndexOf(" ", length));
        }

        public static string RemoveHtmlMarkup(string data)
        {
            return StripTagsCharArray(data);
        }

        private static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        public static byte[] StringToByteArray(string str)
        {
            return StringToByteArray(str, EncodingType.Unicode);
        }
        public static byte[] StringToByteArray(string str, EncodingType encodingType)
        {
            System.Text.Encoding encoding = null;
            switch (encodingType)
            {
                case EncodingType.ASCII:
                    encoding = new System.Text.ASCIIEncoding();
                    break;
                case EncodingType.Unicode:
                    encoding = new System.Text.UnicodeEncoding();
                    break;
                case EncodingType.UTF7:
                    encoding = new System.Text.UTF7Encoding();
                    break;
                case EncodingType.UTF8:
                    encoding = new System.Text.UTF8Encoding();
                    break;
            }
            return encoding.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            return ByteArrayToString(bytes, EncodingType.Unicode);
        }

        public static string ByteArrayToString(byte[] bytes, EncodingType encodingType)
        {
            System.Text.Encoding encoding = null;
            switch (encodingType)
            {
                case EncodingType.ASCII:
                    encoding = new System.Text.ASCIIEncoding();
                    break;
                case EncodingType.Unicode:
                    encoding = new System.Text.UnicodeEncoding();
                    break;
                case EncodingType.UTF7:
                    encoding = new System.Text.UTF7Encoding();
                    break;
                case EncodingType.UTF8:
                    encoding = new System.Text.UTF8Encoding();
                    break;
            }
            return encoding.GetString(bytes);
        }

        public enum EncodingType
        {
            ASCII,
            Unicode,
            UTF7,
            UTF8
        }

        /// <summary>
        ///  Credit: http://stackoverflow.com/a/11743162
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        ///  Credit: http://stackoverflow.com/a/11743162
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
