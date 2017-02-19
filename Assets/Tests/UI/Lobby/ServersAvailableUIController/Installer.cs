namespace Assets.Tests.UI.Lobby.ServersAvailableUIController
{
    using Assets.Scripts.Controllers;
    using Assets.Scripts.Controllers.Lobby;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Network;

    using UnityEngine;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUiController;

        public Scripts.Controllers.EveryBodyVsTheTeacher.SelectPlayerTypeUIController
            EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            var obj = new GameObject("BasicEXamSelectPlayerType");
            this.BasicExamServerSelectPlayerTypeUiController =
                obj.AddComponent<BasicExamServerSelectPlayerTypeUIController>();

            var obj2 = new GameObject("EveryBodyVsTheTeacherSelectPlayerTypeUI");
            this.EveryBodyVsTheTeacherSelectPlayerTypeUiController =
                obj2.AddComponent<Scripts.Controllers.EveryBodyVsTheTeacher.SelectPlayerTypeUIController>();

            Container.Bind<ILANServersDiscoveryService>()
                .To<DummyLANServersDiscoveryService>()
                .AsSingle();

            Container.Bind<ISimpleTcpClient>()
                .To<DummySimpleTcpClient>()
                .AsSingle();

            Container.Bind<ISimpleTcpServer>()
                .To<DummySimpleTcpServer>()
                .AsSingle();

            Container.Bind<CreatedGameInfoReceiverService>()
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