using UnityEngine;
using UnityEngine.UI;
using System;

public class AndroidUIController : MonoBehaviour
{
    public GameObject ConnectedUI;
    public GameObject QuestionPanelUI;
    public GameObject DialogUI;
    public GameObject ConnectionSettingsUI;
    public GameObject EnterNameUI;

    QuestionUIController questionUIController = null;
    ClientNetworkManager clientNetworkManager = null;
    DialogUIController dialogUIController = null;
    EnterNameUIController enterNameUIController = null;

    Text ipText = null;

    GameState currentState = GameState.Playing;

    void Start()
    {
        LoadControllers();
        AttachEventsHooks();
    }

    void AttachEventsHooks()
    {
        clientNetworkManager.OnConnectedEvent += OnConnected;
        clientNetworkManager.OnRecievedDataEvent += OnDataRecievedFromServer;
        clientNetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;

        enterNameUIController.OnUsernameSet += OnUsernameSet;
    
        questionUIController.OnAnswerClick += OnAnswerClick;
    }

    void LoadControllers()
    {
        clientNetworkManager = GameObject.FindWithTag("MainCamera").GetComponent<ClientNetworkManager>();
        questionUIController = QuestionPanelUI.GetComponent<QuestionUIController>();
        dialogUIController = DialogUI.GetComponent<DialogUIController>();
        enterNameUIController = EnterNameUI.GetComponent<EnterNameUIController>();
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        clientNetworkManager.SendData(args.Answer);
        QuestionPanelUI.SetActive(false);
    }

    void OnUsernameSet(object sender, EventArgs args)
    {
        ConnectionSettingsUI.SetActive(true);
    }

    void OnConnected(object sender, EventArgs args)
    {
        currentState = GameState.Playing;
        ConnectionSettingsUI.SetActive(false);
        ConnectedUI.SetActive(true);
    }

    void OnDataRecievedFromServer(object sender, DataSentEventArgs args)
    {
        switch (currentState)
        {
            case GameState.Playing:

                if (args.Message == "AskFriend")
                {
                    currentState = GameState.AskingAFriend;
                }
                else if (args.Message == "AskAudience")
                {
                    currentState = GameState.AskingAudience;
                }

                break;

            case GameState.AskingAFriend:
                
            case GameState.AskingAudience:

                LoadQuestion(args.Message);
                currentState = GameState.Playing;

                break;
        }
    }

    void OnDisconnectFromServer(object sender, System.EventArgs args)
    {
        currentState = GameState.Playing;
        ConnectedUI.SetActive(false);
        ConnectionSettingsUI.SetActive(true);
    }

    void LoadQuestion(string questionJSON)
    {
        var question = JsonUtility.FromJson<Question>(questionJSON);
        QuestionPanelUI.SetActive(true);
        questionUIController.LoadQuestion(question);
    }

    void ShowDialogMessage(string message)
    {
        DialogUI.SetActive(true);
        dialogUIController.SetErrorMessage(message);
    }

    public void Connect(string ip)
    {
        try
        {
            clientNetworkManager.ConnectToHost(ip);    
        }
        catch (Exception e)
        {
            ShowDialogMessage(e.Message);
        }
    }

    public void Disconnect()
    {
        try
        {
            clientNetworkManager.Disconnect();
        }
        catch (Exception ex)
        {
            ShowDialogMessage(ex.Message);
        }
    }
}
