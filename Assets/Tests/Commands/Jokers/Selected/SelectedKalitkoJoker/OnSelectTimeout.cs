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

    public class OnSelectTimeout : MonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.MainPlayersConnectionIds = Enumerable.Range(1, 10);

            this.command.OnSelectTimeout += (sender, args) =>
                {
                    IntegrationTest.Pass();
                };

            this.StartCoroutine(this.SimulatePartOfMainPlayersSelectedKalitkoJoker());
        }

        private IEnumerator SimulatePartOfMainPlayersSelectedKalitkoJoker()
        {
            yield return new WaitForSeconds(0.5f);

            var optionValues = new Dictionary<string, string>()
                               {
                                   { "ConnectionId", "0" }
                               };

            for (int i = 0; i < this.server.MainPlayersConnectionIds.Count() - 2; i++)
            {
                var mainPlayerConnectionId = this.server.MainPlayersConnectionIds.Skip(i)
                    .First();
                optionValues["ConnectionId"] = mainPlayerConnectionId.ToString();
                this.command.Execute(optionValues);

                yield return null;
            }
        }
    }

}