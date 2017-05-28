namespace Tests.DummyObjects
{

    using System;
    using System.Collections.Generic;

    using DTOs;

    using EventArgs;

    using Interfaces.Controllers;

    public class DummyAudiencePlayersContainerUIController : IAudiencePlayersContainerUIController
    {
        public event EventHandler<ConnectedClientDataEventArgs> OnShowAudiencePlayer = delegate
            { };

        public event EventHandler<ClientConnectionIdEventArgs> OnHideAudiencePlayer = delegate
            { };

        private List<int> audiencePlayers = new List<int>();

        public void ShowAudiencePlayer(int connectionId, string username)
        {
            if (this.audiencePlayers.Contains(connectionId))
            {
                throw new InvalidOperationException();
            }

            this.audiencePlayers.Add(connectionId);
            this.OnShowAudiencePlayer(this, new ConnectedClientDataEventArgs(new ConnectedClientData(connectionId, username)));
        }

        public void HideAudiencePlayer(int connectionId)
        {
            if (!this.audiencePlayers.Contains(connectionId))
            {
                throw new InvalidOperationException();
            }

            this.audiencePlayers.Remove(connectionId);
            this.OnHideAudiencePlayer(this, new ClientConnectionIdEventArgs(connectionId));
        }

        public void HideAll()
        {
            for (int i = 0; i < this.audiencePlayers.Count; i++)
            {
                var connectionId = this.audiencePlayers[i];
                this.HideAudiencePlayer(connectionId);
            }
        }

        public bool IsOnScreen(int connectionId)
        {
            return this.audiencePlayers.Contains(connectionId);
        }
    }
}
