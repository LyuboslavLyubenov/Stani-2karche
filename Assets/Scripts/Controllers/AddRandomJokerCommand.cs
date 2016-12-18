using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class AddRandomJokerCommand : INetworkManagerCommand
{
    SelectRandomJokerUIController selectRandomJokerUIController;

    ClientNetworkManager networkManager;

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
        var jokersTypeNames = JsonHelper.FromJson<JokerType_DTO>(jokersTypesJSON); 
        var jokers = new Type[jokersTypeNames.Length];

        for (int i = 0; i < jokersTypeNames.Length; i++)
        {
            var jokerTypeName = jokersTypeNames[i].JokerType;
            var jokerType = Type.GetType(jokerTypeName, true, true);
            jokers[i] = jokerType;
        }

        var selectedJokerIndex = int.Parse(commandsOptionsValues["SelectedJokerIndex"]);

        selectRandomJokerUIController.gameObject.SetActive(true);
        selectRandomJokerUIController.PlaySelectRandomJokerAnimation(jokers, selectedJokerIndex);
    }
}
