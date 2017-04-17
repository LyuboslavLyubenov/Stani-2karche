namespace Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server
{
    using System;

    public interface IRoundsSwitcher
    {
        event EventHandler OnSwitchedToNextRound;
        event EventHandler OnTooManyWrongAnswers;
        event EventHandler OnNoMoreRounds;

        void SwitchToNextRound();
    }
}