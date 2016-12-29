namespace Assets.Scripts.Controllers
{
    using System.Collections.Generic;

    using DTOs.KinveySerializableObj;
    using Network.Broadcast;

    using UnityEngine;
    using UnityEngine.UI;

    using DTOs;
    using Enums;
    using EventArgs;
    using Network;
    using Notifications;
    using Utils.Unity;

    using Debug = UnityEngine.Debug;

    public class ServersAvailableUIController : ExtendedMonoBehaviour
    {
        public LANServersDiscoveryService LANServersDiscoveryService;
        public CreatedGameInfoReceiverService GameInfoReceiverService;
        public BasicExamServerSelectPlayerTypeUIController BasicExamSelectPlayerTypeController;

        public NotificationsServiceController NotificationsService;

        public ObjectsPool ServerFoundElementsPool;

        public GameObject Container;

        private List<string> foundServers = new List<string>();

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.CoroutineUtils.RepeatEverySeconds(10f, () =>
                {
                    this.ClearFoundServerList();
                    this.StartLoadingExternalServersIfConnectedToInternet();
                });
            this.LANServersDiscoveryService.OnFound += this.OnLocalServerFound;
        }

        private void StartLoadingExternalServersIfConnectedToInternet()
        {
            NetworkUtils.CheckInternetConnection((isConnectedToInternet) =>
                {
                    var kinveyWrapper = new KinveyWrapper();

                    if (!isConnectedToInternet || !kinveyWrapper.IsLoggedIn)
                    {
                        return;
                    }

                    kinveyWrapper.RetrieveEntityAsync<ServerInfo_DTO>("Servers", null, this.OnLoadedServers, Debug.LogException);
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
            var ip = args.IPAddress;
            this.BeginReceiveServerGameInfo(ip);
        }

        private void BeginReceiveServerGameInfo(string ip)
        {
            if (this.foundServers.Contains(ip))
            {
                return;
            }

            this.GameInfoReceiverService.ReceiveFrom(ip, this.OnReceivedGameInfo);
            this.foundServers.Add(ip);
        }

        private void OnReceivedGameInfo(GameInfoReceivedDataEventArgs receivedData)
        {
            var gameInfo = receivedData.GameInfo;

            switch (gameInfo.GameType)
            {
                case GameType.BasicExam:
                    var basicExamGameInfo = JsonUtility.FromJson<BasicExamGameInfo_DTO>(receivedData.JSON);
                    this.OnFoundBasicExam(basicExamGameInfo);
                    break;    
            }
        }

        private void OnFoundBasicExam(BasicExamGameInfo_DTO gameInfo_DTO)
        {
            var obj = this.ServerFoundElementsPool.Get();
            var controller = obj.GetComponent<ServerDiscoveredElementController>();

            obj.SetParent(this.Container.transform, true);
            this.CoroutineUtils.WaitForFrames(0, () => controller.SetData(gameInfo_DTO));

            var button = obj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => this.OpenBasicExamSelectMenu(gameInfo_DTO));
        }

        private void OpenBasicExamSelectMenu(BasicExamGameInfo_DTO gameInfo_DTO)
        {
            if (gameInfo_DTO.ServerInfo.IsFull)
            {
                this.NotificationsService.AddNotification(Color.red, "Server is full");
                return;
            }

            this.BasicExamSelectPlayerTypeController.gameObject.SetActive(true);
            this.CoroutineUtils.WaitForFrames(0, () => this.BasicExamSelectPlayerTypeController.Initialize(gameInfo_DTO));
        }

        private void ClearFoundServerList()
        {
            var serversCount = this.Container.transform.childCount;

            for (int i = 0; i < serversCount; i++)
            {
                var foundServer = this.Container.transform.GetChild(i);
                foundServer.gameObject.SetActive(false);
            }

            this.foundServers.Clear();
        }
    }
}