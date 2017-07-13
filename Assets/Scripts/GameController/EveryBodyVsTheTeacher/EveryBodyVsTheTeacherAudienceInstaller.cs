using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using StateMachine;

    using Zenject;

    public class EveryBodyVsTheTeacherAudienceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .FromInstance(ClientNetworkManager.Instance);

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();
        }
    }
}