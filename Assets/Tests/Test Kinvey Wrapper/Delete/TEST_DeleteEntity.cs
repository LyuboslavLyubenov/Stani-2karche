using UnityEngine;
using System.Collections;

public class TEST_DeleteEntity : MonoBehaviour
{
    void Start()
    {
        KinveyWrapper.Instance.LoginAsync("ivan", "ivan", (data) =>
            {
                KinveyWrapper.Instance.DeleteEntityAsync("Servers", "58356df6f08321f70dc31bd3", (data1) =>
                    {
                        Debug.Log("Deleted count " + data1.count);
                    }, Debug.LogException);
            }, Debug.LogException);
    }
	
}
