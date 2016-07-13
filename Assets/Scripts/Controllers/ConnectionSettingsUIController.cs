using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ConnectionSettingsUIController : MonoBehaviour
{
    public GameObject IPTextObj;
    public AndroidUIController AndroidUIController;

    Text ipText = null;

    void Start()
    {
        if (IPTextObj == null)
        {
            throw new NullReferenceException("IPTextObj is null on ConnectionSettingsUIController obj");
        }

        if (AndroidUIController == null)
        {
            throw new NullReferenceException("AndroidUIController is null on ConnectionSettingsUIController obj");
        }

        ipText = IPTextObj.GetComponent<Text>();
    }

    public void ConnectToServer()
    {
        AndroidUIController.Connect(ipText.text);
    }
}
