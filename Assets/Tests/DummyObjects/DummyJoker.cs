namespace Tests.DummyObjects
{

    using System;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class DummyJoker : IJoker
    {
        public Sprite Image { get; set; }

        public event EventHandler OnActivated = delegate {};

        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate {};

        public event EventHandler OnFinishedExecution = delegate {};

        public bool Activated { get; private set; }

        public void Activate()
        {
            this.Activated = true;
            this.OnActivated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires OnFinishedExecutionEvent and Activated = false
        /// </summary>
        public void FinishExecution()
        {
            this.Activated = false;
            this.OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
