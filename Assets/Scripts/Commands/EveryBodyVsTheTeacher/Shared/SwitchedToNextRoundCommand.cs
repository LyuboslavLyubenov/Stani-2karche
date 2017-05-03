using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared
{

    using System.Collections.Generic;

    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher;
    using Assets.Scripts.Interfaces.Controllers;

    using Extensions;

    using UnityEngine;

    public class SwitchedToNextRoundCommand : INetworkManagerCommand
    {
        private readonly GameObject changedRoundUi;
        private readonly IChangedRoundUIController changedRoundUiController;

        public SwitchedToNextRoundCommand(GameObject changedRoundUi)
        {
            this.changedRoundUi = changedRoundUi;
            this.changedRoundUiController = this.changedRoundUi.GetComponent<ChangedRoundUIController>();
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var round = commandsOptionsValues["Round"]
                .ConvertTo<int>();

            this.changedRoundUi.SetActive(true);
            this.changedRoundUiController.Round = round;
        }
    }
}
