using ElectionDecision = EventArgs.Jokers.ElectionDecision;
using INetworkManagerCommand = Interfaces.Network.NetworkManager.INetworkManagerCommand;

namespace Assets.Scripts.Commands.Jokers.Election
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class ElectionJokerResultCommand : INetworkManagerCommand
    {
        private readonly GameObject electionJokerUI;
        private readonly GameObject succesfullyVotedForJokerUi;
        private readonly GameObject unsuccessfullyVotedForJokerUi;

        private readonly IJoker joker;

        public ElectionJokerResultCommand(
            GameObject electionJokerUI,
            GameObject succesfullyVotedForJokerUI,
            GameObject unsuccessfullyVotedForJokerUI)
        {
            if (electionJokerUI == null)
            {
                throw new ArgumentNullException("electionJokerUI");
            }

            if (succesfullyVotedForJokerUI == null)
            {
                throw new ArgumentNullException("succesfullyVotedForJokerUI");
            }

            if (unsuccessfullyVotedForJokerUI == null)
            {
                throw new ArgumentNullException("unsuccessfullyVotedForJokerUI");
            }

            if (joker == null)
            {
                throw new ArgumentNullException("joker");
            }

            this.electionJokerUI = electionJokerUI;
            this.succesfullyVotedForJokerUi = succesfullyVotedForJokerUI;
            this.unsuccessfullyVotedForJokerUi = unsuccessfullyVotedForJokerUI;
        }

        protected virtual void OnSuccessfullyActivated()
        {

        }

        protected virtual void OnUnSuccessfullyActivated()
        {

        }

        public void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            this.electionJokerUI.SetActive(false);

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