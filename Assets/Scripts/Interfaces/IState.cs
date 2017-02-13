namespace Assets.Scripts.Interfaces
{

    using Assets.Scripts.StateMachine;

    public interface IState
    {    
        void OnStateEnter(SimpleFiniteStateMachine stateMachine);

        void OnStateExit(SimpleFiniteStateMachine stateMachine);
    }

}