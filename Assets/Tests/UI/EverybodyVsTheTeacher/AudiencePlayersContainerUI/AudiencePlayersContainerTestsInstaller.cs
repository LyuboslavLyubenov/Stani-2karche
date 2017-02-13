namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using Assets.Scripts.Controllers.GameController;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;

    using Zenject;

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