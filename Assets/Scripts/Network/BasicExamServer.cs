using UnityEngine;
using System;

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

    public AskPlayerQuestionRouter AskPlayerQuestionRouter;
    public AudienceAnswerPollRouter AudiencePollRouter;
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
    bool playerSentAnswer = false;

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
                playerSentAnswer = false;
            }
        };
        
        NetworkManager.OnClientConnected += (sender, args) =>
        {
            if (IsGameOver)
            {
                SendEndGameInfo();
            }
        };
        
        AskPlayerQuestionRouter.OnFinished += (sender, args) => paused = false;
        AudiencePollRouter.OnSentResult += (sender, args) => paused = false;
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
        var selectedAskPlayerQuestionCommand = new SelectedAskPlayerQuestionCommand(NetworkManager, MainPlayerData, AskPlayerQuestionRouter, 60);
        var selectedAudiencePollCommand = new SelectedAudiencePollCommand(MainPlayerData, AudiencePollRouter, NetworkManager, 60);
        var selectedFifthyFifthyChanceCommand = new SelectedDisableRandomAnswersJokerCommand(MainPlayerData, DisableRandomAnswersJokerRouter, NetworkManager, 2);
        var surrenderCommand = new SurrenderBasicExamOneTimeCommand(MainPlayerData, OnMainPlayerSurrender);
        var selectedJokerCommands = new INetworkOperationExecutedCallback[] { selectedAudiencePollCommand, selectedFifthyFifthyChanceCommand, selectedAskPlayerQuestionCommand };

        for (var i = 0; i < selectedJokerCommands.Length; i++)
        {
            selectedJokerCommands[i].OnExecuted += OnMainPlayerSelectedJoker;
        }

        NetworkManager.CommandsManager.AddCommand("AnswerSelected", selectedAnswerCommand);
        NetworkManager.CommandsManager.AddCommand(selectedAskPlayerQuestionCommand);
        NetworkManager.CommandsManager.AddCommand(selectedAudiencePollCommand);
        NetworkManager.CommandsManager.AddCommand(selectedFifthyFifthyChanceCommand);
        NetworkManager.CommandsManager.AddCommand("Surrender", surrenderCommand);
    }

    void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
    {
        if (args.QuestionType == QuestionRequestType.Next)
        {
            if (args.ClientId != MainPlayerData.ConnectionId)
            {
                throw new Exception("Client id " + args.ClientId + " doesnt have premission to get next question.");    
            }

            if (!playerSentAnswer)
            {
                throw new Exception("MainPlayer must answer current question before requesting new one.");
            }
        }
    }

    void OnMainPlayerSelectedJoker(object sender, EventArgs args)
    {
        var jokerName = sender.GetType().Name
                .Replace("Selected", "")
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
        NetworkManager.CommandsManager.RemoveCommand("AnswerSelected");
        NetworkManager.CommandsManager.RemoveCommand<SelectedAskPlayerQuestionCommand>();
        NetworkManager.CommandsManager.RemoveCommand<SelectedAudiencePollCommand>();
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
        AskPlayerQuestionRouter.Deactivate();
        AudiencePollRouter.Deactivate();
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

                CoroutineUtils.WaitForFrames(1, EndGame);
            }, 
            Debug.LogException);

        playerSentAnswer = true;
    }

    void SendEndGameInfo()
    {
        var commandData = NetworkCommandData.From<BasicExamGameEndCommand>();
        commandData.AddOption("Mark", GameData.CurrentMark.ToString());
        NetworkManager.SendAllClientsCommand(commandData);
    }

    void SavePlayerScoreToLeaderboard()
    {
        var mainPlayerName = MainPlayerData.Username;
        var playerScore = new PlayerScore(mainPlayerName, StatisticsCollector.PlayerScore, DateTime.Now);

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