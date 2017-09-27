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

    public class KalitkoJokerUtils
    {
        KalitkoJokerUtils()
        {
        }

        public static void SimulateMainPlayerDecision(
            MonoBehaviour monoBehaviour,
            IEveryBodyVsTheTeacherServer server, 
            SelectedKalitkoJokerCommand selectedKalitkoJokerCommand,
            ElectionDecision decision)
        {
            monoBehaviour.StartCoroutine(SimulateMainPlayerDecisionCoroutine(server, selectedKalitkoJokerCommand, decision));
        }

        private static IEnumerator SimulateMainPlayerDecisionCoroutine(
            IEveryBodyVsTheTeacherServer server, 
            SelectedKalitkoJokerCommand selectedKalitkoJokerCommand,
            ElectionDecision decision)
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

            for (int i = 0; i < server.ConnectedMainPlayersConnectionIds.Count(); i++)
            {
                var mainPlayerConnectionId = server.ConnectedMainPlayersConnectionIds.Skip(i)
                    .First();
                optionValues["ConnectionId"] = mainPlayerConnectionId.ToString();
                optionValues["Decision"] = decision.ToString();

                selectedKalitkoJokerCommand.Execute(optionValues);

                yield return null;
            }
        }

    }

}