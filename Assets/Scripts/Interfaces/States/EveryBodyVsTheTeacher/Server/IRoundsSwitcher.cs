namespace Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server
{
    using System;

    public interface IRoundsSwitcher
    {
        event EventHandler OnSwitchedToNextRound;
        event EventHandler OnMustEndGame;
        event EventHandler OnSelectedInCorrectAnswer;
        event EventHandler OnNoMoreRounds;

        int CurrentRoundNumber { get; }

        void SwitchToNextRound();
    }
}