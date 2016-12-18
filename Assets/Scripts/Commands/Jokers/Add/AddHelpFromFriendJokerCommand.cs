using UnityEngine;

//Mediator
using System.Collections.Generic;

public class AddHelpFromFriendJokerCommand : AddJokerAbstractCommand
{
    IJoker joker;

    public AddHelpFromFriendJokerCommand(AvailableJokersUIController jokersUIController, 
                                         ClientNetworkManager networkManager,
                                         GameObject callAFriendUI, 
                                         GameObject friendAnswerUI, 
                                         GameObject waitingToAnswerUI, 
                                         GameObject loadingUI)
        : base(jokersUIController)
    {
        joker = new HelpFromFriendJoker(networkManager, callAFriendUI, friendAnswerUI, waitingToAnswerUI, loadingUI);
    }

    public override void Execute(Dictionary<string, string> commandsOptionsValues)
    {
        base.AddJoker(joker);
    }
}