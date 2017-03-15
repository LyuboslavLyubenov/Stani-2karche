namespace Assets.Tests.Jokers.Routers.AnswerPoll
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers.Routers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Jokers.Routers;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

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