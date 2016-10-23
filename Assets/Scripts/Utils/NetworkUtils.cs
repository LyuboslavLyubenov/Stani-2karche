using UnityEngine;
using System.Collections;
using System;
using System.Net;

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
    public static IEnumerator CheckInternetConnectionPromise(Action<bool> OnCheckCompleted)
    {
        WWW www = new WWW("http://google.com");
        yield return www;

        if (www.error != null)
        {
            OnCheckCompleted(false);    
        }
        else
        {
            OnCheckCompleted(true);
        }
    }

    public static IEnumerator GetExternalIP(Action<string> OnFound, Action<string> OnNetworkError)
    {
        WWW www = new WWW("http://icanhazip.com/");
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            OnNetworkError(www.error);
        }
        else
        {
            OnFound(www.text);
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
