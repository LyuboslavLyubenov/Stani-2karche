namespace Assets.Scripts.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Statistics;

    using Commands.Server;
    using EventArgs;
    using Interfaces;
    using IO;
    using Network;
    using Network.NetworkManagers;
    using Network.Servers;

    using EventArgs = System.EventArgs;

    public class BasicExamStatisticsCollector : IBasicExamStatisticsCollector
    {
        private readonly IServerNetworkManager networkManager;
        private readonly BasicExamServer server;
        private readonly IGameDataQuestionsSender gameDataQuestionsSender;
        private readonly IGameDataIterator gameDataIterator;
        
        private Dictionary<ISimpleQuestion, int> questionSpentTime = new Dictionary<ISimpleQuestion, int>();
        private Dictionary<ISimpleQuestion, List<Type>> questionsUsedJokers = new Dictionary<ISimpleQuestion, List<Type>>();
        private Dictionary<Type, int> jokersUsedTimes = new Dictionary<Type, int>();
        private List<ISimpleQuestion> correctAnsweredQuestions = new List<ISimpleQuestion>();

        private ISimpleQuestion lastQuestion = null;

        private string lastSelectedAnswer = string.Empty;
        
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
                if (this.QuestionsSpentTime.Count < 1)
                {
                    return 0;
                }
                else if (this.questionSpentTime.Count == 1)
                {
                    return this.QuestionsSpentTime.First()
                        .Value;
                }

                var correctAnsweredQuestionsCount = this.CorrectAnsweredQuestions.Count;
                var totalTimeSpentThinking = this.QuestionsSpentTime.Values.ToList().Sum();
                var avgSpentTimeThinking = totalTimeSpentThinking / this.QuestionsSpentTime.Values.Count;
                var score = (correctAnsweredQuestionsCount * 10000) / avgSpentTimeThinking;

                return score;
            }
        }
        public BasicExamStatisticsCollector(
           IServerNetworkManager networkManager,
           BasicExamServer server,
           IGameDataQuestionsSender gameDataQuestionsSender,
           IGameDataIterator gameDataIterator)
        {
            this.networkManager = networkManager;
            this.server = server;
            this.gameDataQuestionsSender = gameDataQuestionsSender;
            this.gameDataIterator = gameDataIterator;

            this.networkManager.CommandsManager.AddCommand("AnswerSelected", new SelectedAnswerCommand(this.OnReceivedAnswer));

            var jokersData = this.server.MainPlayerData.JokersData;

            jokersData.OnUsedJoker += this.OnUsedJoker;

            this.gameDataIterator.OnLoaded += this.OnGameDataLoaded;
            this.gameDataIterator.OnMarkIncrease += this.OnMarkIncrease;

            this.gameDataQuestionsSender.OnBeforeSend += this.OnBeforeSendQuestion;
            this.gameDataQuestionsSender.OnSentQuestion += this.OnSentQuestion;

            this.server.OnGameOver += this.OnGameOver;
        }

        private void OnGameOver(object sender, EventArgs args)
        {
            this.questionSpentTime[this.lastQuestion] = this.gameDataIterator.SecondsForAnswerQuestion - this.server.RemainingTimetoAnswerInSeconds;
        }

        private void OnReceivedAnswer(int connectionId, string answer)
        {
            this.lastSelectedAnswer = answer;
        }

        private void OnMarkIncrease(object sender, MarkEventArgs args)
        {
            this.EndMark = args.Mark;
        }

        private void OnUsedJoker(object sender, JokerTypeEventArgs args)
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

        private void OnGameDataLoaded(object sender, EventArgs args)
        {
            this.SetCurrentQuestion();
        }

        private void OnBeforeSendQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            if (args.QuestionType == QuestionRequestType.Next)
            {
                this.correctAnsweredQuestions.Add(this.lastQuestion);    
            }

            this.questionSpentTime[this.lastQuestion] = this.gameDataIterator.SecondsForAnswerQuestion - this.server.RemainingTimetoAnswerInSeconds;
        }

        private void OnSentQuestion(object sender, ServerSentQuestionEventArgs args)
        {
            this.SetCurrentQuestion();
        }

        private void OnLoadedCurrentQuestion(ISimpleQuestion question)
        {
            this.lastQuestion = question;
        }

        private void SetCurrentQuestion()
        {
            this.gameDataIterator.GetCurrentQuestion(this.OnLoadedCurrentQuestion);
        }
    }
}