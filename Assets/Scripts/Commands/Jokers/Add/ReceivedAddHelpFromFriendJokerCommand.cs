using UnityEngine;
using System;
using System.Collections;

//Mediator
using System.Collections.Generic;

public class ReceivedAddHelpFromFriendJokerCommand : ReceivedAddJokerAbstractCommand
{
    IJoker joker;

    public ReceivedAddHelpFromFriendJokerCommand(AvailableJokersUIController jokersUIController, 
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