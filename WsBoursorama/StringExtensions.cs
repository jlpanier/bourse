using System.Globalization;
using System.Text.RegularExpressions;

namespace Business
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string ReplaceAll(this string text, string search, string replace)
        {
            string result = text;
            while (result.Contains(search))
            {
                result = result.ReplaceFirst(search, replace);
            }
            return result;
        }

        public static string Substring(this string text, string startwith, string endwith)
        {
            int a = text.IndexOf(startwith);
            int b = text.IndexOf(endwith);
            return text.Substring(a, b - a + 1);
        }

        public static string TruncTag(this string tag)
        {
            if (tag == null)
            {
                return "";
            }
            else if (tag.Length < 23)
            {
                return tag;
            }
            else
            {
                return tag.Substring(0, 23);
            }
        }

        public static bool IsPhoneNumber(this string tag, CultureInfo cultureinfo = null)
        {
            string phonenumber = Regex.Replace(tag, "[() ]", "");
            if (string.IsNullOrEmpty(phonenumber))
            {
                return false;
            }
            else
            {
                bool result = false;
                CultureInfo info = cultureinfo == null ? new CultureInfo("fr-FR") : cultureinfo;
                switch (info.Name)
                {
                    case "fr-FR":
                        result = Regex.IsMatch(phonenumber, @"^(33|0|0033)[1-9]\d{8}$");
                        break;
                }
                return result;
            }
        }

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
