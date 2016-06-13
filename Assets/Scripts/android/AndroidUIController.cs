using UnityEngine;
using UnityEngine.UI;
using System;

public class AndroidUIController : MonoBehaviour
{
    public GameObject ConnectedUI;
    public GameObject QuestionPanelUI;
    public GameObject DialogUI;
    public GameObject ConnectionSettingsUI;

    QuestionUIController questionUIController = null;
    ClientNetworkManager clientNetworkManager = null;
    DialogUIController dialogUIController = null;

    Text ipText = null;

    GameState currentState = GameState.Playing;

    void Start()
    {
        clientNetworkManager = GameObject.FindWithTag("MainCamera").GetComponent<ClientNetworkManager>();
        questionUIController = QuestionPanelUI.GetComponent<QuestionUIController>();
        dialogUIController = DialogUI.GetComponent<DialogUIController>();

        clientNetworkManager.OnConnectedEvent += OnConnected;
        clientNetworkManager.OnRecievedDataEvent += OnDataRecievedFromServer;
        clientNetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;
    }

    void OnConnected(object sender, System.EventArgs args)
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
        dialogUIController.SetMessage(message);
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
