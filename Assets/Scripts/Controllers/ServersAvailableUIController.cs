using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;

    using Debug = UnityEngine.Debug;

    public class ServersAvailableUIController : ExtendedMonoBehaviour
    {
        public LANServersDiscoveryService LANServersDiscoveryService;
        public CreatedGameInfoReceiverService GameInfoReceiverService;
        public BasicExamServerSelectPlayerTypeUIController BasicExamSelectPlayerTypeController;

        public NotificationsServiceController NotificationsService;

        public ObjectsPool ServerFoundElementsPool;

        public GameObject Container;

        List<string> foundServers = new List<string>();

        void Start()
        {
            this.CoroutineUtils.RepeatEverySeconds(10f, () =>
                {
                    this.ClearFoundServerList();
                    this.StartLoadingExternalServersIfConnectedToInternet();
                });
            this.LANServersDiscoveryService.OnFound += this.OnLocalServerFound;
        }

        void StartLoadingExternalServersIfConnectedToInternet()
        {
            NetworkUtils.CheckInternetConnectionPromise((isConnectedToInternet) =>
                {
                    if (!isConnectedToInternet || !KinveyWrapper.Instance.IsLoggedIn)
                    {
                        return;
                    }

                    KinveyWrapper.Instance.RetrieveEntityAsync<ServerInfo_Serializable>("Servers", null, (servers) =>
                        {
                            for (int i = 0; i < servers.Length; i++)
                            {
                                var entity = servers[i].Entity;
                                var ip = entity.ExternalIpAddress;
                                this.BeginReceiveServerGameInfo(ip);
                            }
                        }, Debug.LogException);
                });
        }

        void OnLocalServerFound(object sender, IpEventArgs args)
        {
            var ip = args.IPAddress;
            this.BeginReceiveServerGameInfo(ip);
        }

        void BeginReceiveServerGameInfo(string ip)
        {
            if (this.foundServers.Contains(ip))
            {
                return;
            }

            this.GameInfoReceiverService.ReceiveFrom(ip, this.OnReceivedGameInfo);
            this.foundServers.Add(ip);
        }

        void OnReceivedGameInfo(GameInfoReceivedDataEventArgs receivedData)
        {
            var gameInfo = receivedData.GameInfo;

            switch (gameInfo.GameType)
            {
                case GameType.BasicExam:
                    var basicExamGameInfo = JsonUtility.FromJson<BasicExamGameInfo_Serializable>(receivedData.JSON);
                    this.OnFoundBasicExam(basicExamGameInfo);
                    break;    
            }
        }

        void OnFoundBasicExam(BasicExamGameInfo_Serializable gameInfo)
        {
            var obj = this.ServerFoundElementsPool.Get();
            var controller = obj.GetComponent<ServerDiscoveredElementController>();

            obj.SetParent(this.Container.transform, true);
            this.CoroutineUtils.WaitForFrames(0, () => controller.SetData(gameInfo));

            var button = obj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => this.OpenBasicExamSelectMenu(gameInfo));
        }

        void OpenBasicExamSelectMenu(BasicExamGameInfo_Serializable gameInfo)
        {
            if (gameInfo.ServerInfo.IsFull)
            {
                this.NotificationsService.AddNotification(Color.red, "Server is full");
                return;
            }

            this.BasicExamSelectPlayerTypeController.gameObject.SetActive(true);
            this.CoroutineUtils.WaitForFrames(0, () => this.BasicExamSelectPlayerTypeController.Initialize(gameInfo));
        }

        void ClearFoundServerList()
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