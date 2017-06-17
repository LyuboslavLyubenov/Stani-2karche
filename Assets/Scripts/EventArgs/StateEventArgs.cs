namespace Assets.Scripts.EventArgs
{
    using System;

    using Assets.Scripts.Interfaces;

    public class StateEventArgs : EventArgs
    {
        public IState State
        {
            get; set;
        }

        public StateEventArgs(IState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            this.State = state;
        }
    }
}
