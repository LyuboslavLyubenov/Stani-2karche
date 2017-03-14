namespace Assets.Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.GameData;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Jokers.Routers;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var question = new SimpleQuestion("Question text", new[] { "answer1", "answer2", "answer3", "answer4" }, 1);

            Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            var dummyGameDataIterator = new DummyGameDataIterator
            {
                CurrentQuestion = question,
                CurrentMark = 2,
                LevelCategory = "асд",
                Loaded = true,
                RemainingQuestionsToNextMark = 1,
                SecondsForAnswerQuestion = 60
            };

            Container.Bind<IGameDataIterator>()
                .FromInstance(dummyGameDataIterator)
                .AsSingle();

            Container.Bind<IAnswerPollRouter>()
                .To<DummyAnswerPollRouter>()
                .AsSingle();

            Container.Bind<IHelpFromAudienceJokerRouter>()
                .To<HelpFromAudienceJokerRouter>();
        }
    }
}