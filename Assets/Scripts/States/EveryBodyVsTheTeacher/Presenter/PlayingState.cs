﻿using GameEndCommand = Commands.Client.GameEndCommand;
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
    using Interfaces.Controllers;

    using Interfaces;
    using StateMachine;

    using UnityEngine;

    public class PlayingState : IState
    {        
        private readonly IClientNetworkManager networkManager;

        private readonly IElectionQuestionUIController electionQuestionUIController;
        private readonly ISecondsRemainingUIController secondsRemainingUIController;
        private readonly IAvailableElectionJokersUIController availableJokersUIController;

        private readonly IChangedRoundUIController changedRoundUIController;
        private readonly GameObject changedRoundUI;

        private readonly IAnswerPollResultRetriever pollResultRetriever;

        private readonly GameObject endGameUI;

        private readonly GameObject leaderboardUI;

        private readonly ILeaderboardReceiver leaderboardReceiver;

        public PlayingState(
            IClientNetworkManager networkManager,
            IElectionQuestionUIController electionQuestionUIController,
            ISecondsRemainingUIController secondsRemainingUIController,
            IAvailableElectionJokersUIController availableJokersUIController,
            IChangedRoundUIController changedRoundUIController,
            GameObject changedRoundUI,
            IAnswerPollResultRetriever pollResultRetriever,
            GameObject endGameUI,
            GameObject leaderboardUI,
            ILeaderboardReceiver leaderboardReceiver)
        {
            
            this.networkManager = networkManager;
            this.electionQuestionUIController = electionQuestionUIController;
            this.secondsRemainingUIController = secondsRemainingUIController;
            this.availableJokersUIController = availableJokersUIController;
            this.changedRoundUIController = changedRoundUIController;
            this.changedRoundUI = changedRoundUI;
            this.pollResultRetriever = pollResultRetriever;
            this.endGameUI = endGameUI;
            this.leaderboardUI = leaderboardUI;
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

            var switchedToRoundCommand =
                new SwitchedToNextRoundCommand(this.changedRoundUI, this.changedRoundUIController);
            this.networkManager.CommandsManager.AddCommand(switchedToRoundCommand);
            
            var gameEndCommand = new GameEndCommand(this.endGameUI, this.leaderboardUI, this.leaderboardReceiver);
            this.networkManager.CommandsManager.AddCommand(gameEndCommand);
        }
        
        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.CommandsManager.RemoveCommand<LoadQuestionCommand>();
            this.networkManager.CommandsManager.RemoveCommand<SelectedAnswerCommand>();
            this.networkManager.CommandsManager.RemoveCommand<SwitchedToNextRoundCommand>();
            this.networkManager.CommandsManager.RemoveCommand<GameEndCommand>();

            this.availableJokersUIController.Dispose();
            this.pollResultRetriever.Dispose();
            this.leaderboardReceiver.Dispose(); 
        }
    }
}