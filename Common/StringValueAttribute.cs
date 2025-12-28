using System.Reflection;


namespace Common
{
    public class StringValueAttribute : Attribute
    {
        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public static class StringEnum
    {
        public static string GetStringValue(this Enum value)
        {
            string output = string.Empty;
            Type type = value.GetType();
            FieldInfo? fi = type.GetField(value.ToString());
            if (fi!=null && fi.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[]  attrs && attrs.Length > 0)
            {
                output = attrs[0].Value;
            }
            return output;
        }
    }
}
