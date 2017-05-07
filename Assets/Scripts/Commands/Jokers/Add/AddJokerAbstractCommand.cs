namespace Commands.Jokers.Add
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using Controllers;

    using Interfaces;
    using Interfaces.Network.NetworkManager;

    public abstract class AddJokerAbstractCommand : INetworkManagerCommand
    {
        private IAvailableJokersUIController jokersUIController;

        protected AddJokerAbstractCommand(IAvailableJokersUIController jokersUIController)
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
