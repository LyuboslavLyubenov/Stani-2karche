using JokersData = Network.JokersData;

namespace Tests.Network.Jokers.JokersDataSender
{

    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<JokersData>()
                .AsSingle();
            
            Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();
        }
    }

}