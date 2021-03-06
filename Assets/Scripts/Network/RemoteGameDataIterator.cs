﻿namespace Network
{
    using System;
    using System.Collections.Generic;

    using Commands;
    using Commands.GameData;

    using DTOs;

    using EventArgs;

    using Exceptions;

    using Extensions;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    public class RemoteGameDataIterator : IGameDataIterator
    {
        public event EventHandler OnLoaded = delegate {};

        public event EventHandler OnNextQuestionLoaded = delegate {};

        public event EventHandler<MarkEventArgs> OnMarkIncrease = delegate {};

        private IClientNetworkManager networkManager;

        private readonly Stack<GetQuestionRequest> currentQuestionRequests = new Stack<GetQuestionRequest>();
        private readonly Stack<GetQuestionRequest> nextQuestionRequests = new Stack<GetQuestionRequest>();

        private ISimpleQuestion currentQuestionCache = null;

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

        public bool Loaded
        {
            get; private set;
        }
        
        public RemoteGameDataIterator(IClientNetworkManager networkManager)
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
            this.Loaded = true;

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
            GetQuestionRequest questionRequest = null;

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

            if (requestType == QuestionRequestType.Next)
            {
                this.OnNextQuestionLoaded(this, EventArgs.Empty);
            }
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

                var requestData = new GetQuestionRequest((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.currentQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
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

                var requestData = new GetQuestionRequest((question) => onSuccessfullyLoaded(question), (error) => onError(error));
                this.nextQuestionRequests.Push(requestData);
            }
            catch (Exception ex)
            {
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