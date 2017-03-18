﻿using AnswerPollSettingsCommand = Commands.Jokers.Settings.AnswerPollSettingsCommand;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Jokers.Retrievers.PollResultRetriever
{

    using Interfaces.Network.Jokers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

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