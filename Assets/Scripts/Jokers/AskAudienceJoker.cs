using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

public class AskAudienceJoker : IJoker
{
    const int PermissionReceiveTimeoutInSeconds = 10;

    const int MinCorrectAnswerChance = 40;
    const int MaxCorrectAnswerChance = 85;

    const int MinClientsForOnlineVote_Release = 4;
    const int MinClientsForOnlineVote_Development = 1;

    public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
    {
    };

    IGameData gameData;

    ClientNetworkManager networkManager;

    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    List<int> audienceVotedId = new List<int>();

    int connectedClients = 0;

    GameObject waitingToAnswerUI;
    GameObject loadingUI;
    GameObject audienceAnswerUI;

    AudienceAnswerUIController audienceAnswerUIController;

    Timer receivePermissionTimeoutTimer = new Timer(PermissionReceiveTimeoutInSeconds * 1000);

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

        audienceAnswerUI.SetActive(true);//STUPID HACK
        this.audienceAnswerUIController = audienceAnswerUI.GetComponent<AudienceAnswerUIController>();
        audienceAnswerUI.SetActive(false);

        this.loadingUI = loadingUI;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");
    }

    void OnReceivePermissionTimeout(object sender, ElapsedEventArgs args)
    {
        networkManager.CommandsManager.RemoveCommand("AllowedToActivateAskAudienceJoker");
        receivePermissionTimeoutTimer.Dispose();
    }

    public void Activate()
    {
        waitingToAnswerUI.SetActive(true);
        waitingToAnswerUI.GetComponent<WaitingToAnswerUIController>();

        var selectedAskAudienceJokerCommand = new NetworkCommandData("SelectedAskAudienceJoker");
        networkManager.SendServerCommand(selectedAskAudienceJokerCommand);

        var receivePermissionToActivateCommand = new ClientAllowedToActivateAskAudienceJokerCommand();
        receivePermissionToActivateCommand.OnFinishedExecution += (sender, args) => OnReceivedPremissionToActivate();

        networkManager.CommandsManager.AddCommand("AllowedToActivateAskAudienceJoker", receivePermissionToActivateCommand);

        receivePermissionTimeoutTimer = new Timer(PermissionReceiveTimeoutInSeconds * 1000);
        receivePermissionTimeoutTimer.AutoReset = false;
        receivePermissionTimeoutTimer.Elapsed += OnReceivePermissionTimeout;
    }

    void OnReceivedPremissionToActivate()
    {
        receivePermissionTimeoutTimer.Stop();
        ActivateAskAudienceJoker();
    }

    void ActivateAskAudienceJoker()
    {
        connectedClients = networkManager.ServerConnectedClientsCount;

        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);

        var receivedAskAudienceVoteResultCommand = new ReceivedAskAudienceVoteResult(audienceAnswerUI);
        receivedAskAudienceVoteResultCommand.OnFinishedExecution += (sender, args) => waitingToAnswerUI.SetActive(false);

        networkManager.CommandsManager.AddCommand("AskAudienceVoteResult", receivedAskAudienceVoteResultCommand);

        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }
    }
}

public class ReceivedAskAudienceVoteResult : IOneTimeExecuteCommand
{
    public EventHandler OnFinishedExecution
    {
        get;
        set;
    }

    public bool FinishedExecution
    {
        get;
        private set;
    }

    AudienceAnswerUIController audienceAnswerUIController;

    GameObject audienceAnswerUI;

    public ReceivedAskAudienceVoteResult(GameObject audienceAnswerUI)
    {
        if (audienceAnswerUI == null)
        {
            throw new ArgumentNullException("audienceAnswerUI");
        }
         
        this.audienceAnswerUI = audienceAnswerUI;
        this.audienceAnswerUIController = audienceAnswerUI.GetComponent<AudienceAnswerUIController>();
    }

    public void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        var answersVotes = commandsOptionsValues.Where(optionValue => optionValue.Key != "ConnectionId")
            .ToArray();

        var answersVotesData = new Dictionary<string, int>();

        for (int i = 0; i < answersVotes.Length; i++)
        {
            var answer = answersVotes[i].Key;
            var voteCount = int.Parse(answersVotes[i].Value);
            answersVotesData.Add(answer, voteCount);
        }

        audienceAnswerUI.SetActive(true);
        audienceAnswerUIController.SetVoteCount(answersVotesData, true);

        FinishedExecution = true;

        if (OnFinishedExecution != null)
        {
            OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}