namespace Controllers
{
    using System;

    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;
    using UnityEngine.SceneManagement;

    using EventArgs = System.EventArgs;

    public class UnableToConnectUIController : MonoBehaviour, IUnableToConnectUIController
    {
        public EventHandler OnTryingAgainToConnectToServer = delegate
            {
            };

        public string ServerIP
        {
            get;
            set;
        }
        
        /// <summary>
        /// connects to the server with ServerIP
        /// </summary>
        public void TryAgainToConnectToServer(IClientNetworkManager networkManager)
        {
            networkManager.ConnectToHost(this.ServerIP);
            this.OnTryingAgainToConnectToServer(this, new EventArgs());
            this.gameObject.SetActive(false);
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}