using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Common
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
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            CodeValueAttribute[] attrs = fi.GetCustomAttributes(typeof(CodeValueAttribute), false) as CodeValueAttribute[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }
            return output;
        }
    }
}
