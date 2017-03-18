namespace Assets.Tests.UI.EverybodyVsTheTeacher
{
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

    using StateMachine.EveryBodyVsTheTeacher.States.Server;

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
