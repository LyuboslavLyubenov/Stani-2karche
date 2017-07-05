namespace Assets.Scripts.Extensions
{

    using System;
    using System.Linq;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.MainPlayer;

    public static class IJokerExtensions
    {
        private static string GetMainPlayerJokerName(Type jokerType)
        {
            var jokerName = jokerType.GetGenericArguments()
                .First()
                .Name
                .Replace("Joker", "");
            return jokerName;
        }

        public static string GetName(this IJoker joker)
        {
            if (joker == null)
            {
                throw new ArgumentNullException();
            }

            var jokerType = joker.GetType();
            
            if (jokerType.IsGenericType)
            {
                var jokerName = jokerType.GetGenericTypeDefinition()
                    .Name;
                var friendlyJokerName = jokerName.Remove(jokerName.IndexOf("`"));

                if (friendlyJokerName == "MainPlayerJoker")
                {
                    return GetMainPlayerJokerName(jokerType);
                }
            }

            return jokerType.Name.Replace("Joker", "");
        }
    }
}
