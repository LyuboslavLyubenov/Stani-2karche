using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.EveryBodyVsTheTeacher.States.Server.PlayersConnecting
{

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class WhenNotEnteredIntoStateTests : ExtendedMonoBehaviour
    {
        [Inject]
        private PlayersConnectingToTheServerState state;

        void Start()
        {
            if (this.state.AudiencePlayersConnectionIds.Count > 0 || this.state.MainPlayersConnectionIds.Count > 0)
            {
                IntegrationTest.Fail();
                return;
            }

            IntegrationTest.Pass();
        }
    }
}