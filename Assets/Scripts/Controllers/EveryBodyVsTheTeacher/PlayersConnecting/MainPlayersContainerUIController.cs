using PlayersConnectingToTheServerState = StateMachine.EveryBodyVsTheTeacher.States.Server.PlayersConnectingToTheServerState;

namespace Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{

    using System.Collections.Generic;
    using System.Linq;

    using EventArgs;

    using Interfaces.Network.NetworkManager;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class MainPlayersContainerUIController : MonoBehaviour
    {   
        private Dictionary<int, MainPlayerUIController> connectedMainPlayersControllers = new Dictionary<int, MainPlayerUIController>();
        private MainPlayerUIController[] mainPlayerUIControllers;

        [Inject]
        private IServerNetworkManager serverNetworkManager;

        [Inject]
        private PlayersConnectingToTheServerState state;

        void Start()
        {
            this.state.OnMainPlayerConnected += this.OnMainPlayerConnected;
            this.state.OnMainPlayerRequestedGameStart += this.OnMainPlayerRequestedGameStart;
            this.state.OnMainPlayerDisconnected += this.OnMainPlayerDisconnected;

            this.mainPlayerUIControllers = this.GetComponentsInChildren<MainPlayerUIController>();
        }

        private void OnMainPlayerRequestedGameStart(object sender, ClientConnectionDataEventArgs clientConnectionDataEventArgs)
        {
            var controller = this.connectedMainPlayersControllers[clientConnectionDataEventArgs.ConnectionId];
            controller.RequestedGameStart = true;
        }

        private void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            this.ShowMainPlayerOnScreen(args.ConnectionId);
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (!this.IsOnScreen(args.ConnectionId))
            {
                return;
            }
            
            this.HideMainPlayerFromScreen(args.ConnectionId);
        }

        private void HideMainPlayerFromScreen(int connectionId)
        {
            this.connectedMainPlayersControllers[connectionId].RequestedGameStart = false;
            this.connectedMainPlayersControllers[connectionId].ClearUsername();
            this.connectedMainPlayersControllers.Remove(connectionId);
        }

        private void ShowMainPlayerOnScreen(int connectionId)
        {
            var playerUsername = this.serverNetworkManager.GetClientUsername(connectionId);
            var controller = this.GetFirstNotUsedMainPlayerObject();
            controller.Username = playerUsername;

            this.connectedMainPlayersControllers.Add(connectionId, controller);
        }
        
        private MainPlayerUIController GetFirstNotUsedMainPlayerObject()
        {
            var controller = this.mainPlayerUIControllers.FirstOrDefault(m => !this.connectedMainPlayersControllers.ContainsValue(m));
            return controller;
        }

        private bool IsOnScreen(int connectionId)
        {
            return this.connectedMainPlayersControllers.ContainsKey(connectionId);
        }
    }

}