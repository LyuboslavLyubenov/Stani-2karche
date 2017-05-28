using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;
using SwitchedToNextRoundCommand = Scripts.Commands.EveryBodyVsTheTeacher.Shared.SwitchedToNextRoundCommand;

namespace Assets.Tests.States.Presenter.Playing
{

    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;
    using Assets.Tests.DummyObjects.Network;
    using Assets.Tests.DummyObjects.UIControllers;

    using Commands.Client;

    using Controllers;
    using Controllers.EveryBodyVsTheTeacher.PlayersConnecting;

    using DTOs;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.Jokers;
    using Interfaces.Network.Leaderboard;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    using IAvailableJokersUIController = Assets.Scripts.Interfaces.Controllers.EveryBodyVsTheTeacher.Presenter.IAvailableJokersUIController;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private GameObject playingUI;

        [SerializeField]
        private MainPlayersContainerUIController mainPlayersContainerUIController;

        [SerializeField]
        private AudiencePlayersContainerUIController audiencePlayersContainerUIController;
       

        public override void InstallBindings()
        {
            Container.Bind<IClientNetworkManager>()
                .FromInstance(new DummyClientNetworkManager())
                .AsSingle();

            var question = new SimpleQuestion(
                "Question Text",
                new[]
                {
                    "Answer 1",
                    "Answer 2",
                    "Answer 3",
                    "Answer 4"
                },
                0);

            Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            Container.Bind<ISecondsRemainingUIController>()
                .To<DummySecondsRemainingUIController>()
                .AsSingle();

            Container.Bind<IElectionQuestionUIController>()
                .To<DummyElectionQuestionUIController>()
                .AsSingle();

            Container.Bind<IAvailableJokersUIController>()
                .To<DummyPresenterAvailableJokersUIController>()
                .AsSingle();

            Container.Bind<IChangedRoundUIController>()
                .To<DummyChangedRoundUIController>()
                .AsSingle();

            Container.Bind<IMainPlayersContainerUIController>()
                .FromInstance(this.mainPlayersContainerUIController);

            Container.Bind<IAudiencePlayersContainerUIController>()
                .FromInstance(this.audiencePlayersContainerUIController);

            Container.Bind<IAnswerPollResultRetriever>()
                .To<DummyAnswerPollResultRetriever>()
                .AsSingle();

            Container.Bind<ILeaderboardReceiver>()
                .To<DummyLeaderboardReceiver>()
                .AsSingle();

            Container.Bind<SwitchedToNextRoundCommand>()
                .FromMethod(
                    (context) =>
                        {
                            var changedRoundUIController = context.Container.Resolve<IChangedRoundUIController>();
                            var command = new SwitchedToNextRoundCommand(new GameObject(), changedRoundUIController);
                            return command;
                        })
                .WhenInjectedInto<PlayingState>();

            Container.Bind<GameEndCommand>()
                .FromMethod(
                    (context) =>
                        {
                            var endGameUI = new GameObject();
                            endGameUI.AddComponent<EndGameUIController>();
                            var leaderboardReceiver = context.Container.Resolve<ILeaderboardReceiver>();
                            var command = new GameEndCommand(endGameUI, new GameObject(), leaderboardReceiver);
                            return command;
                        })
                .WhenInjectedInto<PlayingState>();

            Container.Bind<GameObject>()
                .WithId("PlayingUI")
                .FromInstance(this.playingUI);

            Container.Bind<GameObject>()
                .FromInstance(this.playingUI)
                .WhenInjectedInto<PlayingState>();

            Container.Bind<PlayingState>()
                .ToSelf();

            Container.Bind<StateMachine>()
                .FromInstance(new StateMachine());
        }
    }
}