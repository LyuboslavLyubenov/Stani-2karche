namespace Assets.Scripts.Utils.Unity.UI
{

    using System;

    using Assets.Scripts.Extensions;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class FPSSettings : MonoBehaviour
    {
        private static FPSSettings instance;

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

            Application.targetFrameRate = 60;

#if !UNITY_STANDALONE 
            QualitySettings.vSyncCount = 2;
#else
            QualitySettings.vSyncCount = 1;
            Application.runInBackground = true;
#endif
            instance = this;
        }

    }

}
