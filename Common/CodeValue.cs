using System.Reflection;

namespace Common
{
    public class CodeValueAttribute : Attribute
    {
        private readonly string _value;

        public CodeValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public static class CodeEnum
    {
        public static string GetCodeValue(this System.Enum value)
        {
            string? output = null;
            Type type = value.GetType();

            FieldInfo? fi = type.GetField(value.ToString());
            if (fi!=null && fi.GetCustomAttributes(typeof(CodeValueAttribute), false) is CodeValueAttribute[] attrs) 
            {
                if (attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }
            }
            return output ?? string.Empty;
        }
    }
}
