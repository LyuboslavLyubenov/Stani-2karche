namespace Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher
{
    using System;
    
    public interface IElectionForJokersBinder : IDisposable
    {
        void Bind(IJoker joker);
    }
}
