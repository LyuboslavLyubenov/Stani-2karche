using UnityEngine;
using System.Collections;

public class ConnectedUIController : MonoBehaviour
{

    // Use this for initialization
    void OnDisable()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
	
}
