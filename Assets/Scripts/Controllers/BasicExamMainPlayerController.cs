using UnityEngine;
using System;
using System.Collections;

//Mediator
using System.Collections.Generic;


public class BasicExamMainPlayerController : ExtendedMonoBehaviour
{
    public GameObject LeaderboardUI;
    public GameObject LoadingUI;
    public GameObject EndGameUI;
    public GameObject CallAFriendUI;
    public GameObject FriendAnswerUI;
    public GameObject WaitingToAnswerUI;
    public GameObject AudienceAnswerUI;
    public GameObject UnableToConnectUI;

    public ClientNetworkManager NetworkManager;

    public BasicExamPlayerTeacherDialogSwitcher DialogSwitcher;
    public BasicExamPlayerTutorialUIController TutorialUIController;

    public NotificationsServiceController NotificationService;

    public AvailableJokersUIController AvailableJokersUIController;
    public QuestionUIController QuestionUIController;
    public MarkPanelController MarkPanelController;
    public QuestionsRemainingUIController QuestionsRemainingUIController;

    public RemoteGameData GameData;

    UnableToConnectUIController unableToConnectUIController;

    void Start()
    {
        NetworkManager.CommandsManager.AddCommand("BasicExamGameEnd", new ReceivedBasicExamGameEndCommand(EndGameUI, LeaderboardUI));
        NetworkManager.CommandsManager.AddCommand("AddHelpFromFriendJoker", new ReceivedAddHelpFromFriendJokerCommand(AvailableJokersUIController, NetworkManager, CallAFriendUI, FriendAnswerUI, WaitingToAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand("AddAskAudienceJoker", new ReceivedAddAskAudienceJokerCommand(AvailableJokersUIController, NetworkManager, WaitingToAnswerUI, AudienceAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand("AddFifthyFifthyJoker", new ReceivedAddFifthyFifthyJokerCommand(AvailableJokersUIController, NetworkManager, GameData, QuestionUIController));

        unableToConnectUIController = UnableToConnectUI.GetComponent<UnableToConnectUIController>();

        unableToConnectUIController.OnTryingAgainToConnectToServer += (sender, args) =>
        {
            LoadingUI.SetActive(true);
            NetworkManager.ConnectToHost(args.IPAddress);
        };

        NetworkManager.OnConnectedEvent += OnConnectedToServer;
        NetworkManager.OnDisconnectedEvent += (sender, args) =>
        {
            LoadingUI.SetActive(false);
            UnableToConnectUI.SetActive(true);   
        };

        QuestionUIController.OnAnswerClick += OnAnswerClick;
        QuestionUIController.OnQuestionLoaded += (sender, args) =>
            QuestionsRemainingUIController.SetRemainingQuestions(GameData.RemainingQuestionsToNextMark);

        GameData.OnMarkIncrease += (sender, args) => MarkPanelController.SetMark(args.Mark.ToString());

        LoadingUI.SetActive(true);

        CoroutineUtils.WaitForFrames(1, () =>
            {
                var ip = PlayerPrefs.GetString("ServerIP");
                NetworkManager.ConnectToHost(ip);


            });
    }

    void OnConnectedToServer(object sender, EventArgs args)
    {
        LoadingUI.SetActive(false);

        var commandData = new NetworkCommandData("MainPlayerConnecting");
        NetworkManager.SendServerCommand(commandData);

        GameData.GetCurrentQuestion(QuestionUIController.LoadQuestion, Debug.LogException);
    }

    void ShowNotification(Color color, string message)
    {
        if (NotificationService != null)
        {
            NotificationService.AddNotification(color, message);
        }
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        StartCoroutine(OnAnswerClickCoroutine(args.Answer, args.IsCorrect));
    }

    IEnumerator OnAnswerClickCoroutine(string answer, bool isCorrect)
    {
        var commandData = new NetworkCommandData("AnswerSelected");
        commandData.AddOption("Answer", answer);

        yield return null;

        NetworkManager.SendServerCommand(commandData);

        yield return null;

        if (isCorrect)
        {
            GameData.GetNextQuestion(QuestionUIController.LoadQuestion, Debug.LogException);
        }
    }
}