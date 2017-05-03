namespace Assets.Tests.DummyObjects.UIControllers
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Controllers;

    using EventArgs;

    public class DummyAvailableJokersUIController : IAvailableJokersUIController
    {
        public event EventHandler<JokerEventArgs> OnAddedJoker = delegate { };
        public event EventHandler<JokerEventArgs> OnUsedJoker = delegate { };

        private List<IJoker> jokers = new List<IJoker>();

        public int JokersCount
        {
            get
            {
                return this.jokers.Count;
            }
        }

        public void AddJoker(IJoker joker)
        {
            if (this.jokers.Contains(joker))
            {
                throw new ArgumentException();
            }

            this.jokers.Add(joker);
            this.OnAddedJoker(this, new JokerEventArgs(joker));
        }

        public void ClearAll()
        {
            this.jokers.Clear();
        }

        public void FakeUsedJoker(IJoker joker)
        {
            if (!this.jokers.Contains(joker))
            {
                throw new ArgumentException();
            }

            this.jokers.Remove(joker);
            this.OnUsedJoker(this, new JokerEventArgs(joker));
        }
    }
}