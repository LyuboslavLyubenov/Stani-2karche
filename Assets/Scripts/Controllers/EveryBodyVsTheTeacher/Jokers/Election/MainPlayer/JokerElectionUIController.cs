using ElectionDecision = EventArgs.Jokers.ElectionDecision;
using Election_JokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.JokerElectionUIController;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Jokers.Election.MainPlayer
{
    using System;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces;

    using UnityEngine.UI;

    using Zenject;

    public class JokerElectionUIController : Election_JokerElectionUIController
    {
        [Inject]
        private IClientNetworkManager networkManager;

        private string jokerName;

        void Awake()
        {
            base.OnVotedFor += this.OnVotedForJoker;
            base.OnVotedAgainst += this.OnVotedAgainstJoker;
        }

        void Start()
        {
            var textComponents = this.GetComponentsInChildren<Text>();
            for (int i = 0; i < textComponents.Length; i++)
            {
                var textComponent = textComponents[i];
                textComponent.gameObject.SetActive(false);
            }
        }

        public override void SetJoker(IJoker joker)
        {
            base.SetJoker(joker);
            this.jokerName = joker.GetName();
        }

        private void SendElectionDecision(ElectionDecision decision)
        {
            var jokerElectionDecision = new NetworkCommandData("Selected" + this.jokerName + "Joker");
            jokerElectionDecision.AddOption("Decision", decision.ToString());
            this.networkManager.SendServerCommand(jokerElectionDecision);
        }

        private void OnVotedForJoker(object sender, EventArgs args)
        {
            this.SendElectionDecision(ElectionDecision.For);
            this.gameObject.SetActive(false);
        }
        
        private void OnVotedAgainstJoker(object sender, EventArgs args)
        {
            this.SendElectionDecision(ElectionDecision.Against);
            this.gameObject.SetActive(false);
        }
    }
}