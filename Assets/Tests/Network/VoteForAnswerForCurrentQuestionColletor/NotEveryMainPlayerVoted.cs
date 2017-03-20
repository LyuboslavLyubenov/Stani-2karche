﻿using NetworkCommandData = Commands.NetworkCommandData;

namespace Tests.Network.VoteForAnswerForCurrentQuestionCollector
{

    using System.Collections;
    using System.Linq;

    using Assets.Scripts.Interfaces.Network;

    using Interfaces;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class NotEveryMainPlayerVoted : MonoBehaviour
    {
        [Inject]
        private ICollectVoteResultForAnswerForCurrentQuestion voteCollector;

        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private ISimpleQuestion question;

        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        void Start()
        {
            var highestVotedAnswer = this.question.Answers[0];
            this.voteCollector.OnCollectedVote += (sender, args) =>
                {
                    if (args.Answer == highestVotedAnswer)
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };

            this.voteCollector.StartCollecting();
            this.StartCoroutine(this.SimulateMainPlayersVoting(highestVotedAnswer));
        }

        IEnumerator SimulateMainPlayersVoting(string answerToSelect)
        {
            var dummyServerNetworkManager = (DummyServerNetworkManager)this.networkManager;
            var mainPlayersConnectionIds = this.server.MainPlayersConnectionIds.Skip(1).ToList();
            var answerSelectedCommand = new NetworkCommandData("AnswerSelected");
            answerSelectedCommand.AddOption("Answer", answerToSelect);
            
            for (int i = 0; i < mainPlayersConnectionIds.Count; i++)
            {
                var connectionId = mainPlayersConnectionIds[i];
                dummyServerNetworkManager.FakeReceiveMessage(connectionId, answerSelectedCommand.ToString());
                yield return null;
            }
        }
    }
}