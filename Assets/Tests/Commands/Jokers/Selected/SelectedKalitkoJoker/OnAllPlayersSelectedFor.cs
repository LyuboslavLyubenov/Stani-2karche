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

    public class OnAllPlayersSelectedFor : MonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;
        
        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 10);
            this.command.OnElectionResult += (sender, args) =>
                {
                    if (args.ElectionDecision == ElectionDecision.For)
                    {
                        IntegrationTest.Pass();
                    }
                };

            Utils.SimulateMainPlayerDecision(this, this.server, this.command, ElectionDecision.For);
        }
    }
}