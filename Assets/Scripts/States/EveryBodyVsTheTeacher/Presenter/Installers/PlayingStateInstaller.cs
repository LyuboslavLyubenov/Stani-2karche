using AskAudienceJoker = Jokers.AskAudienceJoker;
using AskPlayerQuestionResultRetriever = Jokers.Retrievers.AskPlayerQuestionResultRetriever;
using AudienceAnswerPollResultRetriever = Jokers.Retrievers.AudienceAnswerPollResultRetriever;
using ChangedRoundUIController = Scripts.Controllers.EveryBodyVsTheTeacher.ChangedRoundUIController;
using ElectionQuestionUIController = Controllers.ElectionQuestionUIController;
using GameEndCommand = Commands.Client.GameEndCommand;
using IAnswerPollResultRetriever = Interfaces.Network.Jokers.IAnswerPollResultRetriever;
using IAskClientQuestionResultRetriever = Interfaces.Network.Jokers.IAskClientQuestionResultRetriever;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;
using ILeaderboardReceiver = Interfaces.Network.Leaderboard.ILeaderboardReceiver;
using IQuestionUIController = Interfaces.Controllers.IQuestionUIController;
using JokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.JokerElectionUIController;
using KalitkoJokerContainerUIController = Controllers.EveryBodyVsTheTeacher.Jokers.KalitkoJokerContainerUIController;
using LeaderboardReceiver = Network.Leaderboard.LeaderboardReceiver;
using SecondsRemainingUIController = Controllers.SecondsRemainingUIController;
using SwitchedToNextRoundCommand = Scripts.Commands.EveryBodyVsTheTeacher.Shared.SwitchedToNextRoundCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{

    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;

    using UnityEngine;

    using Zenject.Source.Install;

    public class PlayingStateInstaller : MonoInstaller
    {
        [SerializeField]
        private ElectionQuestionUIController electionQuestionUIController;

        [SerializeField]
        private GameObject playingUI;

        [SerializeField]
        private GameObject kalitkoJokerUI;

        [SerializeField]
        private GameObject loadingUI;

        [SerializeField]
        private GameObject secondsRemainingUI;

        [SerializeField]
        private GameObject notReceivedAnswerUI;

        [SerializeField]
        private GameObject playerAnswerUI;

        [SerializeField]
        private GameObject waitingToAnswerUI;

        [SerializeField]
        private GameObject audienceAnswerUI;

        [SerializeField]
        private GameObject endGameUI;

        [SerializeField]
        private GameObject leaderboardUI;

        [SerializeField]
        private GameObject changedRoundUI;

        [SerializeField]
        private GameObject jokerElectionUI;

        [SerializeField]
        private GameObject successfullyActivatedJokerUI;

        [SerializeField]
        private GameObject unsuccessfullyActivatedJokerUI;

        private void BindGameEndCommand(IClientNetworkManager networkManager)
        {
            var leaderboardReceiver = new LeaderboardReceiver(networkManager, 10);
            var gameEndCommand = new GameEndCommand(this.endGameUI, this.leaderboardUI, leaderboardReceiver);

            this.Container.Bind<GameEndCommand>()
                .FromInstance(gameEndCommand)
                .AsSingle();
        }

        private void BindSwitchedToNextRoundCommand()
        {
            var changedRoundUIController = this.changedRoundUI.GetComponent<ChangedRoundUIController>();
            var switchedToRoundCommand = new SwitchedToNextRoundCommand(this.changedRoundUI, changedRoundUIController);

            this.Container.Bind<SwitchedToNextRoundCommand>()
                .FromInstance(switchedToRoundCommand)
                .AsSingle();
        }

        private void BindHelpFromFriendJoker(
            IJokerElectionCommandsBinder binder,
            IClientNetworkManager networkManager,
            IAskClientQuestionResultRetriever askClientQuestionResultRetriever
            )
        {
            //TODO:
            //var helpFromFriendJoker = 
            //    new HelpFromFriendJoker(
            //        networkManager, 
            //        askClientQuestionResultRetriever, )
            //binder.Bind();
        }

        private void BindConsultWithTeacherJoker(
            IJokerElectionCommandsBinder binder,
            IClientNetworkManager networkManager,
            GameObject loadingUI,
            GameObject electionQuestionUI)
        {
            var electionQuestionUIController = electionQuestionUI.GetComponent<ElectionQuestionUIController>();
            var joker = new ConsultWithTeacherJoker(
                networkManager,
                loadingUI,
                electionQuestionUI,
                electionQuestionUIController);
            binder.Bind(joker);

            this.Container.Bind<ConsultWithTeacherJoker>()
                .FromInstance(joker)
                .AsSingle();
        }

        private void BindKalitkoJoker(
            IJokerElectionCommandsBinder binder,
            IClientNetworkManager networkManager,
            GameObject kalitkoJokerUI)
        {
            var kalitkoJokerUIController = kalitkoJokerUI.GetComponent<KalitkoJokerContainerUIController>();
            var joker = new PresenterKalitkoJoker(networkManager, kalitkoJokerUIController, kalitkoJokerUI);
            binder.Bind(joker);

            this.Container.Bind<PresenterKalitkoJoker>()
                .FromInstance(joker)
                .AsSingle();
        }

        private void BindAskAudienceJoker(
            IJokerElectionCommandsBinder binder,
            IAnswerPollResultRetriever answerPollResultRetriever,
            GameObject waitingUI,
            GameObject audienceAnswerUI,
            GameObject loadingUI)
        {
            var joker = new AskAudienceJoker(answerPollResultRetriever, waitingUI, audienceAnswerUI, loadingUI);
            binder.Bind(joker);
        }

        private void BindLittleIsBetterThanNothingJoker(
            IJokerElectionCommandsBinder binder,
            IClientNetworkManager networkManager,
            IQuestionUIController questionUIController)
        {
            var joker = new LittleIsBetterThanNothingJoker(networkManager, questionUIController);
            binder.Bind(joker);
        }

        private void BindTrustRandomPersonJoker(
            IJokerElectionCommandsBinder binder,
            IClientNetworkManager networkManager,
            GameObject loadingUI,
            GameObject secondsRemainingUI,
            GameObject notReceivedAnswerUI,
            GameObject playerAnswerUI)
        {
            var secondsRemainingUIController = secondsRemainingUI.GetComponent<ISecondsRemainingUIController>();
            var playerAnswerUIController = playerAnswerUI.GetComponent<IPlayerAnswerUIController>();
            var joker =
                new TrustRandomPersonJoker(
                    networkManager,
                    loadingUI,
                    secondsRemainingUI,
                    secondsRemainingUIController,
                    notReceivedAnswerUI,
                    playerAnswerUI,
                    playerAnswerUIController);
            binder.Bind(joker);
        }
        
        private void BindAvailableJokersUIControllerDependencies(IClientNetworkManager networkManager)
        {
            var jokerElectionUIController = this.jokerElectionUI.GetComponent<JokerElectionUIController>();
            var electionJokersBinder =
                new JokerJokerElectionUiCommandsBinder(
                    networkManager,
                    jokerElectionUIController,
                    this.jokerElectionUI,
                    this.successfullyActivatedJokerUI,
                    this.unsuccessfullyActivatedJokerUI);

            this.Container.Bind<IJokerElectionCommandsBinder>()
                .FromInstance(electionJokersBinder)
                .AsSingle();
        }

        private void BindPlayingStateDependencies()
        {
            this.Container.Bind<IElectionQuestionUIController>()
                .FromInstance(this.electionQuestionUIController)
                .AsSingle();

            var secondsRemainingUIController = this.secondsRemainingUI.GetComponent<SecondsRemainingUIController>();
            this.Container.Bind<ISecondsRemainingUIController>()
                .FromInstance(secondsRemainingUIController)
                .AsSingle();

            this.Container.Bind<IAnswerPollResultRetriever>()
                .To<AudienceAnswerPollResultRetriever>()
                .AsSingle();

            this.Container.Bind<int>()
                .FromInstance(5)
                .WhenInjectedInto<ILeaderboardReceiver>();

            this.Container.Bind<ILeaderboardReceiver>()
                .To<LeaderboardReceiver>()
                .AsSingle();
        }

        public override void InstallBindings()
        {
            var networkManager = this.Container.Resolve<IClientNetworkManager>();
            
            this.BindGameEndCommand(networkManager);
            this.BindSwitchedToNextRoundCommand();
            this.BindAvailableJokersUIControllerDependencies(networkManager);
            this.BindPlayingStateDependencies();

            this.Container.Bind<int>()
                .FromInstance(5)
                .WhenInjectedInto<IAskClientQuestionResultRetriever>();

            this.Container.Bind<IAskClientQuestionResultRetriever>()
                .To<AskPlayerQuestionResultRetriever>()
                .AsSingle();

            var askPlayerQuestionResultRetriever = this.Container.Resolve<IAskClientQuestionResultRetriever>();

            var electionBinder = this.Container.Resolve<IJokerElectionCommandsBinder>();
            this.BindHelpFromFriendJoker(electionBinder, networkManager, askPlayerQuestionResultRetriever);
            this.BindConsultWithTeacherJoker(electionBinder, networkManager, this.loadingUI, this.electionQuestionUIController.gameObject);
            this.BindKalitkoJoker(electionBinder, networkManager, this.kalitkoJokerUI);
            
            var answerPollResultRetriever = this.Container.Resolve<IAnswerPollResultRetriever>();
            this.BindAskAudienceJoker(
                electionBinder, 
                answerPollResultRetriever, 
                this.waitingToAnswerUI, 
                this.audienceAnswerUI, 
                this.loadingUI);

            this.BindLittleIsBetterThanNothingJoker(electionBinder, networkManager, this.electionQuestionUIController);
            this.BindTrustRandomPersonJoker(
                electionBinder, 
                networkManager, 
                this.loadingUI, 
                this.secondsRemainingUI, 
                this.notReceivedAnswerUI, 
                this.playerAnswerUI);

            this.Container.Bind<PlayingState>()
                .FromMethod(
                    (context) =>
                        {
                            var switchedToNextRoundCommand = context.Container.Resolve<SwitchedToNextRoundCommand>();
                            var pollResultRetriever = context.Container.Resolve<IAnswerPollResultRetriever>();
                            var gameEndCommand = context.Container.Resolve<GameEndCommand>();
                            var leaderboardReceiver = context.Container.Resolve<ILeaderboardReceiver>();
                            return new PlayingState(
                                this.playingUI,
                                networkManager,
                                this.electionQuestionUIController.gameObject,
                                this.secondsRemainingUI,
                                switchedToNextRoundCommand,
                                pollResultRetriever,
                                gameEndCommand,
                                leaderboardReceiver);
                        });
        }
    }
}