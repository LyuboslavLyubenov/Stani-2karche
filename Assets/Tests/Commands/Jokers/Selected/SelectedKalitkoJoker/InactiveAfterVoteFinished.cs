using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Utils.Unity;
    using Assets.Tests.DummyObjects;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;

    public class InactiveAfterVoteFinished : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.MainPlayersConnectionIds = Enumerable.Range(1, 1);

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
                                    this.command.OnAllPlayersSelected += (sender, args) => IntegrationTest.Fail();
                                    this.command.OnPlayerSelected += (sender, args) => IntegrationTest.Fail();
                                    this.command.OnSelectTimeout += (sender, args) => IntegrationTest.Fail();

                                    //5 seconds is default time before command timeout
                                    this.CoroutineUtils.WaitForSeconds(4.2f, IntegrationTest.Pass);
                                });
                    });
        }
    }

}