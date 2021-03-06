﻿namespace Controllers.Lobby
{

    using Assets.Scripts.Network.GameInfo.New;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using Network.NetworkManagers;

    using Notifications;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    using Zenject;

    public class ConnectToExternalServerUIController : ExtendedMonoBehaviour
    {
        private const int ConnectionTimeoutInSeconds = 10;

        public GameObject LoadingUI;

        public Text IPText;

        [Inject]
        private SelectPlayerTypeRouter SelectPlayerTypeRouter;

        private ICreatedGameInfoReceiver gameInfoReceiver;

        private float elapsedTimeTryingToConnect = 0;
        private string ip;
        private bool connecting = false;

        void Start()
        {
            this.gameInfoReceiver = new CreatedGameInfoReceiver(ClientNetworkManager.Instance);
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Update()
        {
            if (!this.connecting)
            {
                return;
            }

            this.elapsedTimeTryingToConnect += Time.deltaTime;

            if (this.elapsedTimeTryingToConnect >= ConnectionTimeoutInSeconds)
            {
                this.ConnectToServerTimeout();
            }
        }

        private void ConnectToServerTimeout()
        {
            this.connecting = false;
            this.LoadingUI.SetActive(false);

            NotificationsController.Instance.AddNotification(Color.red, "Няма връзка със сървъра");

            this.gameInfoReceiver.StopReceivingFrom(this.ip);
        }

        private void OnReceivedGameInfo(GameInfoEventArgs args)
        {
            this.LoadingUI.SetActive(false);
            this.connecting = false;

            var gameType = args.GameInfo.GameType;

            this.RouteReceivedGameInfo(gameType, args.JSON);
        }

        private void RouteReceivedGameInfo(string gameType, string gameTypeJSON)
        {
            this.SelectPlayerTypeRouter.Handle(gameType, gameTypeJSON);
        }

        public void TryToConnect(string ip)
        {
            this.elapsedTimeTryingToConnect = 0;
            this.ip = ip;
            this.connecting = true;

            Debug.Log("Trying to connect to " + ip);

            this.gameInfoReceiver.ReceiveFrom(ip, this.OnReceivedGameInfo, Debug.LogException);
            this.LoadingUI.SetActive(true);
        }

        public void TryToConnect()
        {
            this.TryToConnect(this.IPText.text);
        }
    }
}