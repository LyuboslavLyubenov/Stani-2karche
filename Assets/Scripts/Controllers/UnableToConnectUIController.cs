using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.Network;
    using Assets.Scripts.Network.NetworkManagers;

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

        public ClientNetworkManager NetworkManager;

        public void TryAgainToConnectToServer()
        {
            this.NetworkManager.ConnectToHost(this.ServerIP);
            this.OnTryingAgainToConnectToServer(this, new EventArgs());
            this.gameObject.SetActive(false);
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }

}
