using AddAskAudienceJokerCommand = Commands.Jokers.Add.AddAskAudienceJokerCommand;
using AudienceAnswerPollResultRetriever = Jokers.Retrievers.AudienceAnswerPollResultRetriever;
using ChangedRoundUIController = Scripts.Controllers.EveryBodyVsTheTeacher.ChangedRoundUIController;
using ElectionQuestionUIController = Controllers.ElectionQuestionUIController;
using FriendAnswerUIController = Controllers.FriendAnswerUIController;
using GameEndCommand = Commands.Client.GameEndCommand;
using IAnswerPollResultRetriever = Interfaces.Network.Jokers.IAnswerPollResultRetriever;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;
using ILeaderboardReceiver = Interfaces.Network.Leaderboard.ILeaderboardReceiver;
using JokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.JokerElectionUIController;
using KalitkoJokerContainerUIController = Controllers.EveryBodyVsTheTeacher.Jokers.KalitkoJokerContainerUIController;
using LeaderboardReceiver = Network.Leaderboard.LeaderboardReceiver;
using SecondsRemainingUIController = Controllers.SecondsRemainingUIController;
using SwitchedToNextRoundCommand = Scripts.Commands.EveryBodyVsTheTeacher.Shared.SwitchedToNextRoundCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{
    using Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter;
    using Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter.SecondRound;
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Presenter;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;

    using UnityEngine;

    using Zenject.Source.Install;

    public class PlayingStateInstaller : MonoInstaller
    {
        [SerializeField]
        private AvailableJokersUIController availableJokersUIController;

        [SerializeField]
        private ElectionQuestionUIController electionQuestionUIController;
        
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

        private void BindAddConsultWithTheTeacherJokerCommand(IClientNetworkManager networkManager)
        {
            var addJokerCommand = 
                new AddConsultWithTheTeacherJokerCommand(
                    this.availableJokersUIController,
                    networkManager,
                    this.electionQuestionUIController);

            this.Container.Bind<AddConsultWithTheTeacherJokerCommand>()
                .FromInstance(addJokerCommand)
                .AsSingle();
        }

        private void BindAddKalitkoJokerCommand(IClientNetworkManager networkManager)
        {
            var kalitkoJokerUIController = this.kalitkoJokerUI.GetComponent<KalitkoJokerContainerUIController>();

            var addJokerCommand = 
                new AddKalitkoJokerCommand(
                    this.availableJokersUIController,
                    networkManager,
                    kalitkoJokerUIController,
                    this.kalitkoJokerUI);

            this.Container.Bind<AddKalitkoJokerCommand>()
                .FromInstance(addJokerCommand)
                .AsSingle();
        }

        private void BindAddTrustRandomPersonJokerCommand(IClientNetworkManager networkManager)
        {
            var secondsRemainingUIController = this.secondsRemainingUI.GetComponent<SecondsRemainingUIController>();
            var playerAnswerUIController = this.playerAnswerUI.GetComponent<FriendAnswerUIController>();

            var addJokerCommand = 
                new AddTrustRandomPersonJokerCommand(
                    this.availableJokersUIController,
                    networkManager,
                    this.loadingUI,
                    this.secondsRemainingUI,
                    secondsRemainingUIController,
                    this.notReceivedAnswerUI,
                    this.playerAnswerUI,
                    playerAnswerUIController);

            this.Container.Bind<AddTrustRandomPersonJokerCommand>()
                .FromInstance(addJokerCommand)
                .AsSingle();
        }

        private void BindAddLittleIsBetterThanNothingJokerCommand(IClientNetworkManager networkManager)
        {
            var addJokerCommand = 
                new AddLittleIsBetterThanNothingJokerCommand(
                    this.availableJokersUIController,
                    networkManager,
                    this.electionQuestionUIController);

            this.Container.Bind<AddLittleIsBetterThanNothingJokerCommand>()
                .FromInstance(addJokerCommand)
                .AsSingle();
        }

        private void BindAddAskAudienceJokerCommand(IClientNetworkManager networkManager)
        {
            var pollResultRetriever = new AudienceAnswerPollResultRetriever(networkManager);
            var addJokerCommand = new AddAskAudienceJokerCommand(
                this.availableJokersUIController,
                pollResultRetriever,
                this.waitingToAnswerUI,
                this.audienceAnswerUI,
                this.loadingUI);

            this.Container.Bind<AddAskAudienceJokerCommand>()
                .FromInstance(addJokerCommand)
                .AsSingle();
        }

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

        private void BindAddJokersCommands(IClientNetworkManager networkManager)
        {
            this.BindAddConsultWithTheTeacherJokerCommand(networkManager);
            this.BindAddKalitkoJokerCommand(networkManager);
            this.BindAddTrustRandomPersonJokerCommand(networkManager);
            this.BindAddLittleIsBetterThanNothingJokerCommand(networkManager);
            this.BindAddAskAudienceJokerCommand(networkManager);
        }

        private void BindAvailableJokersUIControllerDependencies(IClientNetworkManager networkManager)
        {
            var jokerElectionUIController = this.jokerElectionUI.GetComponent<JokerElectionUIController>();
            var electionJokersBinder = 
                new ElectionForJokersBinder(
                    networkManager, 
                    jokerElectionUIController, 
                    this.jokerElectionUI,
                    this.successfullyActivatedJokerUI,
                    this.unsuccessfullyActivatedJokerUI);

            this.Container.Bind<IElectionForJokersBinder>()
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

            this.Container.Bind<Interfaces.Controllers.EveryBodyVsTheTeacher.Presenter.IAvailableJokersUIController>()
                .FromInstance(this.availableJokersUIController)
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

            this.BindAddJokersCommands(networkManager);
            this.BindGameEndCommand(networkManager);
            this.BindSwitchedToNextRoundCommand();
            this.BindAvailableJokersUIControllerDependencies(networkManager);
            this.BindPlayingStateDependencies();
            
            this.Container.Bind<PlayingState>()
                .ToSelf()
                .AsSingle();
        }
    }
}