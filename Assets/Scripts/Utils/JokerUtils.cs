namespace Utils
{

    using System;
    using System.Linq;
    using System.Reflection;

    using UnityEngine;

    public class JokerUtils
    {
        public static Type[] AllJokersTypes
        {
            get
            {
                if (allJokerTypes == null)
                {
                    allJokerTypes = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(JokerUtils.IsValidJokerType)
                        .ToArray(); 
                }

                return allJokerTypes;
            }
        }

        private static Type[] allJokerTypes;

        private JokerUtils()
        {
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
            return joker.GetInterface("IJoker") != null;
        }

        public static Sprite LoadSprite(string jokerName)
        {
            return Resources.Load<Sprite>("Images/Buttons/Jokers/" + jokerName);
        }
    }

}