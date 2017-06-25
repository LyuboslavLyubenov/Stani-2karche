using Election_JokerElectionUIController = Controllers.EveryBodyVsTheTeacher.Jokers.Election.JokerElectionUIController;
using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Controllers.EveryBodyVsTheTeacher.Jokers.Election.MainPlayer
{
    using System;

    using UnityEngine.UI;

    using Zenject.Source.Usage;

    public class JokerElectionUIController : Election_JokerElectionUIController
    {
        [Inject]
        private IClientNetworkManager networkManager;
        
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

        private void OnVotedForJoker(object sender, EventArgs args)
        {
            var votedForCommand = new NetworkCommandData("PlayerVotedFor");
            this.networkManager.SendServerCommand(votedForCommand);
            this.gameObject.SetActive(false);
        }
        
        private void OnVotedAgainstJoker(object sender, EventArgs args)
        {
            var votedAgainstCommand = new NetworkCommandData("PlayerVotedAgainst");
            this.networkManager.SendServerCommand(votedAgainstCommand);
            this.gameObject.SetActive(false);
        }
    }
}