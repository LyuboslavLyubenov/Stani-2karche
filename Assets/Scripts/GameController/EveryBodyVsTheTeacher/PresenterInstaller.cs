using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
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
        }
    }
}