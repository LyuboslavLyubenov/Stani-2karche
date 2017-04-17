namespace Assets.Scripts.Utils
{

    using System;
    using System.Collections.Generic;

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

            if (list.Count == 1)
            {
                return list[0];
            }
      
            var rnd = new System.Random(DateTime.Now.Millisecond);
            var randomIndex = rnd.Next(0, list.Count);

            return list[randomIndex];
        }

        public static IList<T> GetRandomElements<T>(this IList<T> list, int count)
        {
            if (list.Count < 1)
            {
                throw new Exception("Cant get random item from empty list");
            }

            if (list.Count == 1 || list.Count <= count)
            {
                return list;
            }
            
            var remainingElements = new List<T>(list);
            var result = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                var element = remainingElements.GetRandomElement();
                result.Add(element);
                remainingElements.Remove(element);
            }

            return result;
        }
	
    }

}
