namespace Assets.Scripts.Commands.Jokers.Add
{
    using System;

    using Controllers.Jokers;
    using DTOs;
    using Interfaces;
    using Network.NetworkManagers;
    using Utils;

    public class AddRandomJokerCommand : INetworkManagerCommand
    {
        private SelectRandomJokerUIController selectRandomJokerUIController;

        private ClientNetworkManager networkManager;

        public AddRandomJokerCommand(SelectRandomJokerUIController selectRandomJokerUIController, ClientNetworkManager networkManager)
        {
            ValidationUtils.ValidateObjectNotNull(selectRandomJokerUIController, "selectRandomJokerUIController");
            ValidationUtils.ValidateObjectNotNull(networkManager, "networkManager");

            this.selectRandomJokerUIController = selectRandomJokerUIController;
            this.networkManager = networkManager;
        }

        public void Execute(System.Collections.Generic.Dictionary<string, string> commandsOptionsValues)
        {
            var jokersTypesJSON = commandsOptionsValues["JokersTypeNamesJSON"];
            var jokersTypeNames = JsonArrayUtility.ArrayFromJson<JokerType_DTO>(jokersTypesJSON); 
            var jokers = new Type[jokersTypeNames.Length];

            for (int i = 0; i < jokersTypeNames.Length; i++)
            {
                var jokerTypeName = jokersTypeNames[i].JokerType;
                var jokerType = Type.GetType(jokerTypeName, true, true);
                jokers[i] = jokerType;
            }

            var selectedJokerIndex = int.Parse(commandsOptionsValues["SelectedJokerIndex"]);

            this.selectRandomJokerUIController.gameObject.SetActive(true);
            this.selectRandomJokerUIController.PlaySelectRandomJokerAnimation(jokers, selectedJokerIndex);
        }
    }

}
