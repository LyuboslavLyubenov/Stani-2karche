﻿namespace Assets.Tests.Jokers.Retrievers.AudiencePollResultRetriever
{

    using Assets.Scripts.Commands;
    using Assets.Scripts.Commands.Jokers;

    using UnityEngine;


    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    public class ReceivedSettings : ExtendedMonoBehaviour
    {
        [Inject]
        private IClientNetworkManager clientNetworkManager;

        [Inject]
        private IAnswerPollResultRetriever audienceAnswerPollResultRetriever;

        void Start()
        {
            var dummyClientNetworkManager = (DummyClientNetworkManager)this.clientNetworkManager;
            var timeToAnswerInSeconds = 5;
            this.audienceAnswerPollResultRetriever.OnReceivedSettings += (sender, args) =>
            {
                if (args.TimeToAnswerInSeconds == 5)
                {
                    this.audienceAnswerPollResultRetriever.Dispose();
                    IntegrationTest.Pass();
                }
            };
            this.audienceAnswerPollResultRetriever.Activate();

            this.CoroutineUtils.WaitForSeconds(1f,
                () =>
                    {
                        var settingsCommand = NetworkCommandData.From<AnswerPollSettingsCommand>();
                        settingsCommand.AddOption("TimeToAnswerInSeconds", timeToAnswerInSeconds.ToString());
                        dummyClientNetworkManager.FakeReceiveMessage(settingsCommand.ToString());
                    });
        }
    }
}