using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Common
{
    public static class Extensions
    {
        public static string Remove(this string text, string from, string to, StringComparison comparisonType)
        {
            int start = text.IndexOf(from, comparisonType);
            int end = text.IndexOf(to, comparisonType);
            return start >= 0 && end > 0 && start < end ? text.Substring(0, start) + text.Substring(end + to.Length) : text;
        }

        public static string Between(this string text, string from, string to, StringComparison comparisonType)
        {
            string result = text;
            int start = text.IndexOf(from, comparisonType);
            if (start > 0)
            {
                string startstring = text.Substring(start);
                int end = startstring.IndexOf(to, comparisonType);
                if (end > 0)
                {
                    result = startstring.Substring(0, end + to.Length);
                }
            }

            return result;
        }
    }

}
