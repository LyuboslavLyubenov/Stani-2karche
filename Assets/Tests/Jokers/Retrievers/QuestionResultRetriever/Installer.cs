using AskPlayerQuestionResultRetriever = Jokers.Retrievers.AskPlayerQuestionResultRetriever;

namespace Tests.Jokers.Retrievers.QuestionResultRetriever
{
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;
    using Interfaces.Network.Jokers;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            var question = new SimpleQuestion("QuestionText", new []{"Answer1", "Answer2", "Answer3", "Answer4"}, 0);
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            this.Container.Bind<int>()
                .FromInstance(5)
                .WhenInjectedInto<IAskClientQuestionResultRetriever>();

            this.Container.Bind<IAskClientQuestionResultRetriever>()
                .To<AskPlayerQuestionResultRetriever>();            
        }
    }

}