using JokersUsedNotifier = Network.JokersUsedNotifier;
using PlayersConnectingStateDataSender = Network.EveryBodyVsTheTeacher.PlayersConnectingState.PlayersConnectingStateDataSender;

namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingState.DataSender
{

    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            this.Container.Bind<IPlayersConnectingToTheServerState>()
                .To<DummyPlayersConnectingToTheServerState>()
                .AsSingle();

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            this.Container.Bind<IPlayersConnectingStateDataSender>()
                .To<PlayersConnectingStateDataSender>()
                .AsSingle();
        }
    }
}