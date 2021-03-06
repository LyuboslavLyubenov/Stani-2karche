﻿namespace Tests.DummyObjects
{

    using System;
    using System.Collections.Generic;

    using DTOs;

    using EventArgs;

    using Interfaces.Controllers;

    public class DummyMainPlayersContainerUIController : IMainPlayersContainerUIController
    {
        public event EventHandler<ConnectedClientDataEventArgs> OnShowMainPlayer = delegate
            { }; 

        public event EventHandler<ClientConnectionIdEventArgs> OnHideMainPlayer = delegate
            { };

        public event EventHandler<ClientConnectionIdEventArgs> OnShowMainPlayerRequestedGameStart = delegate
            { };

        private List<int> mainPlayersOnScreen = new List<int>();
        private List<int> audiencePlayersOnScreen = new List<int>();

        public void ShowMainPlayerRequestedGameStart(int connectionId)
        {
            this.OnShowMainPlayerRequestedGameStart(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void ShowMainPlayer(int connectionId, string username)
        {
            if (this.mainPlayersOnScreen.Contains(connectionId))
            {
                throw new InvalidOperationException();
            }

            var connectedClientData = new ConnectedClientData(connectionId, username);
            this.mainPlayersOnScreen.Add(connectionId);
            this.OnShowMainPlayer(this, new ConnectedClientDataEventArgs(connectedClientData));
        }

        public void HideMainPlayer(int connectionId)
        {
            if (!this.mainPlayersOnScreen.Contains(connectionId))
            {
                throw new InvalidOperationException();
            }
            
            this.mainPlayersOnScreen.Remove(connectionId);
            this.OnHideMainPlayer(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void HideAll()
        {
            for (int i = 0; i < this.mainPlayersOnScreen.Count; i++)
            {
                var connectionId = this.mainPlayersOnScreen[i];
                this.HideMainPlayer(connectionId);
            }
        }

        public bool IsOnScreen(int connectionId)
        {
            return this.mainPlayersOnScreen.Contains(connectionId);
        }
    }
}
