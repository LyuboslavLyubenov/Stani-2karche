using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.EveryBodyVsTheTeacher.Shared
{

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class GameStartedCommand : INetworkManagerCommand
    {
        private readonly GameObject playingUI;

        public GameStartedCommand(GameObject playingUI)
        {
            if (playingUI == null)
            {
                throw new ArgumentNullException("playingUI");
            }

            this.playingUI = playingUI;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.playingUI.SetActive(true);
        }
    }
}
