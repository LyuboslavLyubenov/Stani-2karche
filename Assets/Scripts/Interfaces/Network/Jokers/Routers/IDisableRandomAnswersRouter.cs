namespace Assets.Scripts.Interfaces.Network.Jokers
{

    using System;

    public interface IDisableRandomAnswersRouter : IJokerRouter
    {
        void Activate(int answersToDisableCount);
    }
}