using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;
using ISimpleQuestion = Interfaces.ISimpleQuestion;
using LoadQuestionCommand = Commands.Client.LoadQuestionCommand;
using SelectedAnswerCommand = Commands.Server.SelectedAnswerCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{
    using System;

    using Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared;
    using Assets.Scripts.Interfaces.Controllers;

    using Interfaces;
    using StateMachine;

    using UnityEngine;

    public class PlayingState : IState
    {        
        private readonly IClientNetworkManager networkManager;
        private readonly IElectionQuestionUIController electionQuestionUiController;
        private readonly ISecondsRemainingUIController secondsRemainingUiController;
        private readonly IAvailableJokersUIController availableJokersUiController;
        private readonly IChangedRoundUIController changedRoundUiController;

        private readonly GameObject changedRoundUi;

        public PlayingState(
            IClientNetworkManager networkManager,
            IElectionQuestionUIController electionQuestionUiController,
            ISecondsRemainingUIController secondsRemainingUiController,
            IAvailableJokersUIController availableJokersUiController,
            IChangedRoundUIController changedRoundUiController,
            GameObject changedRoundUi)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (electionQuestionUiController == null)
            {
                throw new ArgumentNullException("electionQuestionUiController");
            }

            if (secondsRemainingUiController == null)
            {
                throw new ArgumentNullException("secondsRemainingUiController");
            }

            if (availableJokersUiController == null)
            {
                throw new ArgumentNullException("availableJokersUiController");
            }

            if (changedRoundUiController == null)
            {
                throw new ArgumentNullException("changedRoundUiController");
            }

            if (changedRoundUi == null)
            {
                throw new ArgumentNullException("changedRoundUi");
            }

            this.networkManager = networkManager;
            this.electionQuestionUiController = electionQuestionUiController;
            this.secondsRemainingUiController = secondsRemainingUiController;
            this.availableJokersUiController = availableJokersUiController;
            this.changedRoundUiController = changedRoundUiController;
            this.changedRoundUi = changedRoundUi;
        }

        private void OnReceivedQuestion(ISimpleQuestion question, int timeToAnswer)
        {
            this.electionQuestionUiController.LoadQuestion(question);
            this.secondsRemainingUiController.InvervalInSeconds = timeToAnswer;
            this.secondsRemainingUiController.StartTimer();
        }

        private void OnReceivedAnswer(int connectionId, string answer)
        {
            this.electionQuestionUiController.AddVoteFor(answer);
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            var loadQuestionCommand = new LoadQuestionCommand(this.OnReceivedQuestion);
            this.networkManager.CommandsManager.AddCommand(loadQuestionCommand);

            var answerSelectedCommand = new SelectedAnswerCommand(this.OnReceivedAnswer);
            this.networkManager.CommandsManager.AddCommand("AnswerSelected", answerSelectedCommand);

            var switchedToRoundCommand =
                new SwitchedToNextRoundCommand(this.changedRoundUi, this.changedRoundUiController);
            this.networkManager.CommandsManager.AddCommand(switchedToRoundCommand);

        }
        
        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.CommandsManager.RemoveCommand<LoadQuestionCommand>();
        }
    }
}
