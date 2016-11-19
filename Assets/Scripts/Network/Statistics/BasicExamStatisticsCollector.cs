using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicExamStatisticsCollector : MonoBehaviour
{
    public BasicExamServer Server;
    public ServerNetworkManager NetworkManager;
    public GameDataSender Sender;
    public LocalGameData GameData;

    Dictionary<System.Type, int> jokersUsedTimes = new Dictionary<System.Type, int>();
    Dictionary<ISimpleQuestion, int> questionSpentTime = new Dictionary<ISimpleQuestion, int>();

    List<ISimpleQuestion> correctAnsweredQuestions = new List<ISimpleQuestion>();

    ISimpleQuestion lastQuestion = null;

    public IDictionary<System.Type,int> JokersUsedTimes
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

    public int EndMark
    {
        get;
        private set;
    }

    void Start()
    {
        var jokersData = Server.MainPlayerData.JokersData;
        jokersData.OnUsedJoker += OnUsedJoker;

        GameData.OnLoaded += OnGameDataLoaded;
        GameData.OnMarkIncrease += OnMarkIncrease;

        Sender.OnBeforeSend += OnBeforeSendQuestion;
        Sender.OnSentQuestion += OnSentQuestion;
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

        jokersUsedTimes[args.JokerType]++;
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