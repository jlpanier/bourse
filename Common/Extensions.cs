namespace Common
{
    public static class Extensions
    {
        /// <summary>
        /// Conversion de la latitude en text
        /// </summary>
        public static string ToLatitude(this double latitude) => Latitude((decimal)latitude);

        /// <summary>
        /// Conversion de la latitude en text
        /// </summary>
        private static string Latitude(this decimal latitude)
        {
            decimal l = Math.Abs(latitude);
            int degree = (int)l;
            decimal left = 60 * (l - degree);
            int minutes = (int)left;
            left = 1000 * (left - minutes);
            int secondes = (int)left;
            string hemisphere = latitude < 0 ? "S" : "N";
            return $"{degree.ToString("000")}{hemisphere}{minutes.ToString("00")}.{secondes.ToString("000")}";
        }

        /// <summary>
        /// Conversion de la longitude en text
        /// </summary>
        public static string ToLongitude(this double longitude) => Longitude((decimal)longitude);

        /// <summary>
        /// Conversion de la longitude en text
        /// </summary>
        private static string Longitude(this decimal longitude)
        {
            decimal l = Math.Abs(longitude);
            int degree = (int)l;
            decimal left = 60 * (l - degree);
            int minutes = (int)left;
            left = 1000 * (left - minutes);
            int secondes = (int)left;
            string hemisphere = longitude < 0 ? "W" : "E";
            return $"{degree.ToString("000")}{hemisphere}{minutes.ToString("00")}.{secondes.ToString("000")}";
        }

        /// <summary>
        /// Suppression d'une portion de texte entre deux chaines
        /// </summary>
        public static string Remove(this string text, string from, string to, StringComparison comparisonType)
        {
            int start = text.IndexOf(from, comparisonType);
            int end = text.IndexOf(to, comparisonType);
            return start >= 0 && end > 0 && start < end ? text.Substring(0, start) + text.Substring(end + to.Length) : text;
        }

        /// <summary>
        /// Obtenir la chaine de texte entre deux chaines
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
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
