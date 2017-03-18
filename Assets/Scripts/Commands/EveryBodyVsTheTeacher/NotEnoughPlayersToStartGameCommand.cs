namespace Commands.EveryBodyVsTheTeacher
{

    using System.Collections.Generic;

    using Interfaces.Network.NetworkManager;

    using UnityEngine.UI;

    public class NotEnoughPlayersToStartGameCommand : INetworkManagerCommand
    {
        private readonly Button startGameButton;

        public NotEnoughPlayersToStartGameCommand(Button startGameButton)
        {
            this.startGameButton = startGameButton;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.startGameButton.gameObject.SetActive(false);
        }
    }

}