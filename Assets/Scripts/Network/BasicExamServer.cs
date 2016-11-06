using UnityEngine;
using System;
using System.Linq;
using System.Timers;

public class BasicExamServer : ExtendedMonoBehaviour
{
    public EventHandler OnGameOver = delegate
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

    MainPlayerDataSynchronizer mainPlayerDataSynchronizer;

    int remainingTimeToAnswerMainQuestion = -1;
    // when using joker
    bool paused = false;

    void Awake()
    {
        LoadServerSettings();

        InitializeMainPlayerData();
        InitializeCommands();

        AttachEventHandlers();

        MainPlayerData.JokersData.AddJoker(typeof(HelpFromFriendJoker));
        MainPlayerData.JokersData.AddJoker(typeof(AskAudienceJoker));
        MainPlayerData.JokersData.AddJoker(typeof(DisableRandomAnswersJoker));
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

    void LoadServerSettings()
    {
        var serverMaxPlayers = PlayerPrefsEncryptionUtils.GetString("ServerMaxPlayers");

        if (!string.IsNullOrEmpty(serverMaxPlayers))
        {
            NetworkManager.MaxConnections = int.Parse(serverMaxPlayers);    
        }
    }

    void AttachEventHandlers()
    {
        MainPlayerData.OnDisconnected += OnMainPlayerDisconnected;
        GameData.OnLoaded += (sender, args) =>
        {
            var gameDataLoadedCommand = new NetworkCommandData("LoadedGameData");
            NetworkManager.SendAllClientsCommand(gameDataLoadedCommand);
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
        HelpFromFriendJokerRouter.OnSentAnswerToMainPlayer += (sender, args) => paused = false;
        AskAudienceJokerRouter.OnSentAnswerToMainPlayer += (sender, args) => paused = false;

        ConnectedClientsUIController.OnSelectedPlayer += (sender, args) =>
        {
            ClientOptionsUIController.gameObject.SetActive(true);
            CoroutineUtils.WaitForFrames(0, () =>
                {
                    var username = NetworkManager.GetClientUsername(args.ConnectionId);
                    var clientData = new ConnectedClientData(args.ConnectionId, username);
                    var role = 
                        (MainPlayerData.IsConnected && MainPlayerData.ConnectionId == args.ConnectionId) 
                            ? 
                            BasicExamClientRole.MainPlayer 
                            : 
                            BasicExamClientRole.Audience;
                    
                    ClientOptionsUIController.Set(clientData, role);
                });
        };
    }

    void InitializeMainPlayerData()
    {
        MainPlayerData = new MainPlayerData(NetworkManager);
        mainPlayerDataSynchronizer = new MainPlayerDataSynchronizer(NetworkManager, MainPlayerData);
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
        NetworkManager.CommandsManager.AddCommand("SelectedFifthyFifthyChanceJoker", selectedFifthyFifthyChanceCommand);
        NetworkManager.CommandsManager.AddCommand("Surrender", surrenderCommand);
    }

    void OnMainPlayerSelectedJoker(object sender, EventArgs args)
    {
        var jokerName = sender.GetType().Name
                .Replace("ReceivedSelected", "")
                .Replace("Command", "")
                .ToUpperInvariant();
        
        if (jokerName == ("DisableRandomAnswersJoker").ToUpperInvariant())
        {
            return;
        }

        paused = true;
    }

    void Cleanup()
    {
        mainPlayerDataSynchronizer = null;
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

    void OnMainPlayerSurrender()
    {
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

                EndGame();
            }, 
            Debug.LogException);
    }

    void SendEndGameInfo()
    {
        var playersScores = LeaderboardSerializer.Leaderboard.Select(ps => new PlayerScore_Serializable(ps))
            .ToArray();
        var leaderboardData = new LeaderboardData_Serializable() { PlayerScore = playersScores };
        var leaderboardDataJSON = JsonUtility.ToJson(leaderboardData);
        var commandData = new NetworkCommandData("BasicExamGameEnd");

        commandData.AddOption("Mark", GameData.CurrentMark.ToString());
        commandData.AddOption("LeaderboardDataJSON", leaderboardDataJSON);

        NetworkManager.SendAllClientsCommand(commandData);
    }

    void SavePlayerScoreToLeaderboard()
    {
        var currentMark = GameData.CurrentMark;
        var mainPlayerName = MainPlayerData.Username;
        var playerScore = new PlayerScore(mainPlayerName, currentMark, DateTime.Now);

        LeaderboardSerializer.SetPlayerScore(playerScore);
    }

    public void EndGame()
    {
        SendEndGameInfo();
        SavePlayerScoreToLeaderboard();
        IsGameOver = true;
        OnGameOver(this, EventArgs.Empty);
        Cleanup();
    }
}