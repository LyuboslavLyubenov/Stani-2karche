using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Responsible for voting (call a friend and audience vote) and showing error dialogs
/// </summary>
public class AndroidUIController : MonoBehaviour
{
    public GameObject QuestionPanelUI;
    public GameObject ConnectionSettingsUI;
    public GameObject ConnectingUI;
    public ClientNetworkManager ClientNetworkManager;
    public NotificationsController NotificationsController;

    QuestionUIController questionUIController = null;

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
    }

    /// <summary>
    /// Attachs the events hooks.
    /// </summary>
    void AttachEventsHooks()
    {
        ClientNetworkManager.OnConnectedEvent += OnConnected;
        ClientNetworkManager.OnReceivedDataEvent += OnDataRecievedFromServer;
        ClientNetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;

        questionUIController.OnAnswerClick += OnAnswerClick;
    }

    //if clicked on any answer
    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        //send answer to the server
        ClientNetworkManager.SendMessage(args.Answer);
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
        NotificationsController.AddNotification(Color.green, "Успешно свързан");
        //vibrate if mobile
        Vibrate();
    }

    void OnDataRecievedFromServer(object sender, DataSentEventArgs args)
    {
        if (args.Message == "AnswerTimeout")
        {
            QuestionPanelUI.SetActive(false);
            return;
        }

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
                else if (args.Message == "RiskyTrust")
                {
                    currentState = GameState.RiskyTrust;
                }

                break;

            case GameState.RiskyTrust: 

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
    /// Connect the specified ip.
    /// </summary>
    /// <param name="ip">Ip</param>
    public void Connect(string ip)
    {
        try
        {
            ClientNetworkManager.ConnectToHost(ip);    
        }
        catch (NetworkException e)
        {
            var error = (NetworkConnectionError)e.ErrorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);
            NotificationsController.AddNotification(Color.red, errorMessage);
        }
    }

    /// <summary>
    /// Disconnect from server.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            ClientNetworkManager.Disconnect();
        }
        catch (NetworkException e)
        {
            var error = (NetworkConnectionError)e.ErrorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);
            NotificationsController.AddNotification(Color.red, errorMessage);
        }
    }
}
