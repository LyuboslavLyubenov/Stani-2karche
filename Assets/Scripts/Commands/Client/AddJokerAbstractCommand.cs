using UnityEngine;
using System;
using System.Collections;

//Mediator
using System.Collections.Generic;

public abstract class AddJokerAbstractCommand : INetworkManagerCommand
{
    AvailableJokersUIController jokersUIController;

    protected AddJokerAbstractCommand(AvailableJokersUIController jokersUIController)
    {
        if (jokersUIController == null)
        {
            throw new ArgumentNullException("jokersUIController");
        }

        this.jokersUIController = jokersUIController;
    }

    protected void AddJoker(IJoker joker)
    {
        if (joker == null)
        {
            throw new ArgumentNullException("joker");
        }

        jokersUIController.AddJoker(joker);
    }

    public abstract void Execute(Dictionary<string, string> commandsOptionsValues);
}
