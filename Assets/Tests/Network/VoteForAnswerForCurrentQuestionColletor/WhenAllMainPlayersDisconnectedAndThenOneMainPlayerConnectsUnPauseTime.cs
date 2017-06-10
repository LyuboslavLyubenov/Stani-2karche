namespace Assets.Tests.Network.VoteForAnswerForCurrentQuestionColletor
{
    using System;
    using System.Collections;
    using System.Linq;

    using EventArgs;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;
    
    public class WhenAllMainPlayersDisconnectedAndThenOneMainPlayerConnectsUnPauseTime : WhenAllMainPlayersDisconnectedAbstract
    {
        void Start()
        {
            this.StartCoroutine(this.TestCoroutine());
        }

        private void OnCollectedVoteFail(object sender, AnswerEventArgs args)
        {
            IntegrationTest.Fail();
        }

        private void OnNoVotesCollectedFail(object sender, EventArgs args)
        {
            IntegrationTest.Fail();
        }

        private void OnCollectedVotePass(object sender, AnswerEventArgs args)
        {
            IntegrationTest.Pass();
        }

        private void OnNoVotesCollectedPass(object sender, EventArgs args)
        {
            IntegrationTest.Pass();
        }

        private IEnumerator TestCoroutine()
        {
            yield return null;

            base.voteResultCollector.OnCollectedVote += this.OnCollectedVoteFail;
            base.voteResultCollector.OnNoVotesCollected += this.OnNoVotesCollectedFail;

            base.ConfigureServerMainPlayers();
            base.voteResultCollector.StartCollecting();

            yield return null;

            var mainPlayersConnectionIds = base.server.MainPlayersConnectionIds.ToArray();

            for (int i = 0; i < mainPlayersConnectionIds.Length; i++)
            {
                var connectionId = mainPlayersConnectionIds[i];
                base.DisconnectMainPlayer(connectionId);
                yield return null;
            }

            yield return new WaitForSeconds(base.timeToAnswer + 1f);

            base.ConnectMainPlayer(1);

            base.voteResultCollector.OnCollectedVote -= this.OnCollectedVoteFail;
            base.voteResultCollector.OnNoVotesCollected -= this.OnNoVotesCollectedFail;

            base.voteResultCollector.OnCollectedVote += this.OnCollectedVotePass;
            base.voteResultCollector.OnNoVotesCollected += this.OnNoVotesCollectedPass;

            yield return new WaitForSeconds(base.timeToAnswer);
        }
    }
}