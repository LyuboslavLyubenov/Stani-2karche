using DummyClientNetworkManager = Tests.DummyObjects.DummyClientNetworkManager;

namespace Assets.Tests.States.EveryBodyVsTheTeacher.MainPlayer.ConnectedToServerState
{

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.MainPlayer;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityEngine;
    using UnityEngine.UI;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        [SerializeField]
        private Button gameStartButton;

        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .To<DummyClientNetworkManager>()
                .AsSingle();

            this.Container.Bind<Button>()
                .FromInstance(this.gameStartButton)
                .AsSingle();

            this.Container.Bind<IState>()
                .To<ConnectedToServerState>()
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();

            var stateMachine = this.Container.Resolve<StateMachine>();
            var state = this.Container.Resolve<IState>();
            stateMachine.SetCurrentState(state);
        }
    }
}