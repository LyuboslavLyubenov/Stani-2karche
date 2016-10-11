using UnityEngine;

public class FPSSettings : MonoBehaviour
{
    void Start()
    {
        #if !UNITY_STANDALONE
        Application.targetFrameRate = 30;    
        #else
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        #endif

        QualitySettings.vSyncCount = 1;
    }
}
