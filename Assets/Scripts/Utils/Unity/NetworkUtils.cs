﻿namespace Utils.Unity
{

    using System;
    using System.Collections;
    using System.Net;

    using UnityEngine;

    public class NetworkUtils
    {
        public delegate void OnCheckCompleted(bool isConnectedToInternet);

        private static IEnumerator CheckInternetConnectionCoroutine(OnCheckCompleted onCheckCompleted)
        {
            WWW www = new WWW("http://icanhazip.com/");
            yield return www;

            var haveConnection = string.IsNullOrEmpty(www.error);
            onCheckCompleted(haveConnection);
        }

        private static IEnumerator GetExternalIPCoroutine(Action<string> onFound, Action<string> onNetworkError = null)
        {
            WWW www = new WWW("http://icanhazip.com/");
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                if (onNetworkError != null)
                {
                    onNetworkError(www.error);    
                }
            }
            else
            {
                onFound(www.text);
            }
        }

        public static void GetExternalIP(Action<string> onFound, Action<string> onNetworkError = null)
        {
            ThreadUtils.Instance.RunOnMainThread(GetExternalIPCoroutine(onFound, onNetworkError));
        }

        public static void CheckInternetConnection(OnCheckCompleted onCheckCompleted)
        {
            ThreadUtils.Instance.RunOnMainThread(CheckInternetConnectionCoroutine(onCheckCompleted));
        }

        public static string GetLocalIP()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
     
            foreach (IPAddress ipAddress in ipEntry.AddressList)
            {
                if (ipAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    return ipAddress.ToString();
                }
            }
 
            return null;
        }
    }
}
