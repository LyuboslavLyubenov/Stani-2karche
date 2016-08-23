using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServersAvailableUIController : ExtendedMonoBehaviour
{
    public LANServersDiscoveryBroadcastService ServerDiscoveryService;
    public CreatedGameInfoReceiverService GameInfoReceiverService;
    public BasicExamServerSelectPlayerTypeUIController basicExamSelectPlayerTypeController;

    public NotificationsServiceController NotificationsService;

    public ObjectsPool ServerFoundElementsPool;

    public GameObject Container;

    List<string> foundServers = new List<string>();

    void Start()
    {
        //CoroutineUtils.RepeatEverySeconds(5f, Refresh);
        ServerDiscoveryService.OnFound += OnLocalServerFound;
    }

    void OnLocalServerFound(object sender, IpEventArgs args)
    {
        var ip = args.IPAddress;

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
        obj.SetParent(Container.transform, true);

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

        basicExamSelectPlayerTypeController.gameObject.SetActive(true);
        CoroutineUtils.WaitForFrames(0, () => basicExamSelectPlayerTypeController.Initialize(gameInfo));
    }

    void ClearServerFoundList()
    {
        var serversCount = Container.transform.childCount;

        for (int i = 0; i < serversCount; i++)
        {
            var foundServer = Container.transform.GetChild(i);
            foundServer.gameObject.SetActive(false);
        }
    }

    public void Refresh()
    {
        ClearServerFoundList();
    }
}
