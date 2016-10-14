using UnityEngine;
using System.Collections.Generic;
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

    MainPlayerData mainPlayerData;
    MainPlayerDataSynchronizer mainPlayerDataSynchronizer;

    int remainingTimeToAnswer;
    // when using joker
    bool paused = false;

    void Start()
    {
        LoadServerSettings();

        InitializeCommands();
        InitializeMainPlayerData();

        AttachEventHandlers();

        GameData.LoadDataAsync();
        LeaderboardSerializer.LoadDataAsync();

        mainPlayerData.JokersData.AddJoker(typeof(HelpFromFriendJoker));
        mainPlayerData.JokersData.AddJoker(typeof(AskAudienceJoker));
        mainPlayerData.JokersData.AddJoker(typeof(DisableRandomAnswersJoker));

        CoroutineUtils.RepeatEverySeconds(1, () =>
            {
                if (!NetworkManager.IsRunning || IsGameOver || paused)
                {
                    return;
                }

                UpdateRemainingTime();
            });

        CoroutineUtils.WaitUntil(() => GameData.Loaded, () => remainingTimeToAnswer = GameData.SecondsForAnswerQuestion);
    }

    void LoadServerSettings()
    {
        var levelCategory = PlayerPrefsEncryptionUtils.GetString("ServerLevelCategory");
        var serverMaxPlayers = PlayerPrefsEncryptionUtils.GetString("ServerMaxPlayers");

        if (string.IsNullOrEmpty(levelCategory))
        {
            throw new Exception("Cant load server level category");
        }

        if (string.IsNullOrEmpty(serverMaxPlayers))
        {
            throw new Exception("Cant load server max players");
        }

        GameData.LevelCategory = levelCategory;
        LeaderboardSerializer.LevelCategory = levelCategory;

        NetworkManager.MaxConnections = int.Parse(serverMaxPlayers);
    }

    void AttachEventHandlers()
    {
        mainPlayerData.OnDisconnected += OnMainPlayerDisconnected;
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
        var selectedHelpFromFriendJokerCommand = new ReceivedSelectedHelpFromFriendJokerCommand(NetworkManager, mainPlayerData, HelpFromFriendJokerRouter, 10);
        var selectedAskAudienceJokerCommand = new ReceivedSelectedAskAudienceJokerCommand(mainPlayerData, AskAudienceJokerRouter, NetworkManager, 10);
        var selectedFifthyFifthyChanceCommand = new ReceivedSelectedFifthyFifthyChanceJokerCommand(mainPlayerData, DisableRandomAnswersJokerRouter, NetworkManager, 4);
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

