namespace Controllers.EveryBodyVsTheTeacher.PlayersConnecting
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;

    public class MainPlayersContainerUIController : MonoBehaviour, IMainPlayersContainerUIController
    {   
        private Dictionary<int, MainPlayerUIController> connectedMainPlayersControllers = new Dictionary<int, MainPlayerUIController>();
        private MainPlayerUIController[] mainPlayerUIControllers;
        
        void Start()
        {
            this.mainPlayerUIControllers = this.GetComponentsInChildren<MainPlayerUIController>();
        }

        public void ShowMainPlayerRequestedGameStart(int connectionId)
        {
            if (!this.connectedMainPlayersControllers.ContainsKey(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }

            var controller = this.connectedMainPlayersControllers[connectionId];
            controller.RequestedGameStart = true;
        }

        public void ShowMainPlayer(int connectionId, string username)
        {
            if (connectionId <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionId");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            var controller = this.GetFirstNotUsedMainPlayerObject();
            controller.Username = username;
            this.connectedMainPlayersControllers.Add(connectionId, controller);
        }

        public void HideMainPlayer(int connectionId)
        {
            if (!this.IsOnScreen(connectionId))
            {
                return;
            }

            this.connectedMainPlayersControllers[connectionId].RequestedGameStart = false;
            this.connectedMainPlayersControllers[connectionId].ClearUsername();
            this.connectedMainPlayersControllers.Remove(connectionId);
        }
        
        private MainPlayerUIController GetFirstNotUsedMainPlayerObject()
        {
            var controller = this.mainPlayerUIControllers.FirstOrDefault(m => !this.connectedMainPlayersControllers.ContainsValue(m));
            return controller;
        }

        public bool IsOnScreen(int connectionId)
        {
            return this.connectedMainPlayersControllers.ContainsKey(connectionId);
        }
    }
}