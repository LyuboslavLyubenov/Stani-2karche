using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ServerDiscoveredElementController : ExtendedMonoBehaviour
{
    Text category;
    Text creatorName;
    Text connectedClients;

    public string ServerIPAddress
    {
        get;
        private set;
    }

    void Start()
    {
        category = transform.Find("CategoryType").GetComponent<Text>();
        creatorName = transform.Find("CreatorName").GetComponent<Text>();
        connectedClients = transform.Find("ConnectedClients").GetComponent<Text>();
    }

    public void SetData(CreatedGameInfo_Serializable gameInfo)
    {
        category.text = TranslateGameType(gameInfo.GameType);
        creatorName.text = gameInfo.HostUsername;
        connectedClients.text = gameInfo.ServerInfo.ConnectedClientsCount + "/" + gameInfo.ServerInfo.MaxConnectionsAllowed;
        ServerIPAddress = gameInfo.ServerInfo.LocalIPAddress;
    }

    string TranslateGameType(GameType gameType)
    {
        var enumName = Enum.GetName(typeof(GameType), gameType);
        return LanguagesManager.Instance.GetValue(enumName);
    }
}
