using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            string str = "";
            if (!string.IsNullOrEmpty(input))
            {
                string[] strArray = input.Split(' ', '-', '.');
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(strArray[0].ToLower());
                strArray[0] = string.Empty;
                foreach (char[] chArray in Enumerable.Select<string, char[]>(strArray,s => s.ToCharArray()))
                {
                    if (chArray.Length > 0)
                        chArray[0] = new string(chArray[0], 1).ToUpper().ToCharArray()[0];
                    stringBuilder.Append(new string(chArray));
                }
                str = stringBuilder.ToString();
            }
            return str;
        }
    }
}