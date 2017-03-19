namespace Commands.EveryBodyVsTheTeacher.PlayersConnectingStateDataSender
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using Interfaces.Network.NetworkManager;

    public class MainPlayerRequestedGameStartCommand : INetworkManagerCommand
    {
        private readonly IMainPlayersContainerUIController mainPlayersContainerUiController;

        public MainPlayerRequestedGameStartCommand(IMainPlayersContainerUIController mainPlayersContainerUiController)
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

            if (!this.mainPlayersContainerUiController.IsOnScreen(connectionId))
            {
                return;
            }

            this.mainPlayersContainerUiController.ShowMainPlayerRequestedGameStart(connectionId);
        }
    }
}
