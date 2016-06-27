using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PlayingUIController : MonoBehaviour
{
    public GameObject EndGameUI;
    public GameObject AskAudienceUI;
    public GameObject FriendAnswerUI;
    public GameObject WaitingToAnswerUI;
    public GameObject LeaderboardUI;
    public GameObject CallAFriendUI;

    public ServerNetworkManager serverNetworkManager = null;
    public GameData gameData = null;
    public BasicExamController basicExamController = null;

    EndGameUIController endGameUIController = null;
    AskAudienceUIController askAudienceUIController = null;
    FriendAnswerUIController friendAnswerUIController = null;
    CallAFriendUIController callAFriendUIController = null;
    QuestionUIController questionUIController = null;

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
        askAudienceUIController = AskAudienceUI.GetComponent<AskAudienceUIController>();
        friendAnswerUIController = FriendAnswerUI.GetComponent<FriendAnswerUIController>();
        callAFriendUIController = CallAFriendUI.GetComponent<CallAFriendUIController>();
        questionUIController = GetComponentInChildren<QuestionUIController>();

        callAFriendUIController.OnCalledPlayer += OnCalledPlayer;
        gameData.MarkIncrease += OnMarkChange;
        questionUIController.OnAnswerClick += OnAnswerClick;

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
        yield return new WaitUntil(() => gameData.Loaded);
        var question = gameData.GetNextQuestion();
        questionUIController.LoadQuestion(question);
    }

    void InitializeHelpPanel()
    {
        helpFromFriendButton = GameObject.FindGameObjectWithTag("HelpFromFriendButton");
        helpFromAudienceButton = GameObject.FindGameObjectWithTag("HelpFromAudienceButton");
        fifthyChanceButton = GameObject.FindGameObjectWithTag("FifthyChanceButton");
        surrenderButton = GameObject.FindGameObjectWithTag("SurrenderButton");
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
        var currentQuestion = gameData.GetCurrentQuestion();
        basicExamController.AskFriend(currentQuestion, args.PlayerConnectionId);
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        if (args.IsCorrect)
        {
            StartCoroutine(LoadNextQuestionCoroutine());
        }
        else
        {
            StartCoroutine(EndGameCoroutine());    
        }
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForEndOfFrame();
        EndGame();
    }

    IEnumerator LoadNextQuestionCoroutine()
    {
        yield return new WaitForEndOfFrame();

        var nextQuestion = gameData.GetNextQuestion();

        if (nextQuestion == null)
        {
            EndGame();
        }
        else
        {
            questionUIController.LoadQuestion(nextQuestion);
        }
    }

    IEnumerator ShowFriendAnswerCoroutine(string answer)
    {
        yield return new WaitForEndOfFrame();
        FriendAnswerUI.SetActive(true);
        friendAnswerUIController.SetResponse(answer);
    }

    IEnumerator FifthyChanceCoroutine()
    {
        List<int> disabledAnswersIndex = new List<int>();
        var correctAnswerIndex = gameData.GetCurrentQuestion().CorrectAnswerIndex;

        for (int i = 0; i < 2; i++)
        {
            int n;

            while (true)
            {
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
            questionUIController.DeactivateAnswerObj(disabledIndex);
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
    }

    public void CallAFriend()
    {
        var currentQuestion = gameData.GetCurrentQuestion();

        if (serverNetworkManager.ConnectedClientsId.Count <= 0)
        {
            var rightAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            StartCoroutine(ShowFriendAnswerCoroutine(rightAnswer));
        }
        else
        {
            var clientsConnectionIdNames = serverNetworkManager.ConnectedClientsNames;

            CallAFriendUI.SetActive(true);
            callAFriendUIController.SetContacts(clientsConnectionIdNames);    
        }
    }

    public void AskAudience()
    {
        var currentQuestion = gameData.GetCurrentQuestion();

        if (serverNetworkManager.ConnectedClientsId.Count < 4)
        {
            var generatedAudienceAnswers = new Dictionary<string, int>();
            var correctAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            var correctAnswerChance = Random.Range(40, 80);
            var wrongAnswersLeftOverChance = 100 - correctAnswerChance;

            generatedAudienceAnswers.Add(correctAnswer, correctAnswerChance);

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
            basicExamController.AskAudience(currentQuestion);
        }
    }

    public void FifthyChance()
    {
        StartCoroutine(FifthyChanceCoroutine());
    }
}
