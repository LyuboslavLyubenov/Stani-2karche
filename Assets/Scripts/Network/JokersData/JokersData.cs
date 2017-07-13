namespace Network
{
    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces;

    using EventArgs;

    using Utils;

    public class JokersData
    {
        public event EventHandler<JokerTypeEventArgs> OnAddedJoker = delegate
        {
        };

        public event EventHandler<JokerTypeEventArgs> OnRemovedJoker = delegate
        {
        };

        private readonly List<Type> availableJokers = new List<Type>();

        public ICollection<Type> AvailableJokers
        {
            get
            {
                return this.availableJokers.ToArray();
            }
        }

        private void _AddJoker(Type joker)
        {
            this.availableJokers.Add(joker);
            this.OnAddedJoker(this, new JokerTypeEventArgs(joker));
        }

        public void AddJoker(Type joker)
        {
            JokerUtils.ValidateJokerType(joker);
            this._AddJoker(joker);
        }

        public void AddJoker<T>() where T : IJoker
        {
            var joker = typeof(T);
            this._AddJoker(joker);
        }

        public void RemoveJoker(Type joker)
        {
            if (!this.availableJokers.Contains(joker))
            {
                throw new ArgumentException("Main player doesnt have this type joker");
            }

            this.availableJokers.Remove(joker);
            this.OnRemovedJoker(this, new JokerTypeEventArgs(joker));
        }

        public void RemoveJoker<T>()
        {
            var joker = typeof(T);
            this.RemoveJoker(joker);
        }
    }

}