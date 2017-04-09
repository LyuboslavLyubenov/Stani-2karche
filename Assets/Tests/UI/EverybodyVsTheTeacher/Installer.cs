namespace Tests.UI.EverybodyVsTheTeacher
{

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    using States.EveryBodyVsTheTeacher.Server;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller {
    
        public override void InstallBindings()
        {
            var dummyNetworkManager = DummyServerNetworkManager.Instance;

            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(dummyNetworkManager)
                .AsSingle();
            
            this.Container.Bind<PlayersConnectingToTheServerState>()
                .AsSingle();

            this.Container.Bind<IState>()
                .To<PlayersConnectingToTheServerState>()
                .AsSingle();
        }
    }
}
