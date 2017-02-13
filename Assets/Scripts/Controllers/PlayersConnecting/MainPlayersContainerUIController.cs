namespace Assets.Scripts.Controllers.PlayersConnecting
{

    using System.Collections.Generic;
    using System.Linq;
    
    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;

    using UnityEngine;

    using Zenject;

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
            this.state.OnMainPlayerDisconnected += this.OnMainPlayerDisconnected;

            this.mainPlayerUIControllers = this.GetComponentsInChildren<MainPlayerUIController>();
        }
        
        private void OnMainPlayerConnected(object sender, ClientConnectionDataEventArgs args)
        {
            ShowMainPlayerOnScreen(args.ConnectionId);
        }

        private void OnMainPlayerDisconnected(object sender, ClientConnectionDataEventArgs args)
        {
            if (!this.IsOnScreen(args.ConnectionId))
            {
                return;
            }

            HideMainPlayerFromScreen(args.ConnectionId);
        }

        private void HideMainPlayerFromScreen(int connectionId)
        {
            this.connectedMainPlayersControllers[connectionId].ClearUsername();
            this.connectedMainPlayersControllers.Remove(connectionId);
        }

        private void ShowMainPlayerOnScreen(int connectionId)
        {
            var playerUsername = serverNetworkManager.GetClientUsername(connectionId);
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