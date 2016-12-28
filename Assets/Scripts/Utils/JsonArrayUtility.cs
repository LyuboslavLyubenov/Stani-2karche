namespace Assets.Scripts.Utils
{
    using System;
    using UnityEngine;

    public static class JsonArrayUtility
    {
        public static T[] ArrayFromJson<T>(string json)
        {
            var wrapper = JsonUtility.FromJson < _ArrayWrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ArrayToJson<T>(T[] array)
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