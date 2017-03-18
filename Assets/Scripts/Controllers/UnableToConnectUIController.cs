
namespace Controllers
{

    using System;

    using Network.NetworkManagers;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using EventArgs = System.EventArgs;

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
        
        public void TryAgainToConnectToServer()
        {
            ClientNetworkManager.Instance.ConnectToHost(this.ServerIP);
            this.OnTryingAgainToConnectToServer(this, new EventArgs());
            this.gameObject.SetActive(false);
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }

}
