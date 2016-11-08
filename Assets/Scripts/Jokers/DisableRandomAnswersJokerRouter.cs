using UnityEngine;
using System;

public class DisableRandomAnswersJokerRouter : ExtendedMonoBehaviour, IJokerRouter
{
    public EventHandler OnActivated
    {
        get;
        set;
    }

    public EventHandler OnFinished
    {
        get;
        set;
    }

    public EventHandler<UnhandledExceptionEventArgs> OnError
    {
        get;
        set;
    }

    void Awake()
    {
        OnActivated = delegate
        {
        };

        OnFinished = delegate
        {
        };

        OnError = delegate
        {
        };
    }

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

        OnActivated(this, EventArgs.Empty);
        OnFinished(this, EventArgs.Empty);
    }
}

public interface IJokerRouter
{
    EventHandler OnActivated { get; set; }

    EventHandler OnFinished { get; set; }

    EventHandler<UnhandledExceptionEventArgs> OnError { get; set; }
}