using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

/// <summary>
/// Basic exam controller. Used to controll UI for "Standart play mode" or "Нормално изпитване" 
/// </summary>
public class BasicExamController : MonoBehaviour
{
    public GameObject WaitingToAnswerUI;
    public GameObject FriendAnswerUI;
    public GameObject AudienceAnswerUI;
    public GameObject PlayingUI;
    public GameObject LeaderboardUI;
    public GameObject LoadingUI;

    public ServerNetworkManager ServerNetworkManager;
    public LeaderboardSerializer LeaderboardSerializer;
    public GameData GameData;

    FriendAnswerUIController friendAnswerUIController = null;
    AudienceAnswerUIController audienceAnswerUIController = null;
    PlayingUIController playingUIController = null;

    //how many votes each answer have
    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    //voted clients id
    List<int> audienceVotedId = new List<int>();

    GameState currentState = GameState.Playing;

    void Start()
    {
        //load all controllers
        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        audienceAnswerUIController = AudienceAnswerUI.GetComponent<AudienceAnswerUIController>();
        playingUIController = PlayingUI.GetComponent<PlayingUIController>();

        ServerNetworkManager.OnReceivedDataEvent += OnClientSendMessage;
        playingUIController.OnGameEnd += OnGameEnd;

        LoadingUI.SetActive(true);

        StartCoroutine(HideLoadingUIWhenLoaded());
    }

    void OnGameEnd(object sender, MarkEventArgs args)
    {
        //if game ends set player mark on the leaderboard
        StartCoroutine(SetPlayerScore(args.Mark));
    }

    void OnClientSendMessage(object sender, DataSentEventArgs args)
    {
        //if received data from client

        switch (currentState)
        {
            case GameState.RiskyTrust:

                var currentQuestion = GameData.GetCurrentQuestion();
                var correctAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];

                if (args.Message == correctAnswer)
                {
                    StartCoroutine(EnableRandomDisabledJoker());
                }
                else
                {
                    //WRONG ANSWER MOTHERFUCKER
                    var markIndex = GameData.CurrentMarkIndex;
                    GameData.QuestionsToTakePerMark[markIndex]++;
                    //TODO: Play animation ?
                }

                break;

            case GameState.AskingAFriend:
                //if we currently are waiting for friend to answer
                //Hide WaitingToAnswerUI and show answer that we received
                WaitingToAnswerUI.SetActive(false);
                FriendAnswerUI.SetActive(true);
                friendAnswerUIController.SetResponse(args.Username, args.Message);
                currentState = GameState.Playing;

                break;

            case GameState.AskingAudience:
                //if we currently are waiting for audience to answer
                //get current user id
                var connectionId = args.ConnectionId;

                if (!audienceVotedId.Contains(connectionId))
                {
                    audienceVotedId.Add(connectionId);
                    audienceAnswerVoteCount[args.Message]++;
                }

                if (audienceVotedId.Count >= ServerNetworkManager.ConnectedClientsId.Count)
                {
                    //we got all audience votes
                    //show them to the user
                    AudienceAnswerUI.SetActive(true);
                    WaitingToAnswerUI.SetActive(false);
                    audienceAnswerUIController.SetVoteCount(audienceAnswerVoteCount, true);
                    audienceAnswerVoteCount.Clear();
                    audienceVotedId.Clear();
                    currentState = GameState.Playing;
                }

                break;
        }
    }

    IEnumerator EnableRandomDisabledJoker()
    {
        yield return new WaitForEndOfFrame();

        var disabledJokers = playingUIController.Jokers.Where(j => !j.interactable).ToList();

        if (disabledJokers.Count > 0)
        {
            var index = UnityEngine.Random.Range(0, disabledJokers.Count);
            disabledJokers[index].interactable = true;
            //TODO: MAYBE PLAY ANIMATION?!?
        }
    }

    IEnumerator HideLoadingUIWhenLoaded()
    {
        yield return new WaitUntil(() => GameData.Loaded); //wait until all levels are loaded (3.csv, 4.csv, 5.csv, 6.csv)
        yield return new WaitUntil(() => LeaderboardSerializer.Loaded); // wait until leaderboard file is loaded
        yield return new WaitForSeconds(3f); //additional seconds loading screen (because i can)
        LoadingUI.SetActive(false);//hide me
    }

    IEnumerator SetPlayerScore(int mark)
    {
        yield return new WaitUntil(() => LeaderboardSerializer.Loaded);//first leaderboard file must be loaded
        //if we dont have name use this one 
        var playerName = "Анонимен играч";

        if (PlayerPrefs.HasKey("Username"))
        {
            playerName = PlayerPrefs.GetString("Username");
        }

        var playerScore = new PlayerScore(playerName, mark);
        LeaderboardSerializer.SetPlayerScore(playerScore);
    }

    void StopReceivingAnswer(int clientConnectionId)
    {
        ServerNetworkManager.SendClientMessage(clientConnectionId, "AnswerTimeout");
        currentState = GameState.Playing;
    }

    public void AskFriend(Question question, int clientConnectionId)
    {
        //tell to the serverNetworkManager to send request to the given client
        ServerNetworkManager.CallFriend(question, clientConnectionId);
        //tell the user that we wait for answer
        WaitingToAnswerUI.SetActive(true);

        WaitingToAnswerUI.GetComponent<DisableAfterDelay>().OnTimeEnd += (object sender, EventArgs e) => StopReceivingAnswer(clientConnectionId);

        currentState = GameState.AskingAFriend;
    }

    public void AskAudience(Question question)
    {
        //make sure all answers have 0 votes
        audienceAnswerVoteCount.Clear();

        for (int i = 0; i < question.Answers.Length; i++)
        {
            audienceAnswerVoteCount.Add(question.Answers[i], 0);
        }

        ServerNetworkManager.AskAudience(question);
        //user wait until all answers are collected
        WaitingToAnswerUI.SetActive(true);

        WaitingToAnswerUI.GetComponent<DisableAfterDelay>().OnTimeEnd += (object sender, EventArgs e) =>
        {
            currentState = GameState.Playing;
        };

        currentState = GameState.AskingAudience;
    }
}