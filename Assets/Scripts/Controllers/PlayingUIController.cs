using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class PlayingUIController : MonoBehaviour
{
    public GameObject EndGameUI;
    public GameObject AskAudienceUI;
    public GameObject FriendAnswerUI;
    public GameObject WaitingToAnswerUI;

    EndGameUIController endGameUIController;
    AskAudienceUIController askAudienceUIController;
    FriendAnswerUIController friendAnswerUIController;
    BasicExamController basicExamController;


    Text questionText = null;
    Button[] answersButtons = null;
    Text[] answersTexts = null;
    Animator[] answersAnimators = null;

    GameObject helpFromFriendButton = null;
    GameObject helpFromAudienceButton = null;
    GameObject fifthyChanceButton = null;
    GameObject surrenderButton = null;

    Text currentMarkText = null;

    ServerNetworkManager serverNetworkManager = null;
    GameData gameData = null;

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
        basicExamController = GameObject.FindWithTag("MainCamera").GetComponent<BasicExamController>();
        serverNetworkManager = GameObject.FindWithTag("MainCamera").GetComponent<ServerNetworkManager>();
        gameData = GameObject.FindWithTag("MainCamera").GetComponent<GameData>();

        gameData.MarkIncrease += OnMarkChange;

        yield return null;
        InitializeHelpPanel();
        yield return null;
        InitializeQuestionPanel();
        yield return null;
        InitializeMarkPanel();
        yield return null;
        LoadFirstQuestion();
    }

    void LoadFirstQuestion()
    {
        var question = gameData.GetNextQuestion();
        LoadQuestion(question);
    }

    void InitializeHelpPanel()
    {
        helpFromFriendButton = GameObject.FindGameObjectWithTag("HelpFromFriendButton");
        helpFromAudienceButton = GameObject.FindGameObjectWithTag("HelpFromAudienceButton");
        fifthyChanceButton = GameObject.FindGameObjectWithTag("FifthyChanceButton");
        surrenderButton = GameObject.FindGameObjectWithTag("SurrenderButton");
    }

    void InitializeQuestionPanel()
    {
        var answers = GameObject.FindGameObjectsWithTag("Answer");

        answersButtons = new Button[answers.Length];
        answersTexts = new Text[answers.Length];
        answersAnimators = new Animator[answers.Length];

        questionText = GameObject.FindWithTag("QuestionText").GetComponent<Text>();

        for (int i = 0; i < answers.Length; i++)
        {
            answersButtons[i] = answers[i].GetComponent<Button>();
            answersTexts[i] = answers[i].transform.GetChild(0).GetComponent<Text>();
            answersAnimators[i] = answers[i].GetComponent<Animator>();
        }     
    }

    void InitializeMarkPanel()
    {
        currentMarkText = GameObject.FindGameObjectWithTag("CurrentMark").GetComponent<Text>();
    }

    void LoadQuestion(Question question)
    {
        questionText.text = question.Text;
        for (int i = 0; i < question.Answers.Length; i++)
        {
            var buttonIndex = i;
            var answerObj = answersButtons[buttonIndex].gameObject;

            answerObj.SetActive(true);

            answersTexts[buttonIndex].text = question.Answers[buttonIndex];
            answersButtons[buttonIndex].interactable = true;
            answersButtons[buttonIndex].onClick.AddListener(() =>
                {
                    DeactivateAnswers();
                    PlayClickedAnimation(buttonIndex, buttonIndex == question.CorrectAnswerIndex);
                }
            );
        }
    }

    void DeactivateAnswers()
    {
        for (int i = 0; i < answersButtons.Length; i++)
        {
            answersButtons[i].interactable = false;
        }
    }

    void PlayClickedAnimation(int buttonIndex, bool isCorrect)
    {
        answersAnimators[buttonIndex].SetTrigger("clicked");
        answersAnimators[buttonIndex].SetBool("isCorrect", isCorrect);
    }

    void OnMarkChange(object sender, MarkEventArgs args)
    {
        currentMarkText.text = args.Mark.ToString();
    }

    public void OnIncorrectAnswerClick()
    {
        //SHOW DEAD SCREEN :(
        StartCoroutine(EndGameCoroutine());
    }


    public void OnCorrectAnswerClick()
    {
        StartCoroutine(LoadNextQuestionCoroutine());
    }


    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForFixedUpdate();
        EndGame();
    }

    IEnumerator LoadNextQuestionCoroutine()
    {
        yield return new WaitForFixedUpdate();

        var nextQuestion = gameData.GetNextQuestion();

        if (nextQuestion == null)
        {
            EndGame();
        }
        else
        {
            LoadQuestion(nextQuestion);
        }
    }

    public void EndGame()
    {
        var currentMark = int.Parse(currentMarkText.text);

        EndGameUI.SetActive(true);
        gameObject.SetActive(false);

        endGameUIController.SetMark(currentMark);
    }

    IEnumerator ShowFriendAnswerCoroutine(string answer)
    {
        yield return new WaitForFixedUpdate();
        FriendAnswerUI.SetActive(true);
        friendAnswerUIController.SetResponse(answer);
    }

    public void CallAFriend()
    {
        var currentQuestion = gameData.GetCurrentQuestion();

        if (serverNetworkManager.ConnectedClientsCount <= 0)
        {
            var rightAnswer = currentQuestion.Answers[currentQuestion.CorrectAnswerIndex];
            StartCoroutine(ShowFriendAnswerCoroutine(rightAnswer));
        }
        else
        {
            basicExamController.AskFriend(currentQuestion);    
        }

    }

    public void AskAudience()
    {
        var currentQuestion = gameData.GetCurrentQuestion();

        if (serverNetworkManager.ConnectedClientsCount <= 4)
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

    IEnumerator FifthyChanceCoroutine()
    {
        List<int> disabledAnswersIndex = new List<int>();
        var correctAnswerIndex = gameData.GetCurrentQuestion().CorrectAnswerIndex;

        for (int i = 0; i < 2; i++)
        {
            int n;

            while (true)
            {
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
            answersButtons[disabledIndex].gameObject.SetActive(false);
        }

        yield return null;
    }

}
