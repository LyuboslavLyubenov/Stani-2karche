using UnityEngine;
using System.Collections.Generic;

public class ObjectsPool : MonoBehaviour
{
    public Transform Prefab;
    public int StartSize = 20;

    List<Transform> pool;

    public int CurrentSize
    {
        get
        {
            return transform.childCount;
        }
    }

    // Use this for initialization
    void Start()
    {
        pool = new List<Transform>();

        for (var i = 0; i < StartSize; i++)
        {
            AddObjToThePool();
        }
    }

    public Transform Get(Vector2 position)
    {
        Transform obj = null;

        for (var i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                obj = pool[i];
                break;
            }    
        }

        if (obj == null)
        {
            obj = AddObjToThePool();
        }

        obj.position = position;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public Transform Get()
    {
        return Get(Vector2.zero);
    }

    Transform AddObjToThePool()
    {
        var poolObj = Instantiate(Prefab);
        poolObj.gameObject.SetActive(false);
        poolObj.SetParent(this.transform, false);
        pool.Add(poolObj);

        return poolObj;
    }
}
