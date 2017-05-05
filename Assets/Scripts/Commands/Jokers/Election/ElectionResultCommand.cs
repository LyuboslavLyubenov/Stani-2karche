using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.Jokers.Election
{

    using System;
    using System.Collections.Generic;

    using EventArgs.Jokers;

    using UnityEngine;

    public class ElectionResultCommand : INetworkManagerCommand
    {
        private readonly GameObject succesfullyVotedForJokerUi;
        private readonly GameObject unsuccessfullyVotedForJokerUi;

        public ElectionResultCommand(
            GameObject succesfullyVotedForJokerUI, 
            GameObject unsuccessfullyVotedForJokerUI)
        {
            if (succesfullyVotedForJokerUI == null)
            {
                throw new ArgumentNullException("succesfullyVotedForJokerUI");
            }

            if (unsuccessfullyVotedForJokerUI == null)
            {
                throw new ArgumentNullException("unsuccessfullyVotedForJokerUI");
            }

            this.succesfullyVotedForJokerUi = succesfullyVotedForJokerUI;
            this.unsuccessfullyVotedForJokerUi = unsuccessfullyVotedForJokerUI;
        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var electionDecision = (ElectionDecision)Enum.Parse(typeof(ElectionDecision), commandsOptionsValues["Decision"]);

            if (electionDecision == ElectionDecision.For)
            {
                this.succesfullyVotedForJokerUi.SetActive(true);
            }
            else
            {
                this.unsuccessfullyVotedForJokerUi.SetActive(true);
            }
        }
    }
}