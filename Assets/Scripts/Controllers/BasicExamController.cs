using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

/// <summary>
/// Basic exam controller. Used to controll UI for "Standart play mode" or "Нормално изпитване" 
/// </summary>
public class BasicExamController : ExtendedMonoBehaviour
{
    public GameObject WaitingToAnswerUI;
    public GameObject FriendAnswerUI;
    public GameObject AudienceAnswerUI;
    public GameObject PlayingUI;
    public GameObject LeaderboardUI;
    public GameObject LoadingUI;
    public GameObject RiskyTrustUI;
    public GameObject EndGameUI;
    public GameObject CallAFriendUI;
    public GameObject BasicExamPlaygroundUI;

    public ServerNetworkManager ServerNetworkManager;
    public LeaderboardSerializer LeaderboardSerializer;
    public GameData GameData;
    public BasicExamPlayerTeacherDialogSwitcher DialogSwitcher;
    public NotificationsServiceController NotificationService;
    public CallAFriendUIController CallAFriendUIController;
    public ChooseThemeUIController ChooseThemeUIController;
    public BasicExamPlayerTutorialUIController TutorialUIController;

    FriendAnswerUIController friendAnswerUIController = null;
    AudienceAnswerUIController audienceAnswerUIController = null;
    BasicPlayerPlayingUIController playingUIController = null;

    //how many votes each answer have
    Dictionary<string, int> audienceAnswerVoteCount = new Dictionary<string, int>();
    //voted clients id
    List<int> audienceVotedId = new List<int>();

    GameState currentState = GameState.Playing;

    Question riskyTrustQuestion = null;

    void Start()
    {
        //load all controllers
        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        audienceAnswerUIController = AudienceAnswerUI.GetComponent<AudienceAnswerUIController>();
        playingUIController = PlayingUI.GetComponent<BasicPlayerPlayingUIController>();

        ServerNetworkManager.OnReceivedDataEvent += OnClientSendMessage;

        playingUIController.OnGameEnd += OnGameEnd;
        playingUIController.OnFriendAnswerGenerated += OnFriendAnswerGenerated;
        playingUIController.OnAudienceVoteGenerated += OnAudienceVoteGenerated;
        playingUIController.ShowRiskyTrustChance += (sender, e) => RiskyTrustUI.SetActive(true);
        playingUIController.ShowOnlineCallFriendMenu += ShowCallFriendUI;
        playingUIController.GetOnlineAudienceAnswer += GetOnlineAudienceAnswer;

        DialogSwitcher.gameObject.SetActive(true);
        DialogSwitcher.ExplainThemeSelect();

        ChooseThemeUIController.OnChoosedTheme += OnChoosedTheme;

        StartCoroutine(HideLoadingUIWhenLoaded());
    }

    void OnChoosedTheme(object sender, EventArgs args)
    {
        LoadingUI.SetActive(true);
        BasicExamPlaygroundUI.SetActive(true);
        CoroutineUtils.WaitForSeconds(2f, TutorialUIController.Activate);
    }

    void GetOnlineAudienceAnswer(object sender, EventArgs args)
    {
        var currentQuestion = GameData.GetCurrentQuestion();
        AskAudience(currentQuestion);
    }

    void ShowCallFriendUI(object sender, System.EventArgs args)
    {
        var contacts = ServerNetworkManager.ConnectedClientsIdsNames;
        
        CallAFriendUI.SetActive(true);
        CallAFriendUIController.SetContacts(contacts);
    }

    void OnAudienceVoteGenerated(object sender, AudienceVoteEventArgs args)
    {
        AudienceAnswerUI.SetActive(true);
        audienceAnswerUIController.SetVoteCount(args.AnswersVotes, true);
    }

    void OnFriendAnswerGenerated(object sender, AnswerEventArgs args)
    {
        FriendAnswerUI.SetActive(true);
        friendAnswerUIController.SetResponse(args.Answer);
    }

    void OnGameEnd(object sender, MarkEventArgs args)
    {
        //if game ends set player mark on the leaderboard
        EndGameUI.SetActive(true);

        var endGameUIController = EndGameUI.GetComponent<EndGameUIController>();

        CoroutineUtils.WaitForFrames(0, () => endGameUIController.SetMark(args.Mark));
        CoroutineUtils.WaitUntil(() => LeaderboardSerializer.Loaded, () => SavePlayerScore(args.Mark));
    }

    void SavePlayerScore(int mark)
    {
        //first leaderboard file must be loaded
        //if we dont have name use this one 
        var playerName = "Анонимен играч";

        if (PlayerPrefs.HasKey("Username"))
        {
            playerName = PlayerPrefs.GetString("Username");
        }

        var playerScore = new PlayerScore(playerName, mark);
        LeaderboardSerializer.SetPlayerScore(playerScore);
    }

    void OnClientSendMessage(object sender, DataSentEventArgs args)
    {
        //if received data from client

        switch (currentState)
        {
            case GameState.RiskyTrust:

                if (riskyTrustQuestion == null)
                {
                    return;
                }

                var correctAnswer = riskyTrustQuestion.Answers[riskyTrustQuestion.CorrectAnswerIndex];

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

                WaitingToAnswerUI.SetActive(false);
                riskyTrustQuestion = null;
                currentState = GameState.Playing;

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

                if (audienceVotedId.Count >= ServerNetworkManager.ConnectedClientsCount)
                {
                    //we got all audience votes
                    //show them to the user

                    WaitingToAnswerUI.SetActive(false);

                    if (audienceAnswerVoteCount.Any(avc => avc.Value > 0))
                    {
                        ShowAudienceVote();
                    }
                    else
                    {
                        NotificationService.AddNotification(Color.blue, "Никой не гласува :(");
                    }

                    currentState = GameState.Playing;
                }

                break;
        }
    }

    void ShowAudienceVote()
    {
        AudienceAnswerUI.SetActive(true);
        audienceAnswerUIController.SetVoteCount(audienceAnswerVoteCount, true);
        audienceAnswerVoteCount.Clear();
        audienceVotedId.Clear();
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
            //TODO: Play sound
        }
    }

    IEnumerator HideLoadingUIWhenLoaded()
    {
        yield return new WaitUntil(() => GameData.Loaded); //wait until all levels are loaded (3.csv, 4.csv, 5.csv, 6.csv)
        yield return new WaitUntil(() => LeaderboardSerializer.Loaded); // wait until leaderboard file is loaded
        yield return new WaitForSeconds(1f); //additional seconds loading screen (because i can)
        LoadingUI.SetActive(false);//hide me
    }

    void StopReceivingAnswer(int clientConnectionId)
    {
        ServerNetworkManager.SendClientMessage(clientConnectionId, "AnswerTimeout");
        currentState = GameState.Playing;
    }

    void SentRemainingTimeToClient(int clientConnectionId, int remainingTimeInSeconds)
    {
        ServerNetworkManager.SendClientMessage(clientConnectionId, "RemainingTime=" + remainingTimeInSeconds);
    }

    public void ActivateRiskyTrustJoker()
    {
        if (ServerNetworkManager.ConnectedClientsCount <= 0)
        {
            throw new Exception("Жокера може да се изпозлва само когато си онлайн.");
        }

        riskyTrustQuestion = GameData.GetRandomQuestion();

        var random = new System.Random(DateTime.Now.Millisecond);
        var randomQuestionJSON = JsonUtility.ToJson(riskyTrustQuestion);
        var friendConnectionIdIndex = random.Next(0, ServerNetworkManager.ConnectedClientsIds.Length);
        var friendConnectionId = ServerNetworkManager.ConnectedClientsIds[friendConnectionIdIndex];

        ServerNetworkManager.SendClientMessage(friendConnectionId, "RiskyTrust");
        ServerNetworkManager.SendClientMessage(friendConnectionId, randomQuestionJSON);

        WaitingToAnswerUI.SetActive(true);
        RiskyTrustUI.SetActive(false);

        currentState = GameState.RiskyTrust;
    }

    public void AskFriend(Question question, int clientConnectionId)
    {
        var questionJSON = JsonUtility.ToJson(question);

        ServerNetworkManager.SendClientMessage(clientConnectionId, "AskFriend");
        ServerNetworkManager.SendClientMessage(clientConnectionId, questionJSON);

        //tell the user that we wait for answer
        WaitingToAnswerUI.SetActive(true);

        var disableAfterDelayComponent = WaitingToAnswerUI.GetComponent<DisableAfterDelay>();

        disableAfterDelayComponent.OnTimePass += (sender, args) => SentRemainingTimeToClient(clientConnectionId, args.Seconds);
        disableAfterDelayComponent.OnTimeEnd += (sender, e) => StopReceivingAnswer(clientConnectionId);

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

        var questionJSON = JsonUtility.ToJson(question);

        ServerNetworkManager.SendAllClientsMessage("AskAudience");
        ServerNetworkManager.SendAllClientsMessage(questionJSON);

        WaitingToAnswerUI.SetActive(true);

        var disableAfterDelayComponent = WaitingToAnswerUI.GetComponent<DisableAfterDelay>();

        disableAfterDelayComponent.OnTimePass += (sender, e) =>
        {
            for (int i = 0; i < ServerNetworkManager.ConnectedClientsIds.Length; i++)
            {
                SentRemainingTimeToClient(ServerNetworkManager.ConnectedClientsIds[i], e.Seconds);       
            }        
        };
        
        disableAfterDelayComponent.OnTimeEnd += (sender, e) =>
        {
            ServerNetworkManager.SendAllClientsMessage("AnswerTimeout");
            currentState = GameState.Playing;

            if (audienceAnswerVoteCount.Any(avc => avc.Value > 0))
            {
                WaitingToAnswerUI.SetActive(false);
                ShowAudienceVote();
            }
        };

        currentState = GameState.AskingAudience;
    }
}