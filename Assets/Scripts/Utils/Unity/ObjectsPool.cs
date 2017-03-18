namespace Utils.Unity
{

    using System.Collections.Generic;

    using UnityEngine;

    public class ObjectsPool : MonoBehaviour
    {
        public Transform Prefab;
        public int StartSize = 20;

        private List<Transform> pool;

        public int CurrentSize
        {
            get
            {
                return this.transform.childCount;
            }
        }

        // Use this for initialization
        void Start()
        {
            this.pool = new List<Transform>();

            for (var i = 0; i < this.StartSize; i++)
            {
                this.AddObjToThePool();
            }
        }

        public Transform Get(Vector2 position)
        {
            Transform obj = null;

            for (var i = 0; i < this.pool.Count; i++)
            {
                if (!this.pool[i].gameObject.activeSelf)
                {
                    obj = this.pool[i];
                    break;
                }    
            }

            if (obj == null)
            {
                obj = this.AddObjToThePool();
            }

            obj.position = position;
            obj.gameObject.SetActive(true);

            return obj;
        }

        public Transform Get()
        {
            return this.Get(Vector2.zero);
        }

        private Transform AddObjToThePool()
        {
            var poolObj = Instantiate(this.Prefab);
            poolObj.gameObject.SetActive(false);
            poolObj.SetParent(this.transform, false);
            this.pool.Add(poolObj);

            return poolObj;
        }
    }

}
