namespace Assets.Scripts.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Assets.Scripts.Interfaces.Network.NetworkManager;

    using Commands;
    using EventArgs;
    using Interfaces;
    using Utils;

    /// <summary>
    /// Tracks player jokers
    /// </summary>
    public class JokersData
    {
        public event EventHandler<JokerTypeEventArgs> OnAddedJoker = delegate
            {
            };

        public event EventHandler<JokerTypeEventArgs> OnUsedJoker = delegate
            {
            };

        public event EventHandler<JokerTypeEventArgs> OnRemovedJoker = delegate
            {
            };

        private List<Type> availableJokers = new List<Type>();

        public ICollection<Type> AvailableJokers
        {
            get
            {
                return this.availableJokers;
            }
        }

        public JokersData(IServerNetworkManager networkManager)
        {
            var jokerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    {
                        var jokerInterfaceType = typeof(IJoker);
                        var isJoker = t.GetInterfaces().Contains(jokerInterfaceType);
                        return isJoker;
                    })
                .ToArray();

            for (int i = 0; i < jokerTypes.Length; i++)
            {
                var jokerType = jokerTypes[i];
                var jokerName = jokerType.Name;
                var executedJokerCommand = new DummyCommand();
                executedJokerCommand.OnExecuted += (sender, args) => this.UsedJoker(jokerType);
                networkManager.CommandsManager.AddCommand("Selected" + jokerName, executedJokerCommand);
            }
        }

        private void UsedJoker(Type jokerType)
        {
            this.OnUsedJoker(this, new JokerTypeEventArgs(jokerType));
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