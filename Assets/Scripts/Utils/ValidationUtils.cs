using System;

namespace Assets.Scripts.Utils
{

    public class ValidationUtils
    {
        ValidationUtils()
        {
        
        }

        public static void ValidateStringNotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(paramName + " cannot be empty");
            }
        }

        public static void ValidateObjectNotNull(object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName + " cannot be null");
            }
        }

        public static void ValidateObjOfType(object obj, Type type, string paramName)
        {
            if (obj.GetType() != type)
            {
                throw new ArgumentException(paramName + " is not of type " + type.Name);
            }
        }
    }
}