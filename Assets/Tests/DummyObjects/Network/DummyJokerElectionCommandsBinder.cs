namespace Assets.Tests.DummyObjects.Network
{

    using System;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.EveryBodyVsTheTeacher;

    using EventArgs;

    public class DummyJokerElectionCommandsBinder : IJokerElectionCommandsBinder
    {
        public event EventHandler<JokerEventArgs> OnBinded = delegate { };
        public event EventHandler<JokerEventArgs> OnUnBinded = delegate { };
        
        public void Bind(IJoker joker)
        {
            this.OnBinded(this, new JokerEventArgs(joker));
        }

        public void Bind(string jokerName)
        {
            
        }

        public void Unbind(IJoker joker)
        {
            this.OnUnBinded(this, new JokerEventArgs(joker));
        }

        public void Unbind(string jokerName)
        {
        }

        public void Dispose()
        {
        }
    }
}
