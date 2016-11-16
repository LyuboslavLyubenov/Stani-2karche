using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class HelpFromFriendJoker : IJoker, INetworkOperationExecutedCallback
{
    const int SettingsReceiveTimeoutInSeconds = 5;
    //0% = 0f, 100% = 1f
    const float ChanceForGeneratingCorrectAnswer = 0.85f;

    public EventHandler<AnswerEventArgs> OnFriendAnswered = delegate
    {
    };

    ClientNetworkManager networkManager;

    CallAFriendUIController callAFriendUIController;

    GameObject callAFriendUI;
    GameObject friendAnswerUI;
    GameObject waitingToAnswerUI;
    GameObject loadingUI;

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

    public EventHandler OnExecuted
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

        Image = Resources.Load<Sprite>("Images/Buttons/Jokers/HelpFromFriend");
    }

    void OnReceivedConnectedClientsIdsNames(OnlineClientsData_Serializable connectedClientsData)
    {
        var connectedClientsIdsNames = connectedClientsData.OnlinePlayers.ToDictionary(c => c.ConnectionId, c => c.Username);
        connectedClientsIdsNames.Add(NetworkCommandData.CODE_OptionClientConnectionIdValueAI, LanguagesManager.Instance.GetValue("Computer"));

        loadingUI.SetActive(false);
        callAFriendUI.SetActive(true);

        callAFriendUIController.SetContacts(connectedClientsIdsNames);
        callAFriendUIController.OnCalledPlayer += (sender, args) =>
        {
            var selectedJokerCommand = new NetworkCommandData("SelectedHelpFromFriendJoker");
            selectedJokerCommand.AddOption("SendClientId", args.PlayerConnectionId.ToString());
            networkManager.SendServerCommand(selectedJokerCommand);
            
            loadingUI.SetActive(true);

            var receivedSettingsCommand = new ReceivedHelpFromFriendJokerSettingsCommand(networkManager, loadingUI, waitingToAnswerUI);
            receivedSettingsCommand.OnFinishedExecution += (s, a) => OnReceivedSettings();

            networkManager.CommandsManager.AddCommand("HelpFromFriendJokerSettings", receivedSettingsCommand);

            receiveSettingsTimeoutTimer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
            receiveSettingsTimeoutTimer.AutoReset = false;
            receiveSettingsTimeoutTimer.Elapsed += (s, e) => OnReceiveSettingsTimeout();
        };
    }

    void BeginReceiveConnectedClientsIdsNames()
    {
        var commandData = new NetworkCommandData("ConnectedClientsIdsNames");
        networkManager.SendServerCommand(commandData);
        networkManager.CommandsManager.AddCommand("ConnectedClientsIdsNames", new ReceivedConnectedClientsDataCommand(OnReceivedConnectedClientsIdsNames));
    }

    void OnReceiveSettingsTimeout()
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                activated = false;
                loadingUI.SetActive(false);
                receiveSettingsTimeoutTimer.Dispose();
                networkManager.CommandsManager.RemoveCommand("HelpFromFriendJokerSettings");
            });
    }

    void OnReceivedSettings()
    {
        receiveSettingsTimeoutTimer.Stop();
        receiveSettingsTimeoutTimer.Dispose();

        var responseCommand = new ReceivedHelpFromFriendResponseCommand(waitingToAnswerUI, friendAnswerUI);
        responseCommand.OnFinishedExecution += (sender, args) =>
        {
            if (OnExecuted != null)
            {
                OnExecuted(this, EventArgs.Empty);
            }
        };

        networkManager.CommandsManager.AddCommand("HelpFromFriendJokerResponse", responseCommand);
    }

    public void Activate()
    {
        loadingUI.SetActive(true);
        BeginReceiveConnectedClientsIdsNames();
    }
}