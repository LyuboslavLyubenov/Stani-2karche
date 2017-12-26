using System;
using UnityEngine;
using Zenject;
using Interfaces.Network.NetworkManager;
using Tests.DummyObjects;
using Commands;
using Commands.Jokers.Selected;
using UnityTestTools.IntegrationTestsFramework.TestRunner;
using Assets.Scripts.Interfaces;

namespace Tests.Jokers.DisableRandomAnswers
{
        
    public class WhenActivatedSendSelectedJokerCommandToServer : MonoBehaviour
    {
        [Inject]
        private IClientNetworkManager networkManager;

        [Inject]
        private IJoker joker;

        void Start()
        {
            var dummyNetworkManager = (DummyClientNetworkManager)this.networkManager;
            dummyNetworkManager.OnSentToServerMessage += 
                (object sender, EventArgs.DataSentEventArgs args) =>
                {
                    var command = NetworkCommandData.Parse(args.Message);
                    if (command.Name == typeof(SelectedDisableRandomAnswersJokerCommand).Name
                                            .Replace("Command", ""))
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };

            this.joker.Activate();
        }
    }
}
