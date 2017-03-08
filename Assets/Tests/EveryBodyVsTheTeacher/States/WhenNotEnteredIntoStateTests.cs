namespace Assets.Tests.EveryBodyVsTheTeacher.States
{
    using Assets.Scripts.Controllers.EveryBodyVsTheTeacher.States.Server;
    using Assets.Scripts.Utils.Unity;
    using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
    using Assets.Zenject.Source.Usage;
    
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