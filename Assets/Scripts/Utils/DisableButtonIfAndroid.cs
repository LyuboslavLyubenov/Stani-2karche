using UnityEngine;
using UnityEngine.UI;

public class DisableButtonIfAndroid : MonoBehaviour
{
    void Start()
    {
        #if UNITY_ANDROID
        GetComponent<Button>().interactable = false;
        #endif
    }
}
