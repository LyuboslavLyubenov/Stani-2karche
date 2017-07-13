using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.SecondsRemainingRemoteUpdater
{

    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Network;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance);

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.Container.Resolve<IEveryBodyVsTheTeacherServer>();
            dummyServer.StartedGame = true;
            dummyServer.IsGameOver = false;

            this.Container.Bind<ISecondsRemainingUICommandsSender>()
                .To<SecondsRemainingUICommandsSender>();
        }
    }
}