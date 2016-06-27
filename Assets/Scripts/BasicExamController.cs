using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class BasicExamController : MonoBehaviour
{
    public GameObject WaitingToAnswerUI;
    public GameObject FriendAnswerUI;
    public GameObject AskAudienceUI;
    public GameObject PlayingUI;
    public GameObject LeaderboardUI;

    public ServerNetworkManager serverNetworkManager;
    public LeaderboardSerializer leaderboardSerializer;

    FriendAnswerUIController friendAnswerUIController = null;
    AskAudienceUIController askAudienceUIController = null;

    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    List<int> audienceVotedId = new List<int>();

    GameState currentState = GameState.Playing;

    void Start()
    {
        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        askAudienceUIController = AskAudienceUI.GetComponent<AskAudienceUIController>();

        serverNetworkManager.OnClientSentMessage += OnClientSendMessage;

        var playingUIController = PlayingUI.GetComponent<PlayingUIController>();

        playingUIController.OnGameEnd += OnGameEnd;
    }

    void OnGameEnd(object sender, MarkEventArgs args)
    {
        StartCoroutine(SetPlayerScore(args.Mark));
    }

    void OnClientSendMessage(object sender, DataSentEventArgs args)
    {
        switch (currentState)
        {
            case GameState.Playing:
                    //nothing
                break;

            case GameState.AskingAFriend:
                
                WaitingToAnswerUI.SetActive(false);
                FriendAnswerUI.SetActive(true);
                friendAnswerUIController.SetResponse(args.Username, args.Message);
                currentState = GameState.Playing;

                break;

            case GameState.AskingAudience:

                var connectionId = args.ConnectionId;

                if (!audienceVotedId.Contains(connectionId))
                {
                    audienceVotedId.Add(connectionId);
                    audienceAnswerVoteCount[args.Message]++;
                }

                if (audienceVotedId.Count >= serverNetworkManager.ConnectedClientsId.Count)
                {
                    AskAudienceUI.SetActive(true);
                    WaitingToAnswerUI.SetActive(false);
                    askAudienceUIController.SetVoteCount(audienceAnswerVoteCount, true);
                    audienceAnswerVoteCount.Clear();
                    audienceVotedId.Clear();
                    currentState = GameState.Playing;
                }

                break;
        }
    }

    IEnumerator SetPlayerScore(int mark)
    {
        yield return new WaitUntil(() => leaderboardSerializer.Loaded);
        var playerName = "Анонимен играч";

        if (PlayerPrefs.HasKey("Username"))
        {
            playerName = PlayerPrefs.GetString("Username");
        }

        var playerScore = new PlayerScore(playerName, mark);
        leaderboardSerializer.SetPlayerScore(playerScore);
    }

    public void AskFriend(Question question, int clientConnectionId)
    {
        serverNetworkManager.CallFriend(question, clientConnectionId);
        WaitingToAnswerUI.SetActive(true);
        currentState = GameState.AskingAFriend;
    }

    public void AskAudience(Question question)
    {
        for (int i = 0; i < question.Answers.Length; i++)
        {
            audienceAnswerVoteCount.Add(question.Answers[i], 0);
        }

        serverNetworkManager.AskAudience(question);

        WaitingToAnswerUI.SetActive(true);
        currentState = GameState.AskingAudience;
    }
}