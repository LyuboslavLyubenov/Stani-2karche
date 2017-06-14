﻿using AnswerEventArgs = EventArgs.AnswerEventArgs;
using AnswerTimeoutCommand = Commands.Client.AnswerTimeoutCommand;
using EnoughPlayersToStartGameCommand = Commands.EveryBodyVsTheTeacher.EnoughPlayersToStartGameCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IQuestionUIController = Interfaces.Controllers.IQuestionUIController;
using ISimpleQuestion = Interfaces.ISimpleQuestion;
using LoadQuestionCommand = Commands.Client.LoadQuestionCommand;
using NetworkCommandData = Commands.NetworkCommandData;
using NotEnoughPlayersToStartGameCommand = Commands.EveryBodyVsTheTeacher.NotEnoughPlayersToStartGameCommand;
using QuestionUIController = Controllers.QuestionUIController;
using StartGameRequestCommand = Commands.EveryBodyVsTheTeacher.StartGameRequestCommand;
using ThreadUtils = Utils.ThreadUtils;
using TimerUtils = Utils.TimerUtils;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer
{
    using System;
    using System.Collections;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared;
    using Assets.Scripts.Interfaces;

    using Notifications;

    using StateMachine;

    using UnityEngine;
    using UnityEngine.UI;

    public class ConnectedToServerState : IState
    {
        private readonly IClientNetworkManager networkManager;
        private readonly Button gameStartButton;
        private readonly GameObject questionUI;
        private readonly GameObject playingUI;
        private readonly IQuestionUIController questionUIController;

        public ConnectedToServerState(
            IClientNetworkManager networkManager,
            Button gameStartButton,
            GameObject questionUI,
            GameObject playingUI)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameStartButton == null)
            {
                throw new ArgumentNullException("gameStartButton");
            }

            if (questionUI == null)
            {
                throw new ArgumentNullException("questionUI");
            }

            if (playingUI == null)
            {
                throw new ArgumentNullException("playingUI");
            }

            this.networkManager = networkManager;
            this.gameStartButton = gameStartButton;
            this.questionUI = questionUI;
            this.playingUI = playingUI;
            this.questionUIController = this.questionUI.GetComponent<QuestionUIController>();

            this.gameStartButton.onClick.AddListener(this.OnRequestedGameStart);
        }

        private void OnRequestedGameStart()
        {
            var requestStartGameCommand = NetworkCommandData.From<StartGameRequestCommand>();
            this.networkManager.SendServerCommand(requestStartGameCommand);
            this.gameStartButton.gameObject.SetActive(false);
        }

        private IEnumerator LoadQuestionCoroutine(ISimpleQuestion question)
        {
            this.questionUI.SetActive(true);

            yield return null;

            this.questionUIController.LoadQuestion(question);
        }

        private void OnReceivedQuestion(ISimpleQuestion question, int timeToAnswerInSeconds)
        {
            ThreadUtils.Instance.RunOnMainThread(this.LoadQuestionCoroutine(question));
            
            var timer = TimerUtils.ExecuteAfter(timeToAnswerInSeconds, () => this.questionUI.SetActive(false));
            timer.AutoDispose = true;
            timer.RunOnUnityThread = true;
            timer.Start();
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.gameStartButton.gameObject.SetActive(false);

            var enoughPlayersCommand = new EnoughPlayersToStartGameCommand(this.gameStartButton);
            this.networkManager.CommandsManager.AddCommand(enoughPlayersCommand);

            var notEnoughPlayersCommand = new NotEnoughPlayersToStartGameCommand(this.gameStartButton);
            this.networkManager.CommandsManager.AddCommand(notEnoughPlayersCommand);

            var gameStartedCommand = new GameStartedCommand(this.playingUI);
            this.networkManager.CommandsManager.AddCommand(gameStartedCommand);

            var loadQuestionCommand = new LoadQuestionCommand(this.OnReceivedQuestion);
            this.networkManager.CommandsManager.AddCommand(loadQuestionCommand);

            var answerTimeoutCommand = new AnswerTimeoutCommand(this.questionUI, NotificationsController.Instance);
            this.networkManager.CommandsManager.AddCommand(answerTimeoutCommand);

            this.questionUIController.OnAnswerClick += OnAnswerClick;
        }

        private void OnAnswerClick(object sender, AnswerEventArgs args)
        {
            this.questionUI.SetActive(false);

            var selectedAnswerCommand = new NetworkCommandData("AnswerSelected");
            selectedAnswerCommand.AddOption("Answer", args.Answer);
            this.networkManager.SendServerCommand(selectedAnswerCommand);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.CommandsManager.RemoveCommand<EnoughPlayersToStartGameCommand>();
            this.networkManager.CommandsManager.RemoveCommand<NotEnoughPlayersToStartGameCommand>();
            this.networkManager.CommandsManager.RemoveCommand<GameStartedCommand>();
            this.networkManager.CommandsManager.RemoveCommand<LoadQuestionCommand>();
            this.networkManager.CommandsManager.RemoveCommand<AnswerTimeoutCommand>();
        }
    }
}