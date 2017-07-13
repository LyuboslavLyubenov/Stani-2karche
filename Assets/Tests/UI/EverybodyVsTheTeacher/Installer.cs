using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.UI.EverybodyVsTheTeacher
{
    using Assets.Scripts.Interfaces;

    using Interfaces.Network.NetworkManager;
    
    using Tests.DummyObjects;

    using Zenject;

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
