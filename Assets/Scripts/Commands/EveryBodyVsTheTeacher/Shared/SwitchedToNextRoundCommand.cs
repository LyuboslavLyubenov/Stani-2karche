using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Scripts.Commands.EveryBodyVsTheTeacher.Shared
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using UnityEngine;

    public class SwitchedToNextRoundCommand : INetworkManagerCommand
    {
        private readonly GameObject changedRoundUI;
        private readonly IChangedRoundUIController changedRoundUIController;

        public SwitchedToNextRoundCommand(GameObject changedRoundUI, IChangedRoundUIController changedRoundUiController)
        {
            if (changedRoundUI == null)
            {
                throw new ArgumentNullException("changedRoundUI");
            }

            if (changedRoundUiController == null)
            {
                throw new ArgumentNullException("changedRoundUiController");
            }

            this.changedRoundUI = changedRoundUI;
            this.changedRoundUIController = changedRoundUiController;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var round = commandsOptionsValues["Round"]
                .ConvertTo<int>();

            this.changedRoundUI.SetActive(true);
            this.changedRoundUIController.Round = round;
        }
    }
}
