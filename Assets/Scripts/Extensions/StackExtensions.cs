namespace Assets.Scripts.Extensions
{

    using System.Collections.Generic;

    public static class StackExtensions
    {
        public static T PopOrDefault<T>(this Stack<T> stack)
        {
            return stack.Count > 0 ? stack.Pop() : default(T);
        }
    }

}
