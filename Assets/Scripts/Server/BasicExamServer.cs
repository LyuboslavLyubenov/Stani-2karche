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

    MainPlayerData mainPlayerData;
    MainPlayerDataSynchronizer mainPlayerDataSynchronizer;

    float remainingTimeToAnswer;
    // Use this for initialization
    void Start()
    {
        mainPlayerData = new MainPlayerData(NetworkManager);
        mainPlayerDataSynchronizer = new MainPlayerDataSynchronizer(NetworkManager, mainPlayerData);

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

        NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ReceivedServerAnswerSelectedCommand(OnReceivedSelectedAnswer));
        NetworkManager.CommandsManager.AddCommand("SelectedHelpFromFriendJoker", new ReceivedSelectedHelpFromFriendJokerCommand(NetworkManager, mainPlayerData, HelpFromFriendJokerRouter, 10));
        NetworkManager.CommandsManager.AddCommand("SelectedAskAudienceJoker", new ReceivedSelectedAskAudienceJokerCommand(mainPlayerData, AskAudienceJokerRouter, NetworkManager, 10));
        NetworkManager.CommandsManager.AddCommand("Surrender", new ReceivedSurrenderBasicExamOneTimeCommand(mainPlayerData, OnMainPlayerSurrender));

        GameData.LoadDataAsync();

        LeaderboardSerializer.LevelCategory = GameData.LevelCategory;
        LeaderboardSerializer.LoadDataAsync();

        mainPlayerData.JokersData.AddJoker(typeof(HelpFromFriendJoker));
        mainPlayerData.JokersData.AddJoker(typeof(AskAudienceJoker));

        CoroutineUtils.WaitUntil(() => GameData.Loaded, () => remainingTimeToAnswer = GameData.SecondsForAnswerQuestion);
    }

    void Cleanup()
    {
        mainPlayerDataSynchronizer = null;
        NetworkManager.CommandsManager.RemoveCommand("SelectedHelpFromFriendJoker");
        NetworkManager.CommandsManager.RemoveCommand("SelectedAskAudienceJoker");
        NetworkManager.CommandsManager.RemoveCommand("Surrender");
    }

    void Update()
    {
        if (!NetworkManager.IsRunning || IsGameOver)
        {
            return;
        }

        UpdateRemainingTime();
    }

    void UpdateRemainingTime()
    {
        if (!mainPlayerData.IsConnected)
        {
            return;
        }

        remainingTimeToAnswer -= Time.deltaTime;

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

