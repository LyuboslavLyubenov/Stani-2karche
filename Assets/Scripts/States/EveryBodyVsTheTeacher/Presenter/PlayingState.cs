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
using ThreadUtils = Utils.ThreadUtils;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{
    using System;
    using System.Collections;

    using Interfaces.Controllers;

    using Interfaces;
    using StateMachine;

    using UnityEngine;

    public class PlayingState : IState
    {
        private readonly GameObject playingUI;

        private readonly IClientNetworkManager networkManager;
        
        private readonly GameObject electionQuestionUI;
        private readonly IElectionQuestionUIController electionQuestionUIController;
        private readonly GameObject secondsRemainingUI;
        private readonly ISecondsRemainingUIController secondsRemainingUIController;

        private readonly SwitchedToNextRoundCommand switchedToNextRoundCommand;

        private readonly IAnswerPollResultRetriever pollResultRetriever;

        private readonly GameEndCommand gameEndCommand;

        private readonly ILeaderboardReceiver leaderboardReceiver;
        
        public PlayingState(
            GameObject playingUI,
            IClientNetworkManager networkManager,
            GameObject electionQuestionUI,
            GameObject secondsRemainingUI,
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

            if (electionQuestionUI == null)
            {
                throw new ArgumentNullException("electionQuestionUI");
            }

            if (secondsRemainingUI == null)
            {
                throw new ArgumentNullException("secondsRemainingUI");
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
            this.electionQuestionUI = electionQuestionUI;
            this.electionQuestionUIController =
                electionQuestionUI.GetComponent(typeof(IElectionQuestionUIController)) as IElectionQuestionUIController;
            this.secondsRemainingUI = secondsRemainingUI;
            this.secondsRemainingUIController = 
                secondsRemainingUI.GetComponent(typeof(ISecondsRemainingUIController)) as ISecondsRemainingUIController;
            this.switchedToNextRoundCommand = switchedToNextRoundCommand;
            this.pollResultRetriever = pollResultRetriever;
            this.gameEndCommand = gameEndCommand;
            this.leaderboardReceiver = leaderboardReceiver;
        }

        private IEnumerator LoadQuestionAndSecondsRemainingCoroutine(ISimpleQuestion question, int timeToAnswer)
        {
            this.electionQuestionUI.SetActive(true);
            this.secondsRemainingUI.SetActive(true);

            yield return null;

            this.electionQuestionUIController.LoadQuestion(question);

            if (this.secondsRemainingUIController.Running)
            {
                this.secondsRemainingUIController.StopTimer();
            }

            this.secondsRemainingUIController.InvervalInSeconds = timeToAnswer;
            this.secondsRemainingUIController.StartTimer();
        }

        private void OnReceivedQuestion(ISimpleQuestion question, int timeToAnswer)
        {
            ThreadUtils.Instance.RunOnMainThread(this.LoadQuestionAndSecondsRemainingCoroutine(question, timeToAnswer));
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
            
            this.pollResultRetriever.Dispose();
            this.leaderboardReceiver.Dispose();

            this.playingUI.SetActive(false);
        }
    }
}