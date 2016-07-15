using UnityEngine;
using UnityEngine.UI;
using System;

public class ConnectionSettingsUIController : MonoBehaviour
{
    public Text IPText;

    public EventHandler<IpEventArgs> OnConnectToServer = delegate
    {
    };

    void Start()
    {
        if (IPText == null)
        {
            throw new NullReferenceException("IPText is null on ConnectionSettingsUIController obj");
        }
    }

    public void ConnectToServer()
    {
        var ipEventArgs = new IpEventArgs(IPText.text);
        OnConnectToServer(this, ipEventArgs);
    }
}
