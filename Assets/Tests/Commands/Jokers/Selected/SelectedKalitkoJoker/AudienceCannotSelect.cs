﻿using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using System.Collections.Generic;
    using System.Linq;

    using Interfaces.Network;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class AudienceCannotSelect : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 10);

            this.command.OnPlayerSelectedFor += (sender, args) =>
                {
                    IntegrationTest.Fail();
                };

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var optionValues = new Dictionary<string, string>()
                                           {
                                               { "ConnectionId", "100" }
                                           };
                        this.command.Execute(optionValues);

                        //5 seconds is default time before command timeout
                        this.CoroutineUtils.WaitForSeconds(5f, IntegrationTest.Pass);
                    });
        }
    }

}