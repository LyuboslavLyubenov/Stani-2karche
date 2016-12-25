using System.Collections.Generic;

namespace Assets.Scripts.Utils
{

    public static class StackExtensions
    {
        public static T PopOrDefault<T>(this Stack<T> stack)
        {
            return stack.Count > 0 ? stack.Pop() : default(T);
        }
    }

}
