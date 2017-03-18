namespace Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Interfaces.Network.NetworkManager;

    using StateMachine.EveryBodyVsTheTeacher.States.Server;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class AudiencePlayersContainerTestsInstaller : Installer<AudiencePlayersContainerTestsInstaller>
    {
        public override void InstallBindings()
        {
            var serverNetworkManager = DummyServerNetworkManager.Instance;

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(serverNetworkManager)
                .AsSingle();

            this.Container.Bind<PlayersConnectingToTheServerState>()
                .AsSingle();
        }
    }

}