using UnityEngine;
using System;
using System.Linq;

public class BasicExamServer : ExtendedMonoBehaviour
{
    public bool IsGameOver
    {
        get;
        private set;
    }

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

    public MainPlayerData MainPlayerData
    {
        get
        {
            return mainPlayerData;
        }
    }

    MainPlayerData mainPlayerData;
    MainPlayerDataSynchronizer mainPlayerDataSynchronizer;

    int remainingTimeToAnswer = -1;
    // when using joker
    bool paused = false;

    void Start()
    {
        LoadServerSettings();

        InitializeMainPlayerData();
        InitializeCommands();

        AttachEventHandlers();

        GameData.LoadDataAsync();
        LeaderboardSerializer.LoadDataAsync();

        mainPlayerData.JokersData.AddJoker(typeof(HelpFromFriendJoker));
        mainPlayerData.JokersData.AddJoker(typeof(AskAudienceJoker));
        mainPlayerData.JokersData.AddJoker(typeof(DisableRandomAnswersJoker));

        CoroutineUtils.RepeatEverySeconds(1, () =>
            {
                if (!NetworkManager.IsRunning || IsGameOver || paused || remainingTimeToAnswer == -1)
                {
                    return;
                }

                UpdateRemainingTime();
            });
    }

    void LoadServerSettings()
    {
        var levelCategory = PlayerPrefsEncryptionUtils.GetString("ServerLevelCategory");
        var serverMaxPlayers = PlayerPrefsEncryptionUtils.GetString("ServerMaxPlayers");

        if (!string.IsNullOrEmpty(levelCategory))
        {
            GameData.LevelCategory = levelCategory;    
            LeaderboardSerializer.LevelCategory = levelCategory;
        }

        if (!string.IsNullOrEmpty(serverMaxPlayers))
        {
            NetworkManager.MaxConnections = int.Parse(serverMaxPlayers);    
        }
    }

    void AttachEventHandlers()
    {
        mainPlayerData.OnDisconnected += OnMainPlayerDisconnected;
        GameData.OnLoaded += (sender, args) =>
        {
            remainingTimeToAnswer = GameData.SecondsForAnswerQuestion;  
        };
        GameDataSender.OnSentQuestion += (sender, args) =>
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                remainingTimeToAnswer = GameData.SecondsForAnswerQuestion;
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
    }

    void InitializeMainPlayerData()
    {
        mainPlayerData = new MainPlayerData(NetworkManager);
        mainPlayerDataSynchronizer = new MainPlayerDataSynchronizer(NetworkManager, mainPlayerData);
    }

    void InitializeCommands()
    {
        var selectedAnswerCommand = new ReceivedServerSelectedAnswerCommand(OnReceivedSelectedAnswer);
        var selectedHelpFromFriendJokerCommand = new ReceivedSelectedHelpFromFriendJokerCommand(NetworkManager, mainPlayerData, HelpFromFriendJokerRouter, 60);
        var selectedAskAudienceJokerCommand = new ReceivedSelectedAskAudienceJokerCommand(mainPlayerData, AskAudienceJokerRouter, NetworkManager, 60);
        var selectedFifthyFifthyChanceCommand = new ReceivedSelectedFifthyFifthyChanceJokerCommand(mainPlayerData, DisableRandomAnswersJokerRouter, NetworkManager, 2);
        var surrenderCommand = new ReceivedSurrenderBasicExamOneTimeCommand(mainPlayerData, OnMainPlayerSurrender);
        var selectedJokerCommands = new ICommandExecutedCallback[] { selectedAskAudienceJokerCommand, selectedFifthyFifthyChanceCommand, selectedHelpFromFriendJokerCommand };

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
                .ToLowerInvariant();

        if (jokerName == ("FifthyFifthyChanceJoker").ToLower())
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
        if (!mainPlayerData.IsConnected)
        {
            return;
        }

        remainingTimeToAnswer -= 1;

        if (remainingTimeToAnswer <= 0)
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
        if (IsGameOver || !mainPlayerData.IsConnected || clientId != mainPlayerData.ConnectionId)
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

    void EndGame()
    {
        SendEndGameInfo();
        SavePlayerScoreToLeaderboard();
        IsGameOver = true;
        OnGameOver(this, EventArgs.Empty);
        Cleanup();
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
        var mainPlayerName = mainPlayerData.Username;
        var playerScore = new PlayerScore(mainPlayerName, currentMark, DateTime.Now);

        LeaderboardSerializer.SetPlayerScore(playerScore);
    }
}

