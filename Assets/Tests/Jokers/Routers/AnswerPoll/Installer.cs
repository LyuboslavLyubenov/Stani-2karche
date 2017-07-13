using AnswerPollRouter = Jokers.Routers.AnswerPollRouter;

namespace Tests.Jokers.Routers.AnswerPoll
{

    using DTOs;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            this.Container.Bind<IAnswerPollRouter>()
                .To<AnswerPollRouter>();

            var question = new SimpleQuestion(
                "SimpleQuestion Text",
                new[]
                {
                    "Otovoro1",
                    "Otgovor2",
                    "Otgovor3",
                    "OTgovoro4"
                },
                0);

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();
        }
    }

}