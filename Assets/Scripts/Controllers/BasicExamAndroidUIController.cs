using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

//Mediator
public class BasicExamAndroidUIController : MonoBehaviour
{
    public GameObject QuestionPanelUI;
    public GameObject ConnectionSettingsUI;
    public GameObject ConnectingUI;

    public ClientNetworkManager NetworkManager;
    public NotificationsServiceController NotificationsController;
    public ConnectionSettingsUIController connectionSettingsUIController;

    public Animator QuestionPanelAnimator;

    QuestionUIController questionUIController = null;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        CheckDependecies();
        LoadControllers();
        LoadCommands();
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

        if (NetworkManager == null)
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

    void LoadControllers()
    {
        questionUIController = QuestionPanelUI.GetComponent<QuestionUIController>();

        if (questionUIController == null)
        {
            throw new Exception("QuestionUIController component is null");
        }
    }

    void LoadCommands()
    {
        NetworkManager.CommandsManager.AddCommand("AnswerTimeout", new ReceivedAnswerTimeoutCommand(QuestionPanelUI, NotificationsController));
        NetworkManager.CommandsManager.AddCommand("RemainingTimeToAnswer", new ReceivedRemainingTimeToAnswerCommand(OnReceivedRemainingTime));
        NetworkManager.CommandsManager.AddCommand("LoadQuestion", new ReceivedLoadQuestionCommand(LoadQuestion));
    }

    void AttachEventsHooks()
    {
        NetworkManager.OnConnectedEvent += OnConnected;
        NetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;

        questionUIController.OnAnswerClick += OnAnswerClick;

        connectionSettingsUIController.OnConnectToServer += ((sender, e) => Connect(e.IPAddress));
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
        answerSelectedCommand.AddOption("Answer", args.Answer);
        NetworkManager.SendServerCommand(answerSelectedCommand);
        QuestionPanelUI.SetActive(false);
    }

    void OnConnected(object sender, EventArgs args)
    {
        //hide loading bar
        ConnectingUI.SetActive(false);
        //vibrate if mobile
        Vibrate();
    }

    void OnReceivedRemainingTime(RemainingTimeEventArgs remainingTime)
    {
        var timeInSeconds = remainingTime.Seconds;

        if (timeInSeconds > 0 && timeInSeconds <= 10 && timeInSeconds % 2 == 0)
        {
            QuestionPanelAnimator.SetTrigger("shake");
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

    void OnDisconnectFromServer(object sender, EventArgs args)
    {
        //TODO: show ui asking if you want to reconnect or to return to main menu
        ConnectingUI.SetActive(true);
    }

    void LoadQuestion(ISimpleQuestion question)
    {
        QuestionPanelUI.SetActive(true);
        questionUIController.LoadQuestion(question);
        NotificationsController.AddNotification(Color.gray, "Имаш 30 секунди за отговор. Кликни на мен за да ме махнеш");
    }

    public void Connect(string ip)
    {
        try
        {
            NetworkManager.ConnectToHost(ip);    
        }
        catch (NetworkException e)
        {
            var error = (NetworkConnectionError)e.ErrorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);
            NotificationsController.AddNotification(Color.red, errorMessage);
        }
    }

    public void Disconnect()
    {
        try
        {
            NetworkManager.Disconnect();
        }
        catch (NetworkException e)
        {
            var error = (NetworkConnectionError)e.ErrorN;
            var errorMessage = NetworkErrorUtils.GetMessage(error);
            NotificationsController.AddNotification(Color.red, errorMessage);
        }
    }
}
