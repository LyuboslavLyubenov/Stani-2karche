using CreatedGameInfoReceiver = Network.GameInfo.CreatedGameInfoReceiver;
using KinveyWrapper = Network.KinveyWrapper;

namespace Tests.UI.Lobby.ServersAvailableUIController
{

    using Controllers;
    using Controllers.Lobby;

    using Interfaces.Network;
    using Interfaces.Network.Kinvey;
    using Interfaces.Network.NetworkManager;
    using Interfaces.Services;

    using Tests.DummyObjects;

    using UnityEngine;

    using Zenject.Source.Install;

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

            this.Container.Bind<ILANServersDiscoverer>()
                .To<DummyIlanServersDiscoverer>()
                .AsSingle();

            this.Container.Bind<ISimpleTcpClient>()
                .To<DummySimpleTcpClient>()
                .AsSingle();

            this.Container.Bind<ISimpleTcpServer>()
                .To<DummySimpleTcpServer>()
                .AsSingle();

            this.Container.Bind<ICreatedGameInfoReceiver>()
                .To<CreatedGameInfoReceiver>()
                .AsSingle();

            var selectPlayerTypeRouter = new SelectPlayerTypeRouter(
                this.BasicExamServerSelectPlayerTypeUiController, 
                this.EveryBodyVsTheTeacherSelectPlayerTypeUiController
                );

            this.Container.Bind<SelectPlayerTypeRouter>()
                .FromInstance(selectPlayerTypeRouter)
                .AsSingle();

            this.Container.Bind<IKinveyWrapper>()
                .To<KinveyWrapper>()
                .AsSingle();
            
        }
    }
}