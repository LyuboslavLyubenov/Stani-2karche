using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

public class HelpFromFriendJoker : IJoker
{
    //0% = 0f, 100% = 1f
    const float ChanceForGeneratingCorrectAnswer = 0.85f;

    public EventHandler<AnswerEventArgs> OnFriendAnswered = delegate
    {
    };

    CallAFriendUIController callAFriendUIController;

    ClientNetworkManager networkManager;

    GameObject callAFriendUI;
    GameObject friendAnswerUI;
    GameObject waitingToAnswerUI;
    GameObject loadingUI;

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

    public HelpFromFriendJoker(ClientNetworkManager networkManager, 
                               GameObject callAFriendUI, 
                               GameObject friendAnswerUI, 
                               GameObject waitingToAnswerUI,
                               GameObject loadingUI)
    {
            
        if (networkManager == null)
        {
            throw new ArgumentNullException("networkManager");
        }

        if (callAFriendUI == null)
        {
            throw new ArgumentNullException("callAFriendUI");
        }

        if (friendAnswerUI == null)
        {
            throw new ArgumentNullException("friendAnswerUI");
        }

        if (waitingToAnswerUI == null)
        {
            throw new ArgumentNullException("waitingToAnswerUI");
        }

        if (loadingUI == null)
        {
            throw new ArgumentNullException("loadingUI");
        }

        this.networkManager = networkManager;
        this.callAFriendUI = callAFriendUI;
        this.friendAnswerUI = friendAnswerUI;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.loadingUI = loadingUI;

        callAFriendUIController = callAFriendUI.GetComponent<CallAFriendUIController>();

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/HelpFromFriend");
    }

    void OnReceivedConnectedClientsIdsNames(OnlineClientsData_Serializable connectedClientsData)
    {
        var connectedClientsIdsNames = connectedClientsData.OnlinePlayers.ToDictionary(c => c.ConnectionId, c => c.Username);
        connectedClientsIdsNames.Add(NetworkCommandData.CODE_Option_ClientConnectionId_AI, LanguagesManager.Instance.GetValue("Computer"));

        loadingUI.SetActive(false);
        callAFriendUI.SetActive(true);

        callAFriendUIController.SetContacts(connectedClientsIdsNames);
        callAFriendUIController.OnCalledPlayer += OnCalledPlayer;
    }

    void OnCalledPlayer(object sender, PlayerCalledEventArgs args)
    {
        callAFriendUI.SetActive(false);
        loadingUI.SetActive(true);

        var resultRetriever = AskPlayerQuestionResultRetriever.Instance;

        resultRetriever.OnReceivedSettings += OnReceivedSettings;
        resultRetriever.OnReceiveSettingsTimeout += OnReceiveSettingsTimeout;
        resultRetriever.OnReceivedAnswer += OnReceivedAnswer;
        resultRetriever.OnReceiveAnswerTimeout += OnReceiveAnswerTimeout;

        resultRetriever.Activate(args.PlayerConnectionId);
    }

    void OnReceivedSettings(object sender, JokerSettingsEventArgs args)
    {
        loadingUI.SetActive(false);
        waitingToAnswerUI.SetActive(true);
        waitingToAnswerUI.GetComponent<DisableAfterDelay>().DelayInSeconds = args.TimeToAnswerInSeconds;
    }

    void OnReceiveSettingsTimeout(object sender, EventArgs args)
    {
        loadingUI.SetActive(false);

        var message = LanguagesManager.Instance.GetValue("Errors/NetworkErrors/Timeout");
        NotificationsServiceController.Instance.AddNotification(Color.red, message);

        Activated = false;
    }

    void OnReceivedAnswer(object sender, AskPlayerResponseEventArgs args)
    {
        waitingToAnswerUI.SetActive(false);
        friendAnswerUI.SetActive(true);
        friendAnswerUI.GetComponent<FriendAnswerUIController>().SetResponse(args.Username, args.Answer);

        Activated = false;
    }

    void OnReceiveAnswerTimeout(object sender, EventArgs args)
    {
        waitingToAnswerUI.SetActive(false);

        var message = LanguagesManager.Instance.GetValue("Errors/NetworkErrors/Timeout");
        NotificationsServiceController.Instance.AddNotification(Color.red, message);

        Activated = false;
    }

    void BeginReceiveConnectedClientsIdsNames()
    {
        var commandData = new NetworkCommandData("ConnectedClientsIdsNames");
        networkManager.SendServerCommand(commandData);
        networkManager.CommandsManager.AddCommand("ConnectedClientsIdsNames", new ConnectedClientsDataCommand(OnReceivedConnectedClientsIdsNames));
    }

    public void Activate()
    {
        loadingUI.SetActive(true);
        BeginReceiveConnectedClientsIdsNames();
        Activated = true;

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);    
        }
    }
}