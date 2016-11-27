using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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
        CoroutineUtils.RepeatEverySeconds(10f, () =>
            {
                ClearFoundServerList();
                StartLoadingExternalServersIfConnectedToInternet();
            });
        LANServersDiscoveryService.OnFound += OnLocalServerFound;
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
                            BeginReceiveServerGameInfo(ip);
                        }
                    }, Debug.LogException);
            });
    }

    void OnLocalServerFound(object sender, IpEventArgs args)
    {
        var ip = args.IPAddress;
        BeginReceiveServerGameInfo(ip);
    }

    void BeginReceiveServerGameInfo(string ip)
    {
        if (foundServers.Contains(ip))
        {
            return;
        }

        GameInfoReceiverService.ReceiveFrom(ip, OnReceivedGameInfo);
        foundServers.Add(ip);
    }

    void OnReceivedGameInfo(GameInfoReceivedDataEventArgs receivedData)
    {
        var gameInfo = receivedData.GameInfo;

        switch (gameInfo.GameType)
        {
            case GameType.BasicExam:
                var basicExamGameInfo = JsonUtility.FromJson<BasicExamGameInfo_Serializable>(receivedData.JSON);
                OnFoundBasicExam(basicExamGameInfo);
                break;    
        }
    }

    void OnFoundBasicExam(BasicExamGameInfo_Serializable gameInfo)
    {
        var obj = ServerFoundElementsPool.Get();
        var controller = obj.GetComponent<ServerDiscoveredElementController>();

        obj.SetParent(Container.transform, true);
        CoroutineUtils.WaitForFrames(0, () => controller.SetData(gameInfo));

        var button = obj.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OpenBasicExamSelectMenu(gameInfo));
    }

    void OpenBasicExamSelectMenu(BasicExamGameInfo_Serializable gameInfo)
    {
        if (gameInfo.ServerInfo.IsFull)
        {
            NotificationsService.AddNotification(Color.red, "Server is full");
            return;
        }

        BasicExamSelectPlayerTypeController.gameObject.SetActive(true);
        CoroutineUtils.WaitForFrames(0, () => BasicExamSelectPlayerTypeController.Initialize(gameInfo));
    }

    void ClearFoundServerList()
    {
        var serversCount = Container.transform.childCount;

        for (int i = 0; i < serversCount; i++)
        {
            var foundServer = Container.transform.GetChild(i);
            foundServer.gameObject.SetActive(false);
        }

        foundServers.Clear();
    }
}