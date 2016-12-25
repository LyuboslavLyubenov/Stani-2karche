using System;
using System.Collections;
using System.Net;

using UnityEngine;

namespace Assets.Scripts.Utils
{

    public class NetworkUtils
    {
        /// <summary>
        /// Usage 
        /// StartCoroutine(
        ///     CheckInternetConnectionPromise(
        ///         (isCompleted) => {
        /// 
        ///         }));
        /// </summary>
        public static IEnumerator CheckInternetConnectionPromise(Action<bool> onCheckCompleted)
        {
            WWW www = new WWW("http://icanhazip.com/");
            yield return www;

            var haveConnection = string.IsNullOrEmpty(www.error);
            onCheckCompleted(haveConnection);
        }

        public static IEnumerator GetExternalIP(Action<string> onFound, Action<string> onNetworkError = null)
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
