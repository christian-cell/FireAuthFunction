using System.ComponentModel;

namespace FireAuth.Services.Extensions
{
    public static class EnumExtension
    {
        public static string GetEnumDescriptionAttribute<T>(this T source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;

            return source.ToString();
        }
    }
};

