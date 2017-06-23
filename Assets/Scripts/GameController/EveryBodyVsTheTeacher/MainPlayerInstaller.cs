using AvailableJokersUIController = Controllers.AvailableJokersUIController;
using ClientNetworkManager = Network.NetworkManagers.ClientNetworkManager;
using GameEndCommand = Commands.Client.GameEndCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;
using LeaderboardReceiver = Network.Leaderboard.LeaderboardReceiver;

namespace Assets.Scripts.GameController.EveryBodyVsTheTeacher
{
    using Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer;

    using StateMachine;

    using UnityEngine;

    using Zenject.Source.Install;

    public class MainPlayerInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject endGameUI;

        [SerializeField]
        private GameObject leaderboardUI;

        [SerializeField]
        private AvailableJokersUIController availableJokersUIController;

        private void AddJokerCommands(IClientNetworkManager networkManager)
        {
            var addJokerCommands = new INetworkManagerCommand[]
                                   {
                                       new AddConsultWithTheTeacherJokerCommand(
                                           this.availableJokersUIController,
                                           networkManager),
                                       new AddTrustRandomPersonJokerCommand(
                                           this.availableJokersUIController,
                                           networkManager),
                                       new AddKalitkoJokerCommand(
                                           this.availableJokersUIController,
                                           networkManager),
                                       new AddLittleIsBetterThanNothingJokerCommand(
                                           this.availableJokersUIController, 
                                           networkManager), 
                                       new AddAskAudienceJokerCommand(
                                           this.availableJokersUIController, 
                                           networkManager), 
                                       new AddHelpFromFriendJokerCommand(
                                           this.availableJokersUIController, 
                                           networkManager)
                                   };
            networkManager.CommandsManager.AddCommands(addJokerCommands);
        }

        public override void InstallBindings()
        {
            var networkManager = ClientNetworkManager.Instance;

            this.Container.Bind<IClientNetworkManager>()
                .FromInstance(networkManager)
                .AsSingle();

            this.Container.Bind<StateMachine>()
                .ToSelf()
                .AsSingle();

            var leaderboardReceiver = new LeaderboardReceiver(networkManager, 5);
            var gameEndCommand = new GameEndCommand(this.endGameUI, this.leaderboardUI, leaderboardReceiver);
            networkManager.CommandsManager.AddCommand(gameEndCommand);

            this.AddJokerCommands(networkManager);
        }
    }
}