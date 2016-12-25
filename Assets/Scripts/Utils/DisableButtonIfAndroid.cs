﻿using UnityEngine;

namespace Assets.Scripts.Utils
{

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
