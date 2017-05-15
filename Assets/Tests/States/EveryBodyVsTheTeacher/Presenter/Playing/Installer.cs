using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.Presenter.Playing
{

    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Presenter;
    using Assets.Tests.DummyObjects.UIControllers;

    using DTOs;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
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
                .To<DummyAvailableJokersUIController>()
                .AsSingle();

            Container.Bind<IChangedRoundUIController>()
                .To<DummyChangedRoundUIController>()
                .AsSingle();

            Container.Bind<GameObject>()
                .FromInstance(new GameObject())
                .WhenInjectedInto<PlayingState>();

            Container.Bind<PlayingState>()
                .ToSelf();

            Container.Bind<StateMachine>()
                .FromInstance(new StateMachine());
        }
    }
}