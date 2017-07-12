using ElectionDecision = EventArgs.Jokers.ElectionDecision;

namespace Assets.Scripts.Commands.Jokers.Election.Presenter
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class ElectionJokerResultCommand : Election.ElectionJokerResultCommand
    {
        private readonly IJoker joker;

        public ElectionJokerResultCommand(
            GameObject electionJokerUI, 
            GameObject succesfullyVotedForJokerUI, 
            GameObject unsuccessfullyVotedForJokerUI,
            IJoker joker)
            : 
            base(
                electionJokerUI, 
                succesfullyVotedForJokerUI, 
                unsuccessfullyVotedForJokerUI)
        {
            if (joker == null)
            {
                throw new ArgumentNullException("joker");
            }

            this.joker = joker;
        }

        public new void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            var decision = commandsOptionsValues["Decision"];

            if (decision == ElectionDecision.For.ToString())
            {
                this.joker.Activate();
            }

            base.Execute(commandsOptionsValues);
        }
    }
}
