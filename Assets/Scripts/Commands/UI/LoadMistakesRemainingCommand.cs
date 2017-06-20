using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.UI
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Controllers;

    public class LoadMistakesRemainingCommand : INetworkManagerCommand
    {
        private readonly IMistakesRemainingUIController mistakesRemainingUIController;

        public LoadMistakesRemainingCommand(IMistakesRemainingUIController mistakesRemainingUIController)
        {
            if (mistakesRemainingUIController == null)
            {
                throw new ArgumentNullException("mistakesRemainingUIController");
            }

            this.mistakesRemainingUIController = mistakesRemainingUIController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var mistakesCount = commandsOptionsValues["Count"]
                .ConvertTo<int>();
            this.mistakesRemainingUIController.RemainingMistakesCount = mistakesCount;
        }
    }
}
