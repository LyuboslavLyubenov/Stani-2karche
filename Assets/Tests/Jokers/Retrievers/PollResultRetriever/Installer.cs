using AudienceAnswerPollResultRetriever = Jokers.Retrievers.AudienceAnswerPollResultRetriever;

namespace Tests.Jokers.Retrievers.PollResultRetriever
{

    using DTOs;

    using Interfaces;
    using Interfaces.Network.Jokers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            this.Container.Bind<int>()
                .FromInstance(5)
                .WhenInjectedInto<IAnswerPollResultRetriever>();

            this.Container.Bind<IAnswerPollResultRetriever>()
                .To<AudienceAnswerPollResultRetriever>();
            
            var question = new SimpleQuestion("QuestionText1", new[] { "Answer1", "Answer2", "Answer3", "Answer4" }, 0);

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);
        }
    }
}