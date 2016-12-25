using System;
using System.Collections.Generic;

//Mediator

namespace Assets.Scripts.Commands.Jokers.Add
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Interfaces;

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

            this.jokersUIController.AddJoker(joker);
        }

        public abstract void Execute(Dictionary<string, string> commandsOptionsValues);
    }

}
