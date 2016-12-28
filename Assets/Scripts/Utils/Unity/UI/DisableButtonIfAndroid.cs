namespace Assets.Scripts.Utils.Unity.UI
{

    using UnityEngine;

    public class DisableButtonIfAndroid : MonoBehaviour
    {
        void Start()
        {
#if UNITY_ANDROID
        GetComponent<Button>().interactable = false;
        #endif
        }
    }

}
