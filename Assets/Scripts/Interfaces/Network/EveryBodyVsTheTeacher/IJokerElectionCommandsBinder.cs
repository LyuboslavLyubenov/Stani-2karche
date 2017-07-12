namespace Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher
{
    using System;
    
    public interface IJokerElectionCommandsBinder : IDisposable
    {
        void Bind(IJoker joker);
        
        void Unbind(IJoker joker);
    }
}
