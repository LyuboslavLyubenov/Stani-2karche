using UnityEngine;
using System.Collections.Generic;
using System;

public class BasicExamController : MonoBehaviour
{
    public GameObject WaitingToAnswerUI;
    public GameObject FriendAnswerUI;
    public GameObject AskAudienceUI;
    public GameObject PlayingUI;
    public GameObject LeaderboardUI;

    FriendAnswerUIController friendAnswerUIController = null;
    AskAudienceUIController askAudienceUIController = null;
    ServerNetworkManager serverNetworkManager = null;

    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    List<int> audienceVotedId = new List<int>();

    GameState currentState = GameState.Playing;

    void Start()
    {
        var mainCamera = GameObject.FindWithTag("MainCamera");

        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        askAudienceUIController = AskAudienceUI.GetComponent<AskAudienceUIController>();
        serverNetworkManager = mainCamera.GetComponent<ServerNetworkManager>();

        serverNetworkManager.OnClientSentMessage += OnClientSendMessage;
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