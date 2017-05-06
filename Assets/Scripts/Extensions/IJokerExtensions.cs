namespace Assets.Scripts.Extensions
{

    using System;

    using Assets.Scripts.Interfaces;

    public static class IJokerExtensions
    {
        public static string GetName(this IJoker joker)
        {
            if (joker == null)
            {
                throw new ArgumentNullException();
            }

            return joker.GetType()
                .Name.Replace("Joker", "");
        }
    }
}
