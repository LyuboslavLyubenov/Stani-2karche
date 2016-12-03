using UnityEngine;
using System;

//Mediator
public class BasicExamAndroidUIController : MonoBehaviour
{
    public GameObject QuestionPanelUI;
    public GameObject ConnectionSettingsUI;
    public GameObject ConnectingUI;
    public GameObject EndGameUI;
    public GameObject LeaderboardUI;
    public GameObject UnableToConnectUI;
    public GameObject SecondsRemainingUI;
    public GameObject MainPlayerDialogUI;

    public ClientNetworkManager NetworkManager;
    public NotificationsServiceController NotificationsController;
    public ConnectionSettingsUIController ConnectionSettingsUIController;
    public SecondsRemainingUIController SecondsRemainingUIController;
    public DialogController MainPlayerDialogController;

    public Animator QuestionPanelAnimator;

    QuestionUIController questionUIController = null;
    UnableToConnectUIController unableToConnectController = null;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadControllers();
        LoadCommands();
        AttachEventsHooks();

        var ipToConnect = PlayerPrefs.GetString("ServerIP");
        ConnectTo(ipToConnect);
    }

    void LoadControllers()
    {
        questionUIController = QuestionPanelUI.GetComponent<QuestionUIController>();
        unableToConnectController = UnableToConnectUI.GetComponent<UnableToConnectUIController>();
    }

    void LoadCommands()
    {
        NetworkManager.CommandsManager.AddCommand("AnswerTimeout", new ReceivedAnswerTimeoutCommand(QuestionPanelUI, NotificationsController));
        NetworkManager.CommandsManager.AddCommand("LoadQuestion", new ReceivedLoadQuestionCommand(LoadQuestion));
        NetworkManager.CommandsManager.AddCommand("BasicExamGameEnd", new ReceivedBasicExamGameEndCommand(EndGameUI, LeaderboardUI));
    }

    void AttachEventsHooks()
    {
        NetworkManager.OnConnectedEvent += OnConnected;
        NetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;
        questionUIController.OnAnswerClick += OnAnswerClick;
        ConnectionSettingsUIController.OnConnectToServer += ((sender, e) => ConnectTo(e.IPAddress));
        unableToConnectController.OnTryingAgainToConnectToServer += (sender, args) => ConnectTo(args.IPAddress);
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
        answerSelectedCommand.AddOption("Answer", args.Answer);
        NetworkManager.SendServerCommand(answerSelectedCommand);

        QuestionPanelUI.SetActive(false);
        SecondsRemainingUI.SetActive(false);
        //MainPlayerDialogUI.SetActive(true);
    }

    void OnConnected(object sender, EventArgs args)
    {
        ConnectingUI.SetActive(false);
        Vibrate();
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
        ConnectingUI.SetActive(false);
        UnableToConnectUI.SetActive(true);
    }

    void LoadQuestion(ISimpleQuestion question, int timeToAnswer)
    {
        QuestionPanelUI.SetActive(true);
        questionUIController.LoadQuestion(question);

        SecondsRemainingUI.SetActive(true);
        SecondsRemainingUIController.SetSeconds(timeToAnswer);
        SecondsRemainingUIController.Paused = false;
    }

    public void ConnectTo(string ip)
    {
        ConnectingUI.SetActive(true);

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
