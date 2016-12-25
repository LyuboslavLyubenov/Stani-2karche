namespace Assets.Scripts.Network
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.GameData;
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Exceptions;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Utils;

    using UnityEngine;

    public class RemoteGameData : IGameData
    {
        public EventHandler OnLoaded
        {
            get;
            set;
        }

        public EventHandler<MarkEventArgs> OnMarkIncrease
        {
            get;
            set;
        }

        public bool Loaded
        {
            get
            {
                return this.loaded; 
            }
        }

        public int RemainingQuestionsToNextMark
        {
            get;
            private set;
        }

        public int CurrentMark
        {
            get;
            private set;
        }

        public int SecondsForAnswerQuestion
        {
            get;
            private set;
        }

        public string LevelCategory
        {
            get;
            private set;
        }

        ClientNetworkManager networkManager;

        bool loaded = false;

        Stack<PendingQuestionRequestData> currentQuestionRequests = new Stack<PendingQuestionRequestData>();
        Stack<PendingQuestionRequestData> nextQuestionRequests = new Stack<PendingQuestionRequestData>();
        Stack<PendingQuestionRequestData> randomQuestionRequests = new Stack<PendingQuestionRequestData>();

        ISimpleQuestion currentQuestionCache = null;

        public RemoteGameData(ClientNetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.InitializeCommands();
        }

        void InitializeCommands()
        {
            this.networkManager.CommandsManager.AddCommand(new LoadedGameDataCommand(this._OnLoadedGameData));
            this.networkManager.CommandsManager.AddCommand(new GameDataQuestionCommand(this.OnReceivedQuestion));
            this.networkManager.CommandsManager.AddCommand(new GameDataMarkCommand(this.OnReceivedMark));
            this.networkManager.CommandsManager.AddCommand(new GameDataNoMoreQuestionsCommand(this.OnNoMoreQuestions));
        }

        void _OnLoadedGameData(string levelCategory)
        {
            this.LevelCategory = levelCategory;

            if (this.OnLoaded != null)
            {
                this.OnLoaded(this, EventArgs.Empty);    
            }
        }

        void OnNoMoreQuestions()
        {
            for (int i = 0; i < this.nextQuestionRequests.Count; i++)
            {
                var questionRequest = this.nextQuestionRequests.Pop();
                questionRequest.OnException(new NoMoreQuestionsException());
            }
        }

        void LoadDataFromServer()
        {
            this.GetCurrentQuestion((question) =>
                {
                    this.currentQuestionCache = question;
                }, 
                Debug.LogException);
        }

        void OnReceivedMark(int mark)
        {
            this.CurrentMark = mark;

            if (this.OnMarkIncrease != null)
            {
                this.OnMarkIncrease(this, new MarkEventArgs(mark));    
            }
        }

        void OnReceivedQuestion(QuestionRequestType requestType, ISimpleQuestion question, int remainingQuestionsToNextMark, int secondsForAnswerQuestion)
        {
            PendingQuestionRequestData questionRequest = null;

            switch (requestType)
            {
                case QuestionRequestType.Current:
                    questionRequest = this.currentQuestionRequests.PopOrDefault();
                    break;
                case QuestionRequestType.Next:
                    questionRequest = this.nextQuestionRequests.PopOrDefault();
                    break;
                case QuestionRequestType.Random:
                    questionRequest = this.randomQuestionRequests.PopOrDefault();
                    break;
            }

            if (questionRequest == null)
            {
                Debug.LogWarning("Received question from server but cant find request source.");
                return;
            }
          
            if (requestType != QuestionRequestType.Random)
            {
                this.currentQuestionCache = question;
            }

            this.RemainingQuestionsToNextMark = remainingQuestionsToNextMark;
            this.SecondsForAnswerQuestion = secondsForAnswerQuestion;

            questionRequest.OnLoaded(question);
        }

        void SendGetQuestionRequest(QuestionRequestType requestType)
        {
            var commandData = new NetworkCommandData("GameDataGetQuestion");
            var requestTypeStr = Enum.GetName(typeof(QuestionRequestType), requestType);
            commandData.AddOption("RequestType", requestTypeStr);
            this.networkManager.SendServerCommand(commandData);
        }

        public void GetCurrentQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            if (this.currentQuestionCache != null)
            {
                onSuccessfullyLoaded(this.currentQuestionCache);
                return;
            }

            try
            {
                this.SendGetQuestionRequest(QuestionRequestType.Current);

                var requestData = new PendingQuestionRequestData((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.currentQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    onError(ex);
                }
                else
                {
                    throw;
                }
            }
        }

        public void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            try
            {
                this.SendGetQuestionRequest(QuestionRequestType.Next);

                var requestData = new PendingQuestionRequestData((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.nextQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    onError(ex);
                }
                else
                {
                    throw;
                }
            }
        }

        public void GetRandomQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            try
            {
                this.SendGetQuestionRequest(QuestionRequestType.Random);

                var requestData = new PendingQuestionRequestData((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.randomQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    onError(ex);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    public enum QuestionRequestType
    {
        Current,
        Next,
        Random
    }

}