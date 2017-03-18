namespace Interfaces
{

    using StateMachine;

    public interface IState
    {    
        void OnStateEnter(StateMachine stateMachine);

        void OnStateExit(StateMachine stateMachine);
    }

}