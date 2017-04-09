using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs.Jokers;

    using Interfaces.Network;

    using Tests.DummyObjects;

    using UnityEngine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class OnTimeoutShowElectionResult : MonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            ((DummyEveryBodyVsTheTeacherServer)this.server).MainPlayersConnectionIds = Enumerable.Range(1, 5);

            this.command.OnElectionResult += (sender, args) =>
                {
                    if (args.ElectionDecision == ElectionDecision.Against)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.StartCoroutine(this.SimulateVoteFromAll());
        }

        private IEnumerator SimulateVoteFromAll()
        {
            yield return null;

            SimulateSingleVote(1, ElectionDecision.For);

            for (int i = 2; i <= 3; i++)
            {
                yield return null;

                SimulateSingleVote(i, ElectionDecision.Against);
            }
        }

        private void SimulateSingleVote(int connectionId, ElectionDecision decision)
        {
            var optionValues = new Dictionary<string, string>()
                               {
                                   {
                                       "ConnectionId", connectionId.ToString()
                                   },
                                   {
                                       "Decision", decision.ToString()
                                   }
                               };
            this.command.Execute(optionValues);
        }
    }
}