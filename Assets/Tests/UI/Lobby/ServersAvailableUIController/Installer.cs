namespace Assets.Tests.UI.Lobby.ServersAvailableUIController
{
    using Assets.Scripts.Controllers;
    using Assets.Scripts.Controllers.Lobby;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.Kinvey;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Interfaces.Services;
    using Assets.Scripts.Network;
    using Assets.Zenject.Source.Install;

    using UnityEngine;

    public class Installer : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUiController;

        public SelectPlayerTypeUIController
            EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            var obj = new GameObject("BasicEXamSelectPlayerType");
            this.BasicExamServerSelectPlayerTypeUiController =
                obj.AddComponent<BasicExamServerSelectPlayerTypeUIController>();

            var obj2 = new GameObject("EveryBodyVsTheTeacherSelectPlayerTypeUI");
            this.EveryBodyVsTheTeacherSelectPlayerTypeUiController =
                obj2.AddComponent<SelectPlayerTypeUIController>();

            Container.Bind<ILANServersDiscoverer>()
                .To<DummyIlanServersDiscoverer>()
                .AsSingle();

            Container.Bind<ISimpleTcpClient>()
                .To<DummySimpleTcpClient>()
                .AsSingle();

            Container.Bind<ISimpleTcpServer>()
                .To<DummySimpleTcpServer>()
                .AsSingle();

            Container.Bind<ICreatedGameInfoReceiver>()
                .To<CreatedGameInfoReceiver>()
                .AsSingle();

            var selectPlayerTypeRouter = new SelectPlayerTypeRouter(
                this.BasicExamServerSelectPlayerTypeUiController, 
                this.EveryBodyVsTheTeacherSelectPlayerTypeUiController
                );

            Container.Bind<SelectPlayerTypeRouter>()
                .FromInstance(selectPlayerTypeRouter)
                .AsSingle();

            Container.Bind<IKinveyWrapper>()
                .To<KinveyWrapper>()
                .AsSingle();
            
        }
    }
}