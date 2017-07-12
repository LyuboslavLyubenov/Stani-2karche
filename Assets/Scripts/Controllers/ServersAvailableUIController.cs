﻿namespace Controllers
{

    using System.Collections.Generic;
    using System.Linq;

    using Controllers.Lobby;

    using DTOs;
    using DTOs.KinveyDtoObjs;

    using EventArgs;

    using Interfaces.Network.Kinvey;
    using Interfaces.Network.NetworkManager;
    using Interfaces.Services;

    using UnityEngine;
    using UnityEngine.UI;

    using Utils.Unity;

    using Zenject.Source.Usage;

    using Debug = UnityEngine.Debug;

    public class ServersAvailableUIController : ExtendedMonoBehaviour
    {
        private const int StartElementPositionY = 130;

        private const int DistanceBetweenElements = 10;

        public ObjectsPool ServerFoundElementsPool;
        public GameObject Container;

        [Inject]
        private ILANServersDiscoverer IlanServersDiscoverer;

        [Inject]
        private ICreatedGameInfoReceiver gameInfoReceiver;

        [Inject]
        private SelectPlayerTypeRouter selectPlayerTypeRouter;

        [Inject]
        private IKinveyWrapper kinveyWrapper;

        private List<string> foundServers = new List<string>();

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.CoroutineUtils.RepeatEverySeconds(10f, () =>
                {
                    this.ClearFoundServerList();
                    this.StartLoadingExternalServersIfConnectedToInternet();
                });

            this.IlanServersDiscoverer.OnFound += this.OnLocalServerFound;
        }

        private void StartLoadingExternalServersIfConnectedToInternet()
        {
            NetworkUtils.CheckInternetConnection((isConnectedToInternet) =>
                {
                    if (!isConnectedToInternet || !this.kinveyWrapper.IsLoggedIn)
                    {
                        return;
                    }
                    
                    this.kinveyWrapper.RetrieveEntityAsync<ServerInfo_DTO>("Servers", null, this.OnLoadedServers, Debug.LogException);
                });
        }

        private void OnLoadedServers(_KinveyEntity<ServerInfo_DTO>[] servers)
        {
            for (int i = 0; i < servers.Length; i++)
            {
                var entity = servers[i].Entity;
                var ip = entity.ExternalIpAddress;
                this.BeginReceiveServerGameInfo(ip);
            }
        }

        private void OnLocalServerFound(object sender, IpEventArgs args)
        {
            Debug.Log("Found server  " + args.IPAddress);

            var ip = args.IPAddress;
            this.BeginReceiveServerGameInfo(ip);
        }

        private void BeginReceiveServerGameInfo(string ip)
        {
            if (this.foundServers.Contains(ip))
            {
                return;
            }

            this.gameInfoReceiver.ReceiveFrom(ip, this.OnReceivedGameInfo);
            this.foundServers.Add(ip);
        }

        private void OnReceivedGameInfo(GameInfoReceivedDataEventArgs receivedData)
        {
            var gameInfo = receivedData.GameInfo;
            var obj = this.ServerFoundElementsPool.Get();
            var controller = obj.GetComponent<ServerDiscoveredElementController>();

            obj.SetParent(this.Container.transform, true);
            this.CoroutineUtils.WaitForFrames(1, () => controller.SetData(gameInfo));

            var yPos = this.foundServers.Count * (StartElementPositionY + DistanceBetweenElements);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -yPos);

            var button = obj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => this.OnClickedOnServerElement(gameInfo.GameType, receivedData.JSON));
        }

        private void OnClickedOnServerElement(string gameType, string gameInfoJSON)
        {
            this.selectPlayerTypeRouter.Handle(gameType, gameInfoJSON);
        }

        private void ClearFoundServerList()
        {
            var serversElements = this.Container.GetComponentsInChildren<ServerDiscoveredElementController>()
                .Select(c => c.gameObject)
                .ToList();

            for (int i = 0; i < serversElements.Count; i++)
            {
                var server = serversElements[i];
                server.gameObject.SetActive(false);
            }

            this.foundServers.Clear();
            this.gameInfoReceiver.StopReceivingFromAll();
        }
    }
}