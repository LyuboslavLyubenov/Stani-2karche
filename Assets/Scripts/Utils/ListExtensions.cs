using System.Collections.Generic;
using System;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {  
        var rnd = new System.Random();
        
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
	
}
