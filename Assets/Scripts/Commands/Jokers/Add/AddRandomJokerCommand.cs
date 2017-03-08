namespace Assets.Scripts.Commands.Jokers.Add
{
    using System;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Controllers.Jokers;
    using DTOs;
    using Interfaces;

    using JetBrains.Annotations;

    using Utils;

    public class AddRandomJokerCommand : INetworkManagerCommand
    {
        private SelectRandomJokerUIController selectRandomJokerUIController;

        public AddRandomJokerCommand(SelectRandomJokerUIController selectRandomJokerUIController)
        {
            if (selectRandomJokerUIController == null)
            {
                throw new ArgumentNullException("selectRandomJokerUIController");
            }
            
            this.selectRandomJokerUIController = selectRandomJokerUIController;
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
