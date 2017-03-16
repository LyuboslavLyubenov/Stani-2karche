namespace Assets.Tests.Jokers.Retrievers.AudiencePollResultRetriever
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Jokers.Retrievers;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            Container.Bind<int>()
                .FromInstance(5)
                .WhenInjectedInto<IAnswerPollResultRetriever>();

            Container.Bind<IAnswerPollResultRetriever>()
                .To<AudienceAnswerPollResultRetriever>();
            
            var question = new SimpleQuestion("QuestionText1", new[] { "Answer1", "Answer2", "Answer3", "Answer4" }, 0);

            Container.Bind<ISimpleQuestion>()
                .FromInstance(question);
        }
    }
}