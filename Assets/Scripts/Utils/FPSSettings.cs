using UnityEngine;

public class FPSSettings : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
}
