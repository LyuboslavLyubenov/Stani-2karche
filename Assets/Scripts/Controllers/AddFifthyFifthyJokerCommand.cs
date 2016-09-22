using UnityEngine;
using System;
using System.Collections;

//Mediator
using System.Collections.Generic;

public class AddFifthyFifthyJokerCommand : AddJokerAbstractCommand
{
    IJoker joker;

    public AddFifthyFifthyJokerCommand(AvailableJokersUIController availableJokersUIController, ClientNetworkManager networkManager, IGameData gameData, QuestionUIController questionUIController)
        : base(availableJokersUIController)
    {
        this.joker = new DisableRandomAnswersJoker(networkManager, gameData, questionUIController);
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        base.AddJoker(joker);
    }
}