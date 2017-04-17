namespace StateMachine
{

    using Assets.Scripts.Interfaces;

    using Interfaces;

    public class StateMachine
    {
        public IState CurrentState
        {
            get; private set;
        }

        public IState LastState
        {
            get; private set;
        }
        
        public StateMachine() : this(null)
        {
        }
        
        public StateMachine(IState startState)
        {
            this.SetCurrentState(startState);
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
            this.ChangeState(state);
        }
    }

}