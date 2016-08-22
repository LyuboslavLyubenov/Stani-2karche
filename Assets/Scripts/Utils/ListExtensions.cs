using System.Collections.Generic;
using System;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {  
        var rnd = new System.Random(DateTime.Now.Millisecond);
        int n = list.Count;  

        while (n > 1)
        {  
            n--;  
            int k = rnd.Next(0, n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
    }

    public static T GetRandomElement<T>(this IList<T> list)
    {
        if (list.Count < 1)
        {
            throw new Exception("Cant get random item from empty list");
        }
        else if (list.Count == 1)
        {
            return list[0];
        }
      
        var rnd = new System.Random(DateTime.Now.Millisecond);
        var randomIndex = rnd.Next(0, list.Count);

        return list[randomIndex];
    }
	
}
