namespace Commands.EveryBodyVsTheTeacher.PlayersConnectingState
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Extensions;

    using Extensions;

    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;

    public class MainPlayerConnectedCommand : INetworkManagerCommand
    {
        private readonly IMainPlayersContainerUIController mainPlayersContainerUiController;
        
        public MainPlayerConnectedCommand(IMainPlayersContainerUIController mainPlayersContainerUiController)
        {
            if (mainPlayersContainerUiController == null)
            {
                throw new ArgumentNullException("mainPlayersContainerUiController");
            }
            
            this.mainPlayersContainerUiController = mainPlayersContainerUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var connectionId = commandsOptionsValues["ConnectionId"].ConvertTo<int>();

            if (this.mainPlayersContainerUiController.IsOnScreen(connectionId))
            {
                return;
            }

            var username = commandsOptionsValues["Username"];
            this.mainPlayersContainerUiController.ShowMainPlayer(connectionId, username);
        }
    }
}