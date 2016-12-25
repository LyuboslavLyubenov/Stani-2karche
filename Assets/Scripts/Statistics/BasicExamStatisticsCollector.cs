using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Statistics
{

    using Assets.Scripts.Commands.Server;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;

    using EventArgs = System.EventArgs;

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
                return new Dictionary<ISimpleQuestion, List<Type>>(this.questionsUsedJokers);
            }
        }

        public IDictionary<Type, int> JokersUsedTimes
        {
            get
            {
                return new Dictionary<System.Type, int>(this.jokersUsedTimes);
            }
        }

        public IDictionary<ISimpleQuestion, int> QuestionsSpentTime
        {
            get
            {
                return new Dictionary<ISimpleQuestion, int>(this.questionSpentTime);
            }
        }

        public IList<ISimpleQuestion> CorrectAnsweredQuestions
        {
            get
            {
                return new List<ISimpleQuestion>(this.correctAnsweredQuestions);
            }
        }

        public ISimpleQuestion LastQuestion
        {
            get
            {
                return this.lastQuestion;
            }
        }

        public string LastSelectedAnswer
        {
            get
            {
                return this.lastSelectedAnswer;
            }
        }

        public int EndMark
        {
            get;
            private set;
        }

        public int PlayerScore
        {
            get
            {
                var correctAnsweredQuestionsCount = this.CorrectAnsweredQuestions.Count;
                var totalTimeSpentThinking = this.QuestionsSpentTime.Values.ToList().Sum();
                var avgSpentTimeThinking = totalTimeSpentThinking / this.QuestionsSpentTime.Values.Count;
                var score = (correctAnsweredQuestionsCount * 10000) / avgSpentTimeThinking;
                return score;
            }
        }

        void Start()
        {
            this.NetworkManager.CommandsManager.AddCommand("AnswerSelected", new ReceivedServerSelectedAnswerCommand(this.OnReceivedAnswer));

            var jokersData = this.Server.MainPlayerData.JokersData;
            jokersData.OnUsedJoker += this.OnUsedJoker;

            this.GameData.OnLoaded += this.OnGameDataLoaded;
            this.GameData.OnMarkIncrease += this.OnMarkIncrease;

            this.Sender.OnBeforeSend += this.OnBeforeSendQuestion;
            this.Sender.OnSentQuestion += this.OnSentQuestion;

            this.Server.OnGameOver += this.OnGameOver;
        }

        void OnGameOver(object sender, EventArgs args)
        {
            this.questionSpentTime[this.lastQuestion] = this.GameData.SecondsForAnswerQuestion - this.Server.RemainingTimetoAnswerInSeconds;
        }

        void OnReceivedAnswer(int connectionId, string answer)
        {
            this.lastSelectedAnswer = answer;
        }

        void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.EndMark = args.Mark;
        }

        void OnUsedJoker(object sender, JokerTypeEventArgs args)
        {
            if (!this.jokersUsedTimes.ContainsKey(args.JokerType))
            {
                this.jokersUsedTimes.Add(args.JokerType, 0);
            }

            if (!this.questionsUsedJokers.ContainsKey(this.lastQuestion))
            {
                this.questionsUsedJokers.Add(this.lastQuestion, new List<Type>());
            }

            this.jokersUsedTimes[args.JokerType]++;
            this.questionsUsedJokers[this.lastQuestion].Add(args.JokerType);
        }

        void OnGameDataLoaded(object sender, EventArgs args)
        {
            this.SetCurrentQuestion();
        }

        void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                this.correctAnsweredQuestions.Add(this.lastQuestion);    
            }

            this.questionSpentTime[this.lastQuestion] = this.GameData.SecondsForAnswerQuestion - this.Server.RemainingTimetoAnswerInSeconds;
        }

        void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            this.SetCurrentQuestion();
        }

        void SetCurrentQuestion()
        {
            this.GameData.GetCurrentQuestion(this.OnLoadedCurrentQuestion);
        }

        void OnLoadedCurrentQuestion(ISimpleQuestion question)
        {
            this.lastQuestion = question;
        }
    }

}