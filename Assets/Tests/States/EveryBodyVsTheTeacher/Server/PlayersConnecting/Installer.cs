using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            this.Container.Bind<PlayersConnectingToTheServerState>()
                .ToSelf();
            
            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();
        }
    }

}