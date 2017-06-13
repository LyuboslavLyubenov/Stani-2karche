namespace Assets.Tests.Network.VoteForAnswerForCurrentQuestionColletor
{
    using System.Collections;
    using System.Linq;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    public class WhenAllMainPlayersDisconnectedPauseTime : WhenAllMainPlayersDisconnectedAbstract
    {
        void Start()
        {
            this.StartCoroutine(this.TestCoroutine());
        }

        private IEnumerator TestCoroutine()
        {
            yield return null;

            base.voteResultCollector.OnCollectedVote += (sender, args) =>
                {
                    IntegrationTest.Fail();
                };

            base.voteResultCollector.OnNoVotesCollected += (sender, args) =>
                {
                    IntegrationTest.Fail();
                };

            base.ConfigureServerMainPlayers();
            base.voteResultCollector.StartCollecting();

            yield return null;

            var mainPlayersConnectionIds = base.server.ConnectedMainPlayersConnectionIds.ToArray();

            for (int i = 0; i < mainPlayersConnectionIds.Length; i++)
            {
                var connectionId = mainPlayersConnectionIds[i];
                base.DisconnectMainPlayer(connectionId);
                yield return null;
            }

            yield return new WaitForSeconds(base.timeToAnswer + 1f);

            IntegrationTest.Pass();
        }
    }
}