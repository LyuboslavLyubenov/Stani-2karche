namespace StateMachine
{

    using System;

    using Assets.Scripts.EventArgs;
    using Assets.Scripts.Interfaces;

    using Zenject;

    public class StateMachine
    {
        public event EventHandler<StateEventArgs> OnChangedState = delegate { };

        public IState CurrentState
        {
            get; private set;
        }

        public IState LastState
        {
            get; private set;
        }
        
        [Inject]
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

            this.OnChangedState(this, new StateEventArgs(this.CurrentState));
        }

        public void SetCurrentState(IState state)
        {
            this.ChangeState(state);
        }
    }
}