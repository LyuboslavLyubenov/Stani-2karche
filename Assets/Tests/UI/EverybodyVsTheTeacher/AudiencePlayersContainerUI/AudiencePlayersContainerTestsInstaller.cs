namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Zenject.Source.Install;

    public class AudiencePlayersContainerTestsInstaller : Installer<AudiencePlayersContainerTestsInstaller>
    {
        public override void InstallBindings()
        {
            var serverNetworkManager = DummyServerNetworkManager.Instance;

            Container.Bind<IServerNetworkManager>()
                .FromInstance(serverNetworkManager)
                .AsSingle();

            Container.Bind<PlayersConnectingToTheServerState>()
                .AsSingle();
        }
    }

}