using IClientNetworkManager = Interfaces.Network.NetworkManager.IClientNetworkManager;
using JokerUtils = Utils.JokerUtils;
using NetworkCommandData = Commands.NetworkCommandData;

namespace Assets.Scripts.Jokers.EveryBodyVsTheTeacher.MainPlayer
{
    using System;

    using Assets.Scripts.Interfaces;

    using UnityEngine;

    public class MainPlayerJoker<T> : IJoker where T : IJoker
    {
        public event EventHandler OnActivated = delegate {};
        public event EventHandler<UnhandledExceptionEventArgs> OnError = delegate {};
        public event EventHandler OnFinishedExecution = delegate {};

        private readonly IClientNetworkManager networkManager;
        private readonly string jokerName;

        public Sprite Image
        {
            get; private set;
        }

        public bool Activated
        {
            get; private set;
        }

        public MainPlayerJoker(IClientNetworkManager networkManager)
        {
            if (typeof(T) == this.GetType())
            {
                throw new InvalidOperationException();
            }
            
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }
            
            this.networkManager = networkManager;
            this.jokerName = typeof(T).Name.Replace("Joker", "");
            this.Image = JokerUtils.LoadSprite(this.jokerName);
        }

        public void Activate()
        {
            this.Activated = true;
            this.OnActivated(this, EventArgs.Empty);

            var selectedJokerCommand = new NetworkCommandData("Selected" + this.jokerName + "Joker");
            selectedJokerCommand.AddOption("Decision", "For");
            this.networkManager.SendServerCommand(selectedJokerCommand);
            
            this.OnFinishedExecution(this, EventArgs.Empty);
        }
    }
}
