using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Jokers.EveryBodyVsTheTeacher.Presenter;

    using UnityEngine;

    public class AddKalitkoJokerCommand : AddJokerAbstractCommand
    {
        private readonly IJoker joker;

        public AddKalitkoJokerCommand(
            IAvailableJokersUIController jokersUIController, 
            IClientNetworkManager networkManager,
            IKalitkoJokerUIController kalitkoJokerUIController,
            GameObject kalitkoJokerUI)
            : base(jokersUIController)
        {
            this.joker = new PresenterKalitkoJoker(networkManager, kalitkoJokerUIController, kalitkoJokerUI);       
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            base.AddJoker(this.joker);
        }
    }
}
