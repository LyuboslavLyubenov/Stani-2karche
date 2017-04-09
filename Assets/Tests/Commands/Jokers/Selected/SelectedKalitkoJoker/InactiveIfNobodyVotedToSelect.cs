using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{
    using System.Linq;

    using Interfaces.Network;

    using Tests.DummyObjects;
    
    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class InactiveIfNobodyVotedToSelect : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.MainPlayersConnectionIds = Enumerable.Range(1, 10);

            this.command.OnElectionResult += (sender, args) => IntegrationTest.Fail();
            this.command.OnPlayerSelectedFor += (sender, args) => IntegrationTest.Fail();
            this.command.OnPlayerSelectedAgainst += (sender, args) => IntegrationTest.Fail();

            //5 seconds is default time before command timeout
            this.CoroutineUtils.WaitForSeconds(5.2f, IntegrationTest.Pass);
        }
    }

}