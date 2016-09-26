using UnityEngine;
using System;
using System.Timers;

public class AskAudienceJoker : IJoker
{
    const int SettingsReceiveTimeoutInSeconds = 10;

    const int MinCorrectAnswerChance = 40;
    const int MaxCorrectAnswerChance = 85;

    public const int MinClientsForOnlineVote_Release = 4;
    public const int MinClientsForOnlineVote_Development = 1;

    public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
    {
    };

    ClientNetworkManager networkManager;

    GameObject waitingToAnswerUI;
    GameObject loadingUI;
    GameObject audienceAnswerUI;

    Timer receiveSettingsTimeoutTimer;

    public Sprite Image
    {
        get;
        private set;
    }

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

    public AskAudienceJoker(ClientNetworkManager networkManager, GameObject waitingToAnswerUI, GameObject audienceAnswerUI, GameObject loadingUI)
    {
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        if (waitingToAnswerUI == null)
        {
            throw new ArgumentNullException("waitingToAnswerUI");
        }

        if (audienceAnswerUI == null)
        {
            throw new ArgumentNullException("audienceAnswerUI");
        }

        if (loadingUI == null)
        {
            throw new ArgumentNullException("loadingUI");
        }
            
        this.networkManager = networkManager;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.audienceAnswerUI = audienceAnswerUI;
        this.loadingUI = loadingUI;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");
    }

    void OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                receiveSettingsTimeoutTimer.Dispose();
                networkManager.CommandsManager.RemoveCommand("AskAudienceJokerSettings");
            });
    }

    void OnReceivedJokerSettings(int timeToAnswerInSeconds)
    {
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);
        waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = timeToAnswerInSeconds;

        receiveSettingsTimeoutTimer.Stop();
        ActivateAskAudienceJoker();
    }

    void ActivateAskAudienceJoker()
    {
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);

        var receivedAskAudienceVoteResultCommand = new ReceivedAskAudienceVoteResultCommand(audienceAnswerUI);
        receivedAskAudienceVoteResultCommand.OnFinishedExecution += (sender, args) => waitingToAnswerUI.SetActive(false);

        networkManager.CommandsManager.AddCommand("AskAudienceJokerSettings", receivedAskAudienceVoteResultCommand);

        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }
    }

    public void Activate()
    {
        loadingUI.SetActive(true);

        var minClients = MinClientsForOnlineVote_Release;

        #if DEVELOPMENT_BUILD
        minClients = MinClientsForOnlineVote_Development;
        #endif

        if (networkManager.ServerConnectedClientsCount < minClients)
        {
            throw new InvalidOperationException("There must be at least " + minClients + " clients to activate this joker.");
        }

        var selectedAskAudienceJokerCommand = new NetworkCommandData("SelectedAskAudienceJoker");
        networkManager.SendServerCommand(selectedAskAudienceJokerCommand);

        var receiveJokerSettingsCommand = new ReceivedAskAudienceJokerSettingsCommand(OnReceivedJokerSettings);

        networkManager.CommandsManager.AddCommand("AskAudienceJokerSettings", receiveJokerSettingsCommand);

        receiveSettingsTimeoutTimer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        receiveSettingsTimeoutTimer.AutoReset = false;
        receiveSettingsTimeoutTimer.Elapsed += OnReceiveSettingsTimeout;
    }
}