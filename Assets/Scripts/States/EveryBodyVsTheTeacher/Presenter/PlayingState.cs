using GameEndCommand = Commands.Client.GameEndCommand;
using IAnswerPollResultRetriever = Interfaces.Network.Jokers.IAnswerPollResultRetriever;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;
using ISimpleQuestion = Interfaces.ISimpleQuestion;
using LoadQuestionCommand = Commands.Client.LoadQuestionCommand;
using SelectedAnswerCommand = Commands.Server.SelectedAnswerCommand;
using IAvailableElectionJokersUIController = Assets.Scripts.Interfaces.Controllers.EveryBodyVsTheTeacher.Presenter.IAvailableJokersUIController;
using ILeaderboardReceiver = Interfaces.Network.Leaderboard.ILeaderboardReceiver;
using SwitchedToNextRoundCommand = Scripts.Commands.EveryBodyVsTheTeacher.Shared.SwitchedToNextRoundCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{

    using System;

    using Interfaces.Controllers;

    using Interfaces;
    using StateMachine;

    using UnityEngine;

    public class PlayingState : IState
    {
        private readonly GameObject playingUI;

        private readonly IClientNetworkManager networkManager;

        private readonly IElectionQuestionUIController electionQuestionUIController;
        private readonly ISecondsRemainingUIController secondsRemainingUIController;
        private readonly IAvailableElectionJokersUIController availableJokersUIController;

        private readonly SwitchedToNextRoundCommand switchedToNextRoundCommand;

        private readonly IAnswerPollResultRetriever pollResultRetriever;

        private readonly GameEndCommand gameEndCommand;

        private readonly ILeaderboardReceiver leaderboardReceiver;

        public PlayingState(
            GameObject playingUI,
            IClientNetworkManager networkManager,
            IElectionQuestionUIController electionQuestionUIController,
            ISecondsRemainingUIController secondsRemainingUIController,
            IAvailableElectionJokersUIController availableJokersUIController,
            SwitchedToNextRoundCommand switchedToNextRoundCommand,
            IAnswerPollResultRetriever pollResultRetriever,
            GameEndCommand gameEndCommand,
            ILeaderboardReceiver leaderboardReceiver)
        {
            if (playingUI == null)
            {
                throw new ArgumentNullException("playingUI");
            }

            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (electionQuestionUIController == null)
            {
                throw new ArgumentNullException("electionQuestionUIController");
            }

            if (secondsRemainingUIController == null)
            {
                throw new ArgumentNullException("secondsRemainingUIController");
            }

            if (availableJokersUIController == null)
            {
                throw new ArgumentNullException("availableJokersUIController");
            }

            if (switchedToNextRoundCommand == null)
            {
                throw new ArgumentNullException("switchedToNextRoundCommand");
            }

            if (pollResultRetriever == null)
            {
                throw new ArgumentNullException("pollResultRetriever");
            }

            if (gameEndCommand == null)
            {
                throw new ArgumentNullException("gameEndCommand");
            }

            if (leaderboardReceiver == null)
            {
                throw new ArgumentNullException("leaderboardReceiver");
            }

            this.playingUI = playingUI;
            this.networkManager = networkManager;
            this.electionQuestionUIController = electionQuestionUIController;
            this.secondsRemainingUIController = secondsRemainingUIController;
            this.availableJokersUIController = availableJokersUIController;
            this.switchedToNextRoundCommand = switchedToNextRoundCommand;
            this.pollResultRetriever = pollResultRetriever;
            this.gameEndCommand = gameEndCommand;
            this.leaderboardReceiver = leaderboardReceiver;
        }

        private void OnReceivedQuestion(ISimpleQuestion question, int timeToAnswer)
        {
            this.electionQuestionUIController.LoadQuestion(question);

            this.secondsRemainingUIController.InvervalInSeconds = timeToAnswer;
            this.secondsRemainingUIController.StartTimer();
        }
        
        private void OnReceivedAnswer(int connectionId, string answer)
        {
            this.electionQuestionUIController.AddVoteFor(answer);
        }
        
        public void OnStateEnter(StateMachine stateMachine)
        {
            var loadQuestionCommand = new LoadQuestionCommand(this.OnReceivedQuestion);
            this.networkManager.CommandsManager.AddCommand(loadQuestionCommand);

            var answerSelectedCommand = new SelectedAnswerCommand(this.OnReceivedAnswer);
            this.networkManager.CommandsManager.AddCommand("AnswerSelected", answerSelectedCommand);
            
            this.networkManager.CommandsManager.AddCommand(this.switchedToNextRoundCommand);
            this.networkManager.CommandsManager.AddCommand(this.gameEndCommand);

            this.playingUI.SetActive(true);
        }
        
        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.CommandsManager.RemoveCommand<LoadQuestionCommand>();
            this.networkManager.CommandsManager.RemoveCommand("AnswerSelected");
            this.networkManager.CommandsManager.RemoveCommand<SwitchedToNextRoundCommand>();
            this.networkManager.CommandsManager.RemoveCommand<GameEndCommand>();

            this.availableJokersUIController.Dispose();
            this.pollResultRetriever.Dispose();
            this.leaderboardReceiver.Dispose();

            this.playingUI.SetActive(false);
        }
    }
}