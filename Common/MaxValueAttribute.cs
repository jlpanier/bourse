using System.Reflection;

namespace Common
{
    public class MaxValueAttribute : Attribute
    {
        public MaxValueAttribute(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }

    public static class MaxEnum
    {
        public static double GetMaxValue(this System.Enum value)
        {
            double output = 0;
            Type type = value.GetType();

            FieldInfo? fi = type.GetField(value.ToString());
            if (fi!=null)
            {
                MaxValueAttribute[]? attrs = fi.GetCustomAttributes(typeof(MaxValueAttribute), false) as MaxValueAttribute[];
                if (attrs!=null && attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }
            }
            return output;
        }
    }
}
