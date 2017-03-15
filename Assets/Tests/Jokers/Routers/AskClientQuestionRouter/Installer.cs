namespace Assets.Tests.Jokers.Routers.AskClientQuestionRouter
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Jokers.Routers;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

    using UnityEngine;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            var question = new SimpleQuestion("QuestionText", new[] { "answer1", "answer2", "answer3", "answer4" }, 0);
            Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            Container.Bind<IAskClientQuestionRouter>()
                .To<AskClientQuestionRouter>();
        }
    }

}
