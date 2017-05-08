using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.TrustRandomPersonJoker
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;
    using Assets.Tests.DummyObjects.UIControllers;
    using Assets.Tests.Utils;
    
    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var loadingUI = new GameObject();
            var secondsRemainingUI = new GameObject();
            var notReceivedAnswerUI = new GameObject();
            var playerAnswerUI = new GameObject();

            Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle()
                .NonLazy();

            Container.Bind<ISecondsRemainingUIController>()
                .To<DummySecondsRemainingUIController>()
                .AsSingle()
                .NonLazy();

            Container.Bind<IPlayerAnswerUIController>()
                .To<DummyPlayerAnswerUIController>()
                .AsSingle()
                .NonLazy();

            Container.Bind<int>()
                .FromInstance(10);

            var question = new QuestionGenerator().GenerateQuestion();
            Container.Bind<ISimpleQuestion>()
                .FromInstance(question);
            
            Container.Bind<GameObject>()
                .FromInstance(loadingUI)
                .WhenInjectedInto<AfterActivatedShowLoadingUIUntilReceivedSettings>();

            Container.Bind<GameObject>()
                .FromInstance(secondsRemainingUI)
                .WhenInjectedInto<WhenReceivedSettingsShowSecondsRemainingUI>();

            Container.Bind<GameObject>()
                .FromInstance(playerAnswerUI)
                .WhenInjectedInto<WhenReceivedAnswerShowPlayerAnswerUI>();

            Container.Bind<GameObject>()
                .FromInstance(notReceivedAnswerUI)
                .WhenInjectedInto<WhenReceivedAnswerTimeoutShowNotReceivedAnswerUI>();

            Container.Bind<IJoker>()
                .To<TrustRandomPersonJoker>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = 
                                context.Container.Resolve<IClientNetworkManager>();
                            var secondsRemainingUIController =
                                context.Container.Resolve<ISecondsRemainingUIController>();
                            var playerAnswerUIController = 
                                context.Container.Resolve<IPlayerAnswerUIController>();
                            var joker = 
                                new TrustRandomPersonJoker(
                                    clientNetworkManager,
                                    loadingUI,
                                    secondsRemainingUI,
                                    secondsRemainingUIController,
                                    notReceivedAnswerUI,
                                    playerAnswerUI,
                                    playerAnswerUIController);

                            return joker;
                        });
        }
    }
}