namespace Assets.Scripts.Utils.Unity.UI
{

    using UnityEngine;
    using UnityEngine.UI;

    public class DisableButtonIfAndroid : MonoBehaviour
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
#if UNITY_ANDROID
        GetComponent<Button>().interactable = false;
        #endif
        }
    }

}
