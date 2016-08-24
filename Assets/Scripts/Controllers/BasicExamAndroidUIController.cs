using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Responsible for voting (call a friend and audience vote) and showing error dialogs
/// </summary>
public class BasicExamAndroidUIController : MonoBehaviour
{
    public GameObject QuestionPanelUI;
    public GameObject ConnectionSettingsUI;
    public GameObject ConnectingUI;

    public ClientNetworkManager ClientNetworkManager;
    public NotificationsServiceController NotificationsController;
    public ConnectionSettingsUIController connectionSettingsUIController;

    public Animator QuestionPanelAnimator;

    QuestionUIController questionUIController = null;

    Text ipText = null;
   
    //shows if currently is answering as friend or as audience
    GameState currentState = GameState.Playing;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        CheckDependecies();
        LoadControllers();
        AttachEventsHooks();

        ConnectingUI.SetActive(true);

        var ipToConnect = PlayerPrefs.GetString("ServerIP");
        Connect(ipToConnect);
    }

    void CheckDependecies()
    {
        if (QuestionPanelUI == null)
        {
            throw new Exception("QuestionPanelUI is null on AndroidUIController obj");
        }

        if (ConnectionSettingsUI == null)
        {
            throw new Exception("ConnectionSettingsUI is null on AndroidUIController obj");
        }

        if (ConnectingUI == null)
        {
            throw new Exception("ConnectingUI is null on AndroidUIController obj");
        }

        if (ClientNetworkManager == null)
        {
            throw new Exception("ClientNetworkManager is null on AndroidUIController obj");
        }

        if (NotificationsController == null)
        {
            throw new Exception("NotificationsController is null on AndroidUIController obj");
        }

        if (QuestionPanelAnimator == null)
        {
            throw new Exception("QuestionPanelAnimator is null on AndroidUIController obj");
        }
    }

    /// <summary>
    /// Loads needed UI controllers
    /// </summary>
    void LoadControllers()
    {
        questionUIController = QuestionPanelUI.GetComponent<QuestionUIController>();

        if (questionUIController == null)
        {
            throw new Exception("QuestionUIController component is null");
        }
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

        connectionSettingsUIController.OnConnectToServer += ((sender, e) => Connect(e.IPAddress));
    }

    //if clicked on any answer
    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        //send answer to the server
        ClientNetworkManager.SendServerMessage(args.Answer);//TODO: SendServerMessage("[AnswerSelected]" + args.Answer)
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
        //vibrate if mobile
        Vibrate();
    }

    void OnDataRecievedFromServer(object sender, DataSentEventArgs args)
    {
        if (currentState == GameState.Playing)
        {
            if (args.Message == "AnswerTimeout")
            {
                QuestionPanelUI.SetActive(false);
                NotificationsController.AddNotification(Color.blue, "Съжалявам, времето за отговаряне на въпроса свърши.");
                return;
            }

            if (args.Message.IndexOf("RemainingTime") > -1)
            {
                var messageParams = args.Message.Split('=');

                if (messageParams.Length != 2)
                {
                    return;
                }

                int remainingTime;
                bool isValidNumber = int.TryParse(messageParams[1], out remainingTime);

                if (!isValidNumber)
                {
                    return;
                }

                if (remainingTime > 0 && remainingTime <= 10 && remainingTime % 2 == 0)
                {
                    QuestionPanelAnimator.SetTrigger("shake");
                }

                return;
            }

            if (args.Message == "AskFriend")
            {
                currentState = GameState.AskingAFriend;
                return;
            }

            if (args.Message == "AskAudience")
            {
                currentState = GameState.AskingAudience;
                return;
            }

            if (args.Message == "RiskyTrust")
            {
                currentState = GameState.RiskyTrust;
                return;
            }

            return;
        }

        if (currentState == GameState.RiskyTrust || currentState == GameState.AskingAudience || currentState == GameState.AskingAFriend)
        {
            //TODO: RECEIVE FIRST "LOADQUESTION" then this
            LoadQuestion(args.Message);
            //Vibrate if mobile
            Vibrate();

            currentState = GameState.Playing;
            return;
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
        StartCoroutine(LoadQuestionCoroutine(questionJSON));
    }

    IEnumerator LoadQuestionCoroutine(string questionJSON)
    {
        //deserialize received question
        var question = JsonUtility.FromJson<Question>(questionJSON);
        //activate question panel
        QuestionPanelUI.SetActive(true);

        yield return null;
        //populate question data
        questionUIController.LoadQuestion(question);

        yield return null;

        NotificationsController.AddNotification(Color.gray, "Имаш 30 секунди за отговор. Кликни на мен за да ме махнеш");
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
