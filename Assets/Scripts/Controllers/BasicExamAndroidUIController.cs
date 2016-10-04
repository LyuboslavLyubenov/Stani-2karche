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
    public GameObject EndGameUI;
    public GameObject LeaderboardUI;
    public GameObject UnableToConnectUI;

    public ClientNetworkManager NetworkManager;
    public NotificationsServiceController NotificationsController;
    public ConnectionSettingsUIController connectionSettingsUIController;

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
        unableToConnectController = unableToConnectController.GetComponent<UnableToConnectUIController>();

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
        NetworkManager.CommandsManager.AddCommand("BasicExamGameEnd", new ReceivedBasicExamGameEndCommand(EndGameUI, LeaderboardUI));
    }

    void AttachEventsHooks()
    {
        NetworkManager.OnConnectedEvent += OnConnected;
        NetworkManager.OnDisconnectedEvent += OnDisconnectFromServer;
        questionUIController.OnAnswerClick += OnAnswerClick;
        connectionSettingsUIController.OnConnectToServer += ((sender, e) => ConnectTo(e.IPAddress));
        unableToConnectController.OnTryingAgainToConnectToServer += (sender, args) => ConnectTo(args.IPAddress);
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
        ConnectingUI.SetActive(false);
        UnableToConnectUI.SetActive(true);
    }

    void LoadQuestion(ISimpleQuestion question)
    {
        QuestionPanelUI.SetActive(true);
        questionUIController.LoadQuestion(question);
        NotificationsController.AddNotification(Color.gray, "Имаш 30 секунди за отговор. Кликни на мен за да ме махнеш");
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
