using UnityEngine;
using System.Collections;
using System;

public class NetworkUtils : MonoBehaviour
{
    /// <summary>
    /// Usage 
    /// StartCoroutine(
    ///     CheckInternetConnectionPromise(
    ///         (isCompleted) => {
    /// 
    ///         }));
    /// </summary>
    IEnumerator CheckInternetConnectionPromise(Action<bool> OnCheckCompleted)
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
}
