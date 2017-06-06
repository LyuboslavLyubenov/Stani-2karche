namespace Controllers
{
    using System;

    using Assets.Scripts.Interfaces.Utils;

    using Utils.Unity;

    public class UnityTimer : ExtendedMonoBehaviour, IUnityTimer
    {
        public event EventHandler OnSecondPassed = delegate { };
        public event EventHandler OnFinished = delegate { };

        private int intervalInSeconds = 0;
        private int elapsedSeconds = 0;

        public int InvervalInSeconds
        {
            get
            {
                return this.intervalInSeconds;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.intervalInSeconds = value;
            }
        }

        public bool Paused
        {
            get; set;
        }

        public bool Running
        {
            get; private set;
        }

        void Start()
        {
            this.Initialize();
            this.CoroutineUtils.RepeatEverySeconds(1f, this.UpdateTimer);
        }

        /// <summary>
        /// Use it for intiializations (when inheriting)
        /// </summary>
        protected virtual void Initialize()
        {
        }

        private void UpdateTimer()
        {
            if (this.Paused || !this.Running)
            {
                return;
            }

            if (this.elapsedSeconds < this.InvervalInSeconds)
            {
                this.elapsedSeconds++;
                this.OnSecondPassed(this, EventArgs.Empty);
            }
            else
            {

                this.OnFinished(this, EventArgs.Empty);
            }
        }

        public virtual void StartTimer()
        {
            if (this.Running)
            {
                throw new InvalidOperationException("Already running");
            }
            
            this.Running = true;
            this.Paused = false;
            this.elapsedSeconds = 0;
        }

        public virtual void StopTimer()
        {
            if (!this.Running)
            {
                throw new InvalidOperationException("Must be active");
            }

            this.Running = false;
            this.Paused = false;
        }
    }
}