using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;

public class AskAudienceJoker : IJoker
{
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

    public AskAudienceJoker(IGameData gameData, ClientNetworkManager networkManager, GameObject waitingToAnswerUI, GameObject audienceAnswerUI)
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
            
        this.gameData = gameData;
        this.networkManager = networkManager;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.audienceAnswerUI = audienceAnswerUI;
        this.audienceAnswerUIController = audienceAnswerUI.GetComponent<AudienceAnswerUIController>();
        this.loadingUI = GameObject.FindWithTag("LoadingUI");

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");
    }

    public void Activate()
    {
        loadingUI.SetActive(true);
        gameData.GetCurrentQuestion(ActivateAskAudienceJoker, (exception) =>
            {
                loadingUI.SetActive(false);
                Debug.LogException(exception);
            });
    }

    void ActivateAskAudienceJoker(Question currentQuestion)
    {
        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }

        connectedClients = networkManager.ServerConnectedClientsCount;

        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);

        var waitingToAnswerUIController = waitingToAnswerUI.GetComponent<WaitingToAnswerUIController>();
        waitingToAnswerUIController.ActivateSendRemainingTimeToClientCommand(NetworkCommandData.CODE_OptionClientConnectionIdValueAll);
        waitingToAnswerUIController.ActivateSendStopReceivingAnswerCommand(NetworkCommandData.CODE_OptionClientConnectionIdValueAll);

        StartReceiveAudienceVote();
    }

    void StartReceiveAudienceVote()
    {
        networkManager.CommandsManager.AddCommand("AskAudienceVote", new AskAudienceVoteCommand(ReceivedAudienceVote));
    }

    public void AddVote(int connectionId, string answer)
    {
        ReceivedAudienceVote(connectionId, answer);
    }

    void ReceivedAudienceVote(int connectionId, string answer)
    {
        if (!Activated)
        {
            return;   
        }

        if (!audienceAnswerVoteCount.ContainsKey(answer) || audienceVotedId.Contains(connectionId))
        {
            StartReceiveAudienceVote();
            return;
        }

        audienceVotedId.Add(connectionId);
        audienceAnswerVoteCount[answer]++;

        //TODO: CHECK AFTER TIMEOUT
        if (audienceVotedId.Count >= connectedClients)
        {
            audienceAnswerUI.SetActive(true);
            audienceAnswerUIController.SetVoteCount(audienceAnswerVoteCount, false);

            if (OnAudienceVoted != null)
            {
                OnAudienceVoted(this, new AudienceVoteEventArgs(audienceAnswerVoteCount));
            }

            return;
        }

        StartReceiveAudienceVote();
    }
}
