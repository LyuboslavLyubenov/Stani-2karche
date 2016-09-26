using System;
using System.Linq;

public class MainPlayerDataSynchronizer
{
    ServerNetworkManager networkManager;

    MainPlayerData mainPlayerData;

    public MainPlayerDataSynchronizer(ServerNetworkManager networkManager, MainPlayerData mainPlayerData)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        if (mainPlayerData == null)
        {
            throw new ArgumentNullException("mainPlayerData");
        }
            
        this.networkManager = networkManager;
        this.mainPlayerData = mainPlayerData;

        mainPlayerData.OnConnected += (sender, args) => SendAvailableJokersToMainPlayer(args.ConnectionId);
        mainPlayerData.JokersData.OnAddedJoker += (sender, args) => SendJokerToPlayer(args.JokerType, mainPlayerData.ConnectionId);
    }

    void SendAvailableJokersToMainPlayer(int connectionId)
    {
        if (!mainPlayerData.IsConnected)
        {
            return;
        }

        var jokers = mainPlayerData.JokersData.AvailableJokers.ToArray();

        for (int i = 0; i < jokers.Length; i++)
        {
            var joker = jokers[i];
            SendJokerToPlayer(joker, connectionId);
        }
    }

    void SendJokerToPlayer(Type jokerType, int connectionId)
    {
        if (!mainPlayerData.IsConnected)
        {
            return;
        }

        JokerUtils.ValidateJokerType(jokerType);

        var jokerName = jokerType.Name;
        var addJokerCommand = new NetworkCommandData("Add" + jokerName);

        networkManager.SendClientCommand(connectionId, addJokerCommand);   
    }
}