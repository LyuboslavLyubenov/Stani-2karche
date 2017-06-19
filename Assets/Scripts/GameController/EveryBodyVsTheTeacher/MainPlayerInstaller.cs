using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using GameEndCommand = Commands.Client.GameEndCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using LeaderboardReceiver = Network.Leaderboard.LeaderboardReceiver;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class MainPlayerInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject endGameUI;

        [SerializeField]
        private GameObject leaderboardUI;

        public override void InstallBindings()
        {
            this.Container.Bind<IClientNetworkManager>()
                .FromInstance(ClientNetworkManager.Instance)
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();
            
            var leaderboardReceiver = new LeaderboardReceiver(ClientNetworkManager.Instance, 5);
            var gameEndCommand = new GameEndCommand(this.endGameUI, this.leaderboardUI, leaderboardReceiver);
            ClientNetworkManager.Instance.CommandsManager.AddCommand(gameEndCommand);
        }
    }
}