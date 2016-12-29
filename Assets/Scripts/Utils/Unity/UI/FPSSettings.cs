namespace Assets.Scripts.Utils.Unity.UI
{

    using UnityEngine;

    public class FPSSettings : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(this);

            Application.targetFrameRate = 60;

#if !UNITY_STANDALONE 
        QualitySettings.vSyncCount = 2;
        #else
            QualitySettings.vSyncCount = 1;
            Application.runInBackground = true;
#endif
        }
    }

}
