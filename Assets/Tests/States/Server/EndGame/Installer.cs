using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.States.Server.EndGame
{

    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server;

    using Interfaces.Network.NetworkManager;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EndGameState>()
                .ToSelf();

            Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance);
        }
    }
}