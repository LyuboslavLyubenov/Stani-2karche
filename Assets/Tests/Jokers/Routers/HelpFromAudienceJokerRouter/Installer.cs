using Routers_HelpFromAudienceJokerRouter = Jokers.Routers.HelpFromAudienceJokerRouter;

namespace Tests.Jokers.Routers.HelpFromAudienceJokerRouter
{

    using DTOs;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var question = new SimpleQuestion("Question text", new[] { "answer1", "answer2", "answer3", "answer4" }, 1);

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            this.Container.Bind<IServerNetworkManager>()
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

            this.Container.Bind<IGameDataIterator>()
                .FromInstance(dummyGameDataIterator)
                .AsSingle();

            this.Container.Bind<IAnswerPollRouter>()
                .To<DummyAnswerPollRouter>()
                .AsSingle();

            this.Container.Bind<IHelpFromAudienceJokerRouter>()
                .To<Routers_HelpFromAudienceJokerRouter>();
        }
    }
}