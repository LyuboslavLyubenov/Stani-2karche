using Commands.Jokers.Selected;
using Network;
using Jokers;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker 
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Interfaces.Network;
    using Commands.Jokers.Selected;
    using Zenject;
    using Network;
    using Tests.DummyObjects;
    using UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Jokers;
    using Utils.Unity;
    using System.Linq;
    using Commands.Jokers.Selected;
    using Network;

    public class WhenJokerNotAvailableDontStart : ExtendedMonoBehaviour 
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        [Inject]
        private JokersData jokersData;

        void Start () 
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.ConnectedMainPlayersConnectionIds = Enumerable.Range(1, 10);

            jokersData.AddJoker<HelpFromFriendJoker>();

            jokersData.OnRemovedJoker += (sender, args) => 
                {
                    IntegrationTest.Fail();
                };

            this.command.OnElectionStarted += (object sender, System.EventArgs args) => 
                {
                    IntegrationTest.Fail();
                };

            this.CoroutineUtils.WaitForFrames(1, () =>
                {
                    KalitkoJokerUtils.SimulateMainPlayerDecision(
                        this,
                        this.server,
                        this.command,
                        EventArgs.Jokers.ElectionDecision.For);

                    this.CoroutineUtils.WaitForSeconds(1, () =>
                        {
                            IntegrationTest.Pass();
                        });
                });
        }
    }
}