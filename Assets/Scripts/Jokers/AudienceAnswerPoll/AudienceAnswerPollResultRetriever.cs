using System;
using System.Timers;
using UnityEngine;

public class AudienceAnswerPollResultRetriever : MonoBehaviour
{
    public const int MinClientsForOnlineVote_Release = 4;
    public const int MinClientsForOnlineVote_Development = 1;

    const int SettingsReceiveTimeoutInSeconds = 10;

    static AudienceAnswerPollResultRetriever instance;

    public static AudienceAnswerPollResultRetriever Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject();
                obj.name = typeof(AudienceAnswerPollResultRetriever).Name;
                instance = obj.AddComponent<AudienceAnswerPollResultRetriever>();
            }

            return instance;        
        }
    }

    public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
    {
    };

    public EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
    {
    };

    public EventHandler OnReceiveSettingsTimeout = delegate
    {
    };
    
    public EventHandler OnReceiveAudienceVoteTimeout = delegate
    {
    };
    
    ClientNetworkManager networkManager;

    Timer timer;

    public EventHandler OnActivated
    {
        get;
        set;
    }

    public bool Activated
    {
        get;
        private set;
    }

    void Awake()
    {
        this.networkManager = GameObject.FindObjectOfType<ClientNetworkManager>();
        this.networkManager.OnDisconnectedEvent += OnDisconnected;
    }

    void OnDisconnected(object sender, EventArgs args)
    {
        if (!Activated)
        {
            return;
        }

        try
        {
            timer.Close();
            networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
        }
        catch
        {
        }
    }

    void OnReceivedJokerSettings(int timeToAnswerInSeconds)
    {
        var receivedAskAudienceVoteResultCommand = 
            new AudiencePollResultCommand(
                (votes) => OnAudienceVoted(this, new AudienceVoteEventArgs(votes)));

        networkManager.CommandsManager.AddCommand(receivedAskAudienceVoteResultCommand);

        timer.Close();

        timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        timer.AutoReset = false;
        timer.Elapsed += Timer_OnReceiveAudienceVoteTimeout;

        OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
    }

    void Timer_OnReceiveAudienceVoteTimeout(object sender, ElapsedEventArgs args)
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                Activated = false;
                networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
                OnReceiveAudienceVoteTimeout(this, EventArgs.Empty);
            });
    }

    void Timer_OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                timer.Close();
                Activated = false;
                networkManager.CommandsManager.RemoveCommand<AudiencePollSettingsCommand>();
                OnReceiveSettingsTimeout(this, EventArgs.Empty);
            });
    }

    public void Activate()
    {
        var selected = NetworkCommandData.From<SelectedAudiencePollCommand>();
        networkManager.SendServerCommand(selected);

        var receiveSettingsCommand = new AudiencePollSettingsCommand(OnReceivedJokerSettings);
        networkManager.CommandsManager.AddCommand(receiveSettingsCommand);

        timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        timer.AutoReset = false;
        timer.Elapsed += Timer_OnReceiveSettingsTimeout;

        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }
    }
}