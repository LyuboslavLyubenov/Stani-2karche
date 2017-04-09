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

    public class OnAllPlayersSelectedFor : MonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;
        
        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.MainPlayersConnectionIds = Enumerable.Range(1, 10);
            this.command.OnElectionResult += (sender, args) =>
                {
                    if (args.ElectionDecision == ElectionDecision.For)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.StartCoroutine(this.SimulateAllMainPlayersSelectedKalitkoJoker());
        }

        private IEnumerator SimulateAllMainPlayersSelectedKalitkoJoker()
        {
            yield return null;

            var optionValues = new Dictionary<string, string>()
                               {
                                   {
                                       "ConnectionId", "0"
                                   },
                                   {
                                       "Decision", ""
                                   }
                               };

            for (int i = 0; i < server.MainPlayersConnectionIds.Count(); i++)
            {
                var mainPlayerConnectionId = server.MainPlayersConnectionIds.Skip(i)
                    .First();
                optionValues["ConnectionId"] = mainPlayerConnectionId.ToString();
                optionValues["Decision"] = ElectionDecision.For.ToString();

                this.command.Execute(optionValues);

                yield return null;
            }
        }
    }

}