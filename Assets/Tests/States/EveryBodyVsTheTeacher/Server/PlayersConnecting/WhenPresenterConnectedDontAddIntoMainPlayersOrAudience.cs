using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;
using NetworkCommandData = Commands.NetworkCommandData;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.States.EveryBodyVsTheTeacher.Server.PlayersConnecting
{
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;

    using Interfaces.Network.NetworkManager;

    using Tests.Extensions;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenPresenterConnectedDontAddIntoMainPlayersOrAudience : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private PlayersConnectingToTheServerState state;


        void Start()
        {
            this.state.OnMainPlayerConnected += (sender, args) => IntegrationTest.Fail();
            this.state.OnAudiencePlayerConnected += (sender, args) => IntegrationTest.Fail();

            var dummyNetworkManager = (DummyServerNetworkManager)this.networkManager;
            dummyNetworkManager.SimulateClientConnected(1, "Presenter");
            var presenterConnectingCommand = NetworkCommandData.From<PresenterConnectingCommand>();
            dummyNetworkManager.FakeReceiveMessage(1, presenterConnectingCommand.ToString());

            this.CoroutineUtils.WaitForSeconds(1f, IntegrationTest.Pass);
        }
    }
}