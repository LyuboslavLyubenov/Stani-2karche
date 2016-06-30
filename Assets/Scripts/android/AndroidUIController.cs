using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

/// <summary>
/// Responsible for voting (call a friend and audience vote) and showing error dialogs
/// </summary>
public class AndroidUIController : MonoBehaviour
{
    public GameObject ConnectedUI;
    public GameObject QuestionPanelUI;
    public GameObject DialogUI;
    public GameObject ConnectionSettingsUI;
    public GameObject ConnectingUI;
    public ClientNetworkManager clientNetworkManager;

    QuestionUIController questionUIController = null;
    DialogUIController dialogUIController = null;

    Text ipText = null;
   
    //shows if currently is answering as friend or as audience
    GameState currentState = GameState.Playing;

    void Start()
    {
        LoadControllers();
        AttachEventsHooks();

        ConnectingUI.SetActive(true);
    }

    /// <summary>
    /// Loads needed UI controllers
    /// </summary>
    void LoadControllers()
    {
        questionUIController = QuestionPanelUI.GetComponent<QuestionUIController>();
        dialogUIController = DialogUI.GetComponent<DialogUIController>();
    }

    /// <summary>
    /// Attachs the events hooks.
    /// </summary>
    void AttachEventsHooks()
    {
        clientNetworkManager.OnConnectedEvent += OnConnected;
        clientNetworkManager.OnReceivedDataEvent += OnDataRecievedFromServer;
        clientNetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;

        questionUIController.OnAnswerClick += OnAnswerClick;
    }

    //if clicked on any answer
    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        //send answer to the server
        clientNetworkManager.SendMessage(args.Answer);
        //hide Question UI
        QuestionPanelUI.SetActive(false);
    }

    void OnConnected(object sender, EventArgs args)
    {
        //if connected
        //reset game state
        currentState = GameState.Playing;
        //hide loading bar
        ConnectingUI.SetActive(false);
        //show successfull connection message
        ConnectedUI.SetActive(true);
        //vibrate if mobile
        Vibrate();
    }

    void OnDataRecievedFromServer(object sender, DataSentEventArgs args)
    {
        //if recieved something from the server
        switch (currentState)
        {
            case GameState.Playing:

                //change our game state
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
                //if you are choosed from "Ask friend" or "Help from Audience"
                //Load question
                LoadQuestion(args.Message);
                //Vibrate if mobile
                Vibrate();

                currentState = GameState.Playing;

                break;
        }
    }

    /// <summary>
    /// Vibrate if mobile.
    /// </summary>
    void Vibrate()
    {
        #if UNITY_ANDROID
        Handheld.Vibrate();
        #endif
    }

    void OnDisconnectFromServer(object sender, System.EventArgs args)
    {
        //if dissconnected from server
        //reset game state
        currentState = GameState.Playing;
        //if connected message is somehow visible, hide it
        ConnectedUI.SetActive(false);
        //show loading bar
        ConnectingUI.SetActive(true);
    }

    void LoadQuestion(string questionJSON)
    {
        //deserialize received question
        var question = JsonUtility.FromJson<Question>(questionJSON);
        //activate question panel
        QuestionPanelUI.SetActive(true);
        //populate question data
        questionUIController.LoadQuestion(question);
    }

    /// <summary>
    /// Shows the dialog message.
    /// </summary>
    /// <param name="message">Message.</param>
    void ShowDialogMessage(string message)
    {
        DialogUI.SetActive(true);
        dialogUIController.SetErrorMessage(message);
    }

    /// <summary>
    /// Connect the specified ip.
    /// </summary>
    /// <param name="ip">Ip</param>
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

    /// <summary>
    /// Disconnect from server.
    /// </summary>
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
