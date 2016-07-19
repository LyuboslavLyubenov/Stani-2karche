using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class BasicPlayerPlayingUIController : ExtendedMonoBehaviour
{
    //0f = 0% 1f = 100%
    const float ChanceToHaveLuckAfterAnswer = 0.6f;
    const float ChanceForGeneratingRightAnswerFriendCall = 0.85f;

    public Text QuestionsRemainingToNextMark;

    public ServerNetworkManager ServerNetworkManager;
    public GameData GameData;

    public QuestionUIController QuestionUIController;
    public MarkPanelController MarkPanelController;

    public System.EventHandler<MarkEventArgs> OnGameEnd = delegate
    {
    };
    
    public System.EventHandler<AnswerEventArgs> OnFriendAnswerGenerated = delegate
    {
    };

    public System.EventHandler<AudienceVoteEventArgs> OnAudienceVoteGenerated = delegate
    {
    };

    public System.EventHandler ShowRiskyTrustChance = delegate
    {
    };

    public System.EventHandler ShowOnlineCallFriendMenu = delegate
    {
    };

    public System.EventHandler GetOnlineAudienceAnswer = delegate
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

    GameObject helpFromFriendButton = null;
    GameObject helpFromAudienceButton = null;
    GameObject fifthyChanceButton = null;
    GameObject surrenderButton = null;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return null;

        GameData.MarkIncrease += OnMarkChange;
        QuestionUIController.OnAnswerClick += OnAnswerClick;

        yield return null;
        InitializeHelpPanel();
        yield return null;
        StartCoroutine(LoadFirstQuestionCoroutine());
    }

    IEnumerator LoadFirstQuestionCoroutine()
    {
        //make sure all levels are loaded
        yield return new WaitUntil(() => GameData.Loaded);
        yield return null;

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

    void OnMarkChange(object sender, MarkEventArgs args)
    {
        MarkPanelController.Mark = args.Mark.ToString();

        var chanceForJoker = Random.value;

        if (ServerNetworkManager.ConnectedClientsIds.Count > 0 &&
            jokers.Count(j => !j.interactable) > 0 &&
            chanceForJoker >= ChanceToHaveLuckAfterAnswer)
        {
            ShowRiskyTrustChance(this, System.EventArgs.Empty);
        }
    }

    void OnAnswerClick(object sender, AnswerEventArgs args)
    {
        if (args.IsCorrect)
        {
            CoroutineUtils.WaitForFrames(0, LoadNextQuestion);
        }
        else
        {
            CoroutineUtils.WaitForFrames(0, EndGame);   
        }
    }

    void LoadNextQuestion()
    {
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

    void ActivateFifthyChanceJoker()
    {
        var currentQuestion = GameData.GetCurrentQuestion();
        var correctAnswerIndex = currentQuestion.CorrectAnswerIndex;
        var disabledAnswersIndex = new List<int>();

        for (int i = 0; i < 2; i++)
        {
            int n;

            while (true)
            {
                //make sure we dont disable correct answer and we dont disable answer 2 times
                n = Random.Range(0, 4);

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
            QuestionUIController.HideAnswer(disabledIndex);
        }
    }

    Dictionary<string, int> GenerateAudienceVotes(Question question)
    {
        var generatedAudienceAnswersVotes = new Dictionary<string, int>();
        var correctAnswer = question.Answers[question.CorrectAnswerIndex];
        var correctAnswerChance = Random.Range(40, 85);
        var wrongAnswersLeftOverChance = 100 - correctAnswerChance;

        generatedAudienceAnswersVotes.Add(correctAnswer, correctAnswerChance);

        //generate chances
        for (int i = 0; i < question.Answers.Length; i++)
        {
            if (i == question.CorrectAnswerIndex)
            {
                continue;
            }

            var wrongAnswerChance = Random.Range(0, wrongAnswersLeftOverChance);
            generatedAudienceAnswersVotes.Add(question.Answers[i], wrongAnswersLeftOverChance);
            wrongAnswersLeftOverChance -= wrongAnswerChance;
        }

        return generatedAudienceAnswersVotes;
    }

    public void EndGame()
    {
        var currentMark = int.Parse(MarkPanelController.Mark);
        OnGameEnd(this, new MarkEventArgs(currentMark));
        gameObject.SetActive(false);
    }

    public void CallAFriend()
    {
        var currentQuestion = GameData.GetCurrentQuestion();

        //if we dont have any clients connected
        if (ServerNetworkManager.ConnectedClientsIds.Count <= 0)
        {
            //generate question
            var answers = currentQuestion.Answers;
            var correctAnswer = answers[currentQuestion.CorrectAnswerIndex];
            var answerSelected = answers[currentQuestion.CorrectAnswerIndex];
            var isCorrect = true;

            if (Random.value >= ChanceForGeneratingRightAnswerFriendCall)
            {
                var wrongAnswers = answers.Where(a => a != correctAnswer).ToArray();
                var wrongAnswerIndex = Random.Range(0, wrongAnswers.Length);

                answerSelected = wrongAnswers[wrongAnswerIndex];
                isCorrect = false;
            }

            var answerEventArgs = new AnswerEventArgs(answerSelected, isCorrect);
            CoroutineUtils.WaitForFrames(0, () => OnFriendAnswerGenerated(this, answerEventArgs));
        }
        else
        {
            ShowOnlineCallFriendMenu(this, System.EventArgs.Empty); 
        }
    }

    public void AskAudience()
    {
        var currentQuestion = GameData.GetCurrentQuestion();
        var minForOnlineVote = 4;

        #if DEVELOPMENT_BUILD
        minForOnlineVote = 1;
        #endif

        //if we have less than 4 connected clients
        if (ServerNetworkManager.ConnectedClientsIds.Count < minForOnlineVote)
        {
            var generatedAudienceAnswersVotes = GenerateAudienceVotes(currentQuestion);
            var audienceVoteEventArgs = new AudienceVoteEventArgs(generatedAudienceAnswersVotes);
            OnAudienceVoteGenerated(this, audienceVoteEventArgs);
        }
        else
        {
            GetOnlineAudienceAnswer(this, System.EventArgs.Empty);
        }
    }

    public void FifthyChance()
    {
        CoroutineUtils.WaitForFrames(0, ActivateFifthyChanceJoker);
    }
}