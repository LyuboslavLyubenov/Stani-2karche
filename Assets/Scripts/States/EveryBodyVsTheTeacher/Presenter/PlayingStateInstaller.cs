using AddAskAudienceJokerCommand = Commands.Jokers.Add.AddAskAudienceJokerCommand;
using AnswerPollResultCommand = Commands.Client.AnswerPollResultCommand;
using AudienceAnswerPollResultRetriever = Jokers.Retrievers.AudienceAnswerPollResultRetriever;
using ChangedRoundUIController = Scripts.Controllers.EveryBodyVsTheTeacher.ChangedRoundUIController;
using FriendAnswerUIController = Controllers.FriendAnswerUIController;
using GameEndCommand = Commands.Client.GameEndCommand;
using IAnswerPollResultRetriever = Interfaces.Network.Jokers.IAnswerPollResultRetriever;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using KalitkoJokerContainerUIController = Controllers.EveryBodyVsTheTeacher.Jokers.KalitkoJokerContainerUIController;
using LeaderboardReceiver = Network.Leaderboard.LeaderboardReceiver;
using SecondsRemainingUIController = Controllers.SecondsRemainingUIController;
using SwitchedToNextRoundCommand = Scripts.Commands.EveryBodyVsTheTeacher.Shared.SwitchedToNextRoundCommand;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter
{

    using Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter;
    using Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter.SecondRound;
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    using Zenject.Source.Install;

    public class PlayingStateInstaller : MonoInstaller
    {
        [SerializeField]
        private IAvailableJokersUIController availableJokersUIController;

        [SerializeField]
        private IElectionQuestionUIController electionQuestionUIController;

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

        private void BindLittleIsBetterThanNothingJokerCommand(IClientNetworkManager networkManager)
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

        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .FromResolve()
                .AsSingle();

            var networkManager = this.Container.Resolve<IClientNetworkManager>();

            this.BindAddConsultWithTheTeacherJokerCommand(networkManager);
            this.BindAddKalitkoJokerCommand(networkManager);
            this.BindAddTrustRandomPersonJokerCommand(networkManager);
            this.BindLittleIsBetterThanNothingJokerCommand(networkManager);
            this.BindAddAskAudienceJokerCommand(networkManager);
            this.BindGameEndCommand(networkManager);
            this.BindSwitchedToNextRoundCommand();
        }
    }
}
