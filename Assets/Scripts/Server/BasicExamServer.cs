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
    public LeaderboardSerializer LeaderboardSerializer;
    public HelpFromFriendJokerRouter HelpFromFriendJokerRouter;
    public AskAudienceJokerRouter AskAudienceJokerRouter;

    MainPlayerData mainPlayerData;
    MainPlayerDataSynchronizer mainPlayerDataSynchronizer;

    // Use this for initialization
    void Start()
    {
        mainPlayerData = new MainPlayerData(NetworkManager);
        mainPlayerDataSynchronizer = new MainPlayerDataSynchronizer(NetworkManager, mainPlayerData);

        mainPlayerData.OnDisconnected += OnMainPlayerDisconnected;

        NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ReceivedServerAnswerSelectedCommand(OnReceivedSelectedAnswer));
        NetworkManager.CommandsManager.AddCommand("SelectedHelpFromFriendJoker", new ReceivedSelectedHelpFromFriendJokerCommand(NetworkManager, mainPlayerData, HelpFromFriendJokerRouter, 10));
        NetworkManager.CommandsManager.AddCommand("SelectedAskAudienceJoker", new ReceivedSelectedAskAudienceJokerCommand(mainPlayerData, AskAudienceJokerRouter, NetworkManager, 10));
        NetworkManager.CommandsManager.AddCommand("Surrender", new ReceivedSurrenderBasicExamOneTimeCommand(mainPlayerData, OnMainPlayerSurrender));

        GameData.LoadDataAsync();

        LeaderboardSerializer.LevelCategory = GameData.LevelCategory;
        LeaderboardSerializer.LoadDataAsync();

        mainPlayerData.JokersData.AddJoker(typeof(HelpFromFriendJoker));
        mainPlayerData.JokersData.AddJoker(typeof(AskAudienceJoker));
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
        CoroutineUtils.WaitUntil(() => LeaderboardSerializer.Loaded, () =>
            {
                SendEndGameInfo();
                SavePlayerScoreToLeaderboard();
                IsGameOver = true;
                OnGameOver(this, EventArgs.Empty);
                NetworkManager.StopServer();
            });
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

