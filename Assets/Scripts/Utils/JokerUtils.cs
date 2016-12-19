using System;
using System.Linq;
using System.Reflection;

public class JokerUtils
{
    public static Type[] AllJokersTypes
    {
        get
        {
            return allJokerTypes;
        }
    }

    static Type[] allJokerTypes;

    JokerUtils()
    {
        allJokerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(JokerUtils.IsValidJokerType)
            .ToArray();  
    }

    public static void ValidateJokerType(Type joker)
    {
        if (!JokerUtils.IsValidJokerType(joker))
        {
            throw new ArgumentException("Invalid joker");
        }
    }

    public static bool IsValidJokerType(Type joker)
    {
        return allJokerTypes.Contains(joker);
    }
}