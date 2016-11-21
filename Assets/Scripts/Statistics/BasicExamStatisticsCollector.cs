using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicExamStatisticsCollector : MonoBehaviour
{
    public ServerNetworkManager NetworkManager;
    public BasicExamServer Server;
    public GameDataSender Sender;
    public LocalGameData GameData;

    Dictionary<ISimpleQuestion, int> questionSpentTime = new Dictionary<ISimpleQuestion, int>();
    Dictionary<ISimpleQuestion, List<Type>> questionsUsedJokers = new Dictionary<ISimpleQuestion, List<Type>>();
    Dictionary<Type, int> jokersUsedTimes = new Dictionary<Type, int>();

    List<ISimpleQuestion> correctAnsweredQuestions = new List<ISimpleQuestion>();

    ISimpleQuestion lastQuestion = null;
    string lastSelectedAnswer = string.Empty;

    public IDictionary<ISimpleQuestion, List<Type>> QuestionsUsedJokers
    {
        get
        {
            return new Dictionary<ISimpleQuestion, List<Type>>(questionsUsedJokers);
        }
    }

    public IDictionary<Type, int> JokersUsedTimes
    {
        get
        {
            return new Dictionary<System.Type, int>(jokersUsedTimes);
        }
    }

    public IDictionary<ISimpleQuestion, int> QuestionsSpentTime
    {
        get
        {
            return new Dictionary<ISimpleQuestion, int>(questionSpentTime);
        }
    }

    public IList<ISimpleQuestion> CorrectAnsweredQuestions
    {
        get
        {
            return new List<ISimpleQuestion>(correctAnsweredQuestions);
        }
    }

    public ISimpleQuestion LastQuestion
    {
        get
        {
            return lastQuestion;
        }
    }

    public string LastSelectedAnswer
    {
        get
        {
            return lastSelectedAnswer;
        }
    }

    public int EndMark
    {
        get;
        private set;
    }

    void Start()
    {
        NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ReceivedServerSelectedAnswerCommand(OnReceivedAnswer));

        var jokersData = Server.MainPlayerData.JokersData;
        jokersData.OnUsedJoker += OnUsedJoker;

        GameData.OnLoaded += OnGameDataLoaded;
        GameData.OnMarkIncrease += OnMarkIncrease;

        Sender.OnBeforeSend += OnBeforeSendQuestion;
        Sender.OnSentQuestion += OnSentQuestion;

        Server.OnGameOver += OnGameOver;
    }

    void OnGameOver(object sender, EventArgs args)
    {
        questionSpentTime[lastQuestion] = GameData.SecondsForAnswerQuestion - Server.RemainingTimetoAnswerInSeconds;
    }

    void OnReceivedAnswer(int connectionId, string answer)
    {
        lastSelectedAnswer = answer;
    }

    void OnMarkIncrease(object sender, MarkEventArgs args)
    {
        EndMark = args.Mark;
    }

    void OnUsedJoker(object sender, JokerTypeEventArgs args)
    {
        if (!jokersUsedTimes.ContainsKey(args.JokerType))
        {
            jokersUsedTimes.Add(args.JokerType, 0);
        }

        if (!questionsUsedJokers.ContainsKey(lastQuestion))
        {
            questionsUsedJokers.Add(lastQuestion, new List<Type>());
        }

        jokersUsedTimes[args.JokerType]++;
        questionsUsedJokers[lastQuestion].Add(args.JokerType);
    }

    void OnGameDataLoaded(object sender, EventArgs args)
    {
        SetCurrentQuestion();
    }

    void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
    {
        if (args.QuestionType == QuestionRequestType.Next)
        {
            correctAnsweredQuestions.Add(lastQuestion);    
        }

        questionSpentTime[lastQuestion] = GameData.SecondsForAnswerQuestion - Server.RemainingTimetoAnswerInSeconds;
    }

    void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
    {
        SetCurrentQuestion();
    }

    void SetCurrentQuestion()
    {
        GameData.GetCurrentQuestion(OnLoadedCurrentQuestion);
    }

    void OnLoadedCurrentQuestion(ISimpleQuestion question)
    {
        lastQuestion = question;
    }
}