using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.Jokers.ConsultWithTeacherJoker
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;
    using Assets.Tests.DummyObjects.UIControllers;
    using Assets.Tests.Utils;

    using Interfaces;
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            var question = new QuestionGenerator().GenerateQuestion();
            Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();

            var loadingUI = new GameObject();
            var electionQuestionUI = new GameObject();

            Container.Bind<IElectionQuestionUIController>()
                .To<DummyElectionQuestionUIController>()
                .AsSingle();

            Container.Bind<GameObject>()
                .WithId("LoadingUI")
                .FromInstance(loadingUI);

            Container.Bind<GameObject>()
                .WithId("ElectionQuestionUI")
                .FromInstance(electionQuestionUI);
            
            Container.Bind<IJoker>()
                .FromMethod(
                    (context) =>
                        {
                            var clientNetworkManager = 
                                context.Container.Resolve<IClientNetworkManager>();
                            var electionQuestionUIController =
                                context.Container.Resolve<IElectionQuestionUIController>();
                            var joker = 
                                new ConsultWithTeacherJoker(
                                    clientNetworkManager,
                                    loadingUI,
                                    electionQuestionUI,
                                    electionQuestionUIController);
                            return joker;
                        });
        }
    }
}