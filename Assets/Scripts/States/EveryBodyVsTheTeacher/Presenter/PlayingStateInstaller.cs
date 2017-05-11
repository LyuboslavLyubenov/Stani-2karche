using AddAskAudienceJokerCommand = Commands.Jokers.Add.AddAskAudienceJokerCommand;
using AnswerPollResultCommand = Commands.Client.AnswerPollResultCommand;
using AudienceAnswerPollResultRetriever = Jokers.Retrievers.AudienceAnswerPollResultRetriever;
using FriendAnswerUIController = Controllers.FriendAnswerUIController;
using IAnswerPollResultRetriever = Interfaces.Network.Jokers.IAnswerPollResultRetriever;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using IElectionQuestionUIController = Interfaces.Controllers.IElectionQuestionUIController;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using KalitkoJokerContainerUIController = Controllers.EveryBodyVsTheTeacher.Jokers.KalitkoJokerContainerUIController;
using SecondsRemainingUIController = Controllers.SecondsRemainingUIController;

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

        private void BindAddAskAudienceJokerCommand(IAnswerPollResultRetriever pollResultRetriever)
        {
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

        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .FromResolve()
                .AsSingle();

            var clientNetworkManager = this.Container.Resolve<IClientNetworkManager>();

            this.BindAddConsultWithTheTeacherJokerCommand(clientNetworkManager);
            this.BindAddKalitkoJokerCommand(clientNetworkManager);
            this.BindAddTrustRandomPersonJokerCommand(clientNetworkManager);
            this.BindLittleIsBetterThanNothingJokerCommand(clientNetworkManager);

            var pollResultRetriever = new AudienceAnswerPollResultRetriever(clientNetworkManager);
            this.BindAddAskAudienceJokerCommand(pollResultRetriever);
        }
    }
}
