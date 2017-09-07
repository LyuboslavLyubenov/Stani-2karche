using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Network.Servers.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;
    using Assets.Scripts.Network;
    using Assets.Scripts.Network.EveryBodyVsTheTeacher;
    using Assets.Scripts.Network.GameInfo.New;
    using Assets.Scripts.Network.JokersData.EveryBodyVsTheTeacher;
    using Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.EveryBodyVsTheTeacher;
    using Interfaces.Network.EveryBodyVsTheTeacher.States;
    using Interfaces.Network.NetworkManager;

    using IO;

    using Network.EveryBodyVsTheTeacher.PlayersConnectingState;
    using Network.NetworkManagers;
    using Network.TcpSockets;

    using StateMachine;

    using UnityEngine;

    using Zenject;

    public class EverybodyVsTheTeacherServerInstaller : MonoInstaller
    {
        [SerializeField]
        private EveryBodyVsTheTeacherServer Server;

        private void InstallServer()
        {
            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .FromInstance(Server)
                .AsSingle();
        }

        private void InstallGameDataExtractor()
        {
            this.Container.Bind<IGameDataExtractor>()
                .To<GameDataExtractor>()
                .AsSingle();
        }

        private void InstallGameDataIterator()
        {
            this.Container.Bind<IGameDataIterator>()
                .To<GameDataIterator>()
                .AsSingle();
        }
    
        private void InstallStateMachine()
        {
            this.Container.Bind<StateMachine>()
                .AsSingle();
        }

        public override void InstallBindings()
        {
            this.InstallServer();
            this.InstallGameDataExtractor();
            this.InstallGameDataIterator();
            
            this.Container.Bind<JokersData>()
                .AsSingle();
            
            this.Container.Bind<IPlayersConnectingToTheServerState>()
                .To<PlayersConnectingToTheServerState>()
                .AsSingle();
            
            this.InstallStateMachine();
        }
    }
}