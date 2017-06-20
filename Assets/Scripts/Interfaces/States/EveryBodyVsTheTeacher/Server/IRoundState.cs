namespace Assets.Scripts.Interfaces.States.EveryBodyVsTheTeacher.Server
{
    using System;

    public interface IRoundState : IState
    {
        event EventHandler OnMustGoOnNextRound;
        event EventHandler OnMustEndGame;
        event EventHandler OnSelectedInCorrectAnswer;
    }
}
