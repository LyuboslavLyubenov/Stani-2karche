using UnityEngine;
using System;

public class AskAudienceJoker : IJoker
{
    public const int MinClientsForOnlineVote_Release = 4;
    public const int MinClientsForOnlineVote_Development = 1;

    public EventHandler<AudienceVoteEventArgs> OnAudienceVoted = delegate
    {
    };

    ClientNetworkManager networkManager;

    GameObject waitingToAnswerUI;
    GameObject loadingUI;
    GameObject audienceAnswerUI;

    AudienceAnswerPollResultRetriever pollDataRetriever;

    NotificationsServiceController notificationsServiceController;

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

    public AskAudienceJoker(
        ClientNetworkManager networkManager, 
        GameObject waitingToAnswerUI, 
        GameObject audienceAnswerUI, 
        GameObject loadingUI,
        NotificationsServiceController notificationsServiceController)
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

        if (notificationsServiceController == null)
        {
            throw new ArgumentNullException("notificationsServiceController");
        }
            
        this.networkManager = networkManager;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.audienceAnswerUI = audienceAnswerUI;
        this.loadingUI = loadingUI;
        this.pollDataRetriever = AudienceAnswerPollResultRetriever.Instance;
        this.notificationsServiceController = notificationsServiceController;

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/AskAudience");

        this.pollDataRetriever.OnReceiveSettingsTimeout += OnReceiveSettingsTimeout;
        this.pollDataRetriever.OnReceivedSettings += OnReceivedJokerSettings;
        this.pollDataRetriever.OnAudienceVoted += Retriever_OnAudienceVoted;
        this.pollDataRetriever.OnReceiveAudienceVoteTimeout += OnReceiveAudienceVoteTimeout;
    }

    void OnReceiveSettingsTimeout(object sender, EventArgs args)
    {
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(false);

        var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
        notificationsServiceController.AddNotification(Color.red, message);
    }

    void OnReceivedJokerSettings(object sender, JokerSettingsEventArgs args)
    {
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);
        waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = args.TimeToAnswerInSeconds;
    }

    void Retriever_OnAudienceVoted(object sender, AudienceVoteEventArgs args)
    {
        waitingToAnswerUI.SetActive(false);
        audienceAnswerUI.SetActive(true);

        var answersVotes = args.AnswersVotes;
        audienceAnswerUI.GetComponent<AudienceAnswerUIController>()
            .SetVoteCount(answersVotes, true);

        OnAudienceVoted(this, args);
    }

    void OnReceiveAudienceVoteTimeout(object sender, EventArgs args)
    {
        waitingToAnswerUI.SetActive(false);

        var message = LanguagesManager.Instance.GetValue("Error/NetworkMessages/Timeout");
        notificationsServiceController.AddNotification(Color.red, message);
    }

    public void Activate()
    {
        loadingUI.SetActive(true);
        pollDataRetriever.Activate();

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);    
        }

        Activated = true;
    }
}