namespace EventArgs
{

    using System;

    using Interfaces;

    public class JokerEventArgs : System.EventArgs
    {
        public IJoker Joker
        {
            get;
            private set;
        }

        public JokerEventArgs(IJoker joker)
        {
            if (joker == null)
            {
                throw new ArgumentNullException("joker");
            }
            
            this.Joker = joker;    
        }
    }

}