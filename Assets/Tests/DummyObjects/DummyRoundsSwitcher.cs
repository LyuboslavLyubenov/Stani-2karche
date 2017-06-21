namespace Assets.Tests.DummyObjects
{

    using System;

    using Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server;

    public class DummyRoundsSwitcher : IRoundsSwitcher
    {
        public event EventHandler OnSwitchedToNextRound = delegate { };
        public event EventHandler OnMustEndGame = delegate { };
        public event EventHandler OnSelectedInCorrectAnswer = delegate { };
        public event EventHandler OnNoMoreRounds = delegate { };

        public int CurrentRoundNumber
        {
            get; set;
        }

        public void SwitchToNextRound()
        {
            this.OnSwitchedToNextRound(this, EventArgs.Empty);
        }

        public void FireOnMustEndGame()
        {
            this.OnMustEndGame(this, EventArgs.Empty);
        }

        public void FireOnSelectedInCorrectAnswer()
        {
            this.OnSelectedInCorrectAnswer(this, EventArgs.Empty);
        }

        public void FireOnNoMoreRounds()
        {
            this.OnNoMoreRounds(this, EventArgs.Empty);
        }
    }
}
