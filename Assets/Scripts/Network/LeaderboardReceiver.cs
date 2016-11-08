using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardReceiver : ExtendedMonoBehaviour
{
    public ClientNetworkManager NetworkManager;
    public LeaderboardUIController Leaderboard;

    List<PlayerScore> playersScores = new List<PlayerScore>();

    public bool Receiving
    {
        get;
        private set;
    }

    int elapsedTimeReceivingInSeconds = 0;
    int timeoutInSeconds = 0;

    Action<PlayerScore[]> onReceived;
    Action onError;

    void Start()
    {
        CoroutineUtils.WaitForFrames(0, () => InitializeCommand());
        CoroutineUtils.RepeatEverySeconds(1, () => UpdateElapsedTime());
    }

    void UpdateElapsedTime()
    {
        if (Receiving)
        {
            elapsedTimeReceivingInSeconds++;

            if (timeoutInSeconds >= elapsedTimeReceivingInSeconds)
            {
                Timeout();
            }
        }
    }

    void Timeout()
    {
        var timeoutCommand = new NetworkCommandData("LeaderboardReceiveTimeout");
        NetworkManager.SendServerCommand(timeoutCommand);
        Receiving = false;
    }

    void OnNoMoreEntities(object sender, EventArgs args)
    {
        onReceived(playersScores.ToArray());
        playersScores.Clear();
        Receiving = false;
    }

    void InitializeCommand()
    {
        var noMoreEntitiesCommand = new DummyCommand();
        noMoreEntitiesCommand.OnExecuted += OnNoMoreEntities;

        NetworkManager.CommandsManager.AddCommand("LeaderboardEntity", new ReceivedLeaderboardEntityCommand(playersScores));
        NetworkManager.CommandsManager.AddCommand("LeaderboardNoMoreEntities", noMoreEntitiesCommand);
    }

    void StartReceiving()
    {
        var receiveLeaderboardEntities = new NetworkCommandData("SendLeaderboardEntities");
        NetworkManager.SendServerCommand(receiveLeaderboardEntities);

        Receiving = true;
        elapsedTimeReceivingInSeconds = 0;
    }

    public void Receive(Action<PlayerScore[]> onReceived, Action onError, int timeoutInSeconds)
    {
        if (onReceived == null)
        {
            throw new ArgumentNullException("onReceived");
        }

        if (onError == null)
        {
            throw new ArgumentNullException("onError");
        }

        if (timeoutInSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException("timeoutInSeconds");
        }
            
        if (Receiving)
        {
            throw new InvalidOperationException("Already receiving leaderboard data");
        }

        this.onReceived = onReceived;
        this.onError = onError;
        this.timeoutInSeconds = timeoutInSeconds;

        StartReceiving();
    }
}