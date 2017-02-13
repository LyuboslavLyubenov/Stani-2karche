namespace Assets.Scripts.Controllers.GameController
{
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.StateMachine;
    using Assets.Scripts.States.EverybodyVsTheTeacherServer;
    using Assets.Scripts.Utils.Unity;

    using Zenject;

    public class EveryBodyVsTheTeacherServer : ExtendedMonoBehaviour
    {
        [Inject]
        private IServerNetworkManager serverNetworkManager;

        private readonly SimpleFiniteStateMachine stateMachine = new SimpleFiniteStateMachine();

        [Inject]
        private PlayersConnectingToTheServerState playersConnectingToTheServerState;

        void Start()
        {
            this.stateMachine.SetCurrentState(this.playersConnectingToTheServerState);
        }
    }
}