using UnityEngine;
using System;
using System.Linq;

public class BasicExamServer : ExtendedMonoBehaviour
{
    const int DefaultServerMaxPlayers = 40;

    public EventHandler OnGameOver = delegate
    {
    };

    public EventHandler<AnswerEventArgs> OnMainPlayerSelectedAnswer = delegate
    {
    };

    public ServerNetworkManager NetworkManager;

    public LocalGameData GameData;
    public GameDataSender GameDataSender;

    public LeaderboardSerializer LeaderboardSerializer;

    public HelpFromFriendJokerRouter HelpFromFriendJokerRouter;
    public AskAudienceJokerRouter AskAudienceJokerRouter;
    public DisableRandomAnswersJokerRouter DisableRandomAnswersJokerRouter;

    public ConnectedClientsUIController ConnectedClientsUIController;
    public BasicExamClientOptionsUIController ClientOptionsUIController;

    public BasicExamStatisticsCollector StatisticsCollector;

    public int RemainingTimetoAnswerInSeconds
    {
        get
        {
            return remainingTimeToAnswerMainQuestion;
        }
    }

    public bool IsGameOver
    {
        get;
        private set;
    }

    public MainPlayerData MainPlayerData
    {
        get;
        private set;
    }

    MainPlayerJokersDataSynchronizer mainPlayerJokersDataSynchronizer;

    int remainingTimeToAnswerMainQuestion = -1;
    // when using joker
    bool paused = false;

    void Awake()
    {
        LoadServerSettings();

        InitializeMainPlayerData();
        InitializeCommands();

        AttachEventHandlers();

        MainPlayerData.JokersData.AddJoker<HelpFromFriendJoker>();
        MainPlayerData.JokersData.AddJoker<AskAudienceJoker>();
        MainPlayerData.JokersData.AddJoker<DisableRandomAnswersJoker>();

    }

    void Start()
    {
        CoroutineUtils.RepeatEverySeconds(1, () =>
            {
                if (!NetworkManager.IsRunning || IsGameOver || paused || remainingTimeToAnswerMainQuestion == -1)
                {
                    return;
                }

                UpdateRemainingTime();
            });
    }

    void OnApplicationQuit()
    {
        EndGame();
    }

    void LoadServerSettings()
    {
        int serverMaxPlayers = DefaultServerMaxPlayers;

        if (PlayerPrefsEncryptionUtils.HasKey("ServerMaxPlayers"))
        {
            serverMaxPlayers = int.Parse(PlayerPrefsEncryptionUtils.GetString("ServerMaxPlayers"));    
        }

        NetworkManager.MaxConnections = serverMaxPlayers;    
    }

    void AttachEventHandlers()
    {
        MainPlayerData.OnDisconnected += OnMainPlayerDisconnected;
        GameData.OnLoaded += (sender, args) =>
        {
            remainingTimeToAnswerMainQuestion = GameData.SecondsForAnswerQuestion;  
        };
        
        GameDataSender.OnSentQuestion += (sender, args) =>
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                remainingTimeToAnswerMainQuestion = GameData.SecondsForAnswerQuestion;
            }
        };
        
        NetworkManager.OnClientConnected += (sender, args) =>
        {
            if (IsGameOver)
            {
                SendEndGameInfo();
            }
        };
        
        HelpFromFriendJokerRouter.OnFinished += (sender, args) => paused = false;
        AskAudienceJokerRouter.OnFinished += (sender, args) => paused = false;
        ConnectedClientsUIController.OnSelectedPlayer += (sender, args) => OnClientSelected(args.ConnectionId);
        GameDataSender.OnBeforeSend += OnBeforeSendQuestion;
    }

    void InitializeMainPlayerData()
    {
        MainPlayerData = new MainPlayerData(NetworkManager);
        mainPlayerJokersDataSynchronizer = new MainPlayerJokersDataSynchronizer(NetworkManager, MainPlayerData);
    }

    void InitializeCommands()
    {
        var selectedAnswerCommand = new ReceivedServerSelectedAnswerCommand(OnReceivedSelectedAnswer);
        var selectedHelpFromFriendJokerCommand = new ReceivedSelectedHelpFromFriendJokerCommand(NetworkManager, MainPlayerData, HelpFromFriendJokerRouter, 60);
        var selectedAskAudienceJokerCommand = new ReceivedSelectedAskAudienceJokerCommand(MainPlayerData, AskAudienceJokerRouter, NetworkManager, 60);
        var selectedFifthyFifthyChanceCommand = new ReceivedSelectedDisableRandomAnswersJokerCommand(MainPlayerData, DisableRandomAnswersJokerRouter, NetworkManager, 2);
        var surrenderCommand = new ReceivedSurrenderBasicExamOneTimeCommand(MainPlayerData, OnMainPlayerSurrender);
        var selectedJokerCommands = new INetworkOperationExecutedCallback[] { selectedAskAudienceJokerCommand, selectedFifthyFifthyChanceCommand, selectedHelpFromFriendJokerCommand };

        for (var i = 0; i < selectedJokerCommands.Length; i++)
        {
            selectedJokerCommands[i].OnExecuted += OnMainPlayerSelectedJoker;
        }

        NetworkManager.CommandsManager.AddCommand("AnswerSelected", selectedAnswerCommand);
        NetworkManager.CommandsManager.AddCommand("SelectedHelpFromFriendJoker", selectedHelpFromFriendJokerCommand);
        NetworkManager.CommandsManager.AddCommand("SelectedAskAudienceJoker", selectedAskAudienceJokerCommand);
        NetworkManager.CommandsManager.AddCommand("SelectedDisableRandomAnswersJoker", selectedFifthyFifthyChanceCommand);
        NetworkManager.CommandsManager.AddCommand("Surrender", surrenderCommand);
    }

    void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
    {
        if (args.QuestionType == QuestionRequestType.Next &&
            args.ClientId != MainPlayerData.ConnectionId)
        {
            throw new Exception("Client id " + args.ClientId + " doesnt have premission to get next question.");
        }
    }

    void OnMainPlayerSelectedJoker(object sender, EventArgs args)
    {
        var jokerName = sender.GetType().Name
                .Replace("ReceivedSelected", "")
                .Replace("Command", "")
                .ToUpperInvariant();
        
        if (jokerName == typeof(DisableRandomAnswersJoker).Name.ToUpperInvariant())
        {
            return;
        }

        paused = true;
    }

    void Cleanup()
    {
        mainPlayerJokersDataSynchronizer = null;
        NetworkManager.CommandsManager.RemoveCommand("SelectedHelpFromFriendJoker");
        NetworkManager.CommandsManager.RemoveCommand("SelectedAskAudienceJoker");
        NetworkManager.CommandsManager.RemoveCommand("Surrender");
    }

    void UpdateRemainingTime()
    {
        if (!MainPlayerData.IsConnected || paused)
        {
            return;
        }

        remainingTimeToAnswerMainQuestion -= 1;

        if (remainingTimeToAnswerMainQuestion <= 0)
        {
            EndGame();
        }
    }

    void OnClientSelected(int connectionId)
    {
        ClientOptionsUIController.gameObject.SetActive(true);
        CoroutineUtils.WaitForFrames(0, () =>
            {
                var username = NetworkManager.GetClientUsername(connectionId);
                var clientData = new ConnectedClientData(connectionId, username);
                var role = 
                    (MainPlayerData.IsConnected && MainPlayerData.ConnectionId == connectionId) 
                    ? 
                    BasicExamClientRole.MainPlayer 
                    : 
                    BasicExamClientRole.Audience;

                ClientOptionsUIController.Set(clientData, role);
            });
    }

    void OnMainPlayerSurrender()
    {
        PlayerPrefs.SetString("Surrender", "true");
        EndGame();
    }

    void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
    {
        HelpFromFriendJokerRouter.Deactivate();
        AskAudienceJokerRouter.Deactivate();
    }

    void OnReceivedSelectedAnswer(int clientId, string answer)
    {
        if (IsGameOver || !MainPlayerData.IsConnected || clientId != MainPlayerData.ConnectionId || paused)
        {
            return;
        }

        GameData.GetCurrentQuestion((question) =>
            {
                var isCorrect = (answer == question.Answers[question.CorrectAnswerIndex]);

                if (isCorrect)
                {
                    return;
                }

                CoroutineUtils.WaitForFrames(1, () => EndGame());
            }, 
            Debug.LogException);
    }

    void SendEndGameInfo()
    {
        var commandData = new NetworkCommandData("BasicExamGameEnd");
        commandData.AddOption("Mark", GameData.CurrentMark.ToString());
        NetworkManager.SendAllClientsCommand(commandData);
    }

    int CalculateScore()
    {
        var correctAnsweredQuestionsCount = StatisticsCollector.CorrectAnsweredQuestions.Count;
        var totalTimeSpentThinking = StatisticsCollector.QuestionsSpentTime.Values.ToList().Sum();
        var avgSpentTimeThinking = totalTimeSpentThinking / StatisticsCollector.QuestionsSpentTime.Values.Count;
        var score = (correctAnsweredQuestionsCount * 10000) / avgSpentTimeThinking;
        return score;
    }

    void SavePlayerScoreToLeaderboard()
    {
        var mainPlayerName = MainPlayerData.Username;
        var playerScore = new PlayerScore(mainPlayerName, CalculateScore(), DateTime.Now);

        LeaderboardSerializer.SavePlayerScore(playerScore);
    }

    void ExportStatistics()
    {
        new BasicExamGeneralStatiticsExporter(StatisticsCollector).Export();
        new BasicExamGameDataStatisticsExporter(StatisticsCollector, GameData).Export();
    }

    public void EndGame()
    {
        IsGameOver = true;
        OnGameOver(this, EventArgs.Empty);

        SavePlayerScoreToLeaderboard();
        SendEndGameInfo();
        ExportStatistics();
        Cleanup();
    }
}