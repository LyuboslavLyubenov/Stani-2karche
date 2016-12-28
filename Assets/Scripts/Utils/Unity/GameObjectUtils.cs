namespace Assets.Scripts.Utils.Unity
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class GameObjectUtils : MonoBehaviour
    {
        GameObjectUtils()
        {
        
        }

        public static GameObject[] GetAllObjectsIncludingInactive(Predicate<GameObject> condition)
        {
            var allObjects = GetAllObjectsIncludingInactive();
            var result = new List<GameObject>();

            for (int i = 0; i < allObjects.Length; i++)
            {
                var obj = allObjects[i];

                if (condition(obj))
                {
                    result.Add(obj);
                }
            }

            return result.ToArray();
        }

        public static GameObject[] GetAllObjectsIncludingInactive()
        {
            var all = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            var objectsInScene = new List<GameObject>();
         
            foreach (UnityEngine.Object obj in all)
            {
                var gameObj = obj as GameObject;

                if (gameObj == null)
                {
                    continue;
                }
                
                if (gameObj.hideFlags == HideFlags.None)
                {
                    objectsInScene.Add(gameObj);
                }
            }

            return objectsInScene.ToArray();
        }
    }

}
