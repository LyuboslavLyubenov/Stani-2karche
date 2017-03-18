namespace Tests.Network.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender
{

    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

    using UnityEngine;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            Container.Bind<IPlayersConnectingToTheServerState>()
                .To<DummyPlayersConnectingToTheServerState>()
                .AsSingle();

            Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            Container.Bind<IPlayersConnectingStateDataSender>()
                .To<PlayersConnectingStateDataSender>()
                .AsSingle();
        }
    }
}