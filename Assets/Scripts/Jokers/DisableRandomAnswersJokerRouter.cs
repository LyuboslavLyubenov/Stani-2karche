using UnityEngine;
using System;

public class DisableRandomAnswersJokerRouter : ExtendedMonoBehaviour
{
    public void Activate(int answersToDisableCount, IPlayerData playerData, ServerNetworkManager networkManager)
    {
        if (answersToDisableCount < 0)
        {
            throw new ArgumentOutOfRangeException("answersToDisableCount");
        }

        if (playerData == null)
        {
            throw new ArgumentNullException("playerData");
        }

        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        var settingsCommand = new NetworkCommandData("DisableRandomAnswersJokerSettings");
        settingsCommand.AddOption("AnswersToDisableCount", answersToDisableCount.ToString());
        networkManager.SendClientCommand(playerData.ConnectionId, settingsCommand);
    }
}