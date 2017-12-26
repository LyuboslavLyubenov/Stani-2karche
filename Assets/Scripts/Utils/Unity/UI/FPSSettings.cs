namespace Utils.Unity.UI
{

    using UnityEngine;

    public class FPSSettings : MonoBehaviour
    {
        private static FPSSettings instance;

        [SerializeField]
        private int TargetFPS = 60;
       
        [SerializeField]
        private int VSync = 1;

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            if (instance != null &&
                instance.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this);

            Application.targetFrameRate = this.TargetFPS;

#if !UNITY_STANDALONE && !UNITY_STANDALONE_LINUX 
            QualitySettings.vSyncCount = 2;
#else
            QualitySettings.vSyncCount = VSync;
            Application.runInBackground = true;
#endif
            instance = this;
        }

    }

}
