namespace Assets.Scripts.Network
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Network.NetworkManagers;

    using Commands;
    using Commands.GameData;
    using EventArgs;
    using Exceptions;
    using Interfaces;
    using Utils;

    using UnityEngine;

    public class RemoteGameDataIterator : MonoBehaviour, IGameDataIterator
    {
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

        public event EventHandler OnLoaded;
        public event EventHandler<MarkEventArgs> OnMarkIncrease;

        private ClientNetworkManager networkManager;

        private readonly Stack<PendingQuestionRequest> currentQuestionRequests = new Stack<PendingQuestionRequest>();

        private readonly Stack<PendingQuestionRequest> nextQuestionRequests = new Stack<PendingQuestionRequest>();

        private ISimpleQuestion currentQuestionCache = null;

        public RemoteGameDataIterator(ClientNetworkManager networkManager)
        {
            this.networkManager = networkManager;
            this.InitializeCommands();
        }

        private void InitializeCommands()
        {
            this.networkManager.CommandsManager.AddCommand(new LoadedGameDataCommand(this._OnLoadedGameData));
            this.networkManager.CommandsManager.AddCommand(new GameDataQuestionCommand(this.OnReceivedQuestion));
            this.networkManager.CommandsManager.AddCommand(new GameDataMarkCommand(this.OnReceivedMark));
            this.networkManager.CommandsManager.AddCommand(new GameDataNoMoreQuestionsCommand(this.OnNoMoreQuestions));
        }

        private void _OnLoadedGameData(string levelCategory)
        {
            this.LevelCategory = levelCategory;

            if (this.OnLoaded != null)
            {
                this.OnLoaded(this, EventArgs.Empty);
            }
        }

        private void OnNoMoreQuestions()
        {
            for (int i = 0; i < this.nextQuestionRequests.Count; i++)
            {
                var questionRequest = this.nextQuestionRequests.Pop();
                questionRequest.OnException(new NoMoreQuestionsException());
            }
        }

        private void OnReceivedMark(int mark)
        {
            this.CurrentMark = mark;

            if (this.OnMarkIncrease != null)
            {
                this.OnMarkIncrease(this, new MarkEventArgs(mark));
            }
        }

        private void OnReceivedQuestion(QuestionRequestType requestType, ISimpleQuestion question, int remainingQuestionsToNextMark, int secondsForAnswerQuestion)
        {
            PendingQuestionRequest questionRequest = null;

            switch (requestType)
            {
                case QuestionRequestType.Current:
                    questionRequest = this.currentQuestionRequests.PopOrDefault();
                    break;
                case QuestionRequestType.Next:
                    questionRequest = this.nextQuestionRequests.PopOrDefault();
                    break;
            }

            if (questionRequest == null)
            {
                Debug.LogWarning("Received question from server but cant find request source.");
                return;
            }
            
            this.currentQuestionCache = question;
            this.RemainingQuestionsToNextMark = remainingQuestionsToNextMark;
            this.SecondsForAnswerQuestion = secondsForAnswerQuestion;

            questionRequest.OnLoaded(question);
        }

        private void SendGetQuestionRequest(QuestionRequestType requestType)
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

                var requestData = new PendingQuestionRequest((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.currentQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                if (onError != null)
                {
                    onError(ex);
                }
            }
        }

        public void GetNextQuestion(Action<ISimpleQuestion> onSuccessfullyLoaded, Action<Exception> onError = null)
        {
            try
            {
                this.SendGetQuestionRequest(QuestionRequestType.Next);

                var requestData = new PendingQuestionRequest((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.nextQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                if (onError != null)
                {
                    onError(ex);
                }
            }
        }
    }

    public enum QuestionRequestType
    {
        Current,
        Next
    }
}