using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UnableToConnectUIController : MonoBehaviour
{
    public EventHandler OnTryingAgainToConnectToServer = delegate
    {
    };

    public string ServerIP
    {
        get;
        set;
    }

    public ClientNetworkManager NetworkManager;

    public void TryAgainToConnectToServer()
    {
        NetworkManager.ConnectToHost(ServerIP);
        OnTryingAgainToConnectToServer(this, new EventArgs());
        gameObject.SetActive(false);
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
