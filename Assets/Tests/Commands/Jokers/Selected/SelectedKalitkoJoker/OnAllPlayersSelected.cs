using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Network;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    public class OnAllPlayersSelected : MonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        private readonly List<int> clientsSelectedKalitko = new List<int>();

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.MainPlayersConnectionIds = Enumerable.Range(1, 10);
            this.command.OnAllPlayersSelected += (sender, args) =>
                {
                    IntegrationTest.Pass();
                };

            this.StartCoroutine(this.SimulateAllMainPlayersSelectedKalitkoJoker());
        }

        private IEnumerator SimulateAllMainPlayersSelectedKalitkoJoker()
        {
            yield return null;

            var optionValues = new Dictionary<string, string>()
            {
                { "ConnectionId", "0" }
            };

            for (int i = 0; i < server.MainPlayersConnectionIds.Count(); i++)
            {
                var mainPlayerConnectionId = server.MainPlayersConnectionIds.Skip(i)
                    .First();
                optionValues["ConnectionId"] = mainPlayerConnectionId.ToString();
                this.command.Execute(optionValues);

                yield return null;
            }
        }
    }

}