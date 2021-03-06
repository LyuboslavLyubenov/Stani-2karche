﻿using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{
    using System.Collections.Generic;
    using System.Linq;

    using EventArgs.Jokers;

    using Interfaces.Network;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class OnPlayerSelectedFor : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 10);

            var connectionId = 1;

            this.command.OnPlayerSelectedFor += (sender, args) =>
                {
                    if (args.ConnectionId == connectionId)
                    {
                        IntegrationTest.Pass();
                    }
                };

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var optionValues = new Dictionary<string, string>()
                                           {
                                               {
                                                   "ConnectionId", connectionId.ToString()
                                               },
                                               {
                                                   "Decision", ElectionDecision.For.ToString()
                                               }
                                           };
                        this.command.Execute(optionValues);
                    });
        }
    }

}