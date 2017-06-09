namespace Assets.Scripts.Extensions
{

    using System;

    public static class StringExtensions
    {
        public static bool IsValidIPV4(this string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Split('.').Length != 4)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ToBoolean(this string value)
        {
            switch (value.ToUpperInvariant())
            {
                case "TRUE":
                    return true;
                case "T":
                    return true;
                case "1":
                    return true;
                case "0":
                    return false;
                case "FALSE":
                    return false;
                case "F":
                    return false;
                default:
                    throw new InvalidCastException("You can't cast a weird value to a bool!");
            }
        }

        public static T ConvertTo<T>(this string value) where T : IConvertible
        {
            var valToConvert = value;

            if (string.IsNullOrEmpty(valToConvert))
            {
                throw new ArgumentNullException();
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T ConvertToOrDefault<T>(this string value) where T : IConvertible
        {
            var result = default(T);

            try
            {
                result = value.ConvertTo<T>();
            }
            catch
            {
            }

            return result;
        }
    }

}
