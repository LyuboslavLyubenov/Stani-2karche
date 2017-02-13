namespace Assets.Scripts.StateMachine
{

    using Assets.Scripts.Interfaces;

    public class SimpleFiniteStateMachine
    {
        public IState CurrentState
        {
            get; private set;
        }

        public IState LastState
        {
            get; private set;
        }
        
        public SimpleFiniteStateMachine() : this(null)
        {
        }
        
        public SimpleFiniteStateMachine(IState startState)
        {
            this.CurrentState = startState;
        }
        
        private void ChangeState(IState newState)
        {
            this.LastState = this.CurrentState;

            if (this.LastState != null)
            {
                this.LastState.OnStateExit(this);
            }
            
            this.CurrentState = newState;

            if (this.CurrentState != null)
            {
                this.CurrentState.OnStateEnter(this);
            }
        }

        public void SetCurrentState(IState state)
        {
            ChangeState(state);
        }
    }

}