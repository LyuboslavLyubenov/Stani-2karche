using UnityEngine;
using System.Collections;

public class TEST_DoesUsernameExists : MonoBehaviour
{
    void Start()
    {
        KinveyWrapper.Instance.DoesUsernameExistsAsync("ivan", (data) =>
            {
                Debug.Log("Exists = " + data.usernameExists);
            }, Debug.LogException);
    }
	
}
