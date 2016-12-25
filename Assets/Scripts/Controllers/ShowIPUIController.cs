﻿using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Localization;
    using Assets.Scripts.Utils;

    public class ShowIPUIController : MonoBehaviour
    {
        public Text IPText;

        void Start()
        {
            this.IPText.text = NetworkUtils.GetLocalIP(); 
            this.StartCoroutine(NetworkUtils.GetExternalIP(this.OnFoundIP, this.OnNetworkError));
        }

        void OnFoundIP(string ip)
        {
            this.IPText.text += Environment.NewLine;
            this.IPText.text += ip.Trim();
        }

        void OnNetworkError(string errorMsg)
        {
            this.IPText.text = LanguagesManager.Instance.GetValue("Errors/NoInternetConnection");
        }
    }

}
