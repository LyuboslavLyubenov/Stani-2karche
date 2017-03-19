namespace Commands.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using Interfaces.Network.NetworkManager;

    public class AudiencePlayerConnectedCommand : INetworkManagerCommand
    {
        private readonly IAudiencePlayersContainerUIController audiencePlayersContainerUiController;

        public AudiencePlayerConnectedCommand(IAudiencePlayersContainerUIController audiencePlayersContainerUiController)
        {
            if (audiencePlayersContainerUiController == null)
            {
                throw new ArgumentNullException("audiencePlayersContainerUiController");
            }

            this.audiencePlayersContainerUiController = audiencePlayersContainerUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();

            if (this.audiencePlayersContainerUiController.IsOnScreen(connectionId))
            {
                return;
            }

            var username = commandsOptionsValues["Username"];
            this.audiencePlayersContainerUiController.ShowAudiencePlayer(connectionId, username);
        }
    }
}
