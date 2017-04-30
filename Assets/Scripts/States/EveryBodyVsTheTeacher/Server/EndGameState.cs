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

        public EndGameState(IServerNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.networkManager = networkManager;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {        
            this.networkManager.OnClientConnected += OnClientConnected;

            var endGameCommand = new NetworkCommandData("EndGame");
            this.networkManager.SendAllClientsCommand(endGameCommand);
        }

        private void OnClientConnected(object sender, ClientConnectionIdEventArgs args)
        {
            var endGameCommand = new NetworkCommandData("EndGame");
            this.networkManager.SendClientCommand(args.ConnectionId, endGameCommand);
        }
        
        public void OnStateExit(StateMachine stateMachine)
        {
            this.networkManager.OnClientConnected -= this.OnClientConnected;
        }
    }
}