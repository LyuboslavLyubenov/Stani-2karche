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
    public GameObject MarkChangedConfetti;

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

    public SelectRandomJokerUIController SelectRandomJokerUIController;

    IGameData gameData;

    UnableToConnectUIController unableToConnectUIController;

    void Start()
    {
        PlayerPrefs.DeleteKey("LoadedGameData");

        gameData = new RemoteGameData(NetworkManager);

        unableToConnectUIController = UnableToConnectUI.GetComponent<UnableToConnectUIController>();

        InitializeCommands();
        AttachEventHandlers();
        StartServerIfPlayerIsHost();

        if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
        {
            PlayerPrefsEncryptionUtils.SetString("ServerLocalIP", "127.0.0.1");
            //wait until server is loaded. starting the server takes about ~7 seconds on i7 + SSD.
            CoroutineUtils.WaitForSeconds(9, ConnectToServer);
        }
        else
        {
            ConnectToServer();
        }

        LoadingUI.SetActive(true);
    }

    void OnFoundServerIP(string ip)
    {
        try
        {
            NetworkManager.ConnectToHost(ip);
            unableToConnectUIController.ServerIP = ip;
        }
        catch
        {
            OnFoundServerIPError();
        }
    }

    void OnFoundServerIPError()
    {
        CoroutineUtils.WaitForSeconds(1f, ConnectToServer);
    }

    void ConnectToServer()
    {
        NetworkManagerUtils.Instance.GetServerIp(OnFoundServerIP, OnFoundServerIPError);
    }

    void InitializeCommands()
    {
        NetworkManager.CommandsManager.AddCommand(new BasicExamGameEndCommand(EndGameUI, LeaderboardUI));
        NetworkManager.CommandsManager.AddCommand(new AddHelpFromFriendJokerCommand(AvailableJokersUIController, NetworkManager, CallAFriendUI, FriendAnswerUI, WaitingToAnswerUI, LoadingUI));
        NetworkManager.CommandsManager.AddCommand(new AddAskAudienceJokerCommand(AvailableJokersUIController, NetworkManager, WaitingToAnswerUI, AudienceAnswerUI, LoadingUI, NotificationService));
        NetworkManager.CommandsManager.AddCommand(new AddDisableRandomAnswersJokerCommand(AvailableJokersUIController, NetworkManager, gameData, QuestionUIController));
        NetworkManager.CommandsManager.AddCommand(new AddRandomJokerCommand(SelectRandomJokerUIController, NetworkManager));
    }

    void OnLoadedGameData(object sender, EventArgs args)
    {
        LoadingUI.SetActive(false);
        ChooseCategoryUIController.gameObject.SetActive(false);
        QuestionUIController.HideAllAnswers();
        SecondsRemainingUIController.Paused = false;
        gameData.GetCurrentQuestion(QuestionUIController.LoadQuestion, Debug.LogException);

        PlayerPrefs.SetString("LoadedGameData", "true");
    }

    void AttachEventHandlers()
    {
        NetworkManager.OnConnectedEvent += OnConnectedToServer;
        NetworkManager.OnDisconnectedEvent += OnDisconnectedFromServer;

        QuestionUIController.OnAnswerClick += OnAnswerClick;
        QuestionUIController.OnQuestionLoaded += OnQuestionLoaded;

        gameData.OnMarkIncrease += OnMarkIncrease;

        ChooseCategoryUIController.OnLoadedCategories += (sender, args) => LoadingUI.SetActive(false);
        ChooseCategoryUIController.OnChoosedCategory += OnChoosedCategory;

        unableToConnectUIController.OnTryingAgainToConnectToServer += (s, a) => LoadingUI.SetActive(true);

        AvailableJokersUIController.OnAddedJoker += OnAddedJoker;
        AvailableJokersUIController.OnUsedJoker += OnUsedJoker;

        gameData.OnLoaded += OnLoadedGameData;
    }

    void OnMarkIncrease(object sender, MarkEventArgs args)
    {
        MarkChangedConfetti.SetActive(true);
        MarkPanelController.SetMark(args.Mark.ToString());
    }

    void OnChoosedCategory(object sender, ChoosedCategoryEventArgs args)
    {
        var selectedCategoryCommand = new NetworkCommandData("SelectedCategory");
        selectedCategoryCommand.AddOption("Category", args.Name);
        NetworkManager.SendServerCommand(selectedCategoryCommand);
    }

    void OnAddedJoker(object sender, JokerEventArgs args)
    {
        //TODO REFACTOR
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

        if (jokerTypeNameUpper == typeof(DisableRandomAnswersJoker).Name.ToUpperInvariant())
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

    void OnDisconnectedFromServer(object sender, EventArgs args)
    {
        ChooseCategoryUIController.gameObject.SetActive(false);
        LoadingUI.SetActive(false);
        SecondsRemainingUIController.Paused = true;
        UnableToConnectUI.SetActive(true);   
    }

    void OnConnectedToServer(object sender, EventArgs args)
    {
        AvailableJokersUIController.ClearAll();

        LoadingUI.SetActive(false);
        UnableToConnectUI.SetActive(false);   
        ChooseCategoryUIController.gameObject.SetActive(false);

        var commandData = NetworkCommandData.From<MainPlayerConnectingCommand>();
        NetworkManager.SendServerCommand(commandData);

        if (PlayerPrefs.HasKey("LoadedGameData"))
        {
            var loadedGameData = new NetworkCommandData("LoadedGameData");
            loadedGameData.AddOption("LevelCategory", gameData.LevelCategory);
            NetworkManager.CommandsManager.Execute(loadedGameData);
            return;
        }

        ChooseCategoryUIController.gameObject.SetActive(true);
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        StartCoroutine(OnAnswerClickCoroutine(args.Answer, args.IsCorrect));
    }

    IEnumerator OnAnswerClickCoroutine(string answer, bool isCorrect)
    {
        var commandData = new NetworkCommandData("AnswerSelected");
        commandData.AddOption("Answer", answer);

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

    void ShowNotification(Color color, string message)
    {
        if (NotificationService != null)
        {
            NotificationService.AddNotification(color, message);
        }
    }

    void StartLoadingCategories()
    {
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

    void StartServerIfPlayerIsHost()
    {
        if (PlayerPrefsEncryptionUtils.HasKey("MainPlayerHost"))
        {
            var serverPath = string.Format("Server\\{0}.exe", ServerBinaryName);
            System.Diagnostics.Process.Start(serverPath);
            SceneManager.activeSceneChanged += OnActivateSceneChanged;
        }
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
}