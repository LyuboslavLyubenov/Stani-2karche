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

    using Zenject;

    public class WhenTieRejectActivation : MonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            ((DummyEveryBodyVsTheTeacherServer)this.server).ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 2);
            
            this.command.OnElectionResult += (sender, args) =>
                {
                    if (args.ElectionDecision == ElectionDecision.Against)
                    {
                        IntegrationTest.Pass();
                    }
                    else
                    {
                        IntegrationTest.Fail();
                    }
                };

            this.StartCoroutine(this.SimulateVoting());
        }

        IEnumerator SimulateVoting()
        {
            for (int i = 1; i <= 2; i++)
            {
                yield return null;

                var optionValues = new Dictionary<string, string>()
                                   {
                                       {
                                           "ConnectionId", i.ToString()
                                       },
                                       {
                                           "Decision", (i == 1) ? ElectionDecision.For.ToString() : ElectionDecision.Against.ToString()
                                       }
                                   };
                this.command.Execute(optionValues);
            }
        }
    }

}