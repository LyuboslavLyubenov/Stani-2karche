namespace Assets.Scripts.States.EveryBodyVsTheTeacher.Server.Rounds
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;

    using StateMachine;

    public class RoundsSwitcher : IRoundsSwitcher
    {
        public event EventHandler OnSwitchedToNextRound = delegate {};
        public event EventHandler OnTooManyWrongAnswers = delegate {};
        public event EventHandler OnNoMoreRounds = delegate { };

        private readonly IRoundState[] rounds;

        private readonly StateMachine stateMachine;

        private int index = -1;

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
            this.SwitchToNextRound();
        }

        private void OnMustGoOnNextRound(object sender, EventArgs args)
        {
            this.SwitchToNextRound();
        }

        private void _OnTooManyWrongAnswers(object sender, EventArgs args)
        {
            this.OnTooManyWrongAnswers(this, args);
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
                this.rounds[this.index].OnStateExit(this.stateMachine);
                this.rounds[this.index].OnMustGoOnNextRound -= this.OnMustGoOnNextRound;
                this.rounds[this.index].OnTooManyWrongAnswers -= this._OnTooManyWrongAnswers;
            }
            
            this.rounds[nextRoundIndex].OnStateEnter(this.stateMachine);
            this.rounds[nextRoundIndex].OnMustGoOnNextRound += this.OnMustGoOnNextRound;
            this.rounds[nextRoundIndex].OnTooManyWrongAnswers += this._OnTooManyWrongAnswers;
            
            this.index = nextRoundIndex;
            this.OnSwitchedToNextRound(this, EventArgs.Empty);
        }
    }
}