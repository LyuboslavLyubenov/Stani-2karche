using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using System.Collections.Generic;
    using System.Linq;

    using Interfaces.Network;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject;

    public class InactiveAfterVoteFinished : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 1);

            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        var optionValues = new Dictionary<string, string>()
                                           {
                                               { "ConnectionId", "1" }
                                           };
                        this.command.Execute(optionValues);

                        this.CoroutineUtils.WaitForSeconds(0.5f,
                            () =>
                                {
                                    //callback hell
                                    this.command.OnElectionResult += (sender, args) => IntegrationTest.Fail();
                                    this.command.OnPlayerSelectedFor += (sender, args) => IntegrationTest.Fail();
                                    this.command.OnPlayerSelectedAgainst += (sender, args) => IntegrationTest.Fail();

                                    //5 seconds is default time before command timeout
                                    this.CoroutineUtils.WaitForSeconds(4.2f, IntegrationTest.Pass);
                                });
                    });
        }
    }

}