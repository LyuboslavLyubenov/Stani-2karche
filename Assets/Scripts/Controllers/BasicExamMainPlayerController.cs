using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class BasicExamMainPlayerController : ExtendedMonoBehaviour
{
    const string ServerBinaryName = "stani2karcheserver";

    public GameObject LeaderboardUI;
    public GameObject LoadingUI;
    public GameObject EndGameUI;
    public GameObject CallAFriendUI;
    public GameObject FriendAnswerUI;
    public GameObject WaitingToAnswerUI;
    public GameObject AudienceAnswerUI;
    public GameObject UnableToConnectUI;
    public GameObject ChooseCategoryUI;

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

    IGameData gameData;

    UnableToConnectUIController unableToConnectUIController;

    void Start()
    {
        PlayerPrefs.DeleteKey("LoadedGameData");

        gameData = new RemoteGameData(NetworkManager);

        unableToConnectUIController = UnableToConnectUI.GetComponent<UnableToConnectUIController>();

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
            LoadingUI.SetActive(false);
            ChooseCategoryUIController.gameObject.SetActive(false);
            gameData.GetCurrentQuestion(QuestionUIController.LoadQuestion, Debug.LogException);
            
            PlayerPrefs.SetString("LoadedGameData", "true");
        };

        NetworkManager.CommandsManager.AddCommand("BasicExamGameEnd", new ReceivedBasicExamGameEndCommand(EndGameUI, LeaderboardUI));
        NetworkManager.CommandsManager.AddCommand("AddHelpFromFriendJoker", new ReceivedAddHelpFromFriendJokerCommand(AvailableJokersUIController, NetworkManager, CallAFriendUI, FriendAnswerUI, WaitingToAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand("AddAskAudienceJoker", new ReceivedAddAskAudienceJokerCommand(AvailableJokersUIController, NetworkManager, WaitingToAnswerUI, AudienceAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand("AddDisableRandomAnswersJoker", new ReceivedAddDisableRandomAnswersJokerCommand(AvailableJokersUIController, NetworkManager, gameData, QuestionUIController));
        NetworkManager.CommandsManager.AddCommand("LoadedGameData", loadedGameDataCommand);
    }

    void StartServerIfPlayerIsHost()
    {
        if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
        {
            var serverPath = string.Format("Server\\{0}.exe", ServerBinaryName);
            System.Diagnostics.Process.Start(serverPath);
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
            ChooseCategoryUIController.gameObject.SetActive(false);
            LoadingUI.SetActive(false);
            UnableToConnectUI.SetActive(true);   
        };

        QuestionUIController.OnAnswerClick += OnAnswerClick;
        QuestionUIController.OnQuestionLoaded += OnQuestionLoaded;

        gameData.OnMarkIncrease += (sender, args) => MarkPanelController.SetMark(args.Mark.ToString());

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

        AvailableJokersUIController.OnAddedJoker += OnAddedJoker;
        AvailableJokersUIController.OnUsedJoker += OnUsedJoker;
    }

    void OnAddedJoker(object sender, JokerEventArgs args)
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
    }

    void OnUsedJoker(object sender, JokerEventArgs args)
    {
        var jokerTypeNameUpper = args.Joker.GetType().Name.ToUpperInvariant();

        if (jokerTypeNameUpper == ("DisableRandomAnswersJoker").ToUpperInvariant())
        {
            return;
        }

        SecondsRemainingUIController.Paused = true;  
    }

    void OnQuestionLoaded(object sender, SimpleQuestionEventArgs args)
    {
        QuestionsRemainingUIController.SetRemainingQuestions(gameData.RemainingQuestionsToNextMark);
        SecondsRemainingUIController.SetSeconds(gameData.SecondsForAnswerQuestion);
    }

    void ConnectToServer()
    {
        CoroutineUtils.WaitForSeconds(8f, () =>
            {
                var ip = PlayerPrefsEncryptionUtils.GetString("ServerIP");
                NetworkManager.ConnectToHost(ip);
            });
    }

    void OnActivateSceneChanged(Scene oldScene, Scene newScene)
    {
        KillLocalServer();
        CleanUp();
        SceneManager.activeSceneChanged -= OnActivateSceneChanged;
    }

    void OnApplicationQuit()
    {
        KillLocalServer();
        CleanUp();
    }

    void CleanUp()
    {
        PlayerPrefs.DeleteKey("LoadedGameData");
        PlayerPrefsEncryptionUtils.DeleteKey("MainPlayerHost");
    }

    void KillLocalServer()
    {
        if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
        {
            var serverProcesses = System.Diagnostics.Process.GetProcessesByName(ServerBinaryName);

            for (int i = 0; i < serverProcesses.Length; i++)
            {
                serverProcesses[i].Kill();
            }
        }
    }

    void OnConnectedToServer(object sender, EventArgs args)
    {
        AvailableJokersUIController.ClearAll();
        LoadingUI.SetActive(false);

        var commandData = new NetworkCommandData("MainPlayerConnecting");
        NetworkManager.SendServerCommand(commandData);

        if (PlayerPrefs.HasKey("LoadedGameData"))
        {
            var loadedGameData = new NetworkCommandData("LoadedGameData");
            NetworkManager.CommandsManager.Execute(loadedGameData);
            return;
        }

        var remoteCategoriesReader = new RemoteAvailableCategoriesReader(NetworkManager, () =>
            {
                var errorMsg = LanguagesManager.Instance.GetValue("Errors/CantLoadCategories");
                Debug.LogError(errorMsg);
                ShowNotification(Color.red, errorMsg);
               
                ChooseCategoryUI.SetActive(false);
                NetworkManager.Disconnect();
            }, 10);

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
            gameData.GetNextQuestion(QuestionUIController.LoadQuestion, Debug.LogException);
        }
        else
        {
            AvailableJokersUIController.ClearAll();
        }
    }
}