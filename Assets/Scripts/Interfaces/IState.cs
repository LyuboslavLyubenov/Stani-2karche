namespace Assets.Scripts.Interfaces
{

    using Assets.Scripts.StateMachine;

    public interface IState
    {    
        void OnStateEnter(StateMachine stateMachine);

        void OnStateExit(StateMachine stateMachine);
    }

}