using Routers_TrustRandomPersonJokerRouter = Jokers.Routers.TrustRandomPersonJokerRouter;

namespace Tests.Jokers.Routers.TrustRandomPersonJokerRouter
{

    using Assets.Tests.Extensions;

    using DTOs;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            var networkManager = DummyServerNetworkManager.Instance;
            networkManager.SimulateClientConnected(1, "Ivan");
            networkManager.SimulateClientConnected(2, "Georgi");

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(networkManager)
                .AsSingle();

            var question = new SimpleQuestion("QuestionText", new[] { "Answer1", "Answer2", "Answer3", "Answer4" }, 1);
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            var dummyGameDataIterator = new DummyGameDataIterator
            {
                Loaded = true,
                CurrentQuestion = question,
                SecondsForAnswerQuestion = 5
            };

            this.Container.Bind<IGameDataIterator>()
                .FromInstance(dummyGameDataIterator)
                .AsSingle();

            var server = new DummyEveryBodyVsTheTeacherServer
            {
                StartedGame = true,
                PresenterId = 2
            };

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(server)
                .AsSingle();

            this.Container.Bind<ITrustRandomPersonJokerRouter>()
                .To<Routers_TrustRandomPersonJokerRouter>();
        }
    }

}