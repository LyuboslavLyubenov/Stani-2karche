using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class AskAudienceJoker : IJoker
{
    const int SettingsReceiveTimeoutInSeconds = 10;

    const int MinCorrectAnswerChance = 40;
    const int MaxCorrectAnswerChance = 85;

    const int MinClientsForOnlineVote_Release = 4;
    const int MinClientsForOnlineVote_Development = 1;

    public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
    {
    };

    IGameData gameData;

    ClientNetworkManager networkManager;

    GameObject waitingToAnswerUI;
    GameObject loadingUI;
    GameObject audienceAnswerUI;

    AudienceAnswerUIController audienceAnswerUIController;
    DisableAfterDelay waitingToAnswerUIDelayController;

    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    List<int> audienceVotedId = new List<int>();

    int connectedClients = 0;

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

    public AskAudienceJoker(IGameData gameData, ClientNetworkManager networkManager, GameObject waitingToAnswerUI, GameObject audienceAnswerUI, GameObject loadingUI)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }

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
            
        this.gameData = gameData;
        this.networkManager = networkManager;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.audienceAnswerUI = audienceAnswerUI;
        this.loadingUI = loadingUI;

        this.waitingToAnswerUIDelayController = this.waitingToAnswerUI.GetComponent<DisableAfterDelay>();
        this.audienceAnswerUIController = audienceAnswerUI.GetComponent<AudienceAnswerUIController>();

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
        connectedClients = networkManager.ServerConnectedClientsCount;

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

        var selectedAskAudienceJokerCommand = new NetworkCommandData("SelectedAskAudienceJoker");
        networkManager.SendServerCommand(selectedAskAudienceJokerCommand);

        var receiveJokerSettingsCommand = new ClientReceiveAskAudienceJokerSettingsCommand(OnReceivedJokerSettings);

        networkManager.CommandsManager.AddCommand("AskAudienceJokerSettings", receiveJokerSettingsCommand);

        receiveSettingsTimeoutTimer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        receiveSettingsTimeoutTimer.AutoReset = false;
        receiveSettingsTimeoutTimer.Elapsed += OnReceiveSettingsTimeout;
    }

}