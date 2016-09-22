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
        friendAnswerUIController = friendAnswerUI.GetComponent<FriendAnswerUIController>();

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

    void OnAppliedAskFriendJokerSettings()
    {
        waitingToAnswerUI.SetActive(true);
        loadingUI.SetActive(false);
    }

    void AskFriendOnline(int clientConnectionId)
    {
        var selectedAskAFriendJokerCommand = new NetworkCommandData("SelectedAskAFriendJoker");
        selectedAskAFriendJokerCommand.AddOption("SendClientId", clientConnectionId.ToString());

        networkManager.SendServerCommand(selectedAskAFriendJokerCommand);

        waitingToAnswerUI.SetActive(true);

        var disableAfterDelay = waitingToAnswerUI.GetComponent<DisableAfterDelay>();
        networkManager.CommandsManager.AddCommand("AskFriendJokerSettings", new ReceivedAskFriendJokerSettingsCommand(disableAfterDelay, OnAppliedAskFriendJokerSettings));

        waitingToAnswerUI.SetActive(false);
        loadingUI.SetActive(true);
    }

    void OnReceivedConnectedClientsIdsNames(OnlineClientsData_Serializable connectedClientsData)
    {
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

    void ActivateCallAFriendJoker()
    {
        callAFriendUIController.OnCalledPlayer += (sender, args) => AskFriendOnline(args.PlayerConnectionId);
        BeginReceiveConnectedClientsIdsNames();

        if (OnActivated != null)
        {
            OnActivated(this, EventArgs.Empty);
        }

        activated = true;
    }

    public void Activate()
    {
        networkManager.CommandsManager.AddCommand("AskAFriendResponse", new ReceivedAskAFriendResponseCommand(OnReceivedAskAFriendResponse));
        networkManager.CommandsManager.AddCommand("ConnectedClientsIdsNames", new ReceivedConnectedClientsDataCommand(OnReceivedConnectedClientsIdsNames));

        ActivateCallAFriendJoker();
    }
}

