namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;

    using StateMachine;

    public class RoundsSwitcher : IRoundsSwitcher
    {
        public event EventHandler OnSwitchedToNextRound = delegate { };
        public event EventHandler OnMustEndGame = delegate { };
        public event EventHandler OnNoMoreRounds = delegate { };

        private readonly IRoundState[] rounds;

        private readonly StateMachine stateMachine;

        private int index = -1;

        public int CurrentRoundNumber
        {
            get
            {
                return this.index + 1;
            }
        }

        public class Builder
        {
            private readonly StateMachine stateMachine;

            private Queue<IRoundState> rounds = new Queue<IRoundState>();

            public Builder(StateMachine stateMachine)
            {
                if (stateMachine == null)
                {
                    throw new ArgumentNullException("stateMachine");
                }

                this.stateMachine = stateMachine;
            }

            public Builder AddRound(IRoundState round)
            {
                if (round == null)
                {
                    throw new ArgumentNullException("round");
                }

                this.rounds.Enqueue(round);
                return this;
            }

            public Builder RemoveRound(IRoundState round)
            {
                if (round == null)
                {
                    throw new ArgumentNullException("round");
                }

                if (!this.rounds.Contains(round))
                {
                    throw new ArgumentException("Round not added");
                }

                this.rounds = new Queue<IRoundState>(this.rounds.Where(r => r != round));

                return this;
            }

            public RoundsSwitcher Build()
            {
                if (this.rounds.Count == 0)
                {
                    throw new InvalidOperationException("Must have at least one round");
                }

                return new RoundsSwitcher(this.rounds.ToArray(), this.stateMachine);
            }
        }

        private RoundsSwitcher(IRoundState[] rounds, StateMachine stateMachine)
        {
            this.rounds = rounds;
            this.stateMachine = stateMachine;
        }

        private void OnMustGoOnNextRound(object sender, EventArgs args)
        {
            this.SwitchToNextRound();
        }

        public void SwitchToNextRound()
        {
            var nextRoundIndex = this.index + 1;

            if (nextRoundIndex >= this.rounds.Length)
            {
                this.OnNoMoreRounds(this, EventArgs.Empty);
                return;
            }

            if (this.index >= 0)
            {
                var previousRound = this.rounds[this.index];
                previousRound.OnMustGoOnNextRound -= this.OnMustGoOnNextRound;
                previousRound.OnMustEndGame -= this.OnMustEndGame;
            }
            
            var nextRound = this.rounds[nextRoundIndex];
            this.stateMachine.SetCurrentState(nextRound);

            this.rounds[nextRoundIndex].OnMustGoOnNextRound += this.OnMustGoOnNextRound;
            this.rounds[nextRoundIndex].OnMustEndGame += this.OnMustEndGame;
            
            this.index = nextRoundIndex;
            this.OnSwitchedToNextRound(this, EventArgs.Empty);
        }
    }
}