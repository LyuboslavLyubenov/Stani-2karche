using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.MistakesRemainingCommandsSender
{
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network;
    using Assets.Tests.DummyObjects;
    using Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher;

    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance)
                .AsSingle();

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            this.Container.Bind<int>()
                .FromInstance(2);

            this.Container.Bind<IRoundsSwitcher>()
                .To<DummyRoundsSwitcher>()
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();

            var roundState = new DummyRoundState
                             {
                                 MistakesRemaining = this.Container.Resolve<int>()
                             };

            this.Container.Bind<IRoundState>()
                .FromInstance(roundState)
                .AsSingle();

            var stateMachine = this.Container.Resolve<StateMachine>();
            stateMachine.SetCurrentState(roundState);

            this.Container.Bind<IMistakesRemainingCommandsSender>()
                .To<MistakesRemainingCommandsSender>()
                .AsSingle();
        }
    }
}