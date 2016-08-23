using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ServerDiscoveredElementController : ExtendedMonoBehaviour
{
    public Text Category;
    public Text CreatorName;
    public Text ConnectedClients;

    Dictionary<GameType, string> gameTypesTranslations = new Dictionary<GameType, string>()
    {
        { GameType.BasicExam, "Нормално изпитване" },
        { GameType.AudienceRevenge, "Отмъщението на публиката" },
        { GameType.FastestWins, "Най-бързият печели" }
    };

    public string ServerIPAddress
    {
        get;
        private set;
    }

    public void SetData(CreatedGameInfo_Serializable gameInfo)
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                Category.text = TranslateGameType(gameInfo.GameType);
                CreatorName.text = gameInfo.HostUsername;
                ConnectedClients.text = gameInfo.ServerInfo.ConnectedClientsCount + "/" + gameInfo.ServerInfo.MaxConnectionsAllowed;
                ServerIPAddress = gameInfo.ServerInfo.LocalIPAddress;
            });
    }

    string TranslateGameType(GameType gameType)
    {
        return gameTypesTranslations[gameType];
    }
}
