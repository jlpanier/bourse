using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MinValueAttribute : Attribute
    {
        public MinValueAttribute(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }

    public static class MinEnum
    {
        public static double GetMinValue(this System.Enum value)
        {
            double output = 0;
            Type type = value.GetType();
            FieldInfo? fi = type.GetField(value.ToString());
            if (fi != null && fi.GetCustomAttributes(typeof(MinValueAttribute), false) is MinValueAttribute[] attrs && attrs.Length > 0) 
            {
                output = attrs[0].Value;
            }
            return output;
        }
    }
}
