using System;
using System.Linq;

public class JokerUtils
{
    JokerUtils()
    {
        
    }

    public static void ValidateJokerType(Type joker)
    {
        var isJoker = joker.GetInterfaces().Contains(typeof(IJoker));

        if (!isJoker)
        {
            throw new ArgumentException("Invalid joker");
        }
    }
}