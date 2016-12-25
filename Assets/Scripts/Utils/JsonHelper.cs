using System;

using UnityEngine;

namespace Assets.Scripts.Utils
{

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            var wrapper = JsonUtility.FromJson < _ArrayWrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            var wrapper = new _ArrayWrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        public class _ArrayWrapper<T>
        {
            public T[] Items;
        }
    }

}