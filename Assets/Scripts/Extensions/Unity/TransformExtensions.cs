namespace Extensions.Unity
{

    using System.Collections.Generic;

    using UnityEngine;

    public static class TransformExtensions
    {
        public static Transform[] GetAllChildren(this Transform transform)
        {
            var childrenCount = transform.childCount;
            var result = new List<Transform>();

            for (int i = 0; i < childrenCount; i++)
            {
                var childTransform = transform.GetChild(i);
                result.Add(childTransform);
            }

            return result.ToArray();
        }
    }
}