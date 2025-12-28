using System.Reflection;


namespace Common
{
    public class ColorValueAttribute : Attribute
    {
        private readonly string _value;

        public ColorValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public static class ColorEnum
    {
        public static string GetColorValue(this Enum value)
        {
            string output = string.Empty;
            Type type = value.GetType();

            FieldInfo? fi = type.GetField(value.ToString());
            if (fi != null && fi.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[] attrs) 
            {
                if (attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }
            }
            return output;
        }
    }
}
