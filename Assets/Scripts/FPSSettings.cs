using UnityEngine;
using System.Collections;

public class FPSSettings : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
}
