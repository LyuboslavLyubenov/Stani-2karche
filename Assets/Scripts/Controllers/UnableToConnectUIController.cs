using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class UnableToConnectUIController : MonoBehaviour
{
    public EventHandler<IpEventArgs> OnTryingAgainToConnectToServer = delegate
    {
    };

    public void TryAgainToConnectToServer()
    {
        var serverIP = PlayerPrefs.GetString("ServerIP");
        OnTryingAgainToConnectToServer(this, new IpEventArgs(serverIP));
        gameObject.SetActive(false);
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
