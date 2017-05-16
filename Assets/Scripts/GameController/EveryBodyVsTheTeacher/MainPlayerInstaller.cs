using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NotConnectedToServerState = States.EveryBodyVsTheTeacher.Shared.NotConnectedToServerState;
using StateActivator = Tests.StateActivator;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class MainPlayerInstaller : MonoInstaller
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