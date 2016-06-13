using UnityEngine;
using System.Collections.Generic;

public class BasicExamController : MonoBehaviour
{
    public GameObject WaitingToAnswerUI;
    public GameObject FriendAnswerUI;
    public GameObject AskAudienceUI;
    public GameObject PlayingUI;

    FriendAnswerUIController friendAnswerUIController = null;
    AskAudienceUIController askAudienceUIController = null;
    ServerNetworkManager serverNetworkManager = null;

    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    List<int> audienceVotedId = new List<int>();

    GameState currentState = GameState.Playing;

    void Start()
    {
        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        askAudienceUIController = AskAudienceUI.GetComponent<AskAudienceUIController>();
        serverNetworkManager = GameObject.FindWithTag("MainCamera").GetComponent<ServerNetworkManager>();
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

                if (audienceVotedId.Count >= serverNetworkManager.ConnectedClientsCount)
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

    public void AskAudience(Question question)
    {
        var questionSeriliazed = JsonUtility.ToJson(question);
        var messageType = "AskAudience";

        for (int i = 0; i < question.Answers.Length; i++)
        {
            audienceAnswerVoteCount.Add(question.Answers[i], 0);
        }

        for (int i = 0; i < serverNetworkManager.ConnectedClientsCount; i++)
        {
            var clientId = serverNetworkManager.ConnectedClientsId[i];
            
            serverNetworkManager.SendClientMessage(clientId, messageType);
            serverNetworkManager.SendClientMessage(clientId, questionSeriliazed);
        }

        WaitingToAnswerUI.SetActive(true);
        currentState = GameState.AskingAudience;
    }

    public void AskFriend(Question question)
    {
        var connectionIndex = UnityEngine.Random.Range(0, serverNetworkManager.ConnectedClientsCount);
        var connectionId = serverNetworkManager.ConnectedClientsId[connectionIndex];
        var questionSeriliazed = JsonUtility.ToJson(question);
        var messageType = "AskFriend";

        serverNetworkManager.SendClientMessage(connectionId, messageType);
        serverNetworkManager.SendClientMessage(connectionId, questionSeriliazed);

        WaitingToAnswerUI.SetActive(true);
        currentState = GameState.AskingAFriend;
    }
}