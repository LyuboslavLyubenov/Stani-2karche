using UnityEngine;
using UnityEngine.UI;
using System;

public class ShowIPUIController : MonoBehaviour
{
    public Text IPText;

    void Start()
    {
        IPText.text = NetworkUtils.GetLocalIP(); 
        StartCoroutine(NetworkUtils.GetExternalIP(OnFoundIP, OnNetworkError));
    }

    void OnFoundIP(string ip)
    {
        IPText.text += Environment.NewLine;
        IPText.text += ip;
    }

    void OnNetworkError(string errorMsg)
    {
        IPText.text = LanguagesManager.Instance.GetValue("Errors/NoInternetConnection");
    }
}
