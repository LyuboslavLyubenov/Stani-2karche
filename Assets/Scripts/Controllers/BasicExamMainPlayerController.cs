using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

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
    public ClientChooseCategoryUIController ChooseCategoryUIController;
    public SecondsRemainingUIController SecondsRemainingUIController;

    public RemoteGameData GameData;

    UnableToConnectUIController unableToConnectUIController;

    void Start()
    {
        unableToConnectUIController = UnableToConnectUI.GetComponent<UnableToConnectUIController>();

        //TODO UPDATE Seconds remaining UI CONTROLLER
        InitializeCommands();
        StartServerIfPlayerIsHost();
        AttachEventHandlers();
        ConnectToServer();

        LoadingUI.SetActive(true);
    }

    void InitializeCommands()
    {
        var loadedGameDataCommand = new DummyOneTimeCommand();
        loadedGameDataCommand.OnFinishedExecution += (sender, args) =>
        {
            GameData.GetCurrentQuestion(QuestionUIController.LoadQuestion, Debug.LogException);
        };

        NetworkManager.CommandsManager.AddCommand("BasicExamGameEnd", new ReceivedBasicExamGameEndCommand(EndGameUI, LeaderboardUI));
        NetworkManager.CommandsManager.AddCommand("AddHelpFromFriendJoker", new ReceivedAddHelpFromFriendJokerCommand(AvailableJokersUIController, NetworkManager, CallAFriendUI, FriendAnswerUI, WaitingToAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand("AddAskAudienceJoker", new ReceivedAddAskAudienceJokerCommand(AvailableJokersUIController, NetworkManager, WaitingToAnswerUI, AudienceAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand("AddFifthyFifthyJoker", new ReceivedAddFifthyFifthyJokerCommand(AvailableJokersUIController, NetworkManager, GameData, QuestionUIController));
        NetworkManager.CommandsManager.AddCommand("LoadedGameData", loadedGameDataCommand);
    }

    void StartServerIfPlayerIsHost()
    {
        if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
        {
            System.Diagnostics.Process.Start("Server\\start server nogui.bat");
            SceneManager.activeSceneChanged += OnActivateSceneChanged;
        }
    }

    void AttachEventHandlers()
    {
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
        QuestionUIController.OnQuestionLoaded += OnQuestionLoaded;

        GameData.OnMarkIncrease += (sender, args) => MarkPanelController.SetMark(args.Mark.ToString());

        ChooseCategoryUIController.OnLoadedCategories += (sender, args) =>
        {
            LoadingUI.SetActive(false);
        };
        ChooseCategoryUIController.OnChoosedCategory += (sender, args) =>
        {
            var selectedCategoryCommand = new NetworkCommandData("SelectedCategory");
            selectedCategoryCommand.AddOption("Category", args.Name);
            NetworkManager.SendServerCommand(selectedCategoryCommand);
        };

        AvailableJokersUIController.OnAddedJoker += (sender, args) =>
        {
            var jokerExecutedCallback = args.Joker as INetworkOperationExecutedCallback;

            if (jokerExecutedCallback == null)
            {
                return;
            }

            jokerExecutedCallback.OnExecuted += (s, a) =>
            {
                SecondsRemainingUIController.Paused = false;
            };
        };

        AvailableJokersUIController.OnUsedJoker += (sender, args) =>
        {
            SecondsRemainingUIController.Paused = true;  
        };
    }

    void OnQuestionLoaded(object sender, SimpleQuestionEventArgs args)
    {
        QuestionsRemainingUIController.SetRemainingQuestions(GameData.RemainingQuestionsToNextMark);
        SecondsRemainingUIController.SetSeconds(GameData.SecondsForAnswerQuestion);
    }

    void ConnectToServer()
    {
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var ip = PlayerPrefs.GetString("ServerIP");
                NetworkManager.ConnectToHost(ip);
            });
    }

    void OnActivateSceneChanged(Scene oldScene, Scene newScene)
    {
        KillLocalServer();
        SceneManager.activeSceneChanged -= OnActivateSceneChanged;
    }

    void OnApplicationQuit()
    {
        KillLocalServer();
    }

    void KillLocalServer()
    {
        if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
        {
            var serverProcesses = System.Diagnostics.Process.GetProcessesByName("stani2karcheserver");

            for (int i = 0; i < serverProcesses.Length; i++)
            {
                serverProcesses[i].Kill();
            }

            PlayerPrefs.DeleteKey("MainPlayerHost");
        }
    }

    void OnConnectedToServer(object sender, EventArgs args)
    {
        var commandData = new NetworkCommandData("MainPlayerConnecting");
        NetworkManager.SendServerCommand(commandData);

        var remoteCategoriesReader = new RemoteAvailableCategoriesReader(NetworkManager, () =>
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/CantLoadCategories");
                Debug.LogError(errorMsg);
                ShowNotification(Color.red, errorMsg);

                ChooseCategoryUIController.gameObject.SetActive(false);
                NetworkManager.Disconnect();
            }, 5);

        ChooseCategoryUIController.gameObject.SetActive(true);
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

