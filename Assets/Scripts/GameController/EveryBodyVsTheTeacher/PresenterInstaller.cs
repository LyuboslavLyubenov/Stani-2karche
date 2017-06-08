using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{

    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Network;

    using StateMachine;

    using Zenject.Source.Install;

    public class PresenterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .FromInstance(ClientNetworkManager.Instance)
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();

            this.Container.Bind<IRemoteStateActivator>()
                .To<RemoteStateActivator>()
                .AsSingle();
        }
    }
}