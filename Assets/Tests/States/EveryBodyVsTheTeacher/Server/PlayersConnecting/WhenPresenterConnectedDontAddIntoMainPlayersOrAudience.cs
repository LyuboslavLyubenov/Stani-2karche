using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;
using NetworkCommandData = Commands.NetworkCommandData;
using PlayersConnectingToTheServerState = States.EveryBodyVsTheTeacher.Server.PlayersConnectingToTheServerState;

namespace Tests.States.EveryBodyVsTheTeacher.Server.PlayersConnecting
{
    using Assets.Scripts.Commands.EveryBodyVsTheTeacher;
    using Assets.Tests.Extensions;

    using Interfaces.Network.NetworkManager;

    using StateMachine;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class WhenPresenterConnectedDontAddIntoMainPlayersOrAudience : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager networkManager;

        [Inject]
        private PlayersConnectingToTheServerState state;
        
        void Start()
        {
            var stateMachine = new StateMachine();
            stateMachine.SetCurrentState(this.state);

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