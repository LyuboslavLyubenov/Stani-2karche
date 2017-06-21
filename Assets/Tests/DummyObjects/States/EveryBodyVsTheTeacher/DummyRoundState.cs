namespace Assets.Tests.DummyObjects.States.EveryBodyVsTheTeacher
{
    using System;

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;

    using StateMachine;

    public class DummyRoundState : IRoundState
    {
        public event EventHandler OnMustGoOnNextRound = delegate { };
        public event EventHandler OnMustEndGame = delegate {};
        public event EventHandler OnSelectedInCorrectAnswer = delegate {};
        
        public event EventHandler OnEnter = delegate { };
        public event EventHandler OnExit = delegate { };

        public int MistakesRemaining
        {
            get; set;
        }

        public void OnStateEnter(StateMachine stateMachine)
        {
            this.OnEnter(this, EventArgs.Empty);
        }

        public void OnStateExit(StateMachine stateMachine)
        {
            this.OnExit(this, EventArgs.Empty);
        }

        public void FireOnMustGoOnNextRound()
        {
            this.OnMustGoOnNextRound(this, EventArgs.Empty);
        }

        public void FireOnTooManyWrongAnswers()
        {
            this.OnMustEndGame(this, EventArgs.Empty);
        }

        public void FireOnSelectedInCorrectAnswer()
        {
            this.OnSelectedInCorrectAnswer(this, EventArgs.Empty);
        }
    }
}