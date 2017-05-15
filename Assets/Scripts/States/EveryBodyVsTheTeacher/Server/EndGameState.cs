using ClientConnectionIdEventArgs = EventArgs.ClientConnectionIdEventArgs;
using IEveryBodyVsTheTeacherServer = Interfaces.Network.IEveryBodyVsTheTeacherServer;
using IGameDataIterator = Interfaces.GameData.IGameDataIterator;
using IServerNetworkManager = Interfaces.Network.NetworkManager.IServerNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server
{
    using System;

    using Assets.Scripts.Interfaces;

    using EventArgs;

    using StateMachine;

    public class EndGameState : IState
    {
        private readonly IServerNetworkManager networkManager;
        private readonly IGameDataIterator gameDataIterator;

        public EndGameState(IServerNetworkManager networkManager, IGameDataIterator gameDataIterator)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (gameDataIterator == null)
            {
                throw new ArgumentNullException("gameDataIterator");
            }

            this.networkManager = networkManager;
            this.gameDataIterator = gameDataIterator;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {        
            this.networkManager.OnClientConnected += OnClientConnected;

            var endGameCommand = new NetworkCommandData("EndGame");
            endGameCommand.AddOption("Mark", this.gameDataIterator.CurrentMark.ToString());
            this.networkManager.SendAllClientsCommand(endGameCommand);
        }

        private void OnClientConnected(object sender, ClientConnectionIdEventArgs args)
        {
            var endGameCommand = new NetworkCommandData("EndGame");
            endGameCommand.AddOption("Mark", this.gameDataIterator.CurrentMark.ToString());
            this.networkManager.SendClientCommand(args.ConnectionId, endGameCommand);
        }
        
        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.OnClientConnected -= this.OnClientConnected;
        }
    }
}