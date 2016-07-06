using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PlayingUIController : MonoBehaviour
{
    //0 to 1 max
    const float ChanceToHaveLuckAfterWrongAnswer = 0f;

    public GameObject EndGameUI;
    public GameObject AskAudienceUI;
    public GameObject FriendAnswerUI;
    public GameObject WaitingToAnswerUI;
    public GameObject LeaderboardUI;
    public GameObject CallAFriendUI;
    public GameObject RiskyTrustUI;
    public Text QuestionsRemainingToNextMark;

    public ServerNetworkManager ServerNetworkManager = null;
    public GameData GameData = null;
    public BasicExamController BasicExamController = null;
    public QuestionUIController QuestionUIController = null;

    public System.EventHandler<MarkEventArgs> OnGameEnd = delegate
    {
    };

    public Button[] Jokers
    {
        get
        {
            return jokers;
        }
    }

    Button[] jokers;

    EndGameUIController endGameUIController = null;
    AudienceAnswerUIController askAudienceUIController = null;
    FriendAnswerUIController friendAnswerUIController = null;
    CallAFriendUIController callAFriendUIController = null;

    GameObject helpFromFriendButton = null;
    GameObject helpFromAudienceButton = null;
    GameObject fifthyChanceButton = null;
    GameObject surrenderButton = null;

    Text currentMarkText = null;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return null;

        endGameUIController = EndGameUI.GetComponent<EndGameUIController>();
        askAudienceUIController = AskAudienceUI.GetComponent<AudienceAnswerUIController>();
        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        callAFriendUIController = CallAFriendUI.GetComponent<CallAFriendUIController>();

        callAFriendUIController.OnCalledPlayer += OnCalledPlayer;
        GameData.MarkIncrease += OnMarkChange;
        QuestionUIController.OnAnswerClick += OnAnswerClick;

        yield return null;
        InitializeHelpPanel();
        yield return null;
        InitializeMarkPanel();
        yield return null;
        LoadFirstQuestion();
    }

    void LoadFirstQuestion()
    {
        StartCoroutine(LoadFirstQuestionCoroutine());
    }

    IEnumerator LoadFirstQuestionCoroutine()
    {
        yield return new WaitForEndOfFrame();
        //make sure all levels are loaded
        yield return new WaitUntil(() => GameData.Loaded);
        var question = GameData.GetCurrentQuestion();
        QuestionsRemainingToNextMark.text = GameData.RemainingQuestionsToNextMark.ToString();
        QuestionUIController.LoadQuestion(question);
    }

    void InitializeHelpPanel()
    {
        helpFromFriendButton = GameObject.FindGameObjectWithTag("HelpFromFriend");
        helpFromAudienceButton = GameObject.FindGameObjectWithTag("HelpFromAudience");
        fifthyChanceButton = GameObject.FindGameObjectWithTag("FifthyChance");
        surrenderButton = GameObject.FindGameObjectWithTag("SurrenderButton");

        jokers = new Button[3];

        jokers[0] = helpFromFriendButton.GetComponent<Button>();
        jokers[1] = helpFromAudienceButton.GetComponent<Button>();
        jokers[2] = fifthyChanceButton.GetComponent<Button>();
    }

    void InitializeMarkPanel()
    {
        currentMarkText = GameObject.FindGameObjectWithTag("CurrentMark").GetComponent<Text>();
    }

    void OnMarkChange(object sender, MarkEventArgs args)
    {
        currentMarkText.text = args.Mark.ToString();
    }

    void OnCalledPlayer(object sender, PlayerCalledEventArgs args)
    {
        var currentQuestion = GameData.GetCurrentQuestion();
        BasicExamController.AskFriend(currentQuestion, args.PlayerConnectionId);
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        if (args.IsCorrect)
        {
            if (ServerNetworkManager.ConnectedClientsId.Count > 0 &&
                jokers.Count(j => !j.interactable) > 0 &&
                Random.value > ChanceToHaveLuckAfterWrongAnswer)
            {
                RiskyTrustUI.SetActive(true);
            }

            StartCoroutine(LoadNextQuestionCoroutine());
        }
        else
        {
            StartCoroutine(EndGameAfterFrameCoroutine());    
        }
    }

    IEnumerator EndGameAfterFrameCoroutine()
    {
        yield return new WaitForEndOfFrame();
        EndGame();
    }

    IEnumerator LoadNextQuestionCoroutine()
    {
        yield return new WaitForEndOfFrame();

        var nextQuestion = GameData.GetNextQuestion();

        //if last question
        if (nextQuestion == null)
        {
            //show game end screen
            EndGame();
        }
        else
        {
            //if not load next question
            QuestionUIController.LoadQuestion(nextQuestion);
            QuestionsRemainingToNextMark.text = GameData.RemainingQuestionsToNextMark.ToString();
        }
    }

    IEnumerator ShowFriendAnswerCoroutine(string answer)
    {
        yield return null;
        FriendAnswerUI.SetActive(true);
        friendAnswerUIController.SetResponse(answer);
    }

    IEnumerator FifthyChanceCoroutine()
    {
        List<int> disabledAnswersIndex = new List<int>();
        var correctAnswerIndex = GameData.GetCurrentQuestion().CorrectAnswerIndex;

        for (int i = 0; i < 2; i++)
        {
            int n;

            while (true)
            {
                //make sure we dont disable correct answer and we dont disable answer 2 times
                n = UnityEngine.Random.Range(0, 4);

                if (n != correctAnswerIndex && !disabledAnswersIndex.Contains(n))
                {
                    break;
                }
            }

            disabledAnswersIndex.Add(n);
        }

        for (int i = 0; i < disabledAnswersIndex.Count; i++)
        {
            var disabledIndex = disabledAnswersIndex[i];
            QuestionUIController.DeactivateAnswerObj(disabledIndex);
        }

        yield return null;
    }

    public void EndGame()
    {
        var currentMark = int.Parse(currentMarkText.text);

        EndGameUI.SetActive(true);
        LeaderboardUI.SetActive(true);
        gameObject.SetActive(false);

        endGameUIController.SetMark(currentMark);

        OnGameEnd(this, new MarkEventArgs(currentMark));
    }

    public void CallAFriend()
    {
        var currentQuestion = GameData.GetCurrentQuestion();

        //if we dont have any clients connected
        if (ServerNetworkManager.ConnectedClientsId.Count <= 0)
        {
            //generate question
            var rightAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            StartCoroutine(ShowFriendAnswerCoroutine(rightAnswer));
        }
        else
        {
            //if we do ask our client (friend)
            var clientsConnectionIdNames = ServerNetworkManager.ConnectedClientsNames;

            CallAFriendUI.SetActive(true);
            callAFriendUIController.SetContacts(clientsConnectionIdNames);    
        }
    }

    public void AskAudience()
    {
        var currentQuestion = GameData.GetCurrentQuestion();

        //if we have less than 4 connected clients
        if (ServerNetworkManager.ConnectedClientsId.Count < 4)
        {
            var generatedAudienceAnswers = new Dictionary<string, int>();
            var correctAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            var correctAnswerChance = Random.Range(40, 80);
            var wrongAnswersLeftOverChance = 100 - correctAnswerChance;

            generatedAudienceAnswers.Add(correctAnswer, correctAnswerChance);

            //generate chances
            for (int i = 0; i < currentQuestion.Answers.Length; i++)
            {
                if (i == currentQuestion.CorrectAnswerIndex)
                {
                    continue;
                }
                    
                var wrongAnswerChance = Random.Range(0, wrongAnswersLeftOverChance);
                generatedAudienceAnswers.Add(currentQuestion.Answers[i], wrongAnswersLeftOverChance);
                wrongAnswersLeftOverChance -= wrongAnswerChance;
            }

            AskAudienceUI.SetActive(true);
            askAudienceUIController.SetVoteCount(generatedAudienceAnswers, true);
        }
        else
        {
            BasicExamController.AskAudience(currentQuestion);
        }
    }

    public void FifthyChance()
    {
        StartCoroutine(FifthyChanceCoroutine());
    }
}
