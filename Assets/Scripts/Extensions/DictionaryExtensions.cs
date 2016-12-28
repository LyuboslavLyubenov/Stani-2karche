namespace Assets.Scripts.Extensions
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DictionaryExtensions
    {
        /// <summary>
        /// Shuffles a dictionary using the Fisher-Yates algorithm
        /// </summary>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="dictionary">Dictionary to shuffle</param>
        public static Dictionary<TKey, TValue> Shuffled<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            Random random = new Random();

            return dictionary.OrderBy(x => random.Next())
                .ToDictionary(item => item.Key, item => item.Value);
        }
    }

}