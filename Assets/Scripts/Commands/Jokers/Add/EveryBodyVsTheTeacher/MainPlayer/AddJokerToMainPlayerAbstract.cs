using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.MainPlayer
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.MainPlayer;

    public class AddJokerToMainPlayerAbstract<T> : AddJokerAbstractCommand where T : IJoker
    {
        private readonly IClientNetworkManager networkManager;

        public AddJokerToMainPlayerAbstract(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager)
            : base(jokersUIController)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            this.networkManager = networkManager;
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(new MainPlayerJoker<T>(this.networkManager));
        }
    }
}
