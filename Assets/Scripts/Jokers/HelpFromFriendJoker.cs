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

    IGameData gameData;
    ClientNetworkManager networkManager;

    CallAFriendUIController callAFriendUIController;
    FriendAnswerUIController friendAnswerUIController;

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
        get
        {
            return activated;
        }
    }

    bool activated = false;

    public HelpFromFriendJoker(IGameData gameData, 
                               ClientNetworkManager networkManager, 
                               GameObject callAFriendUI, 
                               GameObject friendAnswerUI, 
                               GameObject waitingToAnswerUI)
    {
        if (gameData == null)
        {
            throw new ArgumentNullException("gameData");
        }
            
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

        this.gameData = gameData;
        this.networkManager = networkManager;
        this.callAFriendUI = callAFriendUI;
        this.friendAnswerUI = friendAnswerUI;
        this.waitingToAnswerUI = waitingToAnswerUI;
        this.loadingUI = GameObject.FindWithTag("LoadingUI");

        callAFriendUIController = callAFriendUI.GetComponent<CallAFriendUIController>();
        friendAnswerUIController = friendAnswerUI.GetComponent<FriendAnswerUIController>();

        networkManager.CommandsManager.AddCommand("AskAFriendResponse", new AskAFriendResponseCommand(OnReceivedAskAFriendResponse));
        networkManager.CommandsManager.AddCommand("ConnectedClientsIdsNames", new ClientReceiveConnectedClientsDataCommand(OnReceivedConnectedClientsIdsNames));

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/HelpFromFriend");
    }

    void OnReceivedAskAFriendResponse(string username, string answer)
    {
        if (!activated)
        {
            return;
        }

        waitingToAnswerUI.SetActive(false);
        friendAnswerUI.SetActive(true);
        friendAnswerUIController.SetResponse(username, answer);
    }

    void OnReceivedConnectedClientsIdsNames(OnlineClientsData_Serializable connectedClientsData)
    {
        if (!activated)
        {
            return;
        }

        loadingUI.SetActive(false);

        var connectedClientsIdsNames = connectedClientsData.OnlinePlayers.ToDictionary(c => c.ConnectionId, c => c.Username);
        callAFriendUI.SetActive(true);
        callAFriendUIController.SetContacts(connectedClientsIdsNames);
    }

    void BeginReceiveConnectedClientsIdsNames()
    {
        var commandData = new NetworkCommandData("ConnectedClientsIdsNames");
        networkManager.SendServerCommand(commandData);
        loadingUI.SetActive(true);
    }

    void AskFriendOnline(Question question, int clientConnectionId)
    {
        var commandData = new NetworkCommandData("SelectedAskAFriendJoker");
        var questionJSON = JsonUtility.ToJson(question);
        commandData.AddOption("QuestionJSON", questionJSON);
        commandData.AddOption("ClientConnectionId", clientConnectionId.ToString());

        networkManager.SendServerCommand(commandData);

        waitingToAnswerUI.SetActive(true);

        var disableAfterDelayComponent = waitingToAnswerUI.GetComponent<DisableAfterDelay>();

        disableAfterDelayComponent.OnTimePass += (sender, args) => SendRemainingTimeToClient(clientConnectionId, args.Seconds);
        disableAfterDelayComponent.OnTimeEnd += (sender, args) => StopReceivingAnswer(clientConnectionId);
    }

    void StopReceivingAnswer(int clientConnectionId)
    {
        var answerTimeoutCommandData = new NetworkCommandData("AnswerTimeout");
        answerTimeoutCommandData.AddOption("ClientConnectionId", clientConnectionId.ToString());
        networkManager.SendServerCommand(answerTimeoutCommandData);
    }

    void SendRemainingTimeToClient(int clientConnectionId, int remainingTimeInSeconds)
    {
        var commandData = new NetworkCommandData("RemainingTimeToAnswer");
        commandData.AddOption("TimeInSeconds", remainingTimeInSeconds.ToString());
        commandData.AddOption("ClientConnectionId", clientConnectionId.ToString());
        networkManager.SendServerCommand(commandData);
    }

    public void Activate()
    {
        gameData.GetCurrentQuestion(ActivateCallAFriendJoker, Debug.LogException);
    }

    void ActivateCallAFriendJoker(Question currentQuestion)
    {
        callAFriendUIController.OnCalledPlayer += (sender, args) => AskFriendOnline(currentQuestion, args.PlayerConnectionId);
        BeginReceiveConnectedClientsIdsNames();

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }

        activated = true;
    }
}