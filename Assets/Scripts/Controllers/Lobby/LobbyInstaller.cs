namespace Controllers.Lobby
{

    using Assets.Scripts.Network.GameInfo.New;

    using Interfaces.Network.Kinvey;
    using Interfaces.Network.NetworkManager;
    using Interfaces.Services;

    using Network;
    using Network.Broadcast;
    using Network.NetworkManagers;

    using UnityEngine;

    using Utils;

    using Zenject;

    public class LobbyInstaller : MonoInstaller
    {
        public BasicExamServerSelectPlayerTypeUIController BasicExamServerSelectPlayerTypeUIController;
        public SelectPlayerTypeUIController EveryBodyVsTheTeacherSelectPlayerTypeUiController;

        public override void InstallBindings()
        {
            var threadUtils = ThreadUtils.Instance;
            var clientNetworkManager = ClientNetworkManager.Instance;

            this.InstallSelectPlayerTypeRouterBindings();
            
            var connectToExternalServerUIController = GameObject.FindObjectOfType<ConnectToExternalServerUIController>();

            this.Container.Bind<ConnectToExternalServerUIController>()
                .FromInstance(connectToExternalServerUIController)
                .AsSingle();

            var serversAvailableUIController = GameObject.FindObjectOfType<ServersAvailableUIController>();

            this.Container.Bind<ServersAvailableUIController>()
                .FromInstance(serversAvailableUIController)
                .AsSingle();

            this.Container.Bind<IKinveyWrapper>()
                .To<KinveyWrapper>()
                .AsSingle();

            this.Container.Bind<ILANServersDiscoverer>()
                .To<LANServersDiscoverer>()
                .AsSingle();

            this.Container.Bind<IClientNetworkManager>()
                .FromInstance(ClientNetworkManager.Instance)
                .AsSingle();

            this.Container.Bind<CreatedGameInfoReceiver>()
                .AsSingle();

            var createdGameInfoReceiver = Container.Resolve<CreatedGameInfoReceiver>();
            this.Container.Bind<ICreatedGameInfoReceiver>()
                .FromInstance(createdGameInfoReceiver);

        }
        
        private void InstallSelectPlayerTypeRouterBindings()
        {
            this.Container.Bind<BasicExamServerSelectPlayerTypeUIController>()
                .FromInstance(this.BasicExamServerSelectPlayerTypeUIController)
                .AsSingle();

            this.Container.Bind<SelectPlayerTypeUIController>()
                .FromInstance(this.EveryBodyVsTheTeacherSelectPlayerTypeUiController)
                .AsSingle();

            this.Container.Bind<SelectPlayerTypeRouter>()
                .AsSingle();
        }
    }

}