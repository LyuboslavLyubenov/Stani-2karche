using AnswerEventArgs = EventArgs.AnswerEventArgs;
using AnswerTimeoutCommand = Commands.Client.AnswerTimeoutCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IQuestionUIController = Interfaces.Controllers.IQuestionUIController;
using ISimpleQuestion = Interfaces.ISimpleQuestion;
using LoadQuestionCommand = Commands.Client.LoadQuestionCommand;
using NetworkCommandData = Commands.NetworkCommandData;
using QuestionUIController = Controllers.QuestionUIController;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Audience
{
    using System;

    using Assets.Scripts.Interfaces;

    using Notifications;

    using StateMachine;

    using UnityEngine;

    public class ConnectedToServerState : IState
    {
        private readonly IClientNetworkManager networkManager;

        private readonly GameObject questionUI;
        private readonly IQuestionUIController questionUIController;

        public ConnectedToServerState(IClientNetworkManager networkManager, GameObject questionUI)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
            this.questionUI = questionUI;
            this.questionUIController = this.questionUI.GetComponent<QuestionUIController>();

            this.questionUIController.OnAnswerClick += OnAnswerClick;
        }

        private void OnAnswerClick(object sender, AnswerEventArgs args)
        {
            this.questionUI.SetActive(false);

            var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
            answerSelectedCommand.AddOption("Answer", args.Answer);
            this.networkManager.SendServerCommand(answerSelectedCommand); 
        }

        private void OnReceivedQuestion(ISimpleQuestion question, int timeToAnswer)
        {
            this.questionUI.SetActive(true);
            this.questionUIController.LoadQuestion(question);
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            var loadQuestionCommand = new LoadQuestionCommand(this.OnReceivedQuestion);
            this.networkManager.CommandsManager.AddCommand(loadQuestionCommand);

            var answerTimeoutCommand = new AnswerTimeoutCommand(this.questionUI, NotificationsController.Instance);
            this.networkManager.CommandsManager.AddCommand(answerTimeoutCommand);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.CommandsManager.RemoveCommand<LoadQuestionCommand>();
            this.networkManager.CommandsManager.RemoveCommand<AnswerTimeoutCommand>();

            this.questionUIController.OnAnswerClick -= this.OnAnswerClick;
        }
    }
}