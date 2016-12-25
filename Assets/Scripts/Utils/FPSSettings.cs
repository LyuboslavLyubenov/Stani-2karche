﻿using UnityEngine;

namespace Assets.Scripts.Utils
{

    public class FPSSettings : MonoBehaviour
    {
        void Start()
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
